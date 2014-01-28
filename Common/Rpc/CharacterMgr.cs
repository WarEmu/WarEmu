using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MidgarFrameWork.Logger;
using MidgarFrameWork.RpcV3;
using MidgarFrameWork.Database;
using MidgarFrameWork.NetWork;

namespace Common
{
    public class AccountCharacter
    {
        static public readonly byte MAX_CHARS = 10;

        public int _AccountId;
        public GameData.pRealm _Realm = GameData.pRealm.REALM_NONE;
        private RealmCharacters _Rm;

        public Character[] Chars = new Character[MAX_CHARS];

        public AccountCharacter(int AccountId,RealmCharacters Rm)
        {
            _Rm = Rm;
            _AccountId = AccountId;
        }
        public bool AddCharacter(Character Char)
        {
            lock (Chars)
            {
                if (Chars[Char.SlotId] != null)
                    return false;

                Chars[Char.SlotId] = Char;
                _Realm = (GameData.pRealm)Char.Realm;
            }

            return true;
        }
        public byte GetFreeSlot()
        {
            lock (Chars)
                for (byte i = 0; i < Chars.Length; ++i)
                    if (Chars[i] == null)
                        return i;

            return (byte)MAX_CHARS;
        }
        public byte GetCharacterCount()
        {
            byte Count=0;
            for (byte i = 0; i < Chars.Length; ++i)
                if (Chars[i] != null)
                    ++Count;

            return Count;
        }
        public Character CreateCharacter(Character Char)
        {
            byte Free = GetFreeSlot();
            if (Free >= MAX_CHARS)
                return null;

            Char.SlotId = Free;
            Char.CharacterId = _Rm.GenerateCharacterId();
            Char.AccountId = _AccountId;

            CharacterMgr.Database.AddObject(Char);
            foreach (Character_items Itm in Char.Items)
                CharacterMgr.Database.AddObject(Itm);
            CharacterMgr.Database.AddObject(Char.Info[0]);

            AddCharacter(Char);

            return Char;
        }
        public byte[] BuildCharacters()
        {
            PacketOut Out = new PacketOut((byte)0);
            Out.Position = 0;
            Out.WriteUInt16(GetCharacterCount());

            Character Char = null;
            for (int i = 0; i < MAX_CHARS; ++i)
            {
                Char = Chars[i];

                if (Char == null)
                    Out.Write(new byte[280], 0, 280);
                else
                {
                    Out.FillString(Char.Name, 48);
                    Out.WriteByte(Char.Info[0].Level);
                    Out.WriteByte(Char.Career);
                    Out.WriteByte(Char.Realm);
                    Out.WriteByte(Char.Sex);
                    Out.WriteByte(Char.ModelId);
                    Out.WriteUInt16(Char.Info[0].ZoneId);
                    Out.Write(new byte[5], 0, 5);

                    Character_items Item = null;
                    for (UInt16 SlotId = 14; SlotId < 30; ++SlotId)
                    {
                        Item = Char.GetItemInSlot(SlotId);
                        if (Item == null)
                            Out.WriteUInt16(0);
                        else
                            Out.WriteUInt16Reverse(Item.ModelId);

                        Out.Write(new byte[6], 0, 6);
                    }

                    Out.Write(new byte[6], 0, 6);

                    for (int j = 0; j < 5; ++j)
                    {
                        Out.Write(new byte[6], 0, 6);
                        Out.WriteUInt16(0xFF00);
                    }

                    for (UInt16 SlotId = 10; SlotId < 13; ++SlotId)
                    {
                        Item = Char.GetItemInSlot(SlotId);
                        Out.WriteUInt16(0);
                        if (Item == null)
                            Out.WriteUInt16(0);
                        else
                            Out.WriteUInt16Reverse(Item.ModelId);
                    }

                    Out.Write(new byte[10], 0, 10);
                    Out.WriteUInt16(0xFF00);
                    Out.WriteByte(0);
                    Out.WriteByte(Char.Race);
                    Out.WriteUInt16(0);
                    Out.Write(Char.bTraits, 0, Char.bTraits.Length);
                    Out.Write(new byte[10], 0, 10);
                }
            }
            return Out.ToArray();
        }
    }
    public class RealmCharacters
    {
        static public int MAX_CHARACTERS = 65000;
        public byte RealmId;

        public int _MaxCharacterId = 1;
        public Character[] _CharId = new Character[MAX_CHARACTERS];
        public Dictionary<int, AccountCharacter> _CharAcctId = new Dictionary<int, AccountCharacter>();

        public RealmCharacters(byte RealmId)
        {
            this.RealmId = RealmId;
            Load();
        }
        public void Load()
        {
            Log.Debug("RealmCharacters", "Chargement des Personnages du Royaume :" + RealmId);

            Character[] Chars = CharacterMgr.Database.SelectObjects<Character>("RealmId='"+RealmId+"'").ToArray();

            lock(_CharId)
            {
                foreach (Character Char in Chars)
                {
                    _CharId[Char.CharacterId] = Char;
                    GetAccountChar(Char.AccountId).AddCharacter(Char);
                }
            }

            Log.Succes("RealmCharacter", "Chargement de " + Chars.Length + " characters sur le royaume : " + RealmId);
        }

        public int GenerateCharacterId()
        {
            return System.Threading.Interlocked.Increment(ref _MaxCharacterId);
        }
        public AccountCharacter GetAccountChar(int AccountId)
        {
            lock (_CharAcctId)
                if (!_CharAcctId.ContainsKey(AccountId))
                    _CharAcctId.Add(AccountId, new AccountCharacter(AccountId, this));

            return _CharAcctId[AccountId];
        }
        public Character GetCharacter(int CharacterId)
        {
            if (CharacterId >= 0 && CharacterId < _CharId.Length)
                return _CharId[CharacterId];
            else return null;
        }
        public AccountCharacter GetAccountCharacter(int AccountId)
        {
            if (_CharAcctId.ContainsKey(AccountId))
                return _CharAcctId[AccountId];
            return null;
        }
        public bool NameIsUsed(string Name)
        {
            foreach (Character Char in _CharId)
                if (Char.Name == Name)
                    return true;

            return false;
        }
    }


    [V3RpcAttributes(new string[] { "World", "CharacterCacher", "LobbyCharacter", "WorldCharacter" })]
    public class CharacterMgr : ARpc
    {
        static public MySQLObjectDatabase Database = null;

        // Gestion des personnages uniquements
        #region Characters

        static public long ItemGuid = 1;
        static public int CharacterId = 1;

        static public long GenerateItemGuid()
        {
            return System.Threading.Interlocked.Increment(ref ItemGuid);
        }
        static public long GenerateCharacterId()
        {
            return System.Threading.Interlocked.Increment(ref CharacterId);
        }

        static public Dictionary<byte, RealmCharacters> _CharRealms = new Dictionary<byte, RealmCharacters>();
        static public RealmCharacters GetRealmCharacter(byte Realm)
        {
            if (_CharRealms.ContainsKey(Realm))
                return _CharRealms[Realm];

            return null;
        }
        static public Character GetCharacter(byte RealmId, int CharacterId)
        {
            RealmCharacters Rm = GetRealmCharacter(RealmId);
            if (Rm != null) return Rm.GetCharacter(CharacterId);
            return null;
        }
        static public byte[] BuildCharacters(byte RealmId, int AccountId)
        {
            RealmCharacters Rm = GetRealmCharacter(RealmId);
            if (Rm != null)
                return Rm.GetAccountCharacter(AccountId).BuildCharacters();

            return new byte[2];
        }
        static public GameData.pRealm GetAccountRealm(byte RealmId,int AccountId)
        {
            RealmCharacters Rm = GetRealmCharacter(RealmId);
            if (Rm != null)
                return Rm.GetAccountCharacter(AccountId)._Realm;

            return GameData.pRealm.REALM_NONE;
        }
        static public bool NameIsUsed(byte RealmId, string Name)
        {
            RealmCharacters Rm = GetRealmCharacter(RealmId);
            if (Rm != null)
                return Rm.NameIsUsed(Name);

            return false;
        }
        static public Character CreateCharacter(byte RealmId, Character Char)
        {
            RealmCharacters Rm = GetRealmCharacter(RealmId);
            if (Rm != null)
                return Rm.GetAccountCharacter(Char.AccountId).CreateCharacter(Char);

            return null;
        }


        #endregion

        // Gestion des World
        #region Realms

        static public Dictionary<int, Realm> _Realms = new Dictionary<int, Realm>();

        public void LoadRealms()
        {
            try
            {
                Character_items Itm = Database.SelectObject<Character_items>("1=1 ORDER BY `Guid` DESC LIMIT 0, 1");
                if (Itm != null)
                    ItemGuid = ++Itm.Guid;
                Log.Succes("LoadRealm", "ItemGuidMax = " + ItemGuid);


                Realm[] Rms = Database.SelectAllObjects<Realm>().ToArray();

                foreach (Realm Rm in Rms)
                    AddRealm(Rm);
            }
            catch (Exception e)
            {

            }
        }

        public bool AddRealm(Realm Rm)
        {
            lock (_Realms)
            {
                if (_Realms.ContainsKey(Rm.RealmId))
                    return false;

                Log.Info("AddRealm", "Ajout du royaume : " + Rm.Name);
                _Realms.Add(Rm.RealmId, Rm);
                _CharRealms.Add(Rm.RealmId, new RealmCharacters(Rm.RealmId));
            }

            return true;
        }

        public Realm GetRealm(int RealmId)
        {
            try
            {
                lock (_Realms)
                    if (_Realms.ContainsKey(RealmId))
                        return _Realms[RealmId];

                Realm Rm = Database.SelectObject<Realm>("RealmId='" + RealmId + "'");

                if (Rm == null)
                    return null;

                lock (_Realms)
                    _Realms.Add(RealmId, Rm);

                return Rm;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public Realm GetRealmByRpc(int RpcId)
        {
            lock(_Realms)
                foreach (Realm Rm in _Realms.Values)
                {
                    if (Rm.RpcId == RpcId)
                        return Rm;
                }

            return null;
        }

        public bool UpdateRealm(int RpcId,int RealmId)
        {
            try
            {
                Realm ARm = GetRealm(RealmId);
                if (ARm != null)
                {
                    Log.Succes("Realm", "Royaume (" + RpcId + ") en ligne");
                    ARm.RpcId = RpcId;
                }
                else
                {
                    Log.Error("UpdateRealm", "Royaume (" + RealmId + ") Introuvable : veuillez remplir la table Realm");
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }

            return true;
        }

        public byte[] BuildRealms(uint sequence)
        {
            try
            {
                PacketOut Out = new PacketOut((byte)0);
                Out.Position = 0;

                Out.WriteUInt32(sequence);
                Out.WriteUInt16(0);
                lock (_Realms)
                {
                    Log.Info("BuildRealm", "Envoi de " + _Realms.Count + " royaumes");
                    Out.WriteUInt32((uint)_Realms.Count);

                    foreach (Realm Rm in _Realms.Values)
                    {
                        Out.WriteByte(Rm.RealmId);
                        Out.WriteByte((byte)(Rm.RpcId != 0 ? 1 : 0));
                        Out.WriteUInt32(1);
                        Out.WriteByte(Rm.RealmId);
                        Out.WriteByte(Rm.RealmId);
                        Out.WriteString("[" + Rm.Language + "] " + Rm.Name);
                        Out.WriteUInt32(19);
                        Out.WriteString("setting.allow_trials");
                        Out.WriteString(Rm.AllowTrials);
                        Out.WriteString("setting.charxferavailable");
                        Out.WriteString(Rm.CharfxerAvailable);
                        Out.WriteString("setting.language");
                        Out.WriteString(Rm.Language);
                        Out.WriteString("setting.legacy");
                        Out.WriteString(Rm.Legacy);
                        Out.WriteString("setting.manualbonus.realm.destruction");
                        Out.WriteString(Rm.BonusDestruction);
                        Out.WriteString("setting.manualbonus.realm.order");
                        Out.WriteString(Rm.BonusOrder);
                        Out.WriteString("setting.name");
                        Out.WriteString(Rm.Name);
                        Out.WriteString("setting.net.address");
                        Out.WriteString(Rm.Adresse);
                        Out.WriteString("setting.net.port");
                        Out.WriteString(Rm.Port.ToString());
                        Out.WriteString("setting.redirect");
                        Out.WriteString(Rm.Redirect);
                        Out.WriteString("setting.region");
                        Out.WriteString(Rm.Region);
                        Out.WriteString("setting.retired");
                        Out.WriteString(Rm.Retired);
                        Out.WriteString("status.queue.Destruction.waiting");
                        Out.WriteString(Rm.WaitingDestruction);
                        Out.WriteString("status.queue.Order.waiting");
                        Out.WriteString(Rm.WaitingOrder);
                        Out.WriteString("status.realm.destruction.density");
                        Out.WriteString(Rm.DensityDestruction);
                        Out.WriteString("status.realm.order.density");
                        Out.WriteString(Rm.DensityOrder);
                        Out.WriteString("status.servertype.openrvr");
                        Out.WriteString(Rm.OpenRvr);
                        Out.WriteString("status.servertype.rp");
                        Out.WriteString(Rm.Rp);
                        Out.WriteString("status.status");
                        Out.WriteString(Rm.Status);
                    }
                }
                Out.WriteUInt32(0);

                return Out.ToArray();
            }
            catch (Exception e)
            {
                return new byte[0];
            }
        }

        #endregion

        public override void Disconnected(int Id)
        {
            Realm Rm = GetRealmByRpc(Id);
            if (Rm != null)
            {
                Log.Error("Realm", "Déconnection du world : " + Rm.Name);
                Rm.RpcId = 0;
            }
        }

    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common;
using FrameWork;

namespace WorldServer
{
    public class AccountChars
    {
        public int AccountId;
        public GameData.Realms _Realm = GameData.Realms.REALMS_REALM_NEUTRAL;

        public AccountChars(int AccountId)
        {
            this.AccountId = AccountId;
        }

        public Character[] _Chars = new Character[CharMgr.MAX_SLOT];

        public byte GenerateFreeSlot()
        {
            for (byte i = 0; i < _Chars.Length; i++)
                if (_Chars[i] == null)
                    return i;

            return CharMgr.MAX_SLOT;
        }
        public bool AddChar(Character Char)
        {
            if (Char != null)
                _Realm = (GameData.Realms)Char.Realm;

            _Chars[Char.SlotId] = Char;

            return true;
        }
        public UInt32 RemoveCharacter(byte Slot)
        {
            UInt32 CharacterId = 0;
            if (_Chars[Slot] != null)
                CharacterId = _Chars[Slot].CharacterId;

            _Chars[Slot] = null;
            _Realm = GameData.Realms.REALMS_REALM_NEUTRAL;

            foreach(Character Char in _Chars)
                if (Char != null)
                {
                    _Realm = (GameData.Realms)Char.Realm;
                    break;
                }

            return CharacterId;
        }
        public Character GetCharacterBySlot(byte Slot)
        {
            if (Slot < 0 || Slot > _Chars.Length)
                return null;

            return _Chars[Slot];
        }
    };

    static public class CharMgr
    {
        static public MySQLObjectDatabase Database = null;

        #region CharacterInfo

        static public Dictionary<byte, CharacterInfo> _Infos = new Dictionary<byte, CharacterInfo>();
        static public Dictionary<byte, List<CharacterInfo_item>> _InfoItems = new Dictionary<byte, List<CharacterInfo_item>>();
        static public Dictionary<byte, List<CharacterInfo_stats>> _InfoStats = new Dictionary<byte, List<CharacterInfo_stats>>();
        static public List<Random_name> _RandomNames = new List<Random_name>();

        [LoadingFunction(true)]
        static public void LoadCharacterInfo()
        {
            IList<CharacterInfo> Chars = WorldMgr.Database.SelectAllObjects<CharacterInfo>();
            foreach (CharacterInfo Info in Chars)
                if (!_Infos.ContainsKey(Info.Career))
                    _Infos.Add(Info.Career, Info);

            _RandomNames = WorldMgr.Database.SelectAllObjects<Random_name>() as List<Random_name>;

            Log.Success("CharacterMgr", "Loaded " + Chars.Count + " CharacterInfo");
        }

        [LoadingFunction(true)]
        static public void LoadCharacterInfoItems()
        {
            IList<CharacterInfo_item> Chars = WorldMgr.Database.SelectAllObjects<CharacterInfo_item>();
            
            if(Chars != null)
            foreach (CharacterInfo_item Info in Chars)
                if (!_InfoItems.ContainsKey(Info.CareerLine))
                {
                    List<CharacterInfo_item> Items = new List<CharacterInfo_item>(1);
                    Items.Add(Info);
                    _InfoItems.Add(Info.CareerLine, Items);
                }
                else _InfoItems[Info.CareerLine].Add(Info);

            Log.Success("CharacterMgr", "Loaded " + _InfoItems.Count + " CharacterInfo_Item");
        }

        [LoadingFunction(true)]
        static public void LoadCharacterInfoStats()
        {
            IList<CharacterInfo_stats> Chars = WorldMgr.Database.SelectAllObjects<CharacterInfo_stats>();
            foreach (CharacterInfo_stats Info in Chars)
            {
                if (!_InfoStats.ContainsKey(Info.CareerLine))
                {
                    List<CharacterInfo_stats> Stats = new List<CharacterInfo_stats>(1);
                    Stats.Add(Info);
                    _InfoStats.Add(Info.CareerLine, Stats);
                }
                else _InfoStats[Info.CareerLine].Add(Info);
            }

            int i, StatId;
            CharacterInfo_stats Previous, Current, PrevPrevious;
            foreach (KeyValuePair<byte, List<CharacterInfo_stats>> Stats in _InfoStats)
            {
                for (StatId = 0; StatId < (int)GameData.Stats.STATS_COUNT; ++StatId)
                {
                    for (i = 2; i <= 40; ++i)
                    {
                        Previous = Stats.Value.Find(info => info.Level == (byte)(i - 1) && info.StatId == StatId);
                        if (Previous == null)
                            continue;

                        Current = Stats.Value.Find(info => info.Level == (byte)(i) && info.StatId == StatId);
                        if (Current == null)
                            continue;

                        if (Current.StatValue <= Previous.StatValue || Math.Abs(Current.StatValue - Previous.StatValue) > 40)
                        {
                            PrevPrevious = Stats.Value.Find(info => info.Level == (byte)(i - 2) && info.StatId == StatId);
                            if (PrevPrevious != null)
                                Current.StatValue = (ushort)((Previous.StatValue - PrevPrevious.StatValue) + Previous.StatValue + 1);

                            if (Current.StatValue <= Previous.StatValue)
                                Current.StatValue = (ushort)(Previous.StatValue + 3);
                        }
                    }
                }
            }

            Log.Success("CharacterMgr", "Loaded " + Chars.Count + " CharacterInfo_Stats");
        }

        static public CharacterInfo GetCharacterInfo(byte Career)
        {
            lock (_Infos)
                if (_Infos.ContainsKey(Career))
                    return _Infos[Career];

            return null;
        }

        static public Dictionary<ushort, List<CharacterInfo_stats>> _CareerLevelStats = new Dictionary<ushort, List<CharacterInfo_stats>>();

        static public List<CharacterInfo_stats> GetCharacterInfoStats(byte CareerLine, byte Level)
        {
            List<CharacterInfo_stats> Stats = new List<CharacterInfo_stats>();
            if (!_CareerLevelStats.TryGetValue((ushort)((int)CareerLine << 8 + (int)Level), out Stats))
            {
                Stats = new List<CharacterInfo_stats>();

                List<CharacterInfo_stats> InfoStats;
                if (_InfoStats.TryGetValue(CareerLine, out InfoStats))
                {
                    foreach(CharacterInfo_stats Stat in InfoStats)
                        if (Stat.CareerLine == CareerLine && Stat.Level == Level)
                            Stats.Add(Stat);
                }

                _CareerLevelStats.Add((ushort)((int)CareerLine << 8 + (int)Level), Stats);
            }
            return Stats;
        }
        static public List<CharacterInfo_item> GetCharacterInfoItem(byte CareerLine)
        {
            List<CharacterInfo_item> Items;
            if(!_InfoItems.TryGetValue(CareerLine,out Items))
            {
                Items = new List<CharacterInfo_item>();
                _InfoItems.Add(CareerLine,Items);
            }
            return Items;
        }

        static public List<Random_name> GetRandomNames()
        {
            return _RandomNames;
        }


        #endregion

        #region Characters

        static public byte MAX_SLOT = 20;
        static public int MAX_CHARACTERS = 1000000;
        static public int MAX_CHAR_GUID = 1;
        static public Character[] _Chars = new Character[MAX_CHARACTERS];
        static public Dictionary<int, AccountChars> _AcctChars = new Dictionary<int, AccountChars>();

        [LoadingFunction(true)]
        static public void LoadCharacters()
        {
            List<Character> Chars = Database.SelectAllObjects<Character>() as List<Character>;
            List<Character_value> Values = Database.SelectAllObjects<Character_value>() as List<Character_value>;
            List<Character_social> Socials = Database.SelectAllObjects<Character_social>() as List<Character_social>;
            List<Character_tok> Toks = Database.SelectAllObjects<Character_tok>() as List<Character_tok>;
            List<Character_quest> Quests = Database.SelectAllObjects<Character_quest>() as List<Character_quest>;
            List<Characters_influence> Influences = Database.SelectAllObjects<Characters_influence>() as List<Characters_influence>;

            int Count = 0;
            foreach (Character Char in Chars)
            {
                Char.Value = Values.Find(info => info.CharacterId == Char.CharacterId);
                Char.Socials = Socials.FindAll(info => info.CharacterId == Char.CharacterId);
                Char.Toks = Toks.FindAll(info => info.CharacterId == Char.CharacterId);
                Char.Quests = Quests.FindAll(info => info.CharacterId == Char.CharacterId);
                Char.Influences = Influences.FindAll(info => info.CharacterId == Char.CharacterId);

                AddChar(Char);
                ++Count;
            }

            Log.Success("LoadCharacters", Count + "  : Character(s) loaded");
        }

        static public Character LoadCharacter(string Name)
         {
            Character Char = Database.SelectObject<Character>("Name='" + Database.Escape(Name) + "'");
            if (Char != null)
            {
                Char.Value = Database.SelectObject<Character_value>("CharacterId=" + Char.CharacterId);
                Char.Socials = Database.SelectObjects<Character_social>("CharacterId=" + Char.CharacterId) as List<Character_social>;
                Char.Toks = Database.SelectObjects<Character_tok>("CharacterId=" + Char.CharacterId) as List<Character_tok>;
                Char.Quests = Database.SelectObjects<Character_quest>("CharacterId=" + Char.CharacterId) as List<Character_quest>;
                Char.Influences = Database.SelectObjects<Characters_influence>("CharacterId=" + Char.CharacterId) as List<Characters_influence>;
                AddChar(Char);
            }

            return Char;
        }

        static public UInt32 GenerateMaxCharId()
        {
            return (UInt32)System.Threading.Interlocked.Increment(ref MAX_CHAR_GUID);
        }
        static public bool CreateChar(Character Char)
        {
            AccountChars Chars = GetAccountChar(Char.AccountId);
            Char.SlotId = Chars.GenerateFreeSlot();
            if (Char.SlotId < 0)
                return false;

            lock (_Chars)
            {
                UInt32 Id = GenerateMaxCharId();

                while (_Chars[Id] != null)
                    Id = GenerateMaxCharId();

                if (Id >= MAX_CHARACTERS || Id <= 0)
                {
                    Log.Error("CreateChar", "Maximum number of characters reaches !");
                    return false;
                }

                Char.CharacterId = Id;
                _Chars[Id] = Char;
                Chars.AddChar(Char);
            }
           Database.AddObject(Char);

            return true;
        }
        static public bool DeleteChar(Character Char)
        {
            Database.DeleteObject(Char);
            Database.DeleteObject(Char.Value);

            if(Char.Socials != null)
                foreach (Character_social Obj in Char.Socials)
                    Database.DeleteObject(Obj);

            if(Char.Toks != null)
                foreach (Character_tok Obj in Char.Toks)
                    Database.DeleteObject(Obj);

            if(Char.Quests != null)
                foreach (Character_quest Obj in Char.Quests)
                    Database.DeleteObject(Obj);

            if(Char.Influences != null)
                foreach (Characters_influence Obj in Char.Influences)
                    Database.DeleteObject(Obj);

            return true;

        }
        static public void AddChar(Character Char)
        {
            lock (_Chars)
            {
                _Chars[Char.CharacterId] = Char;
                GetAccountChar(Char.AccountId).AddChar(Char);

                if (Char.CharacterId > MAX_CHAR_GUID)
                    MAX_CHAR_GUID = (int)Char.CharacterId;
            }
        }
        static public Character[] GetAccountCharacters(int AccountId)
        {
            return GetAccountChar(AccountId)._Chars;
        }
        static public AccountChars GetAccountChar(int AccountId)
        {
            lock (_AcctChars)
            {
                if (!_AcctChars.ContainsKey(AccountId))
                    _AcctChars[AccountId] = new AccountChars(AccountId);

                return _AcctChars[AccountId];
            }
        }
        static public bool NameIsUsed(string Name)
        {
            for (int i = 0; i < _Chars.Length; ++i)
            {
                if (_Chars[i] != null)
                {
                    if (_Chars[i].Name.ToLower() == Name.ToLower())
                        return true;
                }
            }

            return false;
        }

        public static byte[] BuildCharacters(int AccountId)
        {
            Log.Debug("BuildCharacters", "AcocuntId = " + AccountId);

            Character[] Chars = GetAccountChar(AccountId)._Chars;
            UInt16 Count = 0;

            // On Compte le nombre de personnages existant du joueur
            for (UInt16 c = 0; c < Chars.Length; ++c)
                if (Chars[c] != null) ++Count;

            PacketOut Out = new PacketOut(0);
            Out.Position = 0;

            Out.WriteByte(MAX_SLOT);
            Out.WriteUInt32(0xFF);
            Out.WriteByte(0x14);

            Character Char = null;
            for (int i = 0; i < MAX_SLOT; ++i)
            {
                Char = Chars[i];

                if (Char == null)
                    Out.Fill(0, 284); // 284
                else
                {
                    List<Character_item> Items = CharMgr.GetItemChar(Char.CharacterId);

                    Out.FillString(Char.Name, 48);
                    Out.WriteByte(Char.Value.Level);
                    Out.WriteByte(Char.Career);
                    Out.WriteByte(Char.Realm);
                    Out.WriteByte(Char.Sex);
                    Out.WriteByte(Char.ModelId);
                    Out.WriteUInt16(Char.Value.ZoneId);
                    Out.Fill(0, 5);

                    Character_item Item = null;
                    for (UInt16 SlotId = 14; SlotId < 30; ++SlotId)
                    {
                        Item = Items.Find(item => item != null && item.SlotId == SlotId);
                        if (Item == null)
                            Out.WriteUInt32(0);
                        else
                            Out.WriteUInt32R(Item.ModelId);

                        Out.Fill(0, 4);
                    }

                    Out.Fill(0, 6);

                    for (int j = 0; j < 5; ++j)
                    {
                        Out.Fill(0, 6);
                        Out.WriteUInt16(0xFF00);
                    }

                    for (UInt16 SlotId = 10; SlotId < 13; ++SlotId)
                    {
                        Item = Items.Find(item => item != null && item.SlotId == SlotId);
                        Out.WriteUInt16(0);
                        if (Item == null)
                            Out.WriteUInt16(0);
                        else
                            Out.WriteUInt16R((ushort)Item.ModelId);
                    }


                    Out.Fill(0, 10);
                    Out.WriteUInt16(0xFF00);
                    Out.WriteByte(0);
                    Out.WriteByte(Char.Race);
                    Out.WriteUInt16(0);
                    Out.Write(Char.bTraits, 0, Char.bTraits.Length);
                    Out.Fill(0, 14);// 272
                }
            }
            return Out.ToArray();
        }

        static public GameData.Realms GetAccountRealm(int AccountId)
        {
            return GetAccountChar(AccountId)._Realm;
        }
        static public void RemoveCharacter(byte Slot, int AccountId)
        {
            UInt32 CharacterId = GetAccountChar(AccountId).RemoveCharacter(Slot);

            lock(_Chars)
                if (CharacterId > 0 && _Chars[CharacterId] != null)
                {
                    Log.Debug("RemoveCharacter", "Slot=" + Slot + ",Acct=" + AccountId + ",CharId=" + CharacterId);

                    Character Char = _Chars[CharacterId];
                    _Chars[CharacterId] = null;
                    RemoveItemsChar(CharacterId);
                    DeleteChar(Char);
                    
                    Program.AcctMgr.UpdateRealmCharacters(Program.Rm.RealmId, (uint)CharMgr.Database.GetObjectCount<Character>(" Realm=1"), (uint)CharMgr.Database.GetObjectCount<Character>(" Realm=2"));
                }
        }
        static public Character GetCharacter(string Name)
        {
            Name = Name.ToLower();

            lock(_Chars)
                for (int i = 0; i < _Chars.Length; ++i)
                    if (_Chars[i] != null && _Chars[i].Name.ToLower() == Name)
                        return _Chars[i];

            return null;
        }

        #endregion

        #region CharacterItems

        static public long MAX_ITEMS = 600000;
        static public Character_item[] _Items;
        static public Dictionary<UInt32, List<Character_item>> _CharItems = new Dictionary<UInt32, List<Character_item>>();

        [LoadingFunction(true)]
        static public void LoadItems()
        {
            _Items = new Character_item[MAX_ITEMS];
            IList<Character_item> Items = Database.SelectAllObjects<Character_item>();

            if(Items != null)
                lock (_Items)
                    foreach (Character_item Itm in Items)
                        LoadItem(Itm);

            Log.Success("LoadItems",Items.Count + " : Characters items loaded");
        }
        static public void LoadItem(Character_item CharItem)
        {
            lock (_Items)
            {
                _Items[CharItem.Guid] = CharItem;

                if (!_CharItems.ContainsKey(CharItem.CharacterId))
                    _CharItems.Add(CharItem.CharacterId, new List<Character_item>() { CharItem });
                else
                    _CharItems[CharItem.CharacterId].Add(CharItem);
            }
        }
        static public void DeleteItem(Character_item Itm)
        {
            //Log.Info("DeleteItem", "Guid=" + Itm.Guid + ",CharId=" + Itm.CharacterId);

            lock (_Items)
            {
                if (_CharItems.ContainsKey(Itm.CharacterId))
                    _CharItems[Itm.CharacterId].Remove(Itm);

                _Items[Itm.Guid] = null;
            }

            CharMgr.Database.DeleteObject(Itm);
        }

        static public Character_item GetItem(UInt32 Guid)
        {
            lock (_Items)
            {
                return _Items[Guid];
            }
        }

        static public List<Character_item> GetItemChar(UInt32 CharacterId)
        {
            lock (_Items)
           {
                if (_CharItems.ContainsKey(CharacterId))
                    return _CharItems[CharacterId];
                else
                    return new List<Character_item>();
            }
        }

        static public bool CreateItem(Character_item Item)
        {
            lock (_Items)
            {
                for (int i = 0; i < _Items.Length; ++i)
                {
                    if (_Items[i] == null)
                    {
                        Item.Guid = i;
                        LoadItem(Item);
                        Database.AddObject(Item);
                        return true;
                    }
                }
            }

            Log.Error("CreateItem", "Maximum number of items reaches !");
            return false;
        }
        static public void RemoveItemsChar(UInt32 CharacterId)
        {
            lock (_Items)
            {
                for (int i = 0; i < _Items.Length; ++i)
                    if (_Items[i] != null && _Items[i].CharacterId == CharacterId)
                    {
                        CharMgr.Database.DeleteObject(_Items[i]);
                        _Items[i] = null;
                    }

                _CharItems.Remove(CharacterId);
            }
        }
        static public void SaveItems(UInt32 CharacterId, List<Item> CItems)
        {
            List<Character_item> CItem = new List<Character_item>();
            for (int i = 0; i < CItems.Count; ++i)
                if(CItems[i] != null)
                    CItem.Add(CItems[i].Save(CharacterId));

            lock (_Items)
            {
                _CharItems.Remove(CharacterId);
                _CharItems.Add(CharacterId, CItem);
            }
        }

        #endregion

        #region CharacterMail
        static public int MAX_MAIL_GUID = 1;

        static public int GenerateMailGUID()
        {
            return System.Threading.Interlocked.Increment(ref MAX_MAIL_GUID);
        }
        [LoadingFunction(true)]
        static public void LoadMails()
        {
            Log.Debug("WorldMgr", "Loading Character_mails...");

            IList<Character_mail> Mails = Database.SelectAllObjects<Character_mail>();
            int count = 0;
            if (Mails != null)
                foreach (Character_mail Mail in Mails)
                {
                    if (Mail.Guid > MAX_MAIL_GUID)
                        MAX_MAIL_GUID = Mail.Guid;
                    count++;
                }


            Log.Success("LoadMails", "Loaded " + count + " Character_mails");
        }
        static public IList<Character_mail> GetCharMail(UInt32 characterId)
        {
            IList<Character_mail> Mails = Database.SelectObjects<Character_mail>(string.Format("CharacterId = {0}", characterId));

            return Mails;
        }
        static public void SaveMail(Character_mail mailItem)
        {
            Database.SaveObject(mailItem);
        }
        static public void SaveMail(IList<Character_mail> mailItems)
        {
            foreach (var item in mailItems) { Database.SaveObject(item); }
        }
        #endregion
    }
}

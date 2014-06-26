/*
 * Copyright (C) 2014 WarEmu
 *	http://WarEmu.com
 * 
 * Copyright (C) 2011-2013 APS
 *	http://AllPrivateServer.com
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.IO;

using Common;
using FrameWork;

namespace WorldServer
{
    public class Player : Unit
    {
        #region Statics

        static public int DISCONNECT_TIME = 20000;

        static public List<Player> _Players = new List<Player>();
        static public uint OrderCount = 0;
        static public uint DestruCount = 0;

        static public void AddPlayer(Player Plr)
        {
            lock (_Players)
            {
                if (!_Players.Contains(Plr))
                {
                    _Players.Add(Plr);
                    if (Plr.Realm == GameData.Realms.REALMS_REALM_ORDER)
                        ++OrderCount;
                    else
                        ++DestruCount;

                    Program.Rm.OnlinePlayers = (uint)_Players.Count;
                    Program.AcctMgr.UpdateRealm(Program.Rm.RealmId, Program.Rm.OnlinePlayers, OrderCount, DestruCount);

                    CharMgr.Database.ExecuteNonQuery("UPDATE characters_value SET Online=1 WHERE CharacterId=" + Plr._Info.CharacterId + ";");
                    Plr._Value.Online = true;
                }
            }
        }
        static public void RemovePlayer(Player Plr)
        {
            lock (_Players)
            {
                if (_Players.Remove(Plr))
                {
                    if (Plr._Info.Realm == (byte)GameData.Realms.REALMS_REALM_ORDER)
                        --OrderCount;
                    else
                        --DestruCount;

                    CharMgr.Database.ExecuteNonQuery("UPDATE characters_value SET Online=0 WHERE CharacterId=" + Plr._Info.CharacterId + ";");
                    Plr._Value.Online = false;
                    Program.Rm.OnlinePlayers = (uint)_Players.Count;
                    Program.AcctMgr.UpdateRealm(Program.Rm.RealmId, Program.Rm.OnlinePlayers, OrderCount, DestruCount);
                }
            }
        }
        static public Player GetPlayer(string Name)
        {
            Name = Name.ToLower();

            lock (_Players)
                return _Players.Find(plr => plr.Name.ToLower() == Name);
        }
        static public Player GetPlayer(UInt32 CharacterId)
        {
            lock (_Players)
                return _Players.Find(plr => plr.CharacterId == CharacterId);
        }
        static public Player CreatePlayer(GameClient Client, Character Char)
        {
            GameClient Other = (Client.Server as TCPServer).GetClientByAccount(Client,Char.AccountId);
            if (Other != null)
                Other.Disconnect();

            Player Plr = new Player(Client, Char);

            return Plr;
        }
        static public List<Player> GetPlayers(string Name, string GuildName, UInt16 Career, UInt16 ZoneId, byte MinLevel, byte MaxLevel)
        {
            List<Player> Plrs = new List<Player>();
            lock (_Players)
            {
                Name = Name.ToLower();
                GuildName = GuildName.ToLower();

                foreach (Player Plr in _Players)
                {
                    if (Plr == null || Plr.IsDisposed || !Plr.IsInWorld())
                        continue;

                    if ( Plr.SocInterface.Hide
                        || (Name.Length > 0 && !Plr.Name.ToLower().StartsWith(Name))
                        || (Career != 0 && Career != Plr._Info.Career)
                        || (ZoneId != 255 && Plr.Zone.ZoneId != ZoneId)
                        || (Plr.Level < MinLevel)
                        || (Plr.Level > MaxLevel)
                        )
                        continue;

                    Plrs.Add(Plr);
                }
            }

            return Plrs;
        }
        static public void Stop()
        {
            foreach (Player Plr in _Players)
                Plr.Quit();
        }

        #endregion

        public Character _Info;
        public Character_value _Value;
        public GameClient _Client;
        public GameClient Client
        {
            get { return _Client; }
        }

        public SocialInterface SocInterface;
        public TokInterface TokInterface;
        public MailInterface MlInterface;

        public UInt32 CharacterId
        {
            get
            {
                if (_Info != null)
                    return _Info.CharacterId;
                else
                    return 0;
            }
        }
        public int GmLevel
        {
            get
            {
                if (Client != null)
                    return Client._Account.GmLevel;
                else
                    return 0;
            }
        }
        public bool _Inited = false;

        public Player(GameClient Client,Character Info) : base()
        {
            _Client = Client;
            _Info = Info;
            _Value = Info.Value;

            Name = Info.Name;
            Realm = (GameData.Realms)Info.Realm;
            SetPVPFlag(false);

            EvtInterface = AddInterface(EventInterface.GetEventInterface((uint)_Info.CharacterId)) as EventInterface;
            SocInterface = AddInterface<SocialInterface>();
            TokInterface = AddInterface<TokInterface>();
            MlInterface = AddInterface<MailInterface>();
            
            EvtInterface.AddEventNotify(EventName.ON_MOVE, CancelQuit);
            EvtInterface.AddEventNotify(EventName.ON_RECEIVE_DAMAGE, CancelQuit);
            EvtInterface.AddEventNotify(EventName.ON_DEAL_DAMAGE, CancelQuit);
            EvtInterface.AddEventNotify(EventName.ON_START_CASTING, CancelQuit);
        }

        ~Player()
        {

        }

        public override void OnLoad()
        {
            _Client.State = (int)eClientState.WorldEnter;

            if (!_Inited)
            {
                EvtInterface._Owner = this;
                EvtInterface.AddEventNotify(EventName.PLAYING, Save);
                EvtInterface.Start();

                ItmInterface.Load(CharMgr.GetItemChar(_Info.CharacterId));
                StsInterface.Load(CharMgr.GetCharacterInfoStats(_Info.CareerLine, _Value.Level));
                QtsInterface.Load(this._Info.Quests);
                TokInterface.Load(_Info.Toks);
                SocInterface.Load(_Info.Socials);
                MlInterface.Load(CharMgr.GetCharMail(_Info.CharacterId));
                AbtInterface.Load();
                StsInterface.ApplyStats();

                SetLevel(_Value.Level);
                SetRenownLevel(_Value.RenownRank);
                SetOffset((UInt16)(_Value.WorldX >> 12), (UInt16)(_Value.WorldY >> 12));
                _Inited = true;
            }

            base.OnLoad();

            StartInit();
        }
        public override void Dispose()
        {
            SendLeave();
            StopQuit();

            EvtInterface.Notify(EventName.LEAVE, this, null);
            RemovePlayer(this);
            Save();
            base.Dispose();

            if (_Client != null)
            {
                _Client.Plr = null;
                _Client.State = (int)eClientState.CharScreen;
            }    
        }

        public void SendSniff(string Str)
        {
            string Result = "";
            using (StringReader Reader = new StringReader(Str))
            {
                string Line;
                while ((Line = Reader.ReadLine()) != null)
                {
                    Result+=Line.Substring(1, 48).Replace(" ", string.Empty);
                }
            }

            Result = Result.Remove(0, 4);
            byte Opcode = Convert.ToByte(Result.Substring(0, 2), 16);
            Result = Result.Remove(0, 2);

            PacketOut Out = new PacketOut(Opcode);
            Out.WriteHexStringBytes(Result);
            Out.WritePacketLength();
            SendPacket(Out);
        }

        public void StartInit()
        {
            RemovePlayer(this);
            Client.State = (int)eClientState.WorldEnter;
            SendMoney();
            SendStats();
            SendSpeed(Speed);
            SendInited();
            SendRankUpdate(this);
            SendXpTable();
            WorldMgr.GeneralScripts.OnWorldPlayerEvent("SEND_PACKAGES", this, null);
            SendXp();
            SendRenown();
            TokInterface.SendAllToks();
            SendSkills();

            /*{
                PacketOut Out = new PacketOut((byte)Opcodes.F_INFLUENCE_INFO);
                Out.WriteHexStringBytes("00000000");
                SendPacket(Out);
            }
            {
                PacketOut Out = new PacketOut((byte)Opcodes.F_PLAY_TIME_STATS);
                Out.WriteHexStringBytes("000000000000000000000000");
                SendPacket(Out);
            }

            {
                PacketOut Out = new PacketOut((byte)Opcodes.F_TACTICS);
                Out.WriteHexStringBytes("0300");
                SendPacket(Out);
            }

            {
                PacketOut Out = new PacketOut((byte)Opcodes.F_MORALE_LIST);
                Out.WriteHexStringBytes("00 00 00 00 00 00 00 00 00 00 00".Replace(" ", string.Empty));
                SendPacket(Out);
            }*/

            Health = TotalHealth;
            ItmInterface.SendAllItems(this);
            AbtInterface.SendAbilities();

            QtsInterface.SendQuests();
            MvtInterface.CurrentMount.SendMount(this);

            SendInitComplete();
            SocInterface.SendFriends();

            if (GetGroup() != null)
                GetGroup().Update();

            SendMessage(0, "MOTD: Welcome to WarEmu", "", SystemData.ChatLogFilters.CHATLOGFILTERS_CITY_ANNOUNCE);

            MlInterface.SendMailCounts();
        }

        public long Next = 0;
        public uint Id = 0;
        public override void Update(long Tick)
        {
            if (Client == null)
            {
                Dispose();
                return;
            }

            if (Invitation != null)
                if (Invitation.Expire <= TCPManager.GetTimeStamp())
                    Invitation.DeclineInvitation();

            base.Update(Tick);
            UpdatePackets();
        }

        public void SetPVPFlag(bool State)
        {
            if(State == false)
                Faction = (byte)(Realm == GameData.Realms.REALMS_REALM_DESTRUCTION ? 8 : 6);
            else
                Faction = (byte)(Realm == GameData.Realms.REALMS_REALM_DESTRUCTION ? 72 : 68);

            if (_IsActive && IsInWorld() && _Loaded)
            {
                foreach (Player Plr in _PlayerRanged)
                {
                    if (Plr.HasInRange(this))
                        SendMeTo(Plr);
                }
            }
        }

        #region Packets

        public List<PacketIn> _PacketIn = new List<PacketIn>(20);
        public List<PacketOut> _PacketOut = new List<PacketOut>(20);

        public List<PacketIn> _InternalIn = new List<PacketIn>(20);
        public List<PacketOut> _InternalOut = new List<PacketOut>(20);

        public void ReceivePacket(PacketIn Packet)
        {
            if (_Client == null)
                return;

            if (IsInWorld())
                lock (_PacketIn)
                    _PacketIn.Add(Packet);
            else
                _Client.Server.HandlePacket(_Client, Packet);
        }
        public void SendPacket(PacketOut Out)
        {
            if (_Client == null)
                return;

            if (IsInWorld())
                lock (_PacketOut)
                    _PacketOut.Add(Out);
            else
                _Client.SendPacket(Out);
        }
        public void SendCopy(PacketOut Out)
        {
            Out.WritePacketLength();
            byte[] Buf = Out.ToArray();
            PacketOut packet = new PacketOut(0);
            packet.Position = 0;
            packet.Write(Buf, 0, Buf.Length);
            SendPacket(packet);
            
        }
        public void GetPacketIn(bool Clear)
        {
            _InternalIn.Clear();

            lock (_PacketIn)
            {
                _InternalIn.AddRange(_PacketIn);
                if (Clear) _PacketIn.Clear();
            }
        }
        public void GetPacketOut(bool Clear)
        {
            _InternalOut.Clear();

            lock (_PacketOut)
            {
                _InternalOut.AddRange(_PacketOut.ToArray());
                if (Clear) _PacketOut.Clear();
            }
        }
        public void UpdatePackets()
        {
            if (_Client == null)
                return;

            GetPacketIn(true);
            int i;
            for (i = 0; i < _InternalIn.Count; ++i)
                _Client.Server.HandlePacket(_Client, _InternalIn[i]);

            GetPacketOut(true);
            for (i = 0; i < _InternalOut.Count; ++i)
                if (_InternalOut[i] != null)
                    _Client.SendPacket(_InternalOut[i]);
        }

        #endregion

        #region Money

        public bool HaveMoney(uint Money) { return _Value.Money >= Money; }
        public bool RemoveMoney(uint Money)
        {
            if (!HaveMoney(Money))
                return false;

            _Value.Money -= Money;
            SendMoney();
            return true;
        }
        public uint GetMoney() { return _Value.Money; }
        public void AddMoney(uint Money)
        {
            _Value.Money += Money;
            SendMoney();
        }

        #endregion

        #region Stats

        public void SendStats()
        {
            PacketOut Out = new PacketOut((byte)Opcodes.F_PLAYER_STATS);
            ItmInterface.BuildStats(ref Out);
            StsInterface.BuildStats(ref Out);
            SendPacket(Out);
        }

        public override void SetDeath(Unit Killer)
        {
            UInt16 Time = 600; // Time to AutoResurrect in Seconds. 10 Minutes in Official Servers

            Killer.QtsInterface.HandleEvent(Objective_Type.QUEST_KILL_PLAYERS, 0, 1);

            base.SetDeath(Killer);

            if(Killer.IsPlayer())
                WorldMgr.GenerateRenown(Killer.GetPlayer(), this);

            PacketOut Out = new PacketOut((byte)Opcodes.F_PLAYER_DEATH);
            Out.WriteUInt16(Oid);
            Out.WriteUInt16(Time);
            SendPacket(Out);

            EvtInterface.AddEvent(AutomaticRespawnPlayer, Time * 1000, 1); // If the player don't resurrect. autoresurrect in 10 Minutes.
        }

        public void AutomaticRespawnPlayer()
        {
            RespawnPlayer();
        }
	
        public void PreRespawnPlayer()
        {
            // Remove automatic respawn function
            EvtInterface.RemoveEvent(AutomaticRespawnPlayer);

            if (!EvtInterface.HasEvent(RespawnPlayer))
            {
                SendDialog((ushort)5, (ushort)5);
                EvtInterface.AddEvent(RespawnPlayer, 5000, 1);
            }
        }

        public void RespawnPlayer()
        {
            if (!GetPlayer().IsDead)
                return;

            EvtInterface.RemoveEvent(AutomaticRespawnPlayer);
            EvtInterface.RemoveEvent(RespawnPlayer);

            Zone_Respawn Respawn = WorldMgr.GetZoneRespawn(Zone.ZoneId, (byte)Realm, this);
            if (Respawn != null)
                SafePinTeleport(Respawn.PinX, Respawn.PinY, Respawn.PinZ, Respawn.WorldO);

            RezUnit();
        }

        public override void RezUnit()
        {
            EvtInterface.RemoveEvent(RespawnPlayer);
            PacketOut Out = new PacketOut((byte)Opcodes.F_PLAYER_CLEAR_DEATH);
            Out.WriteUInt16(Oid);
            Out.WriteUInt16(0);
            DispatchPacket(Out,true);

            base.RezUnit();

            foreach (Player Plr in _PlayerRanged)
                SendMeTo(Plr);
        }

        #endregion

        #region Xp

        private Xp_Info CurrentXp = null;
        public void SetLevel(byte NewLevel)
        {
            CurrentXp = WorldMgr.GetXp_Info(NewLevel);
            _Value.Level = NewLevel;
            Dictionary<byte, UInt16> Values = ApplyLevel();

            if (_Loaded && _Inited)
            {
                SendLevelUp(Values);
                SendXp();
            }

            ApplyLevel();
        }
        public void AddXp(uint Xp)
        {
            if (CurrentXp == null || _Value.XpMode == 1)
                return;

            if (Xp + _Value.Xp > CurrentXp.Xp)
                LevelUp((uint)(Xp + _Value.Xp - CurrentXp.Xp));
            else
            {
                _Value.Xp += Xp;
                SendXp();
            }
        }
        public void RemoveXp(uint Xp)
        {

        }
        public void LevelUp(uint RestXp)
        {
            _Value.Xp = 0;
            SetLevel((byte)(Level + 1));
            EvtInterface.Notify(EventName.ON_LEVEL_UP, this, null);
            if (CurrentXp == null)
                return;

            AddXp(RestXp);
        }
        public Dictionary<byte,UInt16> ApplyLevel()
        {
            Dictionary<byte, UInt16> Diff = new Dictionary<byte, ushort>();

            //List<CharacterInfo_stats> NewStats = CharMgr.GetCharacterInfoStats(_Info.CareerLine, _Value.Level);
            CharacterInfo_stats[] NewStats = CharMgr.GetCharacterInfoStats(_Info.CareerLine, _Value.Level);
            if (NewStats == null || NewStats.Length <= 0)
                return Diff;

            foreach (CharacterInfo_stats Stat in NewStats)
            {
                UInt16 Base = StsInterface.GetBaseStat(Stat.StatId);
                
                if(Stat.StatValue > Base)
                    Diff.Add(Stat.StatId,(ushort)(Stat.StatValue-Base));

                StsInterface.SetBaseStat(Stat.StatId, Stat.StatValue);
            }

            StsInterface.ApplyStats();
            return Diff;
        }

        #endregion

        #region Renown

        public Renown_Info CurrentRenown;
        public void SetRenownLevel(byte Level)
        {
            CurrentRenown = WorldMgr.GetRenown_Info(Level);
            _Value.RenownRank = Level;

            if (_Loaded && _Inited)
                SendRenown();
        }
        public void AddRenown(uint Renown)
        {
            if (_Value.Renown + Renown > CurrentRenown.Renown)
                RenownUp(_Value.Renown + Renown - CurrentRenown.Renown);
            else
            {
                _Value.Renown += Renown;
                SendRenown();
            }
        }
        public void RenownUp(uint Rest)
        {
            CurrentRenown = WorldMgr.GetRenown_Info((byte)(_Value.RenownRank+1));
            if (CurrentRenown == null)
                return;

            SetRenownLevel((byte)(_Value.RenownRank + 1));
            AddRenown(Rest);
        }

        #endregion

        #region Senders

        public void SendSpeed(UInt16 NewSpeed)
        {
            PacketOut Out = new PacketOut((byte)Opcodes.F_MAX_VELOCITY);
            Out.WriteUInt16(NewSpeed);
            Out.WriteByte(1);
            Out.WriteByte(100);
            SendPacket(Out);

        }
        public void SendMoney()
        {
            PacketOut Out = new PacketOut((byte)Opcodes.F_PLAYER_WEALTH);
            Out.WriteUInt32(0);
            Out.WriteUInt32(_Value.Money);
            SendPacket(Out);
        }
        public void SendHealh()
        {
            PacketOut Out = new PacketOut((byte)Opcodes.F_PLAYER_HEALTH);
            Out.WriteUInt32(Health);
            Out.WriteUInt32(TotalHealth);
            Out.WriteUInt16(ActionPoints); //  Actionpoints
            Out.WriteUInt16(MaxActionPoints); //  MaxAction
            Out.WriteUInt16(0); // Control le cercle bleu
            Out.WriteUInt16(0x0E10); // Idem
            SendPacket(Out);
        }
        public void SendInited()
        {
            PacketOut Out = new PacketOut((byte)Opcodes.S_PLAYER_INITTED);
            Out.WriteUInt16(_ObjectId);
            Out.WriteUInt16(0);
            Out.WriteUInt32((uint)_Info.CharacterId);
            Out.WriteUInt16((ushort)_Value.WorldZ);
            Out.WriteUInt16(0);
            Out.WriteUInt32((uint)_Value.WorldX);
            Out.WriteUInt32((uint)_Value.WorldY);
            Out.WriteUInt16((ushort)_Value.WorldO);
            Out.WriteByte(0);
            Out.WriteByte((byte)Realm);
            Out.Fill(0, 5); // ??
            Out.WriteByte((byte)Zone.Info.Region);
            Out.WriteUInt16(1);
            Out.WriteByte(0);
            Out.WriteByte(_Info.Career);
            Out.Fill(0, 6);
            Out.WritePascalString(Program.Rm.Name);
            Out.Fill(0, 3);
            SendPacket(Out);
            

          /*  PacketOut Out = new PacketOut((byte)Opcodes.S_PLAYER_INITTED);
            Out.WriteHexStringBytes("00CA00000028D5BF1D7F0000000CF824000CAFC7051700020000000000080001001A000000000000084261646C616E6473000000");
            SendPacket(Out);
            */

        }
        public void SendSkills()
        {
            PacketOut Out = new PacketOut((byte)Opcodes.F_CHARACTER_INFO);
            Out.WriteByte(3); // Skills
            Out.Fill(0, 3);
            Out.WriteByte(_Info.CareerLine);
            Out.WriteByte(_Info.Race);
            Out.WriteUInt32R(_Value.Skills);
            Out.WriteUInt16(_Value.RallyPoint);
            SendPacket(Out);

        }
        public void SendXpTable()
        {
            PacketOut Out = new PacketOut((byte)Opcodes.F_EXPERIENCE_TABLE);
            Out.WritePacketString(@"|1C 00 00 00 0A 96 00 00 00 18 C4 00 00 |................|
|00 28 F0 00 00 00 39 EE 00 00 00 4F B0 00 00 00 |.(....9....O....|
|65 FE 00 00 00 82 32 00 00 00 9E C0 00 00 00 BE |e.....2.........|
|96 00 00 00 E2 04 03 00 00 00 00 00 00 01 05 0E |................|
|00 00 01 30 24 03 00 00 00 00 00 00 01 5A CC 00 |...0$........Z..|
|00 01 89 84 03 00 00 00 00 00 00 01 BC 88 00 00 |................|
|01 EE 74 03 00 00 00 00 00 00 02 29 CA 00 00 02 |..t........)....|
|63 90 03 00 00 00 00 00 00 02 A0 BC 04 00 00 00 |c...............|
|00 00 00 02 D2 6C 03 00 00 00 00 00 00 03 09 62 |.....l.........b|
|03 00 00 00 00 00 00 03 51 2E 03 00 00 00 00 00 |........Q.......|
|00 03 9F 80 03 00 00 00 00 00 00 03 EC 38 03 00 |.............8..|
|00 00 00 00 00 04 3E 04 03 00 00 00 00 00 00 04 |......>.........|
|88 64 03 00 00 00 00 00 00 04 FA 1A 03 00 00 00 |.d..............|
|00 00 00 05 9A 24 03 00 00 00 00 00 00 06 44 24 |.....$........D$|
|03 00 00 00 00 04 00 00 00 00 00 00 06 FC 2A 03 |..............*.|
|00 00 00 00 00 00 07 CE C0 03 00 00 00 00 00 00 |................|
|08 A1 9C 03 00 00 00 00 00 00 09 7F E0 03 00 00 |................|
|00 00 00 00 0A B3 42 03 00 00 00 00 00 00 0B 6E |......B........n|
|A4 03 00 00 00 00 00 00 0C 2E 02 03 00 00 00 00 |................|
|00 00 0D 00 FC 03 00 00 00 00 00 00 0D CC 8A 03 |................|
|00 00 00 00 00 00 0E A1 96 03 00 00 00 00 04 00 |................|
|00 00 00 05 00 00 00 0A 06 00 00 00 00 05 00 00 |................|
|00 50 06 00 00 00 00 05 00 00 00 E6 06 00 00 00 |.P..............|
|00 05 00 00 01 B8 06 00 00 00 00 05 00 00 02 DA |................|
|06 00 00 00 00 05 00 00 04 38 06 00 00 00 00 05 |.........8......|
|00 00 05 DC 06 00 00 00 00 05 00 00 07 D0 06 00 |................|
|00 00 00 05 00 00 0A 00 06 00 00 00 00 05 00 00 |................|
|0C 76 06 00 00 00 00 04 00 00 00 00 05 00 00 0F |.v..............|
|32 06 00 00 00 00 05 00 00 12 2A 06 00 00 00 00 |2.........*.....|
|05 00 00 15 72 06 00 00 00 00 05 00 00 18 F6 06 |....r...........|
|00 00 00 00 05 00 00 1C B6 06 00 00 00 00 05 00 |................|
|00 20 BC 06 00 00 00 00 05 00 00 25 08 06 00 00 |. .........%....|
|00 00 05 00 00 29 90 06 00 00 00 00 05 00 00 2E |.....)..........|
|54 06 00 00 00 00 05 00 00 33 5E 06 00 00 00 00 |T........3^.....|
|04 00 00 00 00 05 00 00 38 A4 06 00 00 00 00 05 |........8.......|
|00 00 3E 30 06 00 00 00 00 05 00 00 43 EE 06 00 |..>0........C...|
|00 00 00 05 00 00 49 F2 06 00 00 00 00 05 00 00 |......I.........|
|50 32 06 00 00 00 00 05 00 00 56 AE 06 00 00 00 |P2........V.....|
|00 05 00 00 5D 66 06 00 00 00 00 05 00 00 64 64 |....]f........dd|
|06 00 00 00 00 05 00 00 6B 94 06 00 00 00 00 05 |........k.......|
|00 00 73 00 06 00 00 00 00 04 00 00 00 00 05 00 |..s.............|
|00 7A A8 06 00 00 00 00 05 00 00 82 8C 06 00 00 |.z..............|
|00 00 05 00 00 8A A2 06 00 00 00 00 05 00 00 92 |................|
|FE 06 00 00 00 00 05 00 00 9B 8C 06 00 00 00 00 |................|
|05 00 00 A4 4C 06 00 00 00 00 05 00 00 AD 52 06 |....L.........R.|
|00 00 00 00 05 00 00 B6 8A 06 00 00 00 00 05 00 |................|
|00 BF F4 06 00 00 00 00 05 00 00 C9 9A 06 00 00 |................|
|00 00 03 00 00 00 00 05 00 00 D3 72 06 00 00 00 |...........r....|
|00 05 00 00 DD 86 06 00 00 00 00 05 00 00 E7 CC |................|
|06 00 00 00 00 05 00 00 F2 44 06 00 00 00 00 05 |.........D......|
|00 00 FC F8 06 00 00 00 00 04 00 00 00 00 05 00 |................|
|01 07 D4 06 00 00 00 00 05 00 01 12 EC 06 00 00 |................|
|00 00 05 00 01 1E 36 06 00 00 00 00 05 00 01 29 |......6........)|
|BC 06 00 00 00 00 05 00 01 35 6A 06 00 00 00 00 |.........5j.....|
|03 00 00 00 00 05 00 01 41 4A 06 00 00 00 00 05 |........AJ......|
|00 01 4E CE 06 00 00 00 00 05 00 01 5E 1E 06 00 |..N.........^...|
|00 00 00 05 00 01 6F 4E 06 00 00 00 00 05 00 01 |......oN........|
|82 90 06 00 00 00 00 05 00 01 98 16 06 00 00 00 |................|
|00 05 00 01 B0 12 06 00 00 00 00 05 00 01 CA AC |................|
|06 00 00 00 00 05 00 01 E8 20 06 00 00 00 00 05 |......... ......|
|00 02 08 A0 06 00 00 00 00 03 00 00 00 00 05 00 |................|
|02 2C 68 06 00 00 00 00 05 00 02 53 A0 06 00 00 |.,h........S....|
|00 00 05 00 02 7E 84 06 00 00 00 00 05 00 02 AD |.....~..........|
|5A 06 00 00 00 00 05 00 02 E0 54 06 00 00 00 00 |Z.........T.....|
|04 00 00 00 00 05 00 03 17 C2 06 00 00 00 00 05 |................|
|00 03 53 CC 06 00 00 00 00 05 00 03 94 B8 06 00 |..S.............|
|00 00 00 05 00 03 DA D6 06 00 00 00 00 05 00 04 |................|
|26 62 06 00 00 00 00 03 00 00 00 00 05 00 04 77 |&b.............w|
|A2 06 00 00 00 00 05 00 04 CE DC 06 00 00 00 00 |................|
|05 00 05 2C 56 06 00 00 00 00 05 00 05 90 6A 06 |...,V.........j.|
|00 00 00 00 05 00 05 FB 54 06 00 00 00 00 04 00 |........T.......|
|00 00 00 05 00 06 6D 6E 06 00 00 00 00 05 00 06 |......mn........|
|E6 FE 06 00 00 00 00 05 00 07 68 54 06 00 00 00 |..........hT....|
|00 05 00 07 F1 CA 06 00 00 00 00 05 00 08 83 BA |................|
|06 00 00 00 00 04 00 00 00 00 05 00 0A 47 3D 06 |.............G=.|
|00 00 00 00 05 00 0A D4 41 06 00 00 00 00 05 00 |........A.......|
|0B 61 43 06 00 00 00 00 05 00 0B EE 46 06 00 00 |.aC.........F...|
|00 00 04 00 00 00 00 05 00 0C 7B 48 06 00 00 00 |..........{H....|
|00 05 00 0D 08 4A 06 00 00 00 00 05 00 0D 95 4C |.....J.........L|
|06 00 00 00 00 05 00 0E 22 4F 06 00 00 00 00 04 |........O......|
|00 00 00 00 05 00 0E AF 51 06 00 00 00 00 05 00 |........Q.......|
|0F 3C 53 06 00 00 00 00 05 00 0F C9 55 06 00 00 |.<S.........U...|
|00 00 05 00 10 56 58 06 00 00 00 00 04 00 00 00 |.....VX.........|
|00 05 00 10 E3 5A 06 00 00 00 00 05 00 11 70 5C |.....Z........p\|
|06 00 00 00 00 05 00 11 FD 5F 06 00 00 00 00 05 |........._......|
|00 12 8A 61 06 00 00 00 00 04 00 00 00 00 05 00 |...a............|
|13 17 63 06 00 00 00 00 05 00 13 A4 65 06 00 00 |..c.........e...|
|00 00 05 00 14 31 68 06 00 00 00 00 05 00 14 BE |.....1h.........|
|6A 06 00 00 00 00 04 00 00 00 00 00 00 00 00 00 |j...............|
|00                                              |.               |");
            SendPacket(Out);
        }
        public void SendXp()
        {
            PacketOut Out = new PacketOut((byte)Opcodes.F_PLAYER_EXPERIENCE);
            Out.WriteUInt32(_Value.Xp);
            Out.WriteUInt32(CurrentXp != null ? CurrentXp.Xp : 0);
            Out.WriteUInt32(0);
            Out.WriteByte(_Value.Level);
            Out.Fill(0, 3);
            SendPacket(Out);
        }
        public void SendLevelUp(Dictionary<byte, UInt16> Diff)
        {
            SendRankUpdate(null);

            PacketOut Out = new PacketOut((byte)Opcodes.F_PLAYER_LEVEL_UP);
            Out.WriteUInt32(0);
            Out.WriteByte((byte)Diff.Count);
            foreach (KeyValuePair<byte, UInt16> Stat in Diff)
            {
                Out.WriteByte(Stat.Key);
                Out.WriteUInt16(Stat.Value);
            }
            SendPacket(Out);
        }
        public void SendRankUpdate(Player Plr)
        {
            PacketOut Out = new PacketOut((byte)Opcodes.F_PLAYER_RANK_UPDATE);
            Out.WriteByte((byte)Level);
            Out.WriteByte(0x20);
            Out.WriteUInt16(Oid);

            if (Plr == null)
                DispatchPacket(Out, true);
            else
                Plr.SendPacket(Out);
        }
        public void SendRenown()
        {
            PacketOut Out = new PacketOut((byte)Opcodes.F_PLAYER_RENOWN);
            Out.WriteUInt32(_Value.Renown);
            Out.WriteUInt32(CurrentRenown.Renown);
            Out.WriteByte(_Value.RenownRank);
            Out.Fill(0, 3);
            SendPacket(Out);
        }
        public void SendInitComplete()
        {
            PacketOut Out = new PacketOut((byte)Opcodes.F_PLAYER_INIT_COMPLETE);
            Out.WriteUInt16R(_ObjectId);
            SendPacket(Out);
        }
        public void SendLocalizeString(string Msg, GameData.Localized_text Type)
        {
            PacketOut Out = new PacketOut((byte)Opcodes.F_LOCALIZED_STRING);
            Out.WriteByte(0);
            Out.WriteByte(0);
            Out.WriteUInt16(0);
            Out.WriteUInt16((ushort)Type);
            Out.WriteUInt16(0);
            Out.WriteByte(0);

            Out.WriteByte(1);
            Out.WriteByte(1);

            Out.WriteByte(0);
            Out.WritePascalString(Msg);
            SendPacket(Out);
        }
        public void SendDialog(UInt16 Type, UInt16 Value)
        {
            PacketOut Out = new PacketOut((byte)Opcodes.F_SHOW_DIALOG);
            Out.WriteUInt16(Type);
            Out.WriteUInt16(0);
            Out.WriteUInt16(Value);
            SendPacket(Out);
        }
        public void SendDialog(UInt16 Type, string Text)
        {
            PacketOut Out = new PacketOut((byte)Opcodes.F_SHOW_DIALOG);
            Out.WriteUInt16(Type);
            Out.WriteByte(0);
            Out.WritePascalString(Text);
            SendPacket(Out);
        }
        public void SendLeave()
        {
            PacketOut Out = new PacketOut((byte)Opcodes.F_PLAYER_QUIT);
            Out.WriteByte(0);
            Out.WriteByte((byte)(CloseClient ? 0 : 1)); // 1 = Page de sélection des perso, 0 = Exit
            SendPacket(Out);
        }

        public void SendHelpMessage(Player Plr, string Text)
        {
            /*|00 3D 06 00 00 23 00 00 00 00 0D 57 68 69 69 74 |.=...#.....Whiit|
|65 63 68 65 72 72 79 00 00 25 4C 46 20 74 61 6E |echerry..%LF tan|
|6B 20 61 6E 64 20 68 65 61 6C 65 72 20 66 6F 72 |k and healer for|
|20 73 63 2F 6F 72 76 72 20 67 72 6F 75 70 00 00 | sc/orvr group..|      */

            PacketOut Out = new PacketOut((byte)Opcodes.F_CHAT);
            Out.WriteUInt16(Plr.Oid);
            Out.WriteByte(0x23);
            Out.WriteUInt32(0);
            Out.WriteStringToZero(Plr.Name);
            Out.WriteUInt16((ushort)(Text.Length + 1));
            Out.WriteStringBytes(Text);
            Out.WriteByte(0);
            Out.WriteByte(0);
            SendPacket(Out);
        }
        public void SendMessage(UInt16 Oid, string NameSender, string Text, SystemData.ChatLogFilters Filter)
        {
            PacketOut Out = new PacketOut((byte)Opcodes.F_CHAT);
            Out.WriteUInt16(Oid);
            Out.WriteByte((byte)Filter);
            Out.Fill(0, 4);
            Out.WritePascalString(NameSender);
            Out.WriteUInt16((ushort)(Text.Length + 1));
            Out.WriteStringBytes(Text);
            Out.WriteByte(0);

            int a = Text.IndexOf("<LINK");
            int b = Text.IndexOf("ITEM:");
            if (a >= 0 && b > 0)
            {
                Out.WriteByte(1);
                long p = Out.Position;
                Out.WriteByte(0);

                int Count = 0;
                while (a >= 0 && b >= 0)
                {
                    int Pos = b + 5;
                    int LastPos = Text.IndexOf(" ", Pos) - 1;
                    string Value = Text.Substring(Pos, LastPos - Pos);
                    uint ItemId = uint.Parse(Value);
                    Item_Info Info = WorldMgr.GetItem_Info(ItemId);
                    if (Info != null)
                    {
                        ++Count;
                        Out.WriteByte(3);
                        Item.BuildItem(ref Out, null, Info, 0, 1);
                    }

                    a = Text.IndexOf("<LINK", Pos);
                    b = Text.IndexOf("ITEM:", Pos);
                }

                Out.Position = p;
                Out.WriteByte((byte)Count);
                Out.Position = Out.Length;
            }

            SendPacket(Out);
        }
        public void SendMessage(Object Sender, string Text,SystemData.ChatLogFilters Filter)
        {
            SendMessage(Sender != null ? Sender.Oid : (UInt16)0, Sender != null ? Sender.Name : "", Text, Filter);
        }
        public void SendObjectiveText(string Text)
        {
            PacketOut Out = new PacketOut((byte)Opcodes.F_OBJECTIVE_INFO);
            Out.WriteUInt32(0); // Entry
            Out.WriteByte(0); // 1
            Out.WriteByte(1); // 2
            Out.WriteByte(0); // 1
            Out.WriteUInt16(0);
            Out.WriteStringToZero(Text);
            Out.WriteUInt16(0);
            Out.WriteUInt16(0); // Time
            Out.WriteUInt16(0);
            Out.WriteByte(0);
            SendPacket(Out);
        }
        public override void SendMeTo(Player Plr)
        {
            if (Plr == null || Plr.IsDisposed || Plr.Client == null)
                return;
 
            if (IsDisposed || Client == null)
                return;

            PacketOut Out = new PacketOut((byte)Opcodes.F_CREATE_PLAYER);
            Out.WriteUInt16((UInt16)_Client.Id);
            Out.WriteUInt16(Oid);
            Out.WriteUInt16(_Info.ModelId);
            Out.WriteUInt16(_Info.CareerLine);
            Out.WriteUInt16((UInt16)Z);
            Out.WriteUInt16(Zone.ZoneId);

            Out.WriteUInt16((UInt16)X);
            Out.WriteUInt16((UInt16)Y);
            Out.WriteUInt16(Heading);

            Out.WriteByte(0); // Level
            Out.WriteByte(_Value.Level); // Level

            Out.WriteByte(0x2B);
            Out.WriteByte((byte)(Faction + (IsDead ? 1 : 0))); // Faction
            Out.WriteByte(0);
            Out.WriteByte(0); // ?

            Out.Write(_Info.bTraits, 0, _Info.bTraits.Length);
            Out.Fill(0, 12);

            Out.WriteByte(_Info.Race);
            Out.WriteByte(0); //sometimes 1
            Out.WriteByte(0); // health/ap?
            Out.WriteByte(PctHealth);
            Out.Fill(0, 8);

            Out.WritePascalString(_Info.Name);
            Out.WritePascalString(""); // suffix. title?
            Out.WritePascalString(""); // guild name
            Out.Fill(0, 4);

            Plr.SendPacket(Out);
            base.SendMeTo(Plr);
        }
        public void SendSwitchRegion(UInt16 ZoneID)
        {
            PacketOut Out = new PacketOut((byte)Opcodes.F_SWITCH_REGION);
            Out.WriteUInt16(ZoneID);
            Out.Fill(0, 5);
            Out.WriteByte(1);
            Out.WriteByte(1);
            Out.Fill(0, 11);
            SendPacket(Out);
        }

        #endregion

        #region Quit

        public int  DisconnectTime= DISCONNECT_TIME; // 20 Secondes = 20000
        public bool CloseClient = false;
        public bool Leaving = false;
        public bool StopQuit()
        {
            if (Leaving)
            {
                EvtInterface.RemoveEvent(Quit);
                DisconnectTime = DISCONNECT_TIME;
                Leaving = false;
                return true;
            }

            return false;
        }
        public bool CancelQuit(Object Sender, object Args)
        {
            if(StopQuit())
                SendLocalizeString("", GameData.Localized_text.TEXT_CANCELLED_LOGOUT);

            return false;
        }

        public void Quit()
        {
            Quit(false);
        }
        public void Quit(bool CloseClient)
        {
            try
            {
                if (IsMoving)
                {
                    SendLocalizeString("", GameData.Localized_text.TEXT_MUST_NOT_MOVE_TO_QUIT);
                    return;
                }

                if (CbtInterface.IsFighting())
                {
                    SendLocalizeString("", GameData.Localized_text.TEXT_CANT_QUIT_IN_COMBAT);
                    return;
                }

                if (AbtInterface.IsCasting())
                {
                    SendLocalizeString("", GameData.Localized_text.TEXT_CANT_QUIT_YOURE_CASTING);
                    return;
                }

                if (IsDead)
                {
                    SendLocalizeString("", GameData.Localized_text.TEXT_CANT_QUIT_YOURE_DEAD);
                    return;
                }

                if (DisconnectTime >= DISCONNECT_TIME)
                {
                    EvtInterface.AddEvent(Quit, 5000, 5);
                }

                Leaving = true;

                SendLocalizeString("" + DisconnectTime / 1000, GameData.Localized_text.TEXT_YOU_WILL_LOG_OUT_IN_X_SECONDS);
                DisconnectTime -= 5000;
                this.CloseClient = CloseClient;

                if (!IsDisposed && ( DisconnectTime < 0 || GmLevel >= 1)) // Leave
                    Dispose();
            }
            catch(Exception e)
            {
                Log.Error("Quit", e.ToString());
            }
        }

        public bool Save(Object Sender, object Args)
        {
            EvtInterface.AddEvent(Save, 20000, 0);
            return true; // True, doit être delete après lancement
        }
        public override void Save()
        {
            CalcWorldPositions();
            CharMgr.Database.SaveObject(_Value);

            if (_Info.Influences != null)
                foreach (Characters_influence Obj in _Info.Influences)
                    CharMgr.Database.SaveObject(Obj);

            base.Save();
        }

        #endregion

        #region Positions

        public int LastCX,LastCY = 0;
        public int LastX, LastY = 0;
        public MapPiece CurrentPiece;

        public override bool SetPosition(ushort PinX, ushort PinY, ushort PinZ, ushort Head, bool SendState=false)
        {
            if (_Client.State != (int)eClientState.Playing)
            {
                _Client.State = (int)eClientState.Playing;
                AddPlayer(this);
                EvtInterface.Notify(EventName.PLAYING, this, null);
            }

            bool Updated = base.SetPosition(PinX, PinY, PinZ, Head);

            _Value.WorldX = WorldPosition.X;
            _Value.WorldY = WorldPosition.Y;
            _Value.WorldZ = WorldPosition.Z;
            _Value.WorldO = Head;

            return Updated;
        }

        public void SafePinTeleport(UInt16 PinX, UInt16 PinY, UInt16 PinZ, UInt16 WorldO)
        {
            if (PinX == 0 || PinY == 0)
                return;

            Point3D World = ZoneMgr.CalculWorldPosition(Zone.Info, PinX, PinY, PinZ);
            SafeWorldTeleport((UInt32)World.X, (UInt32)World.Y, (UInt16)World.Z, WorldO);
        }

        public void SafeWorldTeleport(UInt32 WorldX, UInt32 WorldY, UInt16 WorldZ, UInt16 WorldO)
        {
            if (WorldX == 0 || WorldY == 0)
                return;

            PacketOut Out = new PacketOut((byte)Opcodes.F_PLAYER_JUMP);
            Out.WriteUInt32(WorldX);
            Out.WriteUInt32(WorldY);
            Out.WriteUInt16(Oid);
            Out.WriteUInt16(WorldZ);
            Out.WriteUInt16(WorldO);
            Out.Fill(0, 5);
            Out.WriteByte(1);
            SendPacket(Out);

            X = Zone.CalculPin(WorldX, true);
            Y = Zone.CalculPin(WorldY, true);
            SetPosition((ushort)X, (ushort)Y, WorldZ, WorldO, false);
        }

        public void Teleport(UInt16 ZoneID, UInt32 WorldX, UInt32 WorldY, UInt16 WorldZ, UInt16 WorldO)
        {
            Log.Info("Player", "Teleport : " + ZoneID + "," + WorldX + "," + WorldY + "," + WorldZ);

            Zone_Info Info = WorldMgr.GetZone_Info(ZoneID);
            if(Info == null)
                return;

            // Change Region , so change thread and maps
            if (Zone == null || Zone.Info.Region != Info.Region)
            {
                RegionMgr NewRegion = WorldMgr.GetRegion(Info.Region, true);
                if (NewRegion != null)
                {
                    ZoneMgr NewZone = NewRegion.GetZoneMgr(Info.ZoneId, true);
                    Teleport(NewZone, WorldX, WorldY, WorldZ, WorldO);
                }
            }
            else // Teleport in current Zone
            {
                SafeWorldTeleport(WorldX, WorldY, WorldZ, WorldO);
            }
        }

        public void Teleport(ZoneMgr NewZone, UInt32 WorldX, UInt32 WorldY, UInt16 WorldZ, UInt16 WorldO)
        {
            if (Zone == NewZone)
                SafeWorldTeleport(WorldX, WorldY, WorldZ, WorldO);
            else
            {
                _Value.WorldX = (int)WorldX;
                _Value.WorldZ = (int)WorldZ;
                _Value.WorldY = (int)WorldY;
                _Value.WorldO = WorldO;
                _Value.ZoneId = NewZone.ZoneId;
                _Value.RegionId = NewZone.Info.Region;

                if (NewZone.Region.AddObject(this, NewZone.ZoneId))
                    SendSwitchRegion(NewZone.ZoneId);
            }
        }

        public override void OnRangeUpdate()
        {
            if (CurrentPiece == null || !CurrentPiece.IsOn((ushort)(X / 64), (ushort)(Y / 64), Zone.ZoneId))
            {
                CurrentPiece = Zone.ClientInfo.GetWorldPiece((ushort)X, (ushort)Y, Zone.ZoneId);
                if (CurrentPiece != null)
                {
                    if (CurrentPiece.Area != null)
                        TokInterface.AddTok(CurrentPiece.Area.TokExploreEntry);

                    if (CurrentPiece.IsPvp((byte)Realm))
                        CbtInterface.EnablePvp();
                }
            }
        }

        #endregion

        #region Info

        public override string ToString()
        {
            string Info="";

            Info += "Name=" + Name + ",Ip=" + (Client != null ? Client.GetIp : "Disconnected") + base.ToString();

            return Info;
        }

        #endregion

        #region Group

        private Group _Group;
        public Group GetGroup() { return _Group; }
        public void SetGroup(Group group)
        {
            _Group = group;
            if (group != null)
                group.AddMember(this);
        }
        public GroupInvitation Invitation;

        #endregion
    }
}

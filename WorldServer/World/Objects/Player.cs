
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;

using Common;
using FrameWork;

namespace WorldServer
{
    public class Player : Unit
    {
        #region Statics

        static public int DISCONNECT_TIME = 20000;

        static public List<Player> _Players = new List<Player>();
        static public void AddPlayer(Player Plr)
        {
            lock (_Players)
                if (!_Players.Contains(Plr))
                {
                    _Players.Add(Plr);
                    if (Plr.Realm == GameData.Realms.REALMS_REALM_ORDER)
                        ++Program.Rm.OrderCount;
                    else
                        ++Program.Rm.DestructionCount;

                    Program.Rm.OnlinePlayers = (uint)_Players.Count;
                    Program.AcctMgr.UpdateRealm(Program.Rm.RealmId, Program.Rm.OnlinePlayers, Program.Rm.OrderCount, Program.Rm.DestructionCount);
                }
        }
        static public void RemovePlayer(Player Plr)
        {
            lock (_Players)
            {
                _Players.Remove(Plr);
                if (Plr._Info.Realm == (byte)GameData.Realms.REALMS_REALM_ORDER)
                    --Program.Rm.OrderCount;
                else
                    --Program.Rm.DestructionCount;

                Program.Rm.OnlinePlayers = (uint)_Players.Count;
                Program.AcctMgr.UpdateRealm(Program.Rm.RealmId, Program.Rm.OnlinePlayers, Program.Rm.OrderCount, Program.Rm.DestructionCount);
 
            }
        }
        static public Player GetPlayer(string Name)
        {
            Name = Name.ToLower();

            lock (_Players)
                return _Players.Find(plr => plr.Name.ToLower() == Name);
        }
        static public Player GetPlayer(int CharacterId)
        {
            lock (_Players)
                return _Players.Find(plr => plr.CharacterId == CharacterId);
        }
        static public Player CreatePlayer(GameClient Client, Character Char)
        {
            Log.Success("Player", "CreatePlayer");

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

                Log.Success("GetPlayers", "N=" + Name + ",G=" + GuildName + ",C=" + Career + ",Z=" + ZoneId + ",Ml=" + MinLevel + ",MaL=" + MaxLevel);

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
            Log.Success("Player", "Stop");
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

        public int CharacterId
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
            Log.Success("Player", "Construction de " + Info.Name);

            _Client = Client;
            _Info = Info;
            _Value = Info.Value[0];

            Name = Info.Name;
            SetFaction((byte)(8*(8*Info.Realm)+6));

            EvtInterface = EventInterface.GetEventInterface((uint)_Info.CharacterId);
            SocInterface = new SocialInterface(this);
            TokInterface = new TokInterface(this);
        }

        ~Player()
        {
            Log.Success("Player", "Destruction de " + Name);
        }

        public override void OnLoad()
        {
            _Client.State = (int)eClientState.WorldEnter;

            if (!_Inited)
            {
                EvtInterface.Obj = this;
                EvtInterface.AddEventNotify("Playing", Save);
                EvtInterface.Start();

                ItmInterface.Load(CharMgr.GetItemChar(_Info.CharacterId));
                StsInterface.Load(CharMgr.GetCharacterInfoStats(_Info.CareerLine, _Value.Level));
                QtsInterface.Load(this._Info.Quests);
                TokInterface.Load(_Info.Toks);
                SocInterface.Load(_Info.Socials);
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
            RemovePlayer(this);

            SendLeave();
            StopQuit();

            EvtInterface.Notify("Leave", this, null);
            SocInterface.Stop();

            Save();

            if (_Client != null)
            {
                _Client.Plr = null;
                _Client.Disconnect();
            }

            base.Dispose();
        }

        public void StartInit()
        {
            RemovePlayer(this);
            Client.State = (int)eClientState.WorldEnter;
            SendMoney();
            SendStats();
            SendSpeed();
            SendInited();
            SendRankUpdate(this);
            SendXp();
            SendRenown();
            TokInterface.SendAllToks();
            SendSkills();
            Health = TotalHealth;
           // ItmInterface.SendAllItems(this);
            AbtInterface.SendAbilities();
            




           PacketOut Out = new PacketOut((byte)Opcodes.F_CHARACTER_INFO);
            Out.WriteByte(1);
            Out.WriteByte(1);
            Out.WriteUInt16(0x300);
            Out.WriteUInt16(8159);
            Out.WriteByte(1);
            SendPacket(Out);

            QtsInterface.SendQuests();

            SendInitComplete();
            SocInterface.SendFriends();
            
            

        }
        public override void Update()
        {
            if (Client == null)
            {
                Dispose();
                return;
            }
            base.Update();
            UpdatePackets();
        }

        #region Packets

        public List<PacketIn> _PacketIn = new List<PacketIn>(20);
        public List<PacketOut> _PacketOut = new List<PacketOut>(20);

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
                _Client.SendTCP(Out);
        }
        public void SendCopy(PacketOut Out)
        {
            Out.WritePacketLength();
            PacketOut packet = new PacketOut(0);
            packet.Position = 0;
            packet.Write(Out.ToArray(), 0, Out.ToArray().Length);
            SendPacket(packet);
            
        }
        public PacketIn[] GetPacketIn(bool Clear)
        {
            lock (_PacketIn)
            {
                PacketIn[] Ins = _PacketIn.ToArray();
                if (Clear) _PacketIn.Clear();
                return Ins;
            }
        }
        public PacketOut[] GetPacketOut(bool Clear)
        {
            lock (_PacketOut)
            {
                PacketOut[] Outs = _PacketOut.ToArray();
                if (Clear) _PacketOut.Clear();
                return Outs;
            }
        }
        public void UpdatePackets()
        {
            PacketIn[] Ins = GetPacketIn(true);
            PacketOut[] Outs = GetPacketOut(true);

            if (_Client == null)
                return;

            for (int i = 0; i < Ins.Length; ++i)
                if (Ins[i] != null)
                    _Client.Server.HandlePacket(_Client, Ins[i]);

            for (int i = 0; i < Outs.Length; ++i)
                if (Outs[i] != null)
                    _Client.SendTCP(Outs[i]);
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
            Killer.QtsInterface.HandleEvent(Objective_Type.QUEST_KILL_PLAYERS, 0, 1);

            base.SetDeath(Killer);

            if(Killer.IsPlayer())
                WorldMgr.GenerateRenown(Killer.GetPlayer(), this);

            EvtInterface.AddEvent(RespawnPlayer, 20000, 1);
            SendDialog((ushort)5, (ushort)20);
        }

        public void RespawnPlayer()
        {
            Zone_Respawn Respawn = WorldMgr.GetZoneRespawn(Zone.ZoneId, (byte)Realm, this);
            if (Respawn != null)
                SafePinTeleport(Respawn.PinX, Respawn.PinY, Respawn.PinZ, Respawn.WorldO);

            RezUnit();
        }

        public override void RezUnit()
        {
            PacketOut Out = new PacketOut((byte)Opcodes.F_PLAYER_CLEAR_DEATH);
            Out.WriteUInt16(Oid);
            Out.WriteUInt16(0);
            DispatchPacket(Out,true);

            base.RezUnit();
        }

        #endregion

        #region Xp

        private Xp_Info CurrentXp = null;
        public void SetLevel(byte Level)
        {
            CurrentXp = WorldMgr.GetXp_Info(Level);
            _Value.Level = Level;

            if (_Loaded)
            {
                SendLevelUp(ApplyLevel());
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
            SetLevel((byte)(_Value.Level + 1));

            if (CurrentXp == null)
                return;

            AddXp(RestXp);
        }
        public Dictionary<byte,UInt16> ApplyLevel()
        {
            Dictionary<byte, UInt16> Diff = new Dictionary<byte, ushort>();

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

            if(_Loaded)
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

        public void SendSpeed()
        {
            PacketOut Out = new PacketOut((byte)Opcodes.F_MAX_VELOCITY);
            Out.WriteUInt16(Speed);
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
            Out.WriteByte(0); // 0=déco, 1=Ejecté du serveur
            Out.WriteByte(1);
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
            Out.WriteByte((byte)(Level-1));
            Out.WriteByte(0x20);
            Out.WriteUInt16(Oid);
            if (Plr == null)
                DispatchPacket(Out,true);
            else
                Plr.SendPacket(Out);

        }
        public void SendMessage(UInt16 Oid, string NameSender, string Text, SystemData.ChatLogFilters Filter)
        {
            if (Text.IndexOf("<LINK") >= 0 && Text.IndexOf("ITEM:") > 0)
            {
                int Pos = Text.IndexOf("ITEM:")+5;
                int LastPos = Text.IndexOf(" ",Pos)-1;
                string Value = Text.Substring(Pos, LastPos-Pos);
                uint ItemId = uint.Parse(Value);
                Item_Info Info = WorldMgr.GetItem_Info(ItemId);
                if (Info != null)
                {

                }
            }

            PacketOut Out = new PacketOut((byte)Opcodes.F_CHAT);
            Out.WriteUInt16(Oid);
            Out.WriteByte((byte)Filter);
            Out.Fill(0, 4);
            Out.WritePascalString(NameSender);
            Out.WriteByte(0);
            Out.WritePascalString(Text);
            Out.WriteByte(0);
            SendPacket(Out);
        }
        public void SendMessage(Object Sender, string Text,SystemData.ChatLogFilters Filter)
        {
            SendMessage(Sender != null ? Sender.Oid : (UInt16)0, Sender != null ? Sender.Name : "", Text, Filter);
        }
        public override void SendMeTo(Player Plr)
        {
            Log.Success("SendMeTo", "[" + Plr.Name + "] voit : " + Name);

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

            Out.WriteByte(_Value.Level); // Level
            Out.WriteByte(0); // Level

            Out.WriteByte(0);
            Out.WriteByte(Faction);
            Out.WriteByte(0);
            Out.WriteByte(Faction);

            Out.Write(_Info.bTraits, 0, _Info.bTraits.Length);
            Out.Fill(0, 12);

            Out.WriteByte(_Info.Race);
            Out.Fill(0, 11);
            Out.WritePascalString(_Info.Name);
            Out.WritePascalString("");
            Out.WriteByte(0);
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
        public bool Leaving = false;
        public void StopQuit()
        {
            EvtInterface.RemoveEvent(Quit);
            DisconnectTime = DISCONNECT_TIME;
            Leaving = false;
        }
        public bool MovingStopQuit(Object Sender, EventArgs Args)
        {
            SendLocalizeString("", GameData.Localized_text.TEXT_CANCELLED_LOGOUT);
            StopQuit();
            return true;
        }
        public void Quit()
        {
            Log.Success("Player", "Quit");

            Leaving = true;

            if (IsMoving)
            {
                SendLocalizeString("", GameData.Localized_text.TEXT_MUST_NOT_MOVE_TO_QUIT);
                return;
            }

            if (DisconnectTime >= DISCONNECT_TIME)
            {
                EvtInterface.AddEvent(Quit, 5000, 5);
                EvtInterface.AddEventNotify("Moving", MovingStopQuit);
            }

            SendLocalizeString("" + DisconnectTime / 1000, GameData.Localized_text.TEXT_YOU_WILL_LOG_OUT_IN_X_SECONDS);
            DisconnectTime -= 5000;

            if (DisconnectTime < 0 || GmLevel >= 1) // Leave
                Dispose();
        }

        public bool Save(Object Sender, EventArgs Args)
        {
            EvtInterface.AddEvent(Save, 20000, 0);
            return true; // True, doit être delete après lancement
        }
        public void Save()
        {
            ItmInterface.Save();
            CalcWorldPositions();
            CharMgr.Database.SaveObject(_Value);
        }

        #endregion

        #region Positions

        public int LastCX,LastCY = 0;
        public int LastX, LastY = 0;

        public override bool SetPosition(ushort PinX, ushort PinY, ushort PinZ, ushort Head)
        {
            if (_Client.State != (int)eClientState.Playing)
            {
                _Client.State = (int)eClientState.Playing;
                AddPlayer(this);
                EvtInterface.Notify("Playing", this, null);
            }

            bool Updated = base.SetPosition(PinX, PinY, PinZ, Head);

            if (Updated)
                Zone.AreaInfo.GetTokExplore(TokInterface, PinX, PinY, (byte)Realm);

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

            Log.Info("SafeWorldTeleport", "WorldX=" + WorldX + ",WorldY=" + WorldY);

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

        #endregion

        #region Info

        public override string ToString()
        {
            string Info="";

            Info += "Name=" + Name + ",Ip=" + (Client != null ? Client.GetIp : "Disconnected");

            return Info;
        }

        #endregion

    }
}

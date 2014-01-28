
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Common;
using FrameWork;

namespace WorldServer
{
    static public class WorldMgr
    {
        static public MySQLObjectDatabase Database;

        #region Zones

        static public List<Zone_Info> _Zone_Info;

        [LoadingFunction(true)]
        static public void LoadZone_Info()
        {
            Log.Debug("WorldMgr", "Loading Zone_Info...");

            _Zone_Info = new List<Zone_Info>();
            IList<Zone_Info> Infos = Database.SelectAllObjects<Zone_Info>();
            if (Infos != null)
                _Zone_Info.AddRange(Infos);

            Log.Success("LoadZone_Info", "Loaded " + _Zone_Info.Count + " Zone_Info");
        }
        static public Zone_Info GetZone_Info(UInt16 ZoneId)
        {
            return _Zone_Info.Find(zone => zone != null && zone.ZoneId == ZoneId);
        }
        static public List<Zone_Info> GetZoneRegion(UInt16 RegionId)
        {
            Log.Success("GetZoneRegion", "RegionId=" + RegionId);
            return _Zone_Info.FindAll(zone => zone != null && zone.Region == RegionId);
        }


        static public Dictionary<int, List<Zone_Area>> _Zone_Area;

        [LoadingFunction(true)]
        static public void LoadZone_Area()
        {
            Log.Debug("WorldMgr", "Loading Zone_Area...");
            int PieceInformation = 0;

            _Zone_Area = new Dictionary<int, List<Zone_Area>>();
            IList<Zone_Area> Infos = Database.SelectAllObjects<Zone_Area>();
            foreach (Zone_Area Area in Infos)
            {
                List<Zone_Area> Areas;
                if(!_Zone_Area.TryGetValue(Area.ZoneId,out Areas))
                {
                    Areas = new List<Zone_Area>();
                    _Zone_Area.Add(Area.ZoneId, Areas);
                }

                Areas.Add(Area);

                //Area.PieceId = (byte)Areas.Count;
                
                if (Area.PieceId != 0)
                {
                    Area.Information = GetPieceInformation(Area.ZoneId, Area.PieceId);

                    if(Area.Information != null)
                        ++PieceInformation;
                }
            }

            Log.Success("LoadZone_Info", "Loaded " + Infos.Count + " Zone_Area && " + PieceInformation + " Piece Informations");
        }

        static public PieceInformation GetPieceInformation(UInt16 ZoneID,byte PieceId)
        {
            string PieceInfoFile = Program.Config.ZoneFolder + "zone" + String.Format("{0:000}", ZoneID) + "/mappieces.csv";

            try
            {
                using (StreamReader Reader = new StreamReader(PieceInfoFile))
                {
                    int CurrentPieceId = 0;
                    while (Reader.Peek() > 0)
                    {
                        string Line = Reader.ReadLine();

                        if (CurrentPieceId == PieceId)
                        {
                            string[] Values = Line.Split(',');
                            PieceInformation Info = new PieceInformation();
                            Info.PieceId = byte.Parse(Values[0]);
                            Info.OffsetX = ushort.Parse(Values[1]);
                            Info.OffsetY = ushort.Parse(Values[2]);
                            Info.Width = ushort.Parse(Values[3]);
                            Info.Height = ushort.Parse(Values[4]);
                            return Info;
                        }

                        ++CurrentPieceId;
                    }
                }
            }
            catch
            {
                Log.Error("WorldMgr", "Invalid Piece File Directory : " + PieceInfoFile);
                return null;
            }

            return null;
        }

        static public List<Zone_Area> GetZoneAreas(ushort ZoneID)
        {
            List<Zone_Area> Areas;
            if (!_Zone_Area.TryGetValue(ZoneID, out Areas))
                return new List<Zone_Area>();
            else
                return Areas;
        }

        static public Dictionary<int, List<Zone_Respawn>> _Zone_Respawn;

        [LoadingFunction(true)]
        static public void LoadZone_Respawn()
        {
            Log.Debug("WorldMgr", "Loading LoadZone_Respawn...");

            _Zone_Respawn = new Dictionary<int, List<Zone_Respawn>>();

            IList<Zone_Respawn> Respawns = Database.SelectAllObjects<Zone_Respawn>();
            foreach (Zone_Respawn Respawn in Respawns)
            {
                List<Zone_Respawn> L;
                if (!_Zone_Respawn.TryGetValue(Respawn.ZoneID, out L))
                {
                    L = new List<Zone_Respawn>();
                    _Zone_Respawn.Add(Respawn.ZoneID, L);
                }

                L.Add(Respawn);
            }

            Log.Success("LoadZone_Info", "Loaded " + Respawns.Count + " Zone_Respawn");
        }

        static public Zone_Respawn GetZoneRespawn(UInt16 ZoneID, byte Realm, Point3D PinPosition)
        {
            Zone_Respawn Respawn = null;
            List<Zone_Respawn> Respawns;

            if (_Zone_Respawn.TryGetValue(ZoneID, out Respawns))
            {
                float LastDistance = float.MaxValue;

                foreach (Zone_Respawn Res in Respawns)
                {
                    if (Res.Realm != Realm)
                        continue;

                    Point3D Pos = new Point3D(Res.PinX, Res.PinY, Res.PinZ);
                    float Distance = Pos.GetDistance(PinPosition);

                    if (Distance < LastDistance)
                    {
                        LastDistance = Distance;
                        Respawn = Res;
                    }
                }
            }
            else
                Log.Error("WorldMgr", "Zone Respawn not found for : " + ZoneID);

            return Respawn;
        }


        static public Dictionary<UInt16, Zone_Taxi[]> _Zone_Taxi = new Dictionary<UInt16, Zone_Taxi[]>();

        [LoadingFunction(true)]
        static public void LoadZone_Taxi()
        {
            Log.Debug("LoadZone_Info", "Loading Zone_Taxis...");

            IList<Zone_Taxi> Taxis = Database.SelectAllObjects<Zone_Taxi>();
            _Zone_Taxi = new Dictionary<UInt16, Zone_Taxi[]>();

            foreach (Zone_Taxi Taxi in Taxis)
            {
                Zone_Taxi[] Tax;
                if(!_Zone_Taxi.TryGetValue(Taxi.ZoneID,out Tax))
                {
                    Tax = new Zone_Taxi[(int)(GameData.Realms.REALMS_NUM_REALMS) + 1];
                    _Zone_Taxi.Add(Taxi.ZoneID,Tax);
                }
                 
                _Zone_Taxi[Taxi.ZoneID][Taxi.RealmID]= Taxi;
            }

            Log.Success("LoadZone_Info", "Loaded " + Taxis.Count + " Zone_Taxis");
        }

        static public Zone_Taxi GetZoneTaxi(UInt16 ZoneId, byte Realm)
        {
            Zone_Taxi[] Taxis;
            if (_Zone_Taxi.TryGetValue(ZoneId, out Taxis))
                return Taxis[Realm];

            return null;
        }

        static public List<Zone_Taxi> GetTaxis(Player Plr)
        {
            List<Zone_Taxi> L = new List<Zone_Taxi>();

            foreach (Zone_Taxi[] Taxis in WorldMgr._Zone_Taxi.Values)
            {
                if (Taxis[(byte)Plr.Realm] == null || Taxis[(byte)Plr.Realm].WorldX == 0)
                    continue;

                if(Taxis[(byte)Plr.Realm].Info == null)
                    Taxis[(byte)Plr.Realm].Info = WorldMgr.GetZone_Info(Taxis[(byte)Plr.Realm].ZoneID);
                
                if (Taxis[(byte)Plr.Realm].Info == null)
                    continue;

                L.Add(Taxis[(byte)Plr.Realm]);
            }

            return L;
        }

        #endregion

        #region Chapters

        static public Dictionary<uint, Chapter_Info> _Chapters;

        [LoadingFunction(true)]
        static public void LoadChapter_Infos()
        {
            Log.Debug("WorldMgr", "Loading Chapter_Infos...");

            _Chapters = new Dictionary<uint, Chapter_Info>();

            IList<Chapter_Info> IChapters = Database.SelectAllObjects<Chapter_Info>();

            if(IChapters != null)
            {
                foreach (Chapter_Info Info in IChapters)
                    if (!_Chapters.ContainsKey(Info.Entry))
                        _Chapters.Add(Info.Entry, Info);
            }

            Log.Success("LoadChapter_Infos", "Loaded " + _Chapters.Count + " Chapter_Infos");
        }
        static public Chapter_Info GetChapter(uint Entry)
        {
            Chapter_Info Info = null;
            _Chapters.TryGetValue(Entry, out Info);

            return Info;
        }
        static public List<Chapter_Info> GetChapters(ushort ZoneId)
        {
            List<Chapter_Info> Chapters = new List<Chapter_Info>();

            foreach (Chapter_Info Info in _Chapters.Values)
                if (Info.ZoneId == ZoneId)
                    Chapters.Add(Info);

            return Chapters;
        }

        static public Dictionary<uint, List<Chapter_Reward>> _Chapters_Reward;

        [LoadingFunction(true)]
        static public void LoadChapter_Rewards()
        {
            Log.Debug("WorldMgr", "Loading LoadChapter_Rewards...");

            _Chapters_Reward = new Dictionary<uint, List<Chapter_Reward>>();
            IList<Chapter_Reward> Rewards = Database.SelectAllObjects<Chapter_Reward>();

            foreach (Chapter_Reward Reward in Rewards)
            {
                if (!_Chapters_Reward.ContainsKey(Reward.Entry))
                    _Chapters_Reward.Add(Reward.Entry, new List<Chapter_Reward>());

                _Chapters_Reward[Reward.Entry].Add(Reward);
            }

            Log.Success("LoadChapter_Infos", "Loaded " + Rewards.Count + " Chapter_Rewards");
        }

        #endregion

        #region Public Quests

        static public Dictionary<uint, PQuest_Info> _PQuests;

        [LoadingFunction(true)]
        static public void LoadPQuest_Info()
        {
            _PQuests = new Dictionary<uint, PQuest_Info>();

            IList<PQuest_Info> PQuests = Database.SelectObjects<PQuest_Info>("PinX != 0 AND PinY != 0");

            foreach (PQuest_Info Info in PQuests)
            {
                _PQuests.Add(Info.Entry, Info);
            }

            Log.Success("WorldMgr", "Loaded " + _PQuests.Count + " Public Quests Info");
        }

        static public Dictionary<uint, List<PQuest_Objective>> _PQuest_Objectives;

        [LoadingFunction(true)]
        static public void LoadPQuest_Objective()
        {
            _PQuest_Objectives = new Dictionary<uint, List<PQuest_Objective>>();

            IList<PQuest_Objective> PObjectives = Database.SelectObjects<PQuest_Objective>("Type != 0");

            foreach (PQuest_Objective Obj in PObjectives)
            {
                List<PQuest_Objective> Objs;
                if (!_PQuest_Objectives.TryGetValue(Obj.Entry, out Objs))
                {
                    Objs = new List<PQuest_Objective>();
                    _PQuest_Objectives.Add(Obj.Entry, Objs);
                }

                Objs.Add(Obj);
            }

            Log.Success("WorldMgr", "Loaded " + PObjectives.Count + " Public Quest Objectives");
        }

        static public void GeneratePQuestObjective(PQuest_Objective Obj, PQuest_Info Q)
        {
            switch ((Objective_Type)Obj.Type)
            {
                case Objective_Type.QUEST_KILL_PLAYERS:
                    {
                        if (Obj.Description.Length < 1)
                            Obj.Description = "Enemy Players";
                    } break;

                case Objective_Type.QUEST_SPEACK_TO:
                    goto case Objective_Type.QUEST_KILL_MOB;

                case Objective_Type.QUEST_KILL_MOB:
                    {
                        uint ObjID = 0;
                        uint.TryParse(Obj.ObjectId, out ObjID);

                        if (ObjID != 0)
                            Obj.Creature = GetCreatureProto(ObjID);

                        if (Obj.Description.Length < 1 && Obj.Creature != null)
                            Obj.Description = Obj.Creature.Name;
                    } break;

                case Objective_Type.QUEST_GET_ITEM:
                    {
                        uint ObjID = 0;
                        uint.TryParse(Obj.ObjectId, out ObjID);

                        if (ObjID != 0)
                            Obj.Item = GetItem_Info(ObjID);
                    }
                    break;
            };
        }

        #endregion

        #region Toks

        static public Dictionary<uint, Tok_Info> _Toks;

        [LoadingFunction(true)]
        static public void LoadTok_Infos()
        {
            Log.Debug("WorldMgr", "Loading LoadTok_Infos...");

            _Toks = new Dictionary<uint, Tok_Info>();

            IList<Tok_Info> IToks = Database.SelectAllObjects<Tok_Info>();

            if (IToks != null)
            {
                foreach (Tok_Info Info in IToks)
                    if (!_Toks.ContainsKey(Info.Entry))
                        _Toks.Add(Info.Entry, Info);
            }

            Log.Success("LoadTok_Infos", "Loaded " + _Toks.Count + " Tok_Infos");
        }
        static public Tok_Info GetTok(uint Entry)
        {
            if (_Toks.ContainsKey(Entry))
                return _Toks[Entry];

            return null;
        }
      
        #endregion

        #region Item_Info

        static public Dictionary<uint, Item_Info> _Item_Info;

        [LoadingFunction(true)]
        static public void LoadItem_Info()
        {
            Log.Debug("WorldMgr", "Loading Item_Info...");

            _Item_Info = new Dictionary<uint, Item_Info>(100000);
            Item_Info[] Infos = Database.SelectAllObjects<Item_Info>().ToArray();

            foreach (Item_Info Info in Infos)
                if (!_Item_Info.ContainsKey(Info.Entry))
                    _Item_Info.Add(Info.Entry, Info);

            Log.Success("LoadItem_Info", "Loaded " + _Item_Info.Count + " Item_Info");
        }

        static public Item_Info GetItem_Info(uint Entry)
        {
            if (_Item_Info.ContainsKey(Entry))
                return _Item_Info[Entry];
            return null;
        }

        #endregion

        #region Region

        static public List<RegionMgr> _Regions = new List<RegionMgr>();

        static public RegionMgr GetRegion(UInt16 RegionId,bool Create)
        {
            Log.Success("GetRegion", "RegionId=" + RegionId);

            lock (_Regions)
            {
                RegionMgr Mgr = _Regions.Find(region => region != null && region.RegionId == RegionId);
                if (Mgr == null && Create)
                {
                    Mgr = new RegionMgr(RegionId, GetZoneRegion(RegionId));
                    _Regions.Add(Mgr);
                }

                return Mgr;
            }
        }

        static public void Stop()
        {
            Log.Success("WorldMgr", "Stop");
            foreach (RegionMgr Mgr in _Regions)
                Mgr.Stop();
        }

        #endregion

        #region Xp

        static private Dictionary<byte, Xp_Info> _Xp_Infos;

        [LoadingFunction(true)]
        static public void LoadXp_Info()
        {
            Log.Debug("WorldMgr", "Loading Xp_Infos...");

            _Xp_Infos = new Dictionary<byte, Xp_Info>();
            IList<Xp_Info> Infos = Database.SelectAllObjects<Xp_Info>();
            foreach (Xp_Info Info in Infos)
                _Xp_Infos.Add(Info.Level, Info);

            Log.Success("LoadXp_Info", "Loaded " + _Xp_Infos.Count + " Xp_Infos");
        }

        static public Xp_Info GetXp_Info(byte Level)
        {
            if (_Xp_Infos.ContainsKey(Level))
                return _Xp_Infos[Level];
            else return null;
        }

        static public void GenerateXP(Unit Killer, Unit Victim)
        {
            UInt32 KLvl = Killer.Level;
            UInt32 VLvl = Victim.Level;

            if (KLvl > VLvl + 8)
                return;

            UInt32 XP = VLvl * 60;
            XP += (UInt32)Victim.Rank * 20;

            if (KLvl > VLvl)
                XP -= (UInt32)(((float)XP / (float)100) * (KLvl - VLvl + 1)) * 5;

            if (Program.Config.XpRate > 0)
                XP *= (UInt32)Program.Config.XpRate;

            if (Killer.IsPlayer())
                Killer.GetPlayer().AddXp(XP);
        }

        #endregion

        #region Renown_Info

        static private Dictionary<byte, Renown_Info> _Renown_Infos;

        [LoadingFunction(true)]
        static public void LoadRenown_Info()
        {
            Log.Debug("WorldMgr", "Loading Renown_Info...");

            _Renown_Infos = new Dictionary<byte, Renown_Info>();
            Renown_Info[] Infos = Database.SelectAllObjects<Renown_Info>().ToArray();
            foreach (Renown_Info Info in Infos)
                _Renown_Infos.Add(Info.Level, Info);

            Log.Success("LoadRenown_Info", "Loaded " + Infos.Length + " Renown_Info");
        }

        static public Renown_Info GetRenown_Info(byte Level)
        {
            if (_Renown_Infos.ContainsKey(Level))
                return _Renown_Infos[Level];
            else return null;
        }

        static public void GenerateRenown(Player Killer, Player Victim)
        {
            if (Killer == null || Victim == null)
                return;

            UInt32 VRp = Victim._Value.RenownRank;
            UInt32 VLvl = Victim.Level;

            UInt32 RP = VRp * 50 + VLvl * 60;

            if (Program.Config.RenownRate > 0)
                RP *= (UInt32)Program.Config.RenownRate;

            Killer.AddRenown(RP);
        }

        #endregion

        #region CreatureProto

        static public Dictionary<uint, Creature_proto> CreatureProtos;

        [LoadingFunction(true)]
        static public void LoadCreatureProto()
        {
            Log.Debug("WorldMgr", "Loading Creature_Protos...");

            CreatureProtos = new Dictionary<uint, Creature_proto>();
           IList<Creature_proto> Protos = Database.SelectObjects<Creature_proto>("Model1 != '0' AND Model2 != '0'");

           if (Protos != null)
               foreach (Creature_proto Proto in Protos)
                   CreatureProtos.Add(Proto.Entry, Proto);

           Log.Success("LoadCreatureProto", "Loaded " + CreatureProtos.Count + " Creature_Protos");
        }

        static public Creature_proto GetCreatureProto(uint Entry)
        {
            Creature_proto Proto;
            CreatureProtos.TryGetValue(Entry, out Proto);
            return Proto;
        }

        #endregion

        #region CreatureSpawns

        static public Dictionary<uint, Creature_spawn> CreatureSpawns;
        static public int MaxGUID = 0;

        static public int GenerateSpawnGUID()
        {
            return System.Threading.Interlocked.Increment(ref MaxGUID);
        }

        [LoadingFunction(true)]
        static public void LoadCreatureSpawns()
        {
            Log.Debug("WorldMgr", "Loading Creature_Spawns...");

            CreatureSpawns = new Dictionary<uint, Creature_spawn>();
            IList<Creature_spawn> Spawns = Database.SelectAllObjects<Creature_spawn>();

            if(Spawns != null)
                foreach (Creature_spawn Spawn in Spawns)
                {
                    CreatureSpawns.Add(Spawn.Guid, Spawn);
                    if (Spawn.Guid > MaxGUID)
                        MaxGUID = (int)Spawn.Guid;
                }


            Log.Success("LoadCreatureSpawns", "Loaded " + CreatureSpawns.Count + " Creature_Spawns");
        }

        #endregion

        #region CreatureItems

        static public Dictionary<uint, List<Creature_item>> _CreatureItems;

        [LoadingFunction(true)]
        static public void LoadCreatureItems()
        {
            Log.Debug("WorldMgr", "Loading Creature_Items...");

            _CreatureItems = new Dictionary<uint, List<Creature_item>>();
            IList<Creature_item> Items = Database.SelectAllObjects<Creature_item>();

            if (Items != null)
                foreach (Creature_item Item in Items)
                {
                    if (!_CreatureItems.ContainsKey(Item.Entry))
                        _CreatureItems.Add(Item.Entry, new List<Creature_item>());

                    _CreatureItems[Item.Entry].Add(Item);
                }

            Log.Success("LoadCreatureItems", "Loaded " + (Items != null ? Items.Count : 0) + " Creature_Items");
        }

        static public List<Creature_item> GetCreatureItems(uint Entry)
        {
            if (!_CreatureItems.ContainsKey(Entry))
                return new List<Creature_item>();
            else
                return _CreatureItems[Entry];
        }

        #endregion

        #region CreatureText

        static public Dictionary<uint, List<Creature_text>> _CreatureTexts = new Dictionary<uint, List<Creature_text>>();

        [LoadingFunction(true)]
        static public void LoadCreatureTexts()
        {
            _CreatureTexts = new Dictionary<uint, List<Creature_text>>();

            Log.Debug("WorldMgr", "Loading Creature_texts...");

            IList<Creature_text> Texts = Database.SelectAllObjects<Creature_text>();

            int Count = 0;
            foreach (Creature_text Text in Texts)
            {
                if (!_CreatureTexts.ContainsKey(Text.Entry))
                    _CreatureTexts.Add(Text.Entry, new List<Creature_text>());

                _CreatureTexts[Text.Entry].Add(Text);
                ++Count;
            }

            Log.Success("WorldMgr", "Loaded " + Count + " Creature Texts");
        }

        static public string GetCreatureText(uint Entry)
        {
            string Text = "";

            if (_CreatureTexts.ContainsKey(Entry))
            {
                int RandomNum = RandomMgr.Next(_CreatureTexts[Entry].Count);
                Text = _CreatureTexts[Entry][RandomNum].Text;
            }

            return Text;
        }

        #endregion

        #region CreatureLoots

        static public Dictionary<uint, List<Creature_loot>> _Creature_loots = new Dictionary<uint,List<Creature_loot>>();
        static private void LoadLoots(uint Entry)
        {
            if (!_Creature_loots.ContainsKey(Entry))
            {
                Log.Debug("WorldMgr", "Loading Loots of " + Entry + " ...");

                List<Creature_loot> Loots = new List<Creature_loot>();
                IList<Creature_loot> ILoots = Database.SelectObjects<Creature_loot>("Entry=" + Entry);
                foreach (Creature_loot Loot in ILoots)
                    Loots.Add(Loot);

                _Creature_loots.Add(Entry, Loots);

                long MissingCreature = 0;
                long MissingItemProto = 0;

                if (GetCreatureProto(Entry) == null)
                {
                    Log.Debug("LoadLoots", "[" + Entry + "] Invalid Creature Proto");
                    _Creature_loots.Remove(Entry);
                    ++MissingCreature;
                }

                foreach (Creature_loot Loot in _Creature_loots[Entry].ToArray())
                {
                    Loot.Info = GetItem_Info(Loot.ItemId);

                    if (Loot.Info == null)
                    {
                        Log.Debug("LoadLoots", "[" + Loot.ItemId + "] Invalid Item Info");
                        _Creature_loots[Entry].Remove(Loot);
                        ++MissingItemProto;
                    }
                }

                if (MissingItemProto > 0)
                    Log.Error("LoadLoots", "[" + MissingItemProto + "] Missing Item Info");

                if (MissingCreature > 0)
                    Log.Error("LoadLoots", "[" + MissingCreature + "] Misssing Creature proto");

                Log.Success("LoadCreatureLoots", "Loaded " + _Creature_loots[Entry].Count + " loots of : " + Entry);
            }
        }
        static public List<Creature_loot> GetLoots(uint Entry)
        {
            LoadLoots(Entry);

            List<Creature_loot> Loots;

            if (!_Creature_loots.TryGetValue(Entry,out Loots))
                Loots = new List<Creature_loot>();

            return Loots;
        }

        #endregion

        #region GameObjects

        static public Dictionary<uint, GameObject_proto> GameObjectProtos;
        static public Dictionary<uint, GameObject_spawn> GameObjectSpawns;

        [LoadingFunction(true)]
        static public void LoadGameObjectProtos()
        {
            Log.Debug("WorldMgr", "Loading GameObject_Protos...");

            GameObjectProtos = new Dictionary<uint, GameObject_proto>();

            IList<GameObject_proto> Protos = Database.SelectAllObjects<GameObject_proto>();

            foreach (GameObject_proto Proto in Protos)
                    GameObjectProtos.Add(Proto.Entry, Proto);

            Log.Success("WorldMgr", "Loaded " + GameObjectProtos.Count + " GameObject_Protos");
        }

        static public GameObject_proto GetGameObjectProto(uint Entry)
        {
            GameObject_proto Proto;
            GameObjectProtos.TryGetValue(Entry, out Proto);
            return Proto;
        }

        [LoadingFunction(true)]
        static public void LoadGameObjectSpawns()
        {
            Log.Debug("WorldMgr", "Loading GameObject_Spawns...");

            GameObjectSpawns = new Dictionary<uint, GameObject_spawn>();

            IList<GameObject_spawn> Spawns = Database.SelectAllObjects<GameObject_spawn>();

            foreach (GameObject_spawn Spawn in Spawns)
                GameObjectSpawns.Add(Spawn.Guid, Spawn);

            Log.Success("WorldMgr", "Loaded " + GameObjectSpawns.Count + " GameObject_Spawns");
        }

        #endregion

        #region Vendors

        static public int MAX_ITEM_PAGE = 30;

        static public Dictionary<uint, List<Creature_vendor>> _Vendors = new Dictionary<uint, List<Creature_vendor>>();
        static public List<Creature_vendor> GetVendorItems(uint Entry)
        {
            if (!_Vendors.ContainsKey(Entry))
            {
                Log.Info("WorldMgr", "Loading Vendors of " + Entry +" ...");

                IList<Creature_vendor> IVendors = Database.SelectObjects<Creature_vendor>("Entry="+Entry);
                List<Creature_vendor> Vendors = new List<Creature_vendor>();
                Vendors.AddRange(IVendors);

                _Vendors.Add(Entry, Vendors);

                foreach (Creature_vendor Info in Vendors.ToArray())
                {
                    if ((Info.Info = GetItem_Info(Info.ItemId)) == null)
                    {
                        Vendors.Remove(Info);
                        continue;
                    }

                    foreach (KeyValuePair<uint, UInt16> Kp in Info.ItemsReq)
                    {
                        Item_Info Req = GetItem_Info(Kp.Key);
                        if (Req != null)
                            Info.ItemsReqInfo.Add(Kp.Value, Req);
                    }
                }

                Log.Success("LoadCreatureVendors", "Loaded " + Vendors.Count + " Vendors of " + Entry);
            }

            return _Vendors[Entry];
        }

        static public void SendVendor(Player Plr, uint Entry)
        {
            if (Plr == null)
                return;

            List<Creature_vendor> Items = GetVendorItems(Entry).ToList();
            byte Page = 0;
            int Count = Items.Count;
            while (Count > 0)
            {
                byte ToSend = (byte)Math.Min(Count, MAX_ITEM_PAGE);
                Log.Success("SendVendor", "ToSend=" + ToSend + ",Max=" + Count);
                if (ToSend <= Count)
                    Count -= ToSend;
                else
                    Count = 0;

                SendVendorPage(Plr, ref Items, ToSend, Page);

                ++Page;
            }

            Plr.ItmInterface.SendBuyBack();
        }
        static public void SendVendorPage(Player Plr, ref List<Creature_vendor> Vendors, byte Count,byte Page)
        {
            Count = (byte)Math.Min(Count, Vendors.Count);

            Log.Success("SendVendorPage", "Count=" + Count + ",Page=" + Page + ",ItmC=" + Vendors.Count);

            PacketOut Out = new PacketOut((byte)Opcodes.F_INIT_STORE);
            Out.WriteByte(0);
            Out.WriteByte(Page);
            Out.WriteByte(Count);
            Out.WriteByte((byte)(Page > 0 ? 0 : 1));
            Out.WriteByte(1);
            Out.WriteByte(0);

            for (byte i = 0; i < Count; ++i)
            {
                Out.WriteByte(i);
                Out.WriteByte(3);
                Out.WriteUInt32(Vendors[i].Price);
                Item.BuildItem(ref Out, null, Vendors[i].Info, 0, 1);

                Out.WriteByte((byte)Vendors[i].ItemsReqInfo.Count); // ReqItemSize
                foreach (KeyValuePair<UInt16, Item_Info> Kp in Vendors[i].ItemsReqInfo)
                {
                    Out.WriteUInt32(Kp.Value.Entry);
                    Out.WriteUInt16((UInt16)Kp.Value.ModelId);
                    Out.WritePascalString(Kp.Value.Name);
                    Out.WriteUInt16(Kp.Key);
                    Out.Fill(0, 18);
                }
            }

            Out.WriteByte(0);
            Plr.SendPacket(Out);

            Vendors.RemoveRange(0, Count);
        }

        static public void BuyItemVendor(Player Plr, InteractMenu Menu,uint Entry)
        {
            int Num = (Menu.Page * MAX_ITEM_PAGE) + Menu.Num;            
            ushort Count = (ushort)(Menu.Count > 0 ? Menu.Count : 1);
            List<Creature_vendor> Vendors = GetVendorItems(Entry);

            Log.Success("BuyItemVendor", "Count=" + Count + ",Num=" + Num + ",Size=" + Vendors.Count);

            if (Vendors.Count <= Num)
                return;

            if (!Plr.HaveMoney((Vendors[Num].Price) * Count))
            {
                Plr.SendLocalizeString("", GameData.Localized_text.TEXT_MERCHANT_INSUFFICIENT_MONEY_TO_BUY);
                return;
            }

            foreach (KeyValuePair<UInt16, Item_Info> Kp in Vendors[Num].ItemsReqInfo)
            {
                if (!Plr.ItmInterface.HasItemCount(Kp.Value.Entry, Kp.Key))
                {
                    Plr.SendLocalizeString("", GameData.Localized_text.TEXT_MERCHANT_FAIL_PURCHASE_REQUIREMENT);
                    return;
                }
            }

            ItemError Error = Plr.ItmInterface.CreateItem(Vendors[Num].Info, Count);
            if (Error == ItemError.RESULT_OK)
            {
                Plr.RemoveMoney((Vendors[Num].Price) * Count);
                foreach (KeyValuePair<UInt16, Item_Info> Kp in Vendors[Num].ItemsReqInfo)
                    Plr.ItmInterface.RemoveItem(Kp.Value.Entry, Kp.Key);
            }
            else if (Error == ItemError.RESULT_MAX_BAG)
            {
                Plr.SendLocalizeString("", GameData.Localized_text.TEXT_MERCHANT_INSUFFICIENT_SPACE_TO_BUY);
            }
            else if (Error == ItemError.RESULT_ITEMID_INVALID)
            {
                
            }
        }

        #endregion

        #region Quests

        static public Dictionary<ushort, Quest> _Quests;

        [LoadingFunction(true)]
        static public void LoadQuests()
        {
            _Quests = new Dictionary<ushort, Quest>();

            IList<Quest> Quests = Database.SelectAllObjects<Quest>();

            if (Quests != null)
                foreach (Quest Q in Quests)
                    _Quests.Add(Q.Entry, Q);

            Log.Success("LoadQuests", "Loaded " + _Quests.Count + " Quests");
        }
        static public Quest GetQuest(ushort QuestID)
        {
            Quest Q;
            _Quests.TryGetValue(QuestID, out Q);
            return Q;
        }

        static public Dictionary<int, Quest_Objectives> _Objectives;

        [LoadingFunction(true)]
        static public void LoadQuestsObjectives()
        {
            _Objectives = new Dictionary<int, Quest_Objectives>();

            IList<Quest_Objectives> Objectives = Database.SelectAllObjects<Quest_Objectives>();

            if (Objectives != null)
                foreach (Quest_Objectives Obj in Objectives)
                    _Objectives.Add(Obj.Guid, Obj);

            Log.Success("LoadQuestsObjectives", "Loaded " + _Objectives.Count + " Quests Objectives");
        }

        static public void GenerateObjectif(Quest_Objectives Obj, Quest Q)
        {
            switch ((Objective_Type)Obj.ObjType)
            {
                case Objective_Type.QUEST_KILL_PLAYERS:
                    {
                        if(Obj.Description.Length < 1)
                            Obj.Description = "Enemy Players";
                    }break;

                case Objective_Type.QUEST_SPEACK_TO:
                    goto case Objective_Type.QUEST_KILL_MOB;

                case Objective_Type.QUEST_KILL_MOB:
                    {
                        uint ObjID = 0;
                        uint.TryParse(Obj.ObjID, out ObjID);

                        if(ObjID != 0)
                            Obj.Creature = GetCreatureProto(ObjID);

                        if (Obj.Description.Length < 1 && Obj.Creature != null)
                            Obj.Description = Obj.Creature.Name;
                    } break;

                case Objective_Type.QUEST_GET_ITEM:
                    {
                        uint ObjID = 0;
                        uint.TryParse(Obj.ObjID, out ObjID);

                        if (ObjID != 0)
                            Obj.Item = GetItem_Info(ObjID);
                    }
                    break;
            };
        }

        static public Quest_Objectives GetQuestObjective(int Guid)
        {
            Quest_Objectives Obj;
            _Objectives.TryGetValue(Guid, out Obj);
            return Obj;
        }

        static public Dictionary<uint, List<Quest>> _CreatureStarter;

        [LoadingFunction(true)]
        static public void LoadQuestCreatureStarter()
        {
            _CreatureStarter = new Dictionary<uint, List<Quest>>();

            IList<Quest_Creature_Starter> Starters = Database.SelectAllObjects<Quest_Creature_Starter>();

            if (Starters != null)
            {
                foreach (Quest_Creature_Starter Start in Starters)
                {
                    if (!_CreatureStarter.ContainsKey(Start.CreatureID))
                        _CreatureStarter.Add(Start.CreatureID, new List<Quest>());

                    Quest Q = GetQuest(Start.Entry);

                    if(Q != null)
                        _CreatureStarter[Start.CreatureID].Add(Q);
                }
            }

            Log.Success("LoadCreatureQuests", "Loaded " + _CreatureStarter.Count + " Quests Creature Starter");
        }
        static public List<Quest> GetStartQuests(UInt32 CreatureID)
        {
            List<Quest> Quests;
            _CreatureStarter.TryGetValue(CreatureID, out Quests);
            return Quests;
        }

        static public Dictionary<uint, List<Quest>> _CreatureFinisher;

        [LoadingFunction(true)]
        static public void LoadQuestCreatureFinisher()
        {
            _CreatureFinisher = new Dictionary<uint, List<Quest>>();

            IList<Quest_Creature_Finisher> Finishers = Database.SelectAllObjects<Quest_Creature_Finisher>();

            if (Finishers != null)
            {
                foreach (Quest_Creature_Finisher Finisher in Finishers)
                {
                    if (!_CreatureFinisher.ContainsKey(Finisher.CreatureID))
                        _CreatureFinisher.Add(Finisher.CreatureID, new List<Quest>());

                    Quest Q = GetQuest(Finisher.Entry);

                    if (Q != null)
                        _CreatureFinisher[Finisher.CreatureID].Add(Q);
                }
            }

            Log.Success("LoadCreatureQuests", "Loaded " + _CreatureFinisher.Count + " Quests Creature Finisher");
        }
        static public List<Quest> GetFinishersQuests(UInt32 CreatureID)
        {
            List<Quest> Quests;
            _CreatureFinisher.TryGetValue(CreatureID, out Quests);
            return Quests;
        }

        #endregion

        #region Relation

        static public Dictionary<UInt16, CellSpawns[,]> _RegionCells = new Dictionary<ushort, CellSpawns[,]>();
        static public CellSpawns GetRegionCell(ushort RegionId, UInt16 X, UInt16 Y)
        {
            X = (UInt16)Math.Min(RegionMgr.MAX_CELL_ID - 1, X);
            Y = (UInt16)Math.Min(RegionMgr.MAX_CELL_ID - 1, Y);

            if (!_RegionCells.ContainsKey(RegionId))
                _RegionCells.Add(RegionId, new CellSpawns[RegionMgr.MAX_CELL_ID, RegionMgr.MAX_CELL_ID]);

            if (_RegionCells[RegionId][X, Y] == null)
            {
                CellSpawns Sp = new CellSpawns(RegionId, X, Y);
                _RegionCells[RegionId][X, Y] = Sp;
            }

            return _RegionCells[RegionId][X, Y];
        }
        static public CellSpawns[,] GetCells(UInt16 RegionId)
        {
            if (!_RegionCells.ContainsKey(RegionId))
                _RegionCells.Add(RegionId, new CellSpawns[RegionMgr.MAX_CELL_ID, RegionMgr.MAX_CELL_ID]);

            return _RegionCells[RegionId];
        }

        [LoadingFunction(false)]
        static public void LoadRelation()
        {
            Log.Success("LoadRelation", "Loading Relations");

            LoadRegionSpawns();
            LoadChapters();
            LoadPublicQuests();
            LoadQuestsRelation();
        }

        static public void LoadRegionSpawns()
        {
            long InvalidSpawns = 0;
            Zone_Info Info = null;
            ushort X, Y = 0;
            Dictionary<string, int> RegionCount = new Dictionary<string, int>();
            foreach (Creature_spawn Spawn in CreatureSpawns.Values)
            {
                Spawn.Proto = GetCreatureProto(Spawn.Entry);
                if (Spawn.Proto == null)
                {
                    Log.Debug("LoadRegionSpawns", "Invalid Creature Proto (" + Spawn.Entry + "), spawn Guid(" + Spawn.Guid + ")");
                    ++InvalidSpawns;
                    continue;
                }

                Info = GetZone_Info(Spawn.ZoneId);
                if (Info != null)
                {
                    X = (UInt16)(Spawn.WorldX >> 12);
                    Y = (UInt16)(Spawn.WorldY >> 12);

                    GetRegionCell(Info.Region,X, Y).AddSpawn(Spawn);

                    if (!RegionCount.ContainsKey(Info.Name))
                        RegionCount.Add(Info.Name, 0);

                    ++RegionCount[Info.Name];
                }
                else
                {
                    Log.Debug("LoadRegionSpawns", "ZoneId (" + Spawn.ZoneId + ") invalid, Spawn Guid(" + Spawn.Guid + ")");
                    ++InvalidSpawns;
                }
            }

            foreach (GameObject_spawn Spawn in GameObjectSpawns.Values)
            {
                Spawn.Proto = GetGameObjectProto(Spawn.Entry);
                if (Spawn.Proto == null)
                {
                    Log.Debug("LoadRegionSpawns", "Invalid GameObject Proto (" + Spawn.Entry + "), spawn Guid(" + Spawn.Guid + ")");
                    ++InvalidSpawns;
                    continue;
                }

                Info = GetZone_Info(Spawn.ZoneId);
                if (Info != null)
                {
                    X = (UInt16)(Spawn.WorldX >> 12);
                    Y = (UInt16)(Spawn.WorldY >> 12);

                    GetRegionCell(Info.Region, X, Y).AddSpawn(Spawn);

                    if (!RegionCount.ContainsKey(Info.Name))
                        RegionCount.Add(Info.Name, 0);

                    ++RegionCount[Info.Name];
                }
                else
                {
                    Log.Debug("LoadRegionSpawns", "ZoneId (" + Spawn.ZoneId + ") invalid, Spawn Guid(" + Spawn.Guid + ")");
                    ++InvalidSpawns;
                }
            }

            if (InvalidSpawns > 0)
                Log.Error("LoadRegionSpawns", "[" + InvalidSpawns + "] Invalid Spawns");

            foreach (KeyValuePair<string, int> Counts in RegionCount)
                Log.Success("Region", "[" + Counts.Key + "] : " + Counts.Value);
        }
        static public void LoadChapters()
        {
            Log.Success("LoadChapters", "Loading Zone from Chapters");

            long InvalidChapters = 0;

            Zone_Info Zone = null;
            foreach (Chapter_Info Info in _Chapters.Values)
            {
                Zone = GetZone_Info(Info.ZoneId);

                if (Zone == null || (Info.PinX <= 0 && Info.PinY <= 0))
                {
                    Log.Debug("LoadChapters", "Chapter (" + Info.Entry + ")[" + Info.Name + "] Invalid");
                    ++InvalidChapters;
                }
                else
                {
                    List<Chapter_Reward> Rewards;
                    if (_Chapters_Reward.TryGetValue(Info.Entry, out Rewards))
                        Info.Rewards = Rewards;
                    else
                        Info.Rewards = new List<Chapter_Reward>();

                    foreach (Chapter_Reward Reward in Info.Rewards.ToArray())
                    {
                        Reward.Item = GetItem_Info(Reward.ItemId);
                        Reward.Chapter = Info;

                        if (Reward.Item == null)
                            Info.Rewards.Remove(Reward);
                    }

                    GetRegionCell(Zone.Region, (ushort)((float)(Info.PinX / 4096) + Zone.OffX), (ushort)((float)(Info.PinY / 4096) + Zone.OffY)).AddChapter(Info);
                }

            }

            if (InvalidChapters > 0)
                Log.Error("LoadChapters", "[" + InvalidChapters + "] Invalid Chapter(s)");
        }
        static public void LoadPublicQuests()
        {
            Zone_Info Zone = null;
            foreach (PQuest_Info Info in _PQuests.Values)
            {
                Zone = GetZone_Info(Info.ZoneId);
                if (Zone == null)
                    continue;


                if (!_PQuest_Objectives.TryGetValue(Info.Entry, out Info.Objectives))
                    Info.Objectives = new List<PQuest_Objective>();
                else
                {
                    foreach (PQuest_Objective Obj in Info.Objectives)
                    {
                        Obj.Quest = Info;
                        GeneratePQuestObjective(Obj, Obj.Quest);
                    }
                }

                GetRegionCell(Zone.Region, (ushort)((float)(Info.PinX / 4096) + Zone.OffX), (ushort)((float)(Info.PinY / 4096) + Zone.OffY)).AddPQuest(Info);
            }
        }
        static public void LoadQuestsRelation()
        {
            LoadQuestCreatureStarter();
            LoadQuestCreatureFinisher();

            foreach (Quest_Objectives Obj in _Objectives.Values)
            {
                Quest Q = GetQuest(Obj.Entry);
                if (Q == null)
                    continue;

                GenerateObjectif(Obj, Q);

                Obj.num = (byte)(Q.Objectives.Count+1);
                Obj.Quest = Q;
                Q.Objectives.Add(Obj);
            }

            foreach (Quest Q in _Quests.Values)
            {
                if (Q.Choice.Length <= 0)
                    continue;

                // [5154,12],[128,1]
                string[] Rewards = Q.Choice.Split('[');
                foreach (string Reward in Rewards)
                {
                    if (Reward.Length <= 0)
                        continue;

                    string sItemID = Reward.Substring(0, Reward.IndexOf(','));
                    string sCount = Reward.Substring(sItemID.Length+1, Reward.IndexOf(']') - sItemID.Length - 1);

                    uint ItemID = uint.Parse(sItemID);
                    uint Count = uint.Parse(sCount);

                    Item_Info Info = GetItem_Info(ItemID);
                    if (Info == null)
                        continue;

                    if (!Q.Rewards.ContainsKey(Info))
                        Q.Rewards.Add(Info, Count);
                    else
                        Q.Rewards[Info] += Count;
                }
            }
        }
        static public void RemoveDoubleSpawns()
        {
            uint Space = 250;
            int[] Removed = new int[255];

            Zone_Info Info = null;
            foreach (CellSpawns[,] Cell in _RegionCells.Values)
            {
                for (int i = 0; i < Cell.GetLength(0); ++i)
                {
                    for (int y = 0; y < Cell.GetLength(1); ++y)
                    {
                        if (Cell[i, y] == null)
                            continue;

                        foreach (Creature_spawn Sp in Cell[i, y].CreatureSpawns.ToArray())
                        {
                            if (Sp.Proto == null || GetStartQuests(Sp.Proto.Entry) != null)
                                continue;

                            if (Info == null || Info.ZoneId != Sp.ZoneId)
                                Info = GetZone_Info(Sp.ZoneId);

                            foreach (Creature_spawn SubSp in Cell[i, y].CreatureSpawns.ToArray())
                            {
                                if (SubSp.Proto == null || Sp.Entry != SubSp.Entry || Sp == SubSp)
                                    continue;

                                if (ZoneMgr.CalculPin(Info, Sp.WorldX, true) > ZoneMgr.CalculPin(Info, SubSp.WorldX, true) + Space || ZoneMgr.CalculPin(Info, Sp.WorldX, true) < ZoneMgr.CalculPin(Info, SubSp.WorldX, true) - Space)
                                    continue;

                                if (ZoneMgr.CalculPin(Info, Sp.WorldY, false) > ZoneMgr.CalculPin(Info, SubSp.WorldY, false) + Space || ZoneMgr.CalculPin(Info, Sp.WorldY, false) < ZoneMgr.CalculPin(Info, SubSp.WorldY, false) - Space)
                                    continue;

                                /*if (Sp.WorldZ > SubSp.WorldZ/2 + Space || Sp.WorldZ < SubSp.WorldZ/2 - Space)
                                    continue;*/

                                Removed[SubSp.ZoneId]++;
                                SubSp.Proto = null;
                                Database.DeleteObject(SubSp);
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < 255; ++i)
            {
                if (Removed[i] != 0)
                    Log.Info("Removed", "Zone : " + i + " : " + Removed[i]);
            }
        }

        #endregion
    }
}

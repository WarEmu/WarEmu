
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Reflection;

using Common;
using FrameWork;

namespace WorldServer
{
    static public class WorldMgr
    {
        static public MySQLObjectDatabase Database;
        static public bool FastLoading = false;

        #region Zones

        static public List<Zone_Info> _Zone_Info;
        static public Dictionary<UInt32, Zone_jump> Zone_Jumps;

        [LoadingFunction(true)]
        static public void LoadZone_Info()
        {
            Log.Debug("WorldMgr", "Loading Zone_Info...");

            _Zone_Info = Database.SelectAllObjects<Zone_Info>() as List<Zone_Info>;

            Log.Success("LoadZone_Info", "Loaded " + _Zone_Info.Count + " Zone_Info");

            LoadZoneJumps();
        }
        static public Zone_Info GetZone_Info(UInt16 ZoneId)
        {
            return _Zone_Info.Find(zone => zone != null && zone.ZoneId == ZoneId);
        }
        static public List<Zone_Info> GetZoneRegion(UInt16 RegionId)
        {
            return _Zone_Info.FindAll(zone => zone != null && zone.Region == RegionId);
        }
        static public Zone_Info GetZoneFromOffsets(int OffsetX, int OffsetY)
        {
            foreach (Zone_Info Info in _Zone_Info)
            {
                if (OffsetX >= Info.OffX && OffsetX < Info.OffX + 16
                    && OffsetY >= Info.OffY && OffsetY < Info.OffY + 16)
                {
                    return Info;
                }
            }
 
            return null;
        }

        [LoadingFunction(true)]
        static public void LoadZoneJumps()
        {
            Log.Debug("WorldMgr", "Loading Zone_Jump...");

            Zone_Jumps = new Dictionary<uint, Zone_jump>();
            IList<Zone_jump> Jumps = Database.SelectAllObjects<Zone_jump>() as List<Zone_jump>;

            foreach (Zone_jump Jump in Jumps)
            {
		        Jump.ZoneInfo = GetZone_Info(Jump.ZoneID);

                if (Jump.ZoneInfo != null && !Zone_Jumps.ContainsKey(Jump.Entry))
		        {
                    Zone_Jumps.Add(Jump.Entry, Jump);
		        }
		        else
			        Log.Error("Zone_Jump", "Invalid Jump: " + Jump.Entry + ", Zone=" + Jump.ZoneID);
            }

            Log.Success("LoadZone_Jump", "Loaded " + Zone_Jumps.Count + " Zone_Jump");
        }

        static public Zone_jump GetZoneJump(uint Entry)
        {
            Zone_jump jump;
            Zone_Jumps.TryGetValue(Entry, out jump);
            return jump;
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
                AddZoneArea(Area);
            }

            Log.Success("LoadZone_Info", "Loaded " + Infos.Count + " Zone_Area && " + PieceInformation + " Piece Informations");
        }

        static public void AddZoneArea(Zone_Area Area)
        {
            List<Zone_Area> Areas;
            if (!_Zone_Area.TryGetValue(Area.ZoneId, out Areas))
            {
                Areas = new List<Zone_Area>();
                _Zone_Area.Add(Area.ZoneId, Areas);
            }

            Areas.Add(Area);
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

            Zone_Taxi[] Taxis;
            foreach (KeyValuePair<ushort,Zone_Taxi[]> Kp in WorldMgr._Zone_Taxi)
            {
                Taxis = Kp.Value;
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

            foreach (KeyValuePair<uint,Chapter_Info> Kp in _Chapters)
                if (Kp.Value.ZoneId == ZoneId)
                    Chapters.Add(Kp.Value);

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

        static public Dictionary<ushort, Tok_Info> _Toks;
        static public List<Tok_Info> DiscoveringToks;

        [LoadingFunction(true)]
        static public void LoadTok_Infos()
        {
            Log.Debug("WorldMgr", "Loading LoadTok_Infos...");

            _Toks = new Dictionary<ushort, Tok_Info>();

            IList<Tok_Info> IToks = Database.SelectAllObjects<Tok_Info>();
            DiscoveringToks = new List<Tok_Info>();

            if (IToks != null)
            {
                foreach (Tok_Info Info in IToks)
                {
                    _Toks.Add(Info.Entry, Info);
                    if (Info.EventName.Contains("discovered") || Info.EventName.Contains("unlocked"))
                    {
                        DiscoveringToks.Add(Info);
                    }
                }
            }

            Log.Success("LoadTok_Infos", "Loaded " + _Toks.Count + " Tok_Infos");
        }
        static public Tok_Info GetTok(ushort Entry)
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
            int i;

            List<Item_Info> Items = Database.SelectAllObjects<Item_Info>() as List<Item_Info>;
            foreach (Item_Info Info in Items)
            {
                foreach (KeyValuePair<byte, ushort> Kp in Info._Stats)
                {
                    if (Kp.Key >= byte.MaxValue || Kp.Value >= ushort.MaxValue)
                    {
                        Info.Stats = "";
                        Info.SellPrice = 0;
                        Info._Stats.Clear();
                        break;
                    }
                }
                foreach (KeyValuePair<uint, uint> Kp in Info._Spells)
                {
                    if (Kp.Key >= uint.MaxValue || Kp.Value >= uint.MaxValue)
                    {
                        Info.Spells = "";
                        Info.SellPrice = 0;
                        Info._Spells.Clear();
                        break;
                    }
                }
 
                foreach (KeyValuePair<byte, ushort> Kp in Info._Crafts)
                {
                    if (Kp.Key >= byte.MaxValue || Kp.Value >= ushort.MaxValue)
                    {
                        Info.Crafts = "";
                        Info.SellPrice = 0;
                        Info._Crafts.Clear();
                        break;
                    }
                }

                if (Info.Speed != 0 && Info.Dps == 0)
                {
                    Info.SellPrice = 0;
                    Info.Speed = 0;
                }
                else if (Info.Dps != 0 && Info.Speed == 0)
                {
                    Info.Dps = 0;
                    Info.SellPrice = 0;
                }

                if (Info.Unk27[4] != 3 || Info.Unk27[5] != 2)
                {
                    for (i = 0; i < Info.Unk27.Length; ++i)
                    {
                        Info.Unk27[i] = 0;
                    }

                    Info.Unk27[4] = 3;
                    Info.Unk27[5] = 2;
                }

                _Item_Info.Add(Info.Entry, Info);
            }

            Log.Success("LoadItem_Info", "Loaded " + _Item_Info.Count + " Item_Info");

            foreach (Item_Info Info in Items)
            {
                Info.RequiredItems = new List<KeyValuePair<Item_Info, ushort>>(Info._SellRequiredItems.Count);
                foreach (KeyValuePair<UInt32, UInt16> Kp in Info._SellRequiredItems)
                {
                    Info.RequiredItems.Add(new KeyValuePair<Item_Info, ushort>(GetItem_Info(Kp.Key), Kp.Value));
                }
            }
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

        static public uint GenerateXPCount(Player Plr, Unit Victim)
        {
            UInt32 KLvl = Plr.Level;
            UInt32 VLvl = Victim.Level;

            if (KLvl > VLvl + 8)
                return 0;

            UInt32 XP = VLvl * 70;
            XP += (UInt32)Victim.Rank * 50;

            if (KLvl > VLvl)
                XP -= (UInt32)(((float)XP / (float)100) * (KLvl - VLvl + 1)) * 5;

            if (Program.Config.XpRate > 0)
                XP *= (UInt32)Program.Config.XpRate;

            return XP;
        }

        static public void GenerateXP(Unit Killer, Unit Victim)
        {
             if (Killer.IsPlayer())
            {
                Player Plr = Killer.GetPlayer();

                if (Plr.GetGroup() == null)
                    Plr.AddXp(GenerateXPCount(Plr, Victim));
                else
                    Plr.GetGroup().AddXp(Plr, Victim);
            }
        }

        #endregion

        #region Renown_Info

        static private Dictionary<byte, Renown_Info> _Renown_Infos;

        [LoadingFunction(true)]
        static public void LoadRenown_Info()
        {
            Log.Debug("WorldMgr", "Loading Renown_Info...");

            _Renown_Infos = new Dictionary<byte, Renown_Info>();
            foreach (Renown_Info Info in Database.SelectAllObjects<Renown_Info>())
                _Renown_Infos.Add(Info.Level, Info);

            Log.Success("LoadRenown_Info", "Loaded " + _Renown_Infos.Count + " Renown_Info");
        }

        static public Renown_Info GetRenown_Info(byte Level)
        {
            if (_Renown_Infos.ContainsKey(Level))
                return _Renown_Infos[Level];
            else return null;
        }

        static public uint GenerateRenownCount(Player Killer, Player Victim)
        {
            if (Killer == null || Victim == null || Killer == Victim)
                return 0;

            UInt32 VRp = Victim._Value.RenownRank;
            UInt32 VLvl = Victim.Level;

            UInt32 RP = VRp * 4 + VLvl * 6;

            if (Program.Config.RenownRate > 0)
                RP *= (UInt32)Program.Config.RenownRate;

            return RP;
        }

        static public void GenerateRenown(Player Killer, Player Victim)
        {
            if (Killer == null || Victim == null || Killer == Victim)
                return;

            if (Killer.GetGroup() == null)
            {
                Killer.AddRenown(GenerateRenownCount(Killer, Victim));
            }
            else
            {
                Killer.GetGroup().AddRenown(Killer, Victim);
            }  
        }

        #endregion

        #region CreatureProto

        static public Dictionary<uint, Creature_proto> CreatureProtos;

        [LoadingFunction(true)]
        static public void LoadCreatureProto()
        {
            Log.Debug("WorldMgr", "Loading Creature_Protos...");

            CreatureProtos = new Dictionary<uint, Creature_proto>();

            if (FastLoading)
                return;

            IList<Creature_proto> Protos = Database.SelectAllObjects<Creature_proto>();

            if (Protos != null)
                foreach (Creature_proto Proto in Protos)
                {
                   if(Proto.Model1 == 0 && Proto.Model2 == 0)
                        Proto.Model1 = Proto.Model2 = 1;

                   if (Proto.MinLevel == Proto.MaxLevel && Proto.MinLevel > 1)
                       Proto.MaxLevel = (byte)(Proto.MinLevel + 1);
                   else if (Proto.MaxLevel - Proto.MinLevel > 3)
                       Proto.MaxLevel = (byte)(Proto.MinLevel + 2);

                    CreatureProtos.Add(Proto.Entry, Proto);
                }

           Log.Success("LoadCreatureProto", "Loaded " + CreatureProtos.Count + " Creature_Protos");
        }

        static public Creature_proto GetCreatureProto(uint Entry)
        {
            Creature_proto Proto;
            CreatureProtos.TryGetValue(Entry, out Proto);
            return Proto;
        }

        static public Creature_proto GetCreatureProtoByName(string Name)
        {
            foreach (KeyValuePair<uint, Creature_proto> Kp in CreatureProtos)
                if (Kp.Value.Name.ToLower() == Name.ToLower())
                    return Kp.Value;
            return null;
        }

        #endregion

        #region CreatureSpawns

        static public Dictionary<uint, Creature_spawn> CreatureSpawns;
        static public int MaxCreatureGUID = 0;

        static public int GenerateCreatureSpawnGUID()
        {
            return System.Threading.Interlocked.Increment(ref MaxCreatureGUID);
        }

        [LoadingFunction(true)]
        static public void LoadCreatureSpawns()
        {
            Log.Debug("WorldMgr", "Loading Creature_Spawns...");

            if (FastLoading)
                return;

            CreatureSpawns = new Dictionary<uint, Creature_spawn>();
            IList<Creature_spawn> Spawns = Database.SelectAllObjects<Creature_spawn>();

            if(Spawns != null)
                foreach (Creature_spawn Spawn in Spawns)
                {
                    CreatureSpawns.Add(Spawn.Guid, Spawn);
                    if (Spawn.Guid > MaxCreatureGUID)
                        MaxCreatureGUID = (int)Spawn.Guid;
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

            if (FastLoading)
                return;

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
            List<Creature_item> L;

            lock (_CreatureItems)
            {
                if (!_CreatureItems.TryGetValue(Entry, out L))
                {
                    L = new List<Creature_item>();
                    _CreatureItems.Add(Entry, L);
                }
            }

            return L;
        }

        static public void RemoveCreatureItem(uint Entry, ushort Slot)
        {
            List<Creature_item> Items = GetCreatureItems(Entry);
            Items.RemoveAll(info =>
            {
                if (info.SlotId == Slot)
                {
                    WorldMgr.Database.DeleteObject(info);
                    return true;
                }
                return false;
            });
        }

        static public void AddCreatureItem(Creature_item Item)
        {
            RemoveCreatureItem(Item.Entry, Item.SlotId);

            List<Creature_item> Items = GetCreatureItems(Item.Entry);
            Items.Add(Item);
            Database.AddObject(Item);
        }

        #endregion

        #region CreatureText

        static public Dictionary<uint, List<Creature_text>> _CreatureTexts = new Dictionary<uint, List<Creature_text>>();

        [LoadingFunction(true)]
        static public void LoadCreatureTexts()
        {
            _CreatureTexts = new Dictionary<uint, List<Creature_text>>();

            Log.Debug("WorldMgr", "Loading Creature_texts...");

            if (FastLoading)
                return;

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
            if (FastLoading)
                return;

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

                //Log.Success("LoadCreatureLoots", "Loaded " + _Creature_loots[Entry].Count + " loots of : " + Entry);
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

        #region GameObjectLoots

        static public Dictionary<uint, List<GameObject_loot>> _GameObject_loots = new Dictionary<uint, List<GameObject_loot>>();
        static private void LoadGameObjectLoots(uint Entry)
        {
            if (FastLoading)
                return;

            if (!_GameObject_loots.ContainsKey(Entry))
            {
                Log.Debug("WorldMgr", "Loading GameObject Loots of " + Entry + " ...");

                List<GameObject_loot> Loots = new List<GameObject_loot>();
                IList<GameObject_loot> ILoots = Database.SelectObjects<GameObject_loot>("Entry=" + Entry);
                foreach (GameObject_loot Loot in ILoots)
                    Loots.Add(Loot);

                _GameObject_loots.Add(Entry, Loots);

                long MissingGameObject = 0;
                long MissingItemProto = 0;

                if (GetGameObjectProto(Entry) == null)
                {
                    Log.Debug("LoadLoots", "[" + Entry + "] Invalid GameObject Proto");
                    _Creature_loots.Remove(Entry);
                    ++MissingGameObject;
                }

                foreach (GameObject_loot Loot in _GameObject_loots[Entry].ToArray())
                {
                    Loot.Info = GetItem_Info(Loot.ItemId);

                    if (Loot.Info == null)
                    {
                        Log.Debug("LoadLoots", "[" + Loot.ItemId + "] Invalid Item Info");
                        _GameObject_loots[Entry].Remove(Loot);
                        ++MissingItemProto;
                    }
                }

                if (MissingItemProto > 0)
                    Log.Error("LoadLoots", "[" + MissingItemProto + "] Missing Item Info");

                if (MissingGameObject > 0)
                    Log.Error("LoadLoots", "[" + MissingGameObject + "] Misssing GameObject proto");
            }
        }
        static public List<GameObject_loot> GetGameObjectLoots(uint Entry)
        {
            LoadGameObjectLoots(Entry);

            List<GameObject_loot> Loots;

            if (!_GameObject_loots.TryGetValue(Entry, out Loots))
                Loots = new List<GameObject_loot>();

            return Loots;
        }
        #endregion

        #region GameObjects

        static public Dictionary<uint, GameObject_proto> GameObjectProtos;
        static public Dictionary<uint, GameObject_spawn> GameObjectSpawns;
        static public int MaxGameObjectGUID = 0;

        static public int GenerateGameObjectSpawnGUID()
        {
            return System.Threading.Interlocked.Increment(ref MaxGameObjectGUID);
        }

        [LoadingFunction(true)]
        static public void LoadGameObjectProtos()
        {
            Log.Debug("WorldMgr", "Loading GameObject_Protos...");

            GameObjectProtos = new Dictionary<uint, GameObject_proto>();

            if (FastLoading)
                return;

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

            if (FastLoading)
                return;

            IList<GameObject_spawn> Spawns = Database.SelectAllObjects<GameObject_spawn>();

            foreach (GameObject_spawn Spawn in Spawns)
            {
                GameObjectSpawns.Add(Spawn.Guid, Spawn);
                if (Spawn.Guid > MaxGameObjectGUID)
                    MaxGameObjectGUID = (int)Spawn.Guid;
            }

            Log.Success("WorldMgr", "Loaded " + GameObjectSpawns.Count + " GameObject_Spawns");
        }

        #endregion

        #region Vendors

        static public int MAX_ITEM_PAGE = 3;

        static public Dictionary<uint, List<Creature_vendor>> _Vendors = new Dictionary<uint, List<Creature_vendor>>();
        static public List<Creature_vendor> GetVendorItems(uint Entry)
        {
            if (FastLoading)
                return new List<Creature_vendor>();

            if (!_Vendors.ContainsKey(Entry))
            {
                Log.Info("WorldMgr", "Loading Vendors of " + Entry +" ...");

                IList<Creature_vendor> IVendors = Database.SelectObjects<Creature_vendor>("Entry="+Entry);
                List<Creature_vendor> Vendors = new List<Creature_vendor>();
                Vendors.AddRange(IVendors);

                _Vendors.Add(Entry, Vendors);

                Item_Info Req;
                foreach (Creature_vendor Info in Vendors.ToArray())
                {
                    if ((Info.Info = GetItem_Info(Info.ItemId)) == null)
                    {
                        Vendors.Remove(Info);
                        continue;
                    }

                    foreach (KeyValuePair<uint, UInt16> Kp in Info.ItemsReq)
                    {
                        Req = GetItem_Info(Kp.Key);
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

            PacketOut Out = new PacketOut((byte)Opcodes.F_INIT_STORE);
            Out.WriteByte(3);
            Out.WriteByte(0);
            Out.WriteByte(Page);
            Out.WriteByte(Count);
            Out.WriteByte((byte)(Page > 0 ? 0 : 1));
            Out.WriteByte(1);
            Out.WriteByte(0);

            if (Page == 0)
                Out.WriteByte(0);

            for (byte i = 0; i < Count; ++i)
            {
                Out.WriteByte(i);
                Out.WriteByte(1);
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
                    {
                        uint ObjID = 0;
                        uint.TryParse(Obj.ObjID, out ObjID);
 
                        if(ObjID != 0)
                            Obj.Creature = GetCreatureProto(ObjID);

                        if (Obj.Creature == null)
                        {
                            Obj.Description = "Invalid Npc,plz report to GM. QuestID " + Obj.Entry + ",ObjId=" + Obj.ObjID;
                        }
                        else
                        {
                            if (Obj.Description == null || Obj.Description.Length <= Obj.Creature.Name.Length)
                                Obj.Description = "Speak to " + Obj.Creature.Name;
                        }

                    } break;

                case Objective_Type.QUEST_USE_GO:
                    {
                        uint ObjID = 0;
                        uint.TryParse(Obj.ObjID, out ObjID);

                        if (ObjID != 0)
                            Obj.GameObject = GetGameObjectProto(ObjID);

                        if (Obj.GameObject == null)
                        {
                            Obj.Description = "Invalid Go,plz report to GM. QuestID " + Obj.Entry + ",ObjId=" + Obj.ObjID;
                        }
                        else
                        {
                            if (Obj.Description == null || Obj.Description.Length <= Obj.GameObject.Name.Length)
                                Obj.Description = "Find " + Obj.GameObject.Name;
                        }

                    } break;

                case Objective_Type.QUEST_KILL_MOB:
                    {
                        uint ObjID = 0;
                        uint.TryParse(Obj.ObjID, out ObjID);

                        if(ObjID != 0)
                            Obj.Creature = GetCreatureProto(ObjID);

                        if (Obj.Creature == null)
                        {
                            Obj.Description = "Invalid Creature,plz report to GM. QuestID " + Obj.Entry + ",ObjId=" + Obj.ObjID;
                        }
                        else
                        {
                            if (Obj.Description == null || Obj.Description.Length <= Obj.Creature.Name.Length)
                                Obj.Description = "Kill " + Obj.Creature.Name;
                        }

                    } break;

                case Objective_Type.QUEST_GET_ITEM:
                    {
                        uint ObjID = 0;
                        uint.TryParse(Obj.ObjID, out ObjID);

                        if (ObjID != 0)
                        {
                            Obj.Item = GetItem_Info(ObjID);
                            if (Obj.Item == null)
                            {
                                int a =Obj.Quest.Particular.ToLower().IndexOf("kill the ");
                                if (a >= 0)
                                {
                                    string[] RestWords = Obj.Quest.Particular.Substring(a + 9).Split(' ');
                                    string Name = RestWords[0] + " " + RestWords[1];
                                    Creature_proto Proto = GetCreatureProtoByName(Name);
                                    if (Proto == null)
                                        Proto = GetCreatureProtoByName(RestWords[0]);
                                    if (Proto != null)
                                    {
                                        Obj.Item = new Item_Info();
                                        Obj.Item.Entry = ObjID;
                                        Obj.Item.Name = Obj.Description;
                                        Obj.Item.MaxStack = 20;
                                        Obj.Item.ModelId = 531;
                                        _Item_Info.Add(Obj.Item.Entry, Obj.Item);

                                        Log.Info("WorldMgr", "Creating Quest(" + Obj.Entry + ") Item : " + Obj.Item.Entry + ",  " + Obj.Item.Name + "| Adding Loot to : " + Proto.Name);
                                        Creature_loot loot = new Creature_loot();
                                        loot.Entry = Proto.Entry;
                                        loot.ItemId = Obj.Item.Entry;
                                        loot.Info = Obj.Item;
                                        loot.Pct = 0;
                                        GetLoots(Proto.Entry).Add(loot);
                                    }
                                }
                            }
                        }
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

        static public void LoadQuestCreatureStarter()
        {
            _CreatureStarter = new Dictionary<uint, List<Quest>>();

            if (FastLoading)
                return;

            IList<Quest_Creature_Starter> Starters = Database.SelectAllObjects<Quest_Creature_Starter>();

            if (Starters != null)
            {
                Quest Q;
                foreach (Quest_Creature_Starter Start in Starters)
                {
                    if (!_CreatureStarter.ContainsKey(Start.CreatureID))
                        _CreatureStarter.Add(Start.CreatureID, new List<Quest>());

                    Q = GetQuest(Start.Entry);

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

        static public void LoadQuestCreatureFinisher()
        {
            _CreatureFinisher = new Dictionary<uint, List<Quest>>();

            if (FastLoading)
                return;

            IList<Quest_Creature_Finisher> Finishers = Database.SelectAllObjects<Quest_Creature_Finisher>();

            if (Finishers != null)
            {
                Quest Q;
                foreach (Quest_Creature_Finisher Finisher in Finishers)
                {
                    if (!_CreatureFinisher.ContainsKey(Finisher.CreatureID))
                        _CreatureFinisher.Add(Finisher.CreatureID, new List<Quest>());

                    Q = GetQuest(Finisher.Entry);

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
        static public uint GetQuestCreatureFinisher(ushort QuestId)
        {
            foreach (KeyValuePair<uint, List<Quest>> Kp in _CreatureFinisher)
            {
                foreach (Quest Q in Kp.Value)
                    if (Q.Entry == QuestId)
                        return Kp.Key;
            }
 
            return 0;
        }

        static public bool HasQuestToFinish(UInt32 CreatureID, UInt16 QuestID)
        {
            List<Quest> Quests;
            if (_CreatureFinisher.TryGetValue(CreatureID, out Quests))
            {
                foreach (Quest Q in Quests)
                    if (Q.Entry == QuestID)
                        return true;
            }

            return false;
        }

        #endregion

        #region Announces

        static public List<TimedAnnounce> Announces = new List<TimedAnnounce>();

        [LoadingFunction(true)]
        static public void LoadAnnounces()
        {
            Announces = Database.SelectAllObjects<TimedAnnounce>() as List<TimedAnnounce>;
        }

        static public TimedAnnounce GetNextAnnounce(ref int Id, int ZoneId)
        {
            if (Id >= Announces.Count)
                Id = 0;

            for (; Id < Announces.Count;++Id)
                if (Announces[Id].ZoneId == 0 || Announces[Id].ZoneId == ZoneId)
                    return Announces[Id];

            return null;
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
        static public CSharpScriptCompiler ScriptCompiler;

        [LoadingFunction(false)]
        static public void LoadRelation()
        {
            Log.Success("LoadRelation", "Loading Relations");

            foreach (KeyValuePair<uint,Item_Info> Info in _Item_Info)
            {
                if (Info.Value.Career != 0)
                {
                    foreach (KeyValuePair<byte, CharacterInfo> Kp in CharMgr._Infos)
                    {
                        if ((Info.Value.Career & (1 << (Kp.Value.CareerLine - 1))) == 0)
                            continue;
                        else
                        {
                            Info.Value.Realm = Kp.Value.Realm;
                            break;
                        }

                    }
                }
            }

            LoadRegionSpawns();

            if(Program.Config.CleanSpawns)
                RemoveDoubleSpawns();

            LoadChapters();
            LoadPublicQuests();
            LoadQuestsRelation();
            LoadScripts(false);
            CharMgr.Database.ExecuteNonQuery("UPDATE characters_value SET Online=0;");
        }

        static public void LoadRegionSpawns()
        {
            long InvalidSpawns = 0;
            Zone_Info Info = null;
            ushort X, Y = 0;
            Dictionary<string, int> RegionCount = new Dictionary<string, int>();

            {
                Creature_spawn Spawn;
                foreach (KeyValuePair<uint, Creature_spawn> Kp in CreatureSpawns)
                {
                    Spawn = Kp.Value;
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
            }

            {
                GameObject_spawn Spawn;
                foreach (KeyValuePair<uint,GameObject_spawn> Kp in GameObjectSpawns)
                {
                    Spawn = Kp.Value;
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
            }

            if (InvalidSpawns > 0)
                Log.Error("LoadRegionSpawns", "[" + InvalidSpawns + "] Invalid Spawns");

            foreach (KeyValuePair<string, int> Counts in RegionCount)
                Log.Debug("Region", "[" + Counts.Key + "] : " + Counts.Value);
        }
        static public void LoadChapters()
        {
            Log.Success("LoadChapters", "Loading Zone from Chapters");

            long InvalidChapters = 0;

            Zone_Info Zone = null;
            Chapter_Info Info;
            foreach (KeyValuePair<uint,Chapter_Info> Kp in _Chapters)
            {
                Info = Kp.Value;
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
            PQuest_Info Info;
            foreach (KeyValuePair<uint,PQuest_Info> Kp in _PQuests)
            {
                Info = Kp.Value;
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

            foreach (KeyValuePair<uint, Creature_proto> Kp in CreatureProtos)
            {
                Kp.Value.StartingQuests = GetStartQuests(Kp.Key);
                Kp.Value.FinishingQuests = GetFinishersQuests(Kp.Key);
            }

            Quest quest;

            int MaxGuid = 0;
            foreach (KeyValuePair<int, Quest_Objectives> Kp in _Objectives)
            {
                if (Kp.Value.Guid >= MaxGuid)
                    MaxGuid = Kp.Value.Guid;
            }

            foreach (KeyValuePair<int, Quest_Objectives> Kp in _Objectives)
            {
                quest = Kp.Value.Quest = GetQuest(Kp.Value.Entry);
                if (quest == null)
                    continue;

                quest.Objectives.Add(Kp.Value);
            }

            foreach (KeyValuePair<ushort, Quest> Kp in _Quests)
            {
                quest = Kp.Value;

                if (quest.Objectives.Count == 0)
                {
                    UInt32 Finisher = GetQuestCreatureFinisher(quest.Entry);
                    if (Finisher != 0)
                    {
                        Quest_Objectives NewObj = new Quest_Objectives();
                        NewObj.Guid = ++MaxGuid;
                        NewObj.Entry = quest.Entry;
                        NewObj.ObjType = (uint)Objective_Type.QUEST_SPEACK_TO;
                        NewObj.ObjID = Finisher.ToString();
                        NewObj.ObjCount = 1;
                        NewObj.Quest = quest;

                        quest.Objectives.Add(NewObj);
                        _Objectives.Add(NewObj.Guid, NewObj);

                        Log.Debug("WorldMgr", "Creating Objective for quest with no objectives: " + Kp.Value.Entry + " " + Kp.Value.Name);
                    }
                }
            }

            foreach (KeyValuePair<int, Quest_Objectives> Kp in _Objectives)
            {
                if (Kp.Value.Quest == null)
                    continue;
                GenerateObjectif(Kp.Value, Kp.Value.Quest);
            }

            string sItemID, sCount;
            uint ItemID, Count;
            Item_Info Info;
            foreach (KeyValuePair<ushort, Quest> Kp in _Quests)
            {
                if (Kp.Value.Choice.Length <= 0)
                    continue;

                // [5154,12],[128,1]
                string[] Rewards = Kp.Value.Choice.Split('[');
                foreach (string Reward in Rewards)
                {
                    if (Reward.Length <= 0)
                        continue;

                    sItemID = Reward.Substring(0, Reward.IndexOf(','));
                    sCount = Reward.Substring(sItemID.Length + 1, Reward.IndexOf(']') - sItemID.Length - 1);

                    ItemID = uint.Parse(sItemID);
                    Count = uint.Parse(sCount);

                    Info = GetItem_Info(ItemID);
                    if (Info == null)
                        continue;

                    if (!Kp.Value.Rewards.ContainsKey(Info))
                        Kp.Value.Rewards.Add(Info, Count);
                    else
                        Kp.Value.Rewards[Info] += Count;
                }
            }
        }
        static public void RemoveDoubleSpawns()
        {
            uint Space = 400;
            int[] Removed = new int[255];
            List<uint> Guids = new List<uint>();

            Zone_Info Info = null;
            int i, y, Px, Py, SPx, Spy;
            CellSpawns[,] Cell;

            foreach (KeyValuePair<ushort,CellSpawns[,]> Kp in _RegionCells)
            {
                Cell = Kp.Value;
                for (i = 0; i < Cell.GetLength(0); ++i)
                {
                    for (y = 0; y < Cell.GetLength(1); ++y)
                    {
                        if (Cell[i, y] == null)
                            continue;

                        foreach (Creature_spawn Sp in Cell[i, y].CreatureSpawns.ToArray())
                        {
                            if (Sp != null || Sp.Proto == null || GetStartQuests(Sp.Proto.Entry) != null)
                                continue;

                            if (Info == null || Info.ZoneId != Sp.ZoneId)
                                Info = GetZone_Info(Sp.ZoneId);

                            Px = ZoneMgr.CalculPin(Info, Sp.WorldX, true);
                            Py = ZoneMgr.CalculPin(Info, Sp.WorldY, false);

                            foreach (Creature_spawn SubSp in Cell[i, y].CreatureSpawns.ToArray())
                            {
                                if (SubSp.Proto == null || Sp.Entry != SubSp.Entry || Sp == SubSp)
                                    continue;

                                SPx = ZoneMgr.CalculPin(Info, SubSp.WorldX, true);

                                if (Px > SPx + Space || Px < SPx - Space)
                                    continue;

                                Spy = ZoneMgr.CalculPin(Info, SubSp.WorldY, true);

                                if (Py > Spy + Space || Py < Spy - Space)
                                    continue;

                                Removed[SubSp.ZoneId]++;
                                Guids.Add(SubSp.Guid);
                                Cell[i, y].CreatureSpawns.Remove(SubSp);
                                SubSp.Proto = null;
                            }
                        }
                    }
                }
            }

            if (Guids.Count > 0)
            {
                string L = "(";
                foreach (uint Guid in Guids)
                {
                    L += Guid + ",";
                }

                L = L.Remove(L.Length - 1, 1);
                L += ")";

                Log.Info("Spawns", "DELETE FROM creature_spawns WHERE Guid in " + L + ";");
                Database.ExecuteNonQuery("DELETE FROM creature_spawns WHERE Guid in " + L +";");
            }


            for (i = 0; i < 255; ++i)
            {
                if (Removed[i] != 0)
                    Log.Info("Removed", "Zone : " + i + " : " + Removed[i]);
            }
        }

        #endregion

        #region Mounts

        static public Dictionary<uint, Mount_Info> _Mounts = new Dictionary<uint, Mount_Info>();

        [LoadingFunction(true)]
        static public void LoadMounts()
        {
            List<Mount_Info> Mounts = Database.SelectAllObjects<Mount_Info>() as List<Mount_Info>;
            foreach (Mount_Info M in Mounts)
                _Mounts.Add(M.Id, M);

            Log.Success("Mounts", "Loaded " + _Mounts.Count + " Mounts");
        }

        static public Mount_Info GetMount(uint Id)
        {
            Mount_Info M;
            _Mounts.TryGetValue(Id, out M);
            return M;
        }

        #endregion

        #region Scripts

        static public Dictionary<string, Type> LocalScripts = new Dictionary<string, Type>();
        static public Dictionary<string, AGeneralScript> GlobalScripts = new Dictionary<string, AGeneralScript>();
        static public Dictionary<uint, Type> CreatureScripts = new Dictionary<uint, Type>();
        static public Dictionary<uint, Type> GameObjectScripts = new Dictionary<uint, Type>();
        static public ScriptsInterface GeneralScripts;

        static public void LoadScripts(bool Reload)
        {
            GeneralScripts = new ScriptsInterface();

            ScriptCompiler = new CSharpScriptCompiler();
            ScriptCompiler.LoadScripts();
            GeneralScripts.ClearScripts();

            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.IsClass != true)
                        continue;

                    if (!type.IsSubclassOf(typeof(AGeneralScript)))
                        continue;

                    foreach (GeneralScriptAttribute at in type.GetCustomAttributes(typeof(GeneralScriptAttribute), true))
                    {
                        if (at.ScriptName != null && at.ScriptName != "")
                            at.ScriptName = at.ScriptName.ToLower();

                        Log.Success("Scripting", "Resgistering Script :" + at.ScriptName );

                        if (at.GlobalScript)
                        {
                            AGeneralScript Script = Activator.CreateInstance(type) as AGeneralScript;
                            Script.ScriptName = at.ScriptName;
                            GeneralScripts.RemoveScript(Script.ScriptName);
                            GeneralScripts.AddScript(Script);
                            GlobalScripts[at.ScriptName] = Script;
                        }
                        else
                        {
                            if (at.CreatureEntry != 0)
                            {
                                Log.Success("Scripts", "Registering Creature Script :" + at.CreatureEntry);

                                if (!CreatureScripts.ContainsKey(at.CreatureEntry))
                                {
                                    CreatureScripts[at.CreatureEntry] = type;
                                }
                                else
                                {
                                    CreatureScripts[at.CreatureEntry] = type;
                                }
                            }
                            else if (at.GameObjectEntry != 0)
                            {
                                Log.Success("Scripts", "Registering GameObject Script :" + at.GameObjectEntry);

                                if (!GameObjectScripts.ContainsKey(at.GameObjectEntry))
                                {
                                    GameObjectScripts[at.GameObjectEntry] = type;
                                }
                                else
                                {
                                    GameObjectScripts[at.GameObjectEntry] = type;
                                }
                            }
                            else if(at.ScriptName != null && at.ScriptName != "")
                            {
                                Log.Success("Scripts", "Registering Name Script :" + at.ScriptName);

                                if (!LocalScripts.ContainsKey(at.ScriptName))
                                {
                                    LocalScripts[at.ScriptName] = type;
                                }
                                else
                                {
                                    LocalScripts[at.ScriptName] = type;
                                }
                            }
                        }
                    }
                }
            }

            Log.Success("Scripting", "Loaded  : " + (GeneralScripts.Scripts.Count + LocalScripts.Count) + " Scripts");

            if (Reload)
            {
                if (Program.Server != null)
                    Program.Server.LoadPacketHandler();

                AbilityMgr.LoadAbilityHandlers();
            }
        }

        static public AGeneralScript GetScript(Object Obj, string ScriptName)
        {
            if (ScriptName != null && ScriptName.Length > 0)
            {
                ScriptName = ScriptName.ToLower();

                if (GlobalScripts.ContainsKey(ScriptName))
                    return GlobalScripts[ScriptName];
                else if (LocalScripts.ContainsKey(ScriptName))
                {
                    AGeneralScript Script = Activator.CreateInstance(LocalScripts[ScriptName]) as AGeneralScript;
                    Script.ScriptName = ScriptName;
                    return Script;
                }
            }
            else
            {
                if (Obj.IsCreature() && CreatureScripts.ContainsKey(Obj.GetCreature().Spawn.Entry))
                {
                    AGeneralScript Script = Activator.CreateInstance(CreatureScripts[Obj.GetCreature().Spawn.Entry]) as AGeneralScript;
                    Script.ScriptName = Obj.GetCreature().Spawn.Entry.ToString();
                    return Script;
                }

                if (Obj.IsGameObject() && GameObjectScripts.ContainsKey(Obj.GetGameObject().Spawn.Entry))
                {
                    AGeneralScript Script = Activator.CreateInstance(GameObjectScripts[Obj.GetGameObject().Spawn.Entry]) as AGeneralScript;
                    Script.ScriptName = Obj.GetGameObject().Spawn.Entry.ToString();
                    return Script;
                }
            }

            return null;
        }

        static public void UpdateScripts(long Tick)
        {
            GeneralScripts.Update(Tick);
        }

        #endregion
    }
}

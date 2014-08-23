
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common;
using FrameWork;

namespace WorldServer
{

    public class GmCommandHandler
    {
        public delegate bool GmComHandler(Player Plr,ref List<string> Values);
        public GmCommandHandler(string Name, GmComHandler Handler, List<GmCommandHandler> Handlers, int GmLevel, int ValueCount, string Description)
        {
            this.Name = Name;
            this.Handler = Handler;
            this.Handlers = Handlers;
            this.GmLevel = GmLevel;
            this.ValueCount = ValueCount;
            this.Description = Description;
        }

        public string Name;
        public GmComHandler Handler;
        public List<GmCommandHandler> Handlers;
        public int GmLevel;
        public int ValueCount;
        public string Description;
    }

    static public class GmCommand
    {

        static public List<GmCommandHandler> MountCommands = new List<GmCommandHandler>()
        {
            new GmCommandHandler("set",SetMountCommand, null, 1, 1, "Change the mount of selected Unit <Entry>"),
            new GmCommandHandler("add",AddMountCommand, null, 3, 3, "Add a new mount to Database <Entry,Speed,Name>"),
            new GmCommandHandler("remove",RemoveMountCommand, null, 1, 0, "Remove the mount of selected Unit"),
            new GmCommandHandler("list",ListMountsCommand, null, 1, 0, "Show Mount List"),
        };

        static public List<GmCommandHandler> WaypointCommands = new List<GmCommandHandler>()
        {
            new GmCommandHandler("add",NpcAddWaypoint, null, 2, 0, "Add waypoint on your current position to your current target"),
            new GmCommandHandler("remove",NpcRemoveWaypoint, null, 2, 1, "Remove waypoint from target <Id>"),
            new GmCommandHandler("move",NpcMoveWaypoint, null, 2, 1, "Move waypoint of target to your position <Id>"),
            new GmCommandHandler("list",NpcListWaypoint, null, 2, 0, "Show Waypoints List"),
            new GmCommandHandler("info",NpcInfoWaypoint, null, 2, 0, "Show Current Waypoint Info"),
        };

        static public List<GmCommandHandler> StatesCommand = new List<GmCommandHandler>()
        {
            new GmCommandHandler("add",StatesAdd, null, 2, 1, "Add State To Target <Id>"),
            new GmCommandHandler("remove",StatesRemove, null, 2, 1, "Remove state from target <Id>"),
            new GmCommandHandler("list",StatesList, null, 2, 0, "Show target States List"),
        };

        static public List<GmCommandHandler> ModifyCommands = new List<GmCommandHandler>()
        {
            new GmCommandHandler("level",ModifyLevel, null, 3, 1, "Change the level of selected Player <Rank>"),
            new GmCommandHandler("speed",ModifySpeed, null, 2, 1, "Change the speed of selected Player <0-1000>"),
            new GmCommandHandler("renown",ModifyRenown, null, 3, 1, "Change the renown of selected Player <Level>"),
            new GmCommandHandler("faction",ModifyFaction, null, 3, 1, "Change the faction of selected Unit <Id>"),
        };

        static public List<GmCommandHandler> AddCommands = new List<GmCommandHandler>()
        {
            new GmCommandHandler("xp",AddXp, null, 0, 1, "Add xp to player"),
            new GmCommandHandler("item",AddItem, null, 0, 1, "Add item to player"),
            new GmCommandHandler("money",AddMoney, null, 0, 1, "Add money to player"),
            new GmCommandHandler("tok",AddTok, null, 0, 1, "Add tok to player"),
            new GmCommandHandler("renown",AddRenown, null, 0, 1, "Add renown to player"),
        };

        static public List<GmCommandHandler> ChapterCommands = new List<GmCommandHandler>()
        {
            new GmCommandHandler("save",ChapterSave, null, 0, 1, "Save chapter position"),
            new GmCommandHandler("explore",ChapterTokExplore, null, 0, 2, "Set tok explore"),
            new GmCommandHandler("tokentry",ChapterTok, null, 0, 2, "Set tok entry"),
        };

        static public List<GmCommandHandler> NpcCommands = new List<GmCommandHandler>()
        {
            new GmCommandHandler("spawn",NpcSpawn, null, 0, 1, "Spawn an npc"),
            new GmCommandHandler("reload",ReloadCreaturesAndItems, null, 0, 0, "Reloads all Creatures in your current zone and all creature items"),
            new GmCommandHandler("remove",NpcRemove, null, 0, 1, "Delete the target <(0=World,1=Database)>"),
            new GmCommandHandler("go",NpcGoTo, null, 0, 3, "Npc Go To Target <X,Y,Z>"),
            new GmCommandHandler("come",NpcCome, null, 0, 0, "Move target to my position"),
            new GmCommandHandler("modify",NpcModify, null, 0, 2, "Modify a column value <columnname,value,0 target- 1 all>"),
        };

        static public List<GmCommandHandler> GoCommands = new List<GmCommandHandler>()
        {
            new GmCommandHandler("spawn",GoSpawn, null, 0, 1, "Spawn an Go"),
            new GmCommandHandler("remove",GoRemove, null, 0, 1, "Delete the target (0=World,1=Database)"),
        };

        static public List<GmCommandHandler> RespawnCommands = new List<GmCommandHandler>()
        {
            new GmCommandHandler("add",RespawnAdd, null, 0, 0, "Add respawn point to your position <1=Order or 2=Destruction>"),
            new GmCommandHandler("modify",RespawnModify, null, 0, 1, "Modify existing point to you position <ID>"),
            new GmCommandHandler("remove",RespawnRemove, null, 0, 1, "Delete existing point <ID>"),
        };

        static public List<GmCommandHandler> TeleportCommands = new List<GmCommandHandler>()
        {
            new GmCommandHandler("map",TeleportMap, null, 0, 4, "Teleport to point <zoneid,Wx,Wy,Wz>"),
            new GmCommandHandler("appear",TeleportAppear, null, 0, 1, "Appear to player <name>"),
            new GmCommandHandler("summon",TeleportSummon, null, 0, 1, "Summon player <name>"),
        };

        static public List<GmCommandHandler> EquipCommands = new List<GmCommandHandler>()
        {
            new GmCommandHandler("add",EquipAdd, null, 1, 3, "Add Equipement to target <Model,Slot,Save>"),
            new GmCommandHandler("remove",EquipRemove, null, 1, 2, "Remove Equipement to target <Slot,Save>"),
            new GmCommandHandler("clear",EquipClear, null, 1, 1, "Remove All Equipements to target <Save>"),
            new GmCommandHandler("list",EquipList, null, 1, 0, "Draw Equipement list of target"),
        };

        static public List<GmCommandHandler> DatabaseCommands = new List<GmCommandHandler>()
        {
            new GmCommandHandler("itemsreload",ReloadItems, null, 1, 0, "Reload items information"),
            new GmCommandHandler("characterreload",ReloadCharacter, null, 1, 0, "Reload character <name>"),
        };

        static public List<GmCommandHandler> SearchCommands = new List<GmCommandHandler>()
        {
            new GmCommandHandler("item",SearchItem, null, 2, 1, "Search an item by name <name>"),
            new GmCommandHandler("npc",SearchNpc, null, 2, 1, "Seach an npc by name <name>"),
            new GmCommandHandler("gameobject",SearchGameObject, null, 2, 1, "Seach an gameobject by name <name>"),
            new GmCommandHandler("ability",SearchAbility, null, 2, 1, "Seach an ability by name <name>"),
            new GmCommandHandler("quest",SearchQuest, null, 2, 1, "Seach an quest by name <name>"),
            new GmCommandHandler("zone",SearchZone, null, 2, 1, "Seach an zone by name <name>"),
        };

        static public List<GmCommandHandler> BaseCommand = new List<GmCommandHandler>()
        {
            new GmCommandHandler("info",Info, null, 1, 0, "Info of selected target"),
            new GmCommandHandler("invinsible",Invinsible, null, 1, 0, "Set target invinsible"),
            new GmCommandHandler("save",Save, null, 1, 0, "Save target"),
            new GmCommandHandler("gps",Gps, null, 1, 0, "Print your position"),
            new GmCommandHandler("kill",Kill, null, 1, 0, "Kill target"),
            new GmCommandHandler("revive",Revive, null, 1, 0, "Rez target Unit"),
            new GmCommandHandler("teleport",null, TeleportCommands, 1, 0, "Teleport commands"),
            new GmCommandHandler("gmmode",GmMode, null, 1, 0, "Set Invisible / Invinsible"),

            // Database
            new GmCommandHandler("respawn",null, RespawnCommands, 2, 0, "Respawn points"),
            new GmCommandHandler("search",null, SearchCommands, 2, 0, "Search Commands"),

            // Creatures
            new GmCommandHandler("npc",null, NpcCommands, 2, 0, "All Npc commands"),
            new GmCommandHandler("states",null, StatesCommand, 3, 0, "States Commands"),
            new GmCommandHandler("equip", null, EquipCommands, 3, 0, "Creature Equip Commands"),
            new GmCommandHandler("waypoints",null, WaypointCommands, 3, 0, "Waypoint Commands"),

            // GameObject
            new GmCommandHandler("go",null, GoCommands, 3, 0, "All Go commands"),

            new GmCommandHandler("chapter",null, ChapterCommands, 3, 0, "All Chapter commands"),
            // Players
            new GmCommandHandler("modify",null, ModifyCommands, 2, 0, "All command for modify player info"),
            new GmCommandHandler("add",null, AddCommands, 3, 0, "All commands for add somethink to player"),
            new GmCommandHandler("xpmode",XpMode, null, 3, 0, "XpMode System"),

            new GmCommandHandler("mount",null, MountCommands, 3, 0, "Mounts Commands"),
            new GmCommandHandler("announce", Announce, null, 3, 1, "Send Message to all players <message>"),
            new GmCommandHandler("database", null, DatabaseCommands, 3, 0, "Database Commands"),
        };

        #region Funtions

        static public bool HandleCommand(Player Plr, string Text)
        {
            if (Plr._Client._Account.GmLevel <= 0 || Text.Length <= 0)
                return true;

            if (Text[0] != '.')
                return true;

            Text = Text.Remove(0, 1);
            List<string> Values = Text.Split(' ').ToList();
            Values.RemoveAll(str => str.Length <= 0);

            return !DecodeCommand(Plr,ref Values, BaseCommand,null);
        }
        static public bool DecodeCommand(Player Plr,ref List<string> Values, List<GmCommandHandler> Handlers,List<GmCommandHandler> BaseHandlers)
        {
            string Command = GetString(ref Values);

            //Log.Success("DecodeCommand", "Command = " + Command);

            GmCommandHandler Handler = Handlers.Find(com => com != null && com.Name.StartsWith(Command));

            if(Handler == null) // Si la commande n'existe pas , on affiche la liste des commandes
            {
                List<GmCommandHandler> Base = Handlers == null ? BaseHandlers : Handlers;
                PrintCommands(Plr,Base);
                return true;
            }
            else // Le Handler Existe
            {
                if(Handler.GmLevel > Plr.GmLevel) // GmLevel insuffisant
                {
                    Plr.SendMessage(0, "", "Invalid GM Level : < " + Handler.GmLevel, SystemData.ChatLogFilters.CHATLOGFILTERS_SHOUT);
                    return false;
                }

                if(Handler.ValueCount > Values.Count) // Nombre d'arguments insuffisant
                {
                    Plr.SendMessage(0, "", "Invalid Arguments Count : " + Handler.Description, SystemData.ChatLogFilters.CHATLOGFILTERS_SHOUT);
                    return true;
                }

                if(Handler.Handlers != null && Handler.Handlers.Count > 0)
                {
                    return DecodeCommand(Plr,ref Values,Handler.Handlers,Handlers);
                }

                if(Handler.Handler != null)
                {
                    return Handler.Handler.Invoke(Plr,ref Values);
                }
            }

            return false;
        }
        static public void PrintCommands(Player Plr, List<GmCommandHandler> Handler)
        {
            string Str = "";
            foreach (GmCommandHandler Com in Handler)
                Str += Com.Name + ", ";

            Plr.SendMessage(0, "", Str, SystemData.ChatLogFilters.CHATLOGFILTERS_SHOUT);
        }
        static public string GetTotalString(ref List<string> Values)
        {
            string Str = "";
            foreach (string str in Values)
                Str += str + " ";

            return Str;
        }
        static public string GetString(ref List<string> Values)
        {
            if(Values.Count <= 0)
                return "0";

            string str = Values[0];
            Values.RemoveAt(0);

            return str;
        }
        static public int GetInt(ref List<string> Values)
        {
            return int.Parse(GetString(ref Values));
        }
        static public Player GetTargetOrMe(Player Plr)
        {
            Player Target = null;

            Target = Plr.CbtInterface.GetCurrentTarget() as Player;
            if (Target == null)
                Target = Plr;

            return Target;
        }
        static public Object GetObjectTarget(Player Obj)
        {
            Unit Target = Obj.CbtInterface.GetCurrentTarget();
            if (Target == null)
                Target = Obj;

            return Target;
        }

        #endregion

        #region Commands

        #region Database

        static public bool ReloadCharacter(Player Plr, ref List<string> Values)
        {
            string Name = GetString(ref Values);
            Character Char = CharMgr.LoadCharacter(Name);
            if(Char != null)
                Plr.SendMessage(0, "Server", "Character Loaded : " + Char.CharacterId, SystemData.ChatLogFilters.CHATLOGFILTERS_SHOUT);
            else
                Plr.SendMessage(0, "Server", "Invalid Character : " + Name, SystemData.ChatLogFilters.CHATLOGFILTERS_SHOUT);
            return true;
        }

        static public bool ReloadItems(Player Plr, ref List<string> Values)
        {
            WorldMgr.LoadItem_Info();
            Plr.SendMessage(0, "Server", "Items Loaded : " + WorldMgr._Item_Info.Count, SystemData.ChatLogFilters.CHATLOGFILTERS_SHOUT);
            return true;
        }

        #endregion

        #region Search

        static public bool SearchNpc(Player Plr, ref List<string> Values)
        {
            string Str = GetTotalString(ref Values);
            Str = Str.Replace(" ", "%");

            List<Creature_proto> L = WorldMgr.Database.SelectObjects<Creature_proto>("Name Like '%" + WorldMgr.Database.Escape(Str) + "%' LIMIT 0,30") as List<Creature_proto>;
            Plr.SendMessage(0, "", "Creatures : " + (L != null ? L.Count : 0), SystemData.ChatLogFilters.CHATLOGFILTERS_SHOUT);
            if (L != null)
            {
                foreach (Creature_proto proto in L)
                    Plr.SendMessage(0, "", "ID:" + proto.Entry + ",Name:" + proto.Name + ",Faction:" + proto.Faction + ",Flag:" + proto.Flag + ",Icon:" + proto.Icone + ",Model:" + proto.Model1 + ",Script:" + proto.ScriptName, SystemData.ChatLogFilters.CHATLOGFILTERS_EMOTE);
            }

            return true;
        }

        static public bool SearchGameObject(Player Plr, ref List<string> Values)
        {
            string Str = GetTotalString(ref Values);
            Str = Str.Replace(" ", "%");

            List<GameObject_proto> L = WorldMgr.Database.SelectObjects<GameObject_proto>("Name Like '%" + WorldMgr.Database.Escape(Str) + "%' LIMIT 0,30") as List<GameObject_proto>;
            Plr.SendMessage(0, "", "GameObjects : " + (L != null ? L.Count : 0), SystemData.ChatLogFilters.CHATLOGFILTERS_SHOUT);
            if (L != null)
            {
                foreach (GameObject_proto proto in L)
                    Plr.SendMessage(0, "", "ID:" + proto.Entry + ",Name:" + proto.Name + ",Faction:" + proto.Faction + ",Scale:" + proto.Scale + ",Model:" + proto.DisplayID + ",Script:" + proto.ScriptName, SystemData.ChatLogFilters.CHATLOGFILTERS_EMOTE);
            }

            return true;
        }

        static public bool SearchItem(Player Plr, ref List<string> Values)
        {
            string Str = GetTotalString(ref Values);
            Str = Str.Replace(" ", "%");

            List<Item_Info> L = WorldMgr.Database.SelectObjects<Item_Info>("Name Like '%" + WorldMgr.Database.Escape(Str) + "%' LIMIT 0,30") as List<Item_Info>;
            Plr.SendMessage(0, "", "Items : " + (L != null ? L.Count : 0), SystemData.ChatLogFilters.CHATLOGFILTERS_SHOUT);
            if (L != null)
            {
                foreach (Item_Info proto in L)
                    Plr.SendMessage(0, "", "ID:" + proto.Entry + ",Name:" + proto.Name + ",Type:"+proto.Type +",Race:" + proto.Race + ",Career:" + proto.Career + ",Armor:" + proto.Armor + ",Speed:" + proto.Speed + ",Script:" + proto.ScriptName, SystemData.ChatLogFilters.CHATLOGFILTERS_EMOTE);
            }

            return true;
        }

        static public bool SearchAbility(Player Plr, ref List<string> Values)
        {
            string Str = GetTotalString(ref Values);
            Str = Str.Replace(" ", "%");

            List<Ability_Info> L = WorldMgr.Database.SelectObjects<Ability_Info>("Name Like '%" + WorldMgr.Database.Escape(Str) + "%' LIMIT 0,30") as List<Ability_Info>;
            Plr.SendMessage(0, "", "Abilities : " + (L != null ? L.Count : 0), SystemData.ChatLogFilters.CHATLOGFILTERS_SHOUT);
            if (L != null)
            {
                foreach (Ability_Info proto in L)
                    Plr.SendMessage(0, "", "ID:" + proto.Entry + ",Name:" + proto.Name + ",Ap:" + proto.ApCost + ",Career:" + proto.CareerLine + ",Cast:" + proto.CastTime + ",Cooldown:" + proto.Cooldown + ",Rank:" + proto.MinimumRank + ",Handler:" + proto.HandlerName, SystemData.ChatLogFilters.CHATLOGFILTERS_EMOTE);
            }

            return true;
        }

        static public bool SearchZone(Player Plr, ref List<string> Values)
        {
            string Str = GetTotalString(ref Values);
            Str = Str.Replace(" ", "%");

            List<Zone_Info> L = WorldMgr.Database.SelectObjects<Zone_Info>("Name Like '%" + WorldMgr.Database.Escape(Str) + "%' LIMIT 0,30") as List<Zone_Info>;
            Plr.SendMessage(0, "", "Zones : " + (L != null ? L.Count : 0), SystemData.ChatLogFilters.CHATLOGFILTERS_SHOUT);
            if (L != null)
            {
                foreach (Zone_Info proto in L)
                    Plr.SendMessage(0, "", "ID:" + proto.ZoneId + ",Name:" + proto.Name + ",X:" + proto.OffX + ",Y:" + proto.OffY + ",Region:" + proto.Region + ",Level:" + proto.MinLevel + ",Price:" + proto.Price + ",Tier:" + proto.Tier, SystemData.ChatLogFilters.CHATLOGFILTERS_EMOTE);
            }

            return true;
        }

        static public bool SearchQuest(Player Plr, ref List<string> Values)
        {
            string Str = GetTotalString(ref Values);
            Str = Str.Replace(" ", "%");

            List<Quest> L = WorldMgr.Database.SelectObjects<Quest>("Name Like '%" + WorldMgr.Database.Escape(Str) + "%' LIMIT 0,30") as List<Quest>;
            Plr.SendMessage(0, "", "Quests : " + (L != null ? L.Count : 0), SystemData.ChatLogFilters.CHATLOGFILTERS_SHOUT);
            if (L != null)
            {
                foreach (Quest proto in L)
                    Plr.SendMessage(0, "", "ID:" + proto.Entry + ",Name:" + proto.Name + ",Level:" + proto.Level + ",Objectives:" + proto.Objectives.Count + ",Prev:" + proto.PrevQuest + ",Xp:" + proto.Xp + ",Rewars:" + proto.Rewards.Count + ",Type:" + proto.Type, SystemData.ChatLogFilters.CHATLOGFILTERS_EMOTE);
            }

            return true;
        }

        #endregion

        #region GM

        static public bool GmMode(Player Plr, ref List<string> Values)
        {
            Plr.IsInvinsible = !Plr.IsInvinsible;
            if (Plr.IsInvinsible)
            {
                Plr.IsVisible = false;
            }
            else
                Plr.IsVisible = true;

            return true;
        }

        static public bool Invinsible(Player Plr, ref List<string> Values)
        {
            Object Obj = GetObjectTarget(Plr);
            if (Obj == null)
                Obj = Plr;

            if (!Obj.IsUnit())
                return false;

            Obj.GetUnit().IsInvinsible = !Obj.GetUnit().IsInvinsible;
            if (Obj.IsPlayer())
                Obj.GetPlayer().SendMessage(0, "Invinsibility : " + Obj.GetUnit().IsInvinsible, "", SystemData.ChatLogFilters.CHATLOGFILTERS_CITY_ANNOUNCE);
            return true;
        }

        static public bool Announce(Player Plr, ref List<string> Values)
        {
            string Message = GetTotalString(ref Values);

            lock (Player._Players)
            {
                foreach (Player SubPlayer in Player._Players)
                {
                    SubPlayer.SendMessage(0, Plr.Name, Message, SystemData.ChatLogFilters.CHATLOGFILTERS_TELL_RECEIVE);
                    SubPlayer.SendObjectiveText(Message);
                }
            }

            return true;
        }

        #endregion

        #region Modify

        static public bool ModifyLevel(Player Plr,ref List<string> Values)
        {
            int Level = GetInt(ref Values);
            Plr = GetTargetOrMe(Plr);
            Plr.SetLevel((byte)Level);

            GMCommandLog Log = new GMCommandLog();
            Log.PlayerName = Plr.Name;
            Log.AccountId = (uint)Plr.Client._Account.AccountId;
            Log.Command = "SET LEVEL TO " + Plr.Name + " " + Level;
            Log.Date = DateTime.Now;
            WorldMgr.Database.AddObject(Log);

            return true;
        }
        static public bool ModifySpeed(Player Plr, ref List<string> Values)
        {
            int Speed = GetInt(ref Values);
            if (Speed > 500 && Plr.GmLevel < 3)
                Speed = 500;

            Plr = GetTargetOrMe(Plr);
            Plr.Speed = (UInt16)Speed;
            return true;
        }

        static public bool ModifyRenown(Player Plr, ref List<string> Values)
        {
            int RenownLevel = GetInt(ref Values);
            Plr = GetTargetOrMe(Plr);
            Plr.SetRenownLevel((byte)RenownLevel);

            GMCommandLog Log = new GMCommandLog();
            Log.PlayerName = Plr.Name;
            Log.AccountId = (uint)Plr.Client._Account.AccountId;
            Log.Command = "SET RENOWN TO " + Plr.Name + " " + RenownLevel;
            Log.Date = DateTime.Now;
            WorldMgr.Database.AddObject(Log);

            return true;
        }

        static public bool ModifyFaction(Player Plr, ref List<string> Values)
        {
            byte Faction = (byte)GetInt(ref Values);
            byte Save = (byte)(Values.Count > 0 ? GetInt(ref Values) : 0);

            Object Obj = GetObjectTarget(Plr);

            RegionMgr Region = Obj.Region;
            ushort ZoneId = Obj.Zone.ZoneId;

            Obj.RemoveFromWorld();
            Obj.GetUnit().SetFaction(Faction);
            Region.AddObject(Obj.GetUnit(), ZoneId, true);

            if (Save > 0)
            {
                if (Obj.IsCreature())
                {
                    Creature Crea = Obj.GetCreature();
                    Crea.Spawn.Faction = Faction;
                    WorldMgr.Database.SaveObject(Crea.Spawn);
                }
            }

            return true;
        }

        #endregion

        #region Add

        static public bool AddXp(Player Plr, ref List<string> Values)
        {
            int Xp = GetInt(ref Values);
            Plr = GetTargetOrMe(Plr);
            Plr.AddXp((uint)Xp);

            GMCommandLog Log = new GMCommandLog();
            Log.PlayerName = Plr.Name;
            Log.AccountId = (uint)Plr.Client._Account.AccountId;
            Log.Command = "ADD XP TO " + Plr.Name + " " + Xp;
            Log.Date = DateTime.Now;
            WorldMgr.Database.AddObject(Log);

            return true;
        }

        static public bool AddMoney(Player Plr, ref List<string> Values)
        {
            int Money = GetInt(ref Values);
            Plr = GetTargetOrMe(Plr);
            Plr.AddMoney((uint)Money);

            GMCommandLog Log = new GMCommandLog();
            Log.PlayerName = Plr.Name;
            Log.AccountId = (uint)Plr.Client._Account.AccountId;
            Log.Command = "ADD MONEY TO " + Plr.Name + " " + Money;
            Log.Date = DateTime.Now;
            WorldMgr.Database.AddObject(Log);

            return true;
        }

        static public bool AddItem(Player Plr, ref List<string> Values)
        {
            int ItemId = GetInt(ref Values);
            int Count = 1;
            if (Values.Count > 0)
                Count = GetInt(ref Values);

            Plr = GetTargetOrMe(Plr);
            if (Plr.ItmInterface.CreateItem((uint)ItemId, (ushort)Count) == ItemError.RESULT_OK)
            {
                GMCommandLog Log = new GMCommandLog();
                Log.PlayerName = Plr.Name;
                Log.AccountId = (uint)Plr.Client._Account.AccountId;
                Log.Command = "ADD ITEM TO " + Plr.Name + " " + ItemId + " " + Count;
                Log.Date = DateTime.Now;
                WorldMgr.Database.AddObject(Log);

                return true;
            }

            return false;
        }

        static public bool AddTok(Player Plr, ref List<string> Values)
        {
            int TokEntry = GetInt(ref Values);

            Tok_Info Info = WorldMgr.GetTok((ushort)TokEntry);
            if (Info == null)
                return false;

            Plr = GetTargetOrMe(Plr);
            Plr.TokInterface.AddTok(Info.Entry);

            GMCommandLog Log = new GMCommandLog();
            Log.PlayerName = Plr.Name;
            Log.AccountId = (uint)Plr.Client._Account.AccountId;
            Log.Command = "ADD TOK TO " + Plr.Name + " " + TokEntry;
            Log.Date = DateTime.Now;
            WorldMgr.Database.AddObject(Log);

            return false;
        }

        static public bool AddRenown(Player Plr, ref List<string> Values)
        {
            int Value = GetInt(ref Values);
            Plr = GetTargetOrMe(Plr);
            Plr.AddRenown((uint)Value);

            GMCommandLog Log = new GMCommandLog();
            Log.PlayerName = Plr.Name;
            Log.AccountId = (uint)Plr.Client._Account.AccountId;
            Log.Command = "ADD RENOWN TO " + Plr.Name + " " + Value;
            Log.Date = DateTime.Now;
            WorldMgr.Database.AddObject(Log);

            return true;
        }

        #endregion

        #region Info

        static public bool Info(Player Plr, ref List<string> Values)
        {
            Object Other = GetObjectTarget(Plr);
            Plr.SendMessage(0, "", Other.ToString(), SystemData.ChatLogFilters.CHATLOGFILTERS_EMOTE);
            return true;
        }

        #endregion

        #region Save

        static public bool Save(Player Plr, ref List<string> Values)
        {
            Player Other = GetTargetOrMe(Plr);
            Plr.Save();
            return true;
        }

        #endregion

        #region Gps

        static public bool Gps(Player Plr, ref List<string> Values)
        {
            Plr.CalcWorldPositions();
            Object Obj = Plr.CbtInterface.GetCurrentTarget();

            string Pos = "Px="+Plr.X+",Py="+Plr.Y+",Pz="+Plr.Z;
            Pos += ",Wx=" + Plr._Value.WorldX + ",Wy=" + Plr._Value.WorldY + ",Wz=" + Plr._Value.WorldZ;
            Pos += ",Ox=" + Plr.XOffset + ",Oy=" + Plr.YOffset;
            Pos += ",Wh=" + Plr._Value.WorldO + ",Ph=" + Plr.Heading + ",HeightMap=" + ClientFileMgr.GetHeight(Plr.Zone.ZoneId, Plr.X, Plr.Y);
            Plr.SendMessage(0, "", Pos, SystemData.ChatLogFilters.CHATLOGFILTERS_EMOTE);
            if (Obj != null)
            {
                Plr.SendMessage(0, "", (Obj as Point3D).ToString(), SystemData.ChatLogFilters.CHATLOGFILTERS_EMOTE);
                Plr.SendMessage(0, "", "2Dist=" + Plr.GetRealDistance(Obj) + ",Dist=" + Plr.GetDistanceTo(Obj), SystemData.ChatLogFilters.CHATLOGFILTERS_EMOTE);
            }

            return true;
        }

        #endregion

        #region Equip

        static public bool EquipAdd(Player Plr, ref List<string> Values)
        {
            int Model = GetInt(ref Values);
            int Slot = GetInt(ref Values);
            int Save = GetInt(ref Values);

            Creature Obj = GetObjectTarget(Plr) as Creature;
            if (Obj == null)
                return false;

            Creature_item item = new Creature_item();
            item.SlotId = (ushort)Slot;
            item.ModelId = (ushort)Model;
            item.Entry = Obj.Entry;
            item.EffectId = 0;
            Obj.ItmInterface.AddCreatureItem(item);
            Plr.SendMessage(0, "", "Item Added :" + (ushort)Slot, SystemData.ChatLogFilters.CHATLOGFILTERS_SHOUT);
            if (Save > 0)
            {
                WorldMgr.AddCreatureItem(item);
            }

            return true;
        }

        static public bool EquipRemove(Player Plr, ref List<string> Values)
        {
            int Slot = GetInt(ref Values);
            int Save = GetInt(ref Values);

            Creature Obj = GetObjectTarget(Plr) as Creature;
            if (Obj == null)
                return false;

            if (Obj.ItmInterface.RemoveCreatureItem((ushort)Slot) != null)
            {
                Plr.SendMessage(0, "", "Item Removed :" + (ushort)Slot, SystemData.ChatLogFilters.CHATLOGFILTERS_SHOUT);
                if (Save > 0)
                {
                    WorldMgr.RemoveCreatureItem(Obj.Entry, (ushort)Slot);
                }
            }

            return true;
        }

        static public bool EquipClear(Player Plr, ref List<string> Values)
        {
            int Save = GetInt(ref Values);

            Creature Obj = GetObjectTarget(Plr) as Creature;
            if (Obj == null)
                return false;

            for (int i = 0; i < ItemsInterface.MAX_EQUIPED_SLOT; ++i)
            {
                if (Obj.ItmInterface.Items[i] != null)
                {
                    if (Obj.ItmInterface.RemoveCreatureItem((ushort)i) != null)
                    {
                        Plr.SendMessage(0, "", "Item Removed :" + (ushort)i, SystemData.ChatLogFilters.CHATLOGFILTERS_SHOUT);
                        if (Save > 0)
                        {
                            WorldMgr.RemoveCreatureItem(Obj.Entry, (ushort)i);
                        }
                    }
                }
            }

            return true;
        }

        static public bool EquipList(Player Plr, ref List<string> Values)
        {
            Creature Obj = GetObjectTarget(Plr) as Creature;
            if (Obj == null)
                return false;

            for (int i = 0; i < ItemsInterface.MAX_EQUIPED_SLOT; ++i)
            {
                if (Obj.ItmInterface.Items[i] != null)
                {
                    Plr.SendMessage(0, "", "<" + i + "," + Obj.ItmInterface.Items[i].ModelId + ">", SystemData.ChatLogFilters.CHATLOGFILTERS_SHOUT);
                }
            }
            

            return true;
        }

        #endregion

        #region DeathKill

        static public bool Revive(Player Plr, ref List<string> Values)
        {
            Unit Target = Plr.CbtInterface.GetCurrentTarget();
            if (Target == null || !Target.IsDead)
                return false;

            Target.RezUnit();

            if (Target.IsPlayer())
            {
                GMCommandLog Log = new GMCommandLog();
                Log.PlayerName = Plr.Name;
                Log.AccountId = (uint)Plr.Client._Account.AccountId;
                Log.Command = "REZ PLAYER " + Target.Name;
                Log.Date = DateTime.Now;
                WorldMgr.Database.AddObject(Log);
            }

            return true;
        }

        static public bool Kill(Player Plr, ref List<string> Values)
        {
            Unit Target = Plr.CbtInterface.GetCurrentTarget();
            if (Target == null || Target.IsDead)
                return false;

            if (Target.IsPlayer())
            {
                if (Plr.GmLevel < 3)
                    return false;

                GMCommandLog Log = new GMCommandLog();
                Log.PlayerName = Plr.Name;
                Log.AccountId = (uint)Plr.Client._Account.AccountId;
                Log.Command = "KILL PLAYER " + Target.Name;
                Log.Date = DateTime.Now;
                WorldMgr.Database.AddObject(Log);
            }

            Plr.DealDamages(Target, int.MaxValue);

            return true;
        }

        #endregion

        #region Toks

        static public bool ChapterSave(Player Plr, ref List<string> Values)
        {
            int Entry = GetInt(ref Values);

            Chapter_Info Info = WorldMgr.GetChapter((ushort)Entry);
            if (Info == null)
                return false;

            Info.PinX = (ushort)Plr.X;
            Info.PinY = (ushort)Plr.Y;

            Plr.SendMessage(Plr, "Saved [" + Info.Name + "] to '" + Plr.X + "','" + Plr.Y + "'", SystemData.ChatLogFilters.CHATLOGFILTERS_SAY);

            Info.Dirty = true;
            WorldMgr.Database.SaveObject(Info);

            return true;
        }

        static public bool ChapterTokExplore(Player Plr, ref List<string> Values)
        {
            int Entry = GetInt(ref Values);
            int TokExplore = GetInt(ref Values);

            Chapter_Info Chapter = WorldMgr.GetChapter((ushort)Entry);
            Tok_Info Tok = WorldMgr.GetTok((ushort)Entry);

            if (Tok == null || Chapter == null)
                return false;

            Chapter.TokExploreEntry = (UInt32)TokExplore;
            Chapter.Dirty = true;
            WorldMgr.Database.SaveObject(Chapter);

            return true;
        }

        static public bool ChapterTok(Player Plr, ref List<string> Values)
        {
            int Entry = GetInt(ref Values);
            int TokEntry = GetInt(ref Values);

            Chapter_Info Chapter = WorldMgr.GetChapter((ushort)Entry);
            Tok_Info Tok = WorldMgr.GetTok((ushort)Entry);

            if (Tok == null || Chapter == null)
                return false;

            Chapter.TokEntry = (ushort)TokEntry;
            Chapter.Dirty = true;
            WorldMgr.Database.SaveObject(Chapter);

            return true;
        }

        #endregion

        #region Npc

        static public bool NpcCome(Player Plr, ref List<string> Values)
        {
            Object Target = Plr.CbtInterface.GetCurrentTarget();
            if (Target == null || !Target.IsCreature())
                return false;

            Target.GetCreature().MvtInterface.WalkTo(Plr, MovementInterface.CREATURE_SPEED);
            return true;

        }

        static public bool NpcModify(Player Plr, ref List<string> Values)
        {
            string Column = GetString(ref Values);
            string Value = GetString(ref Values);
            int Target = GetInt(ref Values);

            Plr.SendMessage(0, "Server", "Command not ready", SystemData.ChatLogFilters.CHATLOGFILTERS_SHOUT);
            return true;
        }

        static public bool NpcAddWaypoint(Player Plr, ref List<string> Values)
        {
            Object Target = Plr.CbtInterface.GetCurrentTarget();
            if (Target == null || !Target.IsCreature())
                return false;

            Waypoint Wp = new Waypoint();
            Wp.X = (ushort)Plr.X;
            Wp.Y = (ushort)Plr.Y;
            Wp.Z = (ushort)Plr.Z;
            Wp.WaitAtEndMS = 2000;
            Target.GetUnit().AiInterface.AddWaypoint(Wp);

            // TODO : Save it
            return true;
        }

        static public bool NpcMoveWaypoint(Player Plr, ref List<string> Values)
        {
            Object Target = Plr.CbtInterface.GetCurrentTarget();
            if (Target == null || !Target.IsCreature())
                return false;

            int Id = GetInt(ref Values);
            AIInterface IA = Target.GetCreature().AiInterface;

            Waypoint Wp = IA.GetWaypoint(Id);
            if (Wp == null)
            {
                Plr.SendMessage(0, "Server", "Invalid Waypoint ID. Use .waypoint list", SystemData.ChatLogFilters.CHATLOGFILTERS_SHOUT);
                return true;
            }
            Wp.X = (ushort)Plr.X;
            Wp.Y = (ushort)Plr.Y;
            Wp.Z = (ushort)Plr.Z;
            Target.GetUnit().AiInterface.AddWaypoint(Wp);

            // TODO : Save it
            return true;
        }

        static public bool NpcRemoveWaypoint(Player Plr, ref List<string> Values)
        {
            Object Target = Plr.CbtInterface.GetCurrentTarget();
            if (Target == null || !Target.IsCreature())
                return false;

            int Id = GetInt(ref Values);
            AIInterface IA = Target.GetCreature().AiInterface;

            Waypoint Wp = IA.GetWaypoint(Id);
            if (Wp == null)
            {
                Plr.SendMessage(0, "Server", "Invalid Waypoint ID. Use .waypoint list", SystemData.ChatLogFilters.CHATLOGFILTERS_SHOUT);
                return true;
            }
            Target.GetUnit().AiInterface.RemoveWaypoint(Wp);

            // TODO : Save it
            return true;
        }

        static public bool NpcListWaypoint(Player Plr, ref List<string> Values)
        {
            Object Target = Plr.CbtInterface.GetCurrentTarget();
            if (Target == null || !Target.IsCreature())
                return false;

            AIInterface IA = Target.GetCreature().AiInterface;
            string Message = "Waypoints :" + IA.Waypoints.Count + "\n";
            int Id = 0;
            foreach (Waypoint Wp in IA.Waypoints)
            {
                Message += Id + ":" + Wp.X + "," + Wp.Y + "," + Wp.Z + ",Text=" + Wp.TextOnStart + "\n";
            }

            Plr.SendMessage(0, "Server", Message, SystemData.ChatLogFilters.CHATLOGFILTERS_SHOUT);

            // TODO : Save it
            return true;
        }

        static public bool NpcInfoWaypoint(Player Plr, ref List<string> Values)
        {
            Object Target = Plr.CbtInterface.GetCurrentTarget();
            if (Target == null || !Target.IsCreature())
                return false;

            AIInterface IA = Target.GetCreature().AiInterface;
            string Message = "";
            Message += "Current = " + IA.CurrentWaypointID + ",NextTime=" + (IA.NextAllowedMovementTime - TCPManager.GetTimeStampMS()) + ",Started=" + IA.Started + ",Ended=" + IA.Ended + ",Back=" + IA.IsWalkingBack + ",Type=" + IA.CurrentWaypointType + ",State=" + IA.State;

            Plr.SendMessage(0, "Server", Message, SystemData.ChatLogFilters.CHATLOGFILTERS_SHOUT);

            // TODO : Save it
            return true;
        }

        static public bool NpcGoTo(Player Plr, ref List<string> Values)
        {
            int X = GetInt(ref Values); // 21047
            int Y = GetInt(ref Values); // 53750
            int Z = GetInt(ref Values); // 7018

            if (X == 0)
                X = Plr.X;

            if(Y == 0)
                Y = Plr.Y;

            if(Z == 0)
                Z = Plr.Z;

            Unit T = Plr.CbtInterface.GetCurrentTarget();
            if (T != null)
            {
                T.MvtInterface.WalkTo(X, Y, Z, MovementInterface.CREATURE_SPEED);
            }
            return true;
        }

        static public bool NpcSpawn(Player Plr, ref List<string> Values)
        {
            int Entry = GetInt(ref Values);

            Creature_proto Proto = WorldMgr.GetCreatureProto((uint)Entry);
            if (Proto == null)
            {
                Proto = WorldMgr.Database.SelectObject<Creature_proto>("Entry=" + Entry);
                
                if(Proto != null)
                    Plr.SendMessage(0, "Server", "Npc Entry is valid but npc stats are empty. No sniff data about this npc", SystemData.ChatLogFilters.CHATLOGFILTERS_SHOUT);
                else
                    Plr.SendMessage(0, "Server", "Invalid npc entry(" + Entry + ")", SystemData.ChatLogFilters.CHATLOGFILTERS_SHOUT);

                return false;
            }

            Plr.CalcWorldPositions();

            Creature_spawn Spawn = new Creature_spawn();
            Spawn.Guid = (uint)WorldMgr.GenerateCreatureSpawnGUID();
            Spawn.BuildFromProto(Proto);
            Spawn.WorldO = Plr._Value.WorldO;
            Spawn.WorldY = Plr._Value.WorldY;
            Spawn.WorldZ = Plr._Value.WorldZ;
            Spawn.WorldX = Plr._Value.WorldX;
            Spawn.ZoneId = Plr.Zone.ZoneId;

            WorldMgr.Database.AddObject(Spawn);

            Plr.Region.CreateCreature(Spawn);

            GMCommandLog Log = new GMCommandLog();
            Log.PlayerName = Plr.Name;
            Log.AccountId = (uint)Plr.Client._Account.AccountId;
            Log.Command = "SPAWN CREATURE " + Spawn.Entry + " " + Spawn.Guid + " AT " + Spawn.ZoneId + " " + Plr._Value.WorldX + " " + Plr._Value.WorldY;
            Log.Date = DateTime.Now;
            WorldMgr.Database.AddObject(Log);

            return true;
        }

        static public bool NpcRemove(Player Plr, ref List<string> Values)
        {
            Object Obj = GetObjectTarget(Plr);
            if (!Obj.IsCreature())
                return false;

            int Database = GetInt(ref Values);

            Obj.Dispose();

            if (Database > 0)
            {
                Creature_spawn Spawn = Obj.GetCreature().Spawn;
                WorldMgr.Database.DeleteObject(Spawn);
 
                GMCommandLog Log = new GMCommandLog();
                Log.PlayerName = Plr.Name;
                Log.AccountId = (uint)Plr.Client._Account.AccountId;
                Log.Command = "REMOVE CREATURE " + Spawn.Entry + " " + Spawn.Guid + " AT " + Spawn.ZoneId + " " + Spawn.WorldX + " " + Spawn.WorldY;
                Log.Date = DateTime.Now;
                WorldMgr.Database.AddObject(Log);
            }

            Plr.SendMessage(0, "Server", "Npc Removed : " + Obj.GetCreature().Spawn.Guid, SystemData.ChatLogFilters.CHATLOGFILTERS_SHOUT);

            return true;
        }

        static public bool ReloadCreaturesAndItems(Player Plr, ref List<string> Values)
        {
            WorldMgr.LoadCreatureItems();
            Plr.SendMessage(0, "Server", "NPC Items Loaded : " + WorldMgr._CreatureItems.Count, SystemData.ChatLogFilters.CHATLOGFILTERS_SHOUT);

            WorldMgr.LoadCreatureProto();
            Plr.SendMessage(0, "Server", "NPC's Loaded : " + WorldMgr.CreatureProtos.Count, SystemData.ChatLogFilters.CHATLOGFILTERS_SHOUT);

            List<Object> AllCells = new List<Object>();
            AllCells.AddRange(Plr._Cell._Objects);
            foreach (Object Obj in AllCells)
            {
                if (Obj.IsCreature())
                {
                    Creature Crea = Obj.GetCreature();
                    Creature_proto Proto;
                    try { 
                        Proto = WorldMgr.CreatureProtos[Crea.Entry];
                        Crea.Spawn.Proto = Proto;
                    }
                    catch
                    {
                        Plr.SendMessage(0, "Server", "NPC with Entry " + Crea.Entry + " not found in CreatureProtos, removing NPC", SystemData.ChatLogFilters.CHATLOGFILTERS_SHOUT);
                        Crea.Spawn.Proto = null;
                    }                        
                    Crea.Region.CreateCreature(Crea.Spawn);
                    Crea.Dispose();
                }
            }
            Plr.SendMessage(0, "Server", "NPC spawn's Loaded : " + WorldMgr.CreatureSpawns.Count, SystemData.ChatLogFilters.CHATLOGFILTERS_SHOUT);
            return true;
        }

        #endregion

        #region States

        static public bool StatesAdd(Player Plr, ref List<string> Values)
        {
            int Id = GetInt(ref Values);

            Creature Crea = GetObjectTarget(Plr) as Creature;
            if (Crea == null)
                return false;

            Crea.States.Add((byte)Id);
            Crea.SendMeTo(Plr);
            return true;
        }

        static public bool StatesRemove(Player Plr, ref List<string> Values)
        {
            int Id = GetInt(ref Values);

            Creature Crea = GetObjectTarget(Plr) as Creature;
            if (Crea == null)
                return false;

            Crea.States.Remove((byte)Id);
            Crea.SendMeTo(Plr);
            return true;
        }

        static public bool StatesList(Player Plr, ref List<string> Values)
        {
            int Id = GetInt(ref Values);

            Creature Crea = GetObjectTarget(Plr) as Creature;
            if (Crea == null)
                return false;

            Plr.SendMessage(0, "Server", "States :" + Crea.States.Count, SystemData.ChatLogFilters.CHATLOGFILTERS_SHOUT);
            foreach (byte b in Crea.States)
                Plr.SendMessage(0, "Server", b.ToString(), SystemData.ChatLogFilters.CHATLOGFILTERS_SHOUT);
            return true;
        }

        #endregion

        #region Go

        static public bool GoSpawn(Player Plr, ref List<string> Values)
        {
            int Entry = GetInt(ref Values);

            GameObject_proto Proto = WorldMgr.GetGameObjectProto((uint)Entry);
            if (Proto == null)
            {
                Plr.SendMessage(0, "Server", "Invalid go entry(" + Entry + ")", SystemData.ChatLogFilters.CHATLOGFILTERS_SHOUT);
                return false;
            }

            Plr.CalcWorldPositions();

            GameObject_spawn Spawn = new GameObject_spawn();
            Spawn.Guid = (uint)WorldMgr.GenerateGameObjectSpawnGUID();
            Spawn.BuildFromProto(Proto);
            Spawn.WorldO = Plr._Value.WorldO;
            Spawn.WorldY = Plr._Value.WorldY;
            Spawn.WorldZ = Plr._Value.WorldZ;
            Spawn.WorldX = Plr._Value.WorldX;
            Spawn.ZoneId = Plr.Zone.ZoneId;

            WorldMgr.Database.AddObject(Spawn);

            Plr.Region.CreateGameObject(Spawn);

            return true;
        }

        static public bool GoRemove(Player Plr, ref List<string> Values)
        {
            Object Obj = GetObjectTarget(Plr);
            if (!Obj.IsGameObject())
                return false;

            int Database = GetInt(ref Values);

            Obj.RemoveFromWorld();

            if (Database > 0)
                WorldMgr.Database.DeleteObject(Obj.GetGameObject().Spawn);

            return true;
        }

        #endregion

        #region Respawns

        static public bool RespawnAdd(Player Plr, ref List<string> Values)
        {
            byte Realm = (byte)GetInt(ref Values);
            Zone_Respawn Respawn = new Zone_Respawn();
            Respawn.PinX = (UInt16)Plr.X;
            Respawn.PinY = (UInt16)Plr.Y;
            Respawn.PinZ = (UInt16)Plr.Z;
            Respawn.WorldO = Plr.Heading;
            Respawn.ZoneID = Plr.Zone.ZoneId;
            Respawn.Realm = (byte)Realm;
            WorldMgr.Database.AddObject(Respawn);
            WorldMgr.LoadZone_Respawn();

            GMCommandLog Log = new GMCommandLog();
            Log.PlayerName = Plr.Name;
            Log.AccountId = (uint)Plr.Client._Account.AccountId;
            Log.Command = "ADD RESPAWN TO " + Plr.Zone.ZoneId + " " + (UInt16)Plr.X + " " + (UInt16)Plr.Y;
            Log.Date = DateTime.Now;
            WorldMgr.Database.AddObject(Log);

            return true;
        }

        static public bool RespawnRemove(Player Plr, ref List<string> Values)
        {
            int ID = GetInt(ref Values);

            Zone_Respawn Respawn = WorldMgr.Database.SelectObject<Zone_Respawn>("RespawnID=" + ID);
            if (Respawn != null)
            {
                WorldMgr.Database.DeleteObject(Respawn);
                WorldMgr.LoadZone_Respawn();
            }
            else
                return false;

            return true;
        }

        static public bool RespawnModify(Player Plr, ref List<string> Values)
        {
            int ID = GetInt(ref Values);

            Zone_Respawn Respawn = WorldMgr.Database.SelectObject<Zone_Respawn>("RespawnID=" + ID);
            if (Respawn == null)
                return false;

            Respawn.PinX = (UInt16)Plr.X;
            Respawn.PinY = (UInt16)Plr.Y;
            Respawn.PinZ = (UInt16)Plr.Z;
            Respawn.WorldO = Plr.Heading;
            Respawn.ZoneID = Plr.Zone.ZoneId;
            Respawn.Realm = (byte)Plr.Realm;
            WorldMgr.Database.SaveObject(Respawn);
            WorldMgr.LoadZone_Respawn();

            return true;
        }

        #endregion

        #region Teleport

        static public bool TeleportMap(Player Plr, ref List<string> Values)
        {
            int ZoneID = GetInt(ref Values);
            int WorldX = GetInt(ref Values);
            int WorldY = GetInt(ref Values);
            int WorldZ = GetInt(ref Values);

            Plr.Teleport((UInt16)ZoneID, (uint)WorldX, (uint)WorldY, (UInt16)WorldZ, 0);

            GMCommandLog Log = new GMCommandLog();
            Log.PlayerName = Plr.Name;
            Log.AccountId = (uint)Plr.Client._Account.AccountId;
            Log.Command = "TELEPORT TO " + ZoneID + " " + WorldX + " " + WorldY;
            Log.Date = DateTime.Now;
            WorldMgr.Database.AddObject(Log);

            return true;
        }

        static public bool TeleportAppear(Player Plr, ref List<string> Values)
        {
            string PlayerName = GetString(ref Values);

            Player Target = Player.GetPlayer(PlayerName);

            if (Target == null)
            {
                Plr.SendMessage(0, "Server", "Player not found :" + PlayerName, SystemData.ChatLogFilters.CHATLOGFILTERS_SHOUT);
                return false;
            }

            if (Target.Zone == null)
                return false;

            Plr.Teleport(Target.Zone, (uint)Target.WorldPosition.X, (uint)Target.WorldPosition.Y, (UInt16)Target.WorldPosition.Z, 0);

            GMCommandLog Log = new GMCommandLog();
            Log.PlayerName = Plr.Name;
            Log.AccountId = (uint)Plr.Client._Account.AccountId;
            Log.Command = "APPEAR PLAYER " + Target.Name + " TO " + Target.Zone.ZoneId + " " + Target._Value.WorldX + " " + Target._Value.WorldY;
            Log.Date = DateTime.Now;
            WorldMgr.Database.AddObject(Log);

            return true;
        }

        static public bool TeleportSummon(Player Plr, ref List<string> Values)
        {
            string PlayerName = GetString(ref Values);

            Player Target = Player.GetPlayer(PlayerName);

            if (Target == null)
            {
                Plr.SendMessage(0, "Server", "Player not found :" + PlayerName, SystemData.ChatLogFilters.CHATLOGFILTERS_SHOUT);
                return false;
            }

            Target.Teleport(Plr.Zone, (uint)Plr.WorldPosition.X, (uint)Plr.WorldPosition.Y, (UInt16)Plr.WorldPosition.Z, 0);

            GMCommandLog Log = new GMCommandLog();
            Log.PlayerName = Plr.Name;
            Log.AccountId = (uint)Plr.Client._Account.AccountId;
            Log.Command = "SUMMON PLAYER " + Target.Name + " TO " + Plr.Zone.ZoneId + " " + Plr._Value.WorldX + " " + Plr._Value.WorldY;
            Log.Date = DateTime.Now;
            WorldMgr.Database.AddObject(Log);

            return true;
        }

        #endregion

        static public bool XpMode(Player Plr, ref List<string> Values)
        {
            string XpMode = "XpMode Off";
            string XpMode2 = "XpMode On";
            UInt32 CharacterId = Plr.GetPlayer().CharacterId;
            Player Target = Player.GetPlayer(CharacterId);
            Target = Plr;

            if (Target._Value.XpMode == 0)
            {
                Target._Value.XpMode = 1;
                Plr.SendMessage(0, "", XpMode, SystemData.ChatLogFilters.CHATLOGFILTERS_SHOUT);
            }
            else
            {
                Target._Value.XpMode = 0;
                Plr.SendMessage(0, "", XpMode2, SystemData.ChatLogFilters.CHATLOGFILTERS_SHOUT);
            }

            return true;
        }

        #region Mounts

        static public bool SetMountCommand(Player Plr, ref List<string> Values)
        {
            Unit Target = Plr.CbtInterface.GetCurrentTarget();
            if (Target == null || Target.IsDead)
                return false;

            int Entry = GetInt(ref Values);

            Mount_Info Info = WorldMgr.Database.SelectObject<Mount_Info>("Id=" + Entry);

            if (Info != null)
            {
                Target.MvtInterface.CurrentMount.SetMount(Info);
                Plr.SendMessage(null, "Target mount : " + Info.Name, SystemData.ChatLogFilters.CHATLOGFILTERS_SHOUT);
            }
            else
                Plr.SendMessage(null, "Invalid Mount Id, use .mount list", SystemData.ChatLogFilters.CHATLOGFILTERS_SHOUT);

            return true;
        }

        static public bool AddMountCommand(Player Plr, ref List<string> Values)
        {
            Unit Target = Plr.CbtInterface.GetCurrentTarget();
            if (Target == null || Target.IsDead)
                return false;

            int Entry = GetInt(ref Values);
            int Speed = GetInt(ref Values);
            string Name = GetString(ref Values);

            Mount_Info Info = WorldMgr.Database.SelectObject<Mount_Info>("Entry=" + Entry);

            if (Info == null)
            {
                Info = new Mount_Info();
                Info.Entry = (uint)Entry;
                Info.Speed = (ushort)Speed;
                Info.Name = Name;
                WorldMgr.Database.AddObject(Info);

                Target.MvtInterface.CurrentMount.SetMount(Info);
                Plr.SendMessage(null, "Added mount to Database " + Info.Name, SystemData.ChatLogFilters.CHATLOGFILTERS_SHOUT);
            }
            else
            {
                Info.Entry = (uint)Entry;
                Info.Speed = (ushort)Speed;
                Info.Name = Name;
                Info.Dirty = true;
                WorldMgr.Database.AddObject(Info);

                Plr.SendMessage(null, "Modified mount " + Info.Name, SystemData.ChatLogFilters.CHATLOGFILTERS_SHOUT);
            }


            return true;
        }

        static public bool RemoveMountCommand(Player Plr, ref List<string> Values)
        {
            Unit Target = Plr.CbtInterface.GetCurrentTarget();
            if (Target == null || Target.IsDead)
                return false;

            Target.MvtInterface.CurrentMount.UnMount();
            Plr.SendMessage(null, "Target UnMount", SystemData.ChatLogFilters.CHATLOGFILTERS_SHOUT);
            return true;
        }

        static public bool ListMountsCommand(Player Plr, ref List<string> Values)
        {
            Unit Target = Plr.CbtInterface.GetCurrentTarget();
            if (Target == null || Target.IsDead)
                return false;

            List<Mount_Info> Mounts = WorldMgr.Database.SelectAllObjects<Mount_Info>() as List<Mount_Info>;

            uint i = 1;
            foreach (Mount_Info Info in Mounts)
            {
                if (Info.Id != i)
                {
                    Info.Id = i;
                    Info.Dirty = true;
                    WorldMgr.Database.SaveObject(Info);
                }

                ++i;
                Plr.SendMessage(null, Info.Id + ": " + Info.Speed + ":" + Info.Name, SystemData.ChatLogFilters.CHATLOGFILTERS_SHOUT);
            }
            return true;
        }

        #endregion

        #endregion
    }
}

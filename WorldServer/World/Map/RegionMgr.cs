
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

using Common;
using FrameWork;

namespace WorldServer
{
    public class CellSpawns
    {
        UInt16 X,Y,RegionId;
        public List<Creature_spawn> CreatureSpawns = new List<Creature_spawn>();
        public List<GameObject_spawn> GameObjectSpawns = new List<GameObject_spawn>();
        public List<Chapter_Info> ChapterSpawns = new List<Chapter_Info>();
        public List<PQuest_Info> PublicQuests = new List<PQuest_Info>();

        public CellSpawns(UInt16 RegionId,UInt16 X,UInt16 Y)
        {
            this.RegionId = RegionId;
            this.X = X;
            this.Y = Y;
        }

        public void AddSpawn(Creature_spawn Spawn)
        {
            CreatureSpawns.Add(Spawn);
        }

        public void AddSpawn(GameObject_spawn Spawn)
        {
            GameObjectSpawns.Add(Spawn);
        }

        public void AddChapter(Chapter_Info Chapter)
        {
            Chapter.OffX = X;
            Chapter.OffY = Y;

            ChapterSpawns.Add(Chapter);
        }

        public void AddPQuest(PQuest_Info PQuest)
        {
            PQuest.OffX = X;
            PQuest.OffY = Y;

            PublicQuests.Add(PQuest);
        }
    };

    public class RegionMgr
    {
        static public int REGION_UPDATE_INTERVAL = 50; // 50 Ms entre chaque Update
        static public int COLLECT_UPDATE_INTERVAL = 60000; // Toutes les minutes;
        static public UInt16 MAX_CELL_ID = 800;
        static public UInt16 MAX_CELLS = 16;
        static public int MAX_VISIBILITY_RANGE = 300; // 300 mètre de vue ?

        public UInt16 RegionId;
        public Thread _Updater;
        public bool Running = true;
        public long NextCollect = 0;

        public RegionMgr(UInt16 RegionId,List<Zone_Info> Zones)
        {
            this.RegionId = RegionId;
            this.ZonesInfo = Zones;

            LoadSpawns();

            ThreadStart Start = new ThreadStart(Update);
            _Updater = new Thread(Start);
            _Updater.Start();
        }

        public void Stop()
        {
            Log.Success("RegionMgr", "[" + RegionId + "] Stop");
            Running = false;

            foreach (ZoneMgr Zone in ZonesMgr)
                Zone.Stop();

            foreach (Object Obj in _Objects)
                if (Obj != null && !Obj.IsDisposed)
                    Obj.Dispose();
        }

        public List<Zone_Info> ZonesInfo;
        public Zone_Info GetZone_Info(UInt16 ZoneId)
        {
            return ZonesInfo.Find(zone => zone != null && zone.ZoneId == ZoneId);
        }
        public Zone_Info GetZone(UInt16 OffX, UInt16 OffY)
        {
            return ZonesInfo.Find(zone => zone != null &&
                (zone.OffX <= OffX && zone.OffX + MAX_CELLS > OffX) &&
                (zone.OffY <= OffY && zone.OffY + MAX_CELLS > OffY));
        }

        #region ZoneMgr

        public List<ZoneMgr> ZonesMgr = new List<ZoneMgr>();
        public ZoneMgr GetZoneMgr(UInt16 ZoneId,bool Create)
        {
            Zone_Info Info = GetZone_Info(ZoneId);
            if(Info == null) return null;

            ZoneMgr Mgr = null;
            lock (ZonesMgr)
            {
                Mgr = ZonesMgr.Find(zone => zone != null && zone.Info.ZoneId == ZoneId);
                if (Mgr == null && Create) // Si la zone n'a pas encore été lancée
                {
                    Mgr = new ZoneMgr(this, Info);
                    ZonesMgr.Add(Mgr);
                }
            }

            return Mgr;
        }
        public void CheckZone(Object Obj)
        {
            Zone_Info Info = GetZone(Obj.XOffset, Obj.YOffset);
            if (Info != null && Info != Obj.Zone.Info)
                AddObject(Obj, Info.ZoneId);

            CellMgr CurCell = Obj._Cell;
            CellMgr NewCell = GetCell(Obj.XOffset, Obj.YOffset);

            if (NewCell != null && NewCell != CurCell)
            {
                if(CurCell != null) // Si l'objet est sur un cell, on le supprime
                    CurCell.RemoveObject(Obj);

                NewCell.AddObject(Obj); // On l'ajoute dans le nouveau cell
            }
        }
        public void Update()
        {
            while (Running)
            {
                long Start = TCPManager.GetTimeStampMS();

                try
                {
                    lock (ZonesMgr)
                        ZonesMgr.ForEach(zone => { if (zone != null) zone.Run(); });
                }
                catch (Exception e)
                {
                    Log.Error("Erreur", e.ToString());
                }

                if (NextCollect <= Start)
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    NextCollect = Start + COLLECT_UPDATE_INTERVAL;
                }

                long End = TCPManager.GetTimeStampMS();
                long Elapsed = End - Start;
                if (Elapsed < REGION_UPDATE_INTERVAL)
                    Thread.Sleep((int)(REGION_UPDATE_INTERVAL - Elapsed));
                else
                {
                    Log.Error("RegionMgr", "[" + RegionId + "] La region lag, " + GetObjects() + " objets! " + Elapsed +"ms");
                }
            }
        }

        #endregion

        #region Ranged

        public List<Player> GetRangedPlayer(Object Obj, int Range)
        {
            List<Player> Objs = new List<Player>();
            if (!Obj.IsInWorld())
                return Objs;

            List<CellMgr> Cells = GetCells(Obj.XOffset, Obj.YOffset, Range);
            foreach (CellMgr Cell in Cells)
                Objs.AddRange(Cell._Players);

            Objs.RemoveAll(Dist => Dist != null && Obj.GetDistanceTo(Dist) > MAX_VISIBILITY_RANGE);

            return Objs;
        }
        public List<Object> GetRangedObject(Object Obj, int Range)
        {
            List<Object> Objs = new List<Object>();

            if (!Obj.IsInWorld())
                return Objs;

            List<CellMgr> Cells = GetCells(Obj.XOffset, Obj.YOffset, Range);
            foreach (CellMgr Cell in Cells)
                Objs.AddRange(Cell._Objects);

            Objs.RemoveAll(Dist => Dist != null && Obj.GetDistance(Dist) > MAX_VISIBILITY_RANGE);

            return Objs;
        }
        public List<ZoneMgr> GetRangedZone(int XOffset,int YOffset,int Range)
        {
            List<ZoneMgr> Mgrs = new List<ZoneMgr>();
            lock (ZonesMgr)
            {
                foreach (ZoneMgr Mgr in ZonesMgr)
                {
                    Zone_Info zone = Mgr.Info;

                   if ( (zone.OffX-Range <= XOffset && zone.OffX + MAX_CELLS > XOffset) && (zone.OffY-Range <= YOffset && zone.OffY + MAX_CELLS > YOffset))
                        Mgrs.Add(Mgr);
                }
            }

            return Mgrs;
        }
        static public bool IsRange(int fixe, int move, int range)
        {
            int Max = fixe + range;
            int Min = fixe - range;

            if (move > Max || move < Min)
                return false;

            return true;
        }

        public bool UpdateRange(Object CurObj)
        {
            float Distance = CurObj.GetDistance(CurObj.LastRangeCheck);
            if (Distance > 100)
            {
                CurObj.LastRangeCheck.X = CurObj.X;
                CurObj.LastRangeCheck.Y = CurObj.Y;
            }
            else 
                return false;

            List<Object> Objects = GetRangedObject(CurObj, 1);

            foreach (Object DistObj in Objects) // Ici on check tous les objets visibles
            {
                if(CurObj == DistObj)
                    continue;

                if (DistObj.IsPlayer() && !DistObj.GetPlayer().Client.IsPlaying())
                    continue;

                if(!CurObj.HasInRange(DistObj))
                {
                    CurObj.AddInRange(DistObj);
                    DistObj.AddInRange(CurObj);

                    if (CurObj.IsPlayer())
                        DistObj.SendMeTo(CurObj.GetPlayer());

                    if (DistObj.IsPlayer())
                        CurObj.SendMeTo(DistObj.GetPlayer());
                }
                else
                {

                }
            }

            List<Object> ToDel = CurObj._ObjectRanged.FindAll(dist => dist != null && dist.GetDistance(CurObj) > MAX_VISIBILITY_RANGE);
            foreach (Object Dist in ToDel)
            {
                CurObj.RemoveInRange(Dist);
                Dist.RemoveInRange(CurObj);
            }

            return true;
        }

        #endregion

        #region Oid

        static public UInt16 MAX_OBJECTS = 65000;
        public Object[] _Objects = new Object[MAX_OBJECTS];
        public void GenerateOid(Object Obj)
        {
            lock (_Objects)
            {
                for (UInt16 Id = 2; Id < _Objects.Length; ++Id)
                {
                    if (_Objects[Id] == null)
                    {
                        Obj.Oid = Id;
                        Obj._Loaded = false;
                        _Objects[Id] = Obj;
                         break;
                    }
                }
            }

        }
        public bool AddObject(Object Obj,UInt16 ZoneId,bool MustUpdateRange=false)
        {
            Zone_Info Info = GetZone_Info(ZoneId);
            if (Info == null)
            {
                Log.Error("RegionMgr", "Invalid Zone Id : " + ZoneId);
                return false;
            }

            if (Obj.Region == null || (Obj.Region != null && Obj.Region != this))
            {
                if(Obj.Region != null)
                    Obj.Region.RemoveObject(Obj);

                GenerateOid(Obj);
            }

            ZoneMgr Mgr = GetZoneMgr(ZoneId, true);
            Mgr.AddObject(Obj);

            if (MustUpdateRange)
                UpdateRange(Obj);

            return true;
        }
        public bool RemoveObject(Object Obj)
        {
            if(Obj.IsPlayer())
                Log.Success("RemoveObject", Obj.Name);

            if (Obj.IsInWorld())
                Obj.Zone.RemoveObject(Obj);

            Obj.ClearRange();

            if (Obj._Cell != null)
                Obj._Cell.RemoveObject(Obj);

            lock(_Objects)
                if (_Objects[Obj.Oid] != null && _Objects[Obj.Oid] == Obj)
                {
                    _Objects[Obj.Oid] = null;
                    Obj.Oid = 0;
                    return true;
                }

            return false;
        }
        public Object GetObject(UInt16 Oid)
        {
            if (Oid == 0)
                return null;

            if (Oid >= _Objects.Length)
                return null;
            if (Oid < 2)
                return null;

            lock (_Objects)
                return _Objects[Oid];
        }
        public Player GetPlayer(UInt16 Oid)
        {
            Object Obj = GetObject(Oid);
            if (Obj == null || !Obj.IsPlayer())
                return null;

            return Obj.GetPlayer();
        }
        public UInt16 GetObjects()
        {
            return (UInt16)_Objects.ToList().Count(obj => obj != null);
        }

        #endregion

        #region Cells

        public CellMgr[,] _Cells = new CellMgr[MAX_CELL_ID, MAX_CELL_ID];

        public CellMgr GetCell(UInt16 X, UInt16 Y)
        {
            if (X >= MAX_CELL_ID) X = (UInt16)(MAX_CELL_ID - 1);
            if (Y >= MAX_CELL_ID) Y = (UInt16)(MAX_CELL_ID - 1);

            if (_Cells[X,Y] == null)
                _Cells[X, Y] = new CellMgr(this, X, Y);

            return _Cells[X, Y];
        }
        public void LoadCells(UInt16 X, UInt16 Y,int Range)
        {
            List<CellMgr> Cells = GetCells(X, Y, Range);
            Cells.ForEach(cell => { if (cell != null) cell.Load(); });
        }
        public List<CellMgr> GetCells(UInt16 X, UInt16 Y, int Range)
        {
            List<CellMgr> Cells = new List<CellMgr>();

            UInt16 MinX = (ushort)Math.Max(0, (int)(X - Range));
            UInt16 MaxX = (ushort)Math.Min((int)(MAX_CELL_ID - 1), (int)(X + Range));

            UInt16 MinY = (ushort)Math.Max(0, (int)(Y - Range));
            UInt16 MaxY = (ushort)Math.Min((int)(MAX_CELL_ID - 1), (int)(Y + Range));

            for (UInt16 Ox = MinX; Ox <= MaxX; ++Ox)
                for (UInt16 Oy = MinY; Oy <= MaxY; ++Oy)
                    Cells.Add(GetCell(Ox, Oy));

            return Cells;
        }

        #endregion

        #region Spawns

        public CellSpawns[,] _CellSpawns;

        public void LoadSpawns()
        {
            _CellSpawns = WorldMgr.GetCells(RegionId);
        }

        public CellSpawns GetCellSpawn(UInt16 X, UInt16 Y)
        {
            X = (UInt16)Math.Min(MAX_CELL_ID - 1, X);
            Y = (UInt16)Math.Min(MAX_CELL_ID - 1, Y);

            if (_CellSpawns[X, Y] == null)
            {
                _CellSpawns[X, Y] = new CellSpawns(RegionId, X, Y);
            }

            return _CellSpawns[X, Y];
        }
        public Creature CreateCreature(Creature_spawn Spawn)
        {
            Creature Crea = new Creature(Spawn);
            AddObject((Object)Crea,Spawn.ZoneId);
            return Crea;
        }
        public GameObject CreateGameObject(GameObject_spawn Spawn)
        {
            GameObject Obj = new GameObject(Spawn);
            AddObject(Obj, Spawn.ZoneId);
            return Obj;
        }
        public ChapterObject CreateChapter(Chapter_Info Chapter)
        {
            ChapterObject Obj = new ChapterObject(Chapter);
            AddObject(Obj, Chapter.ZoneId);
            return Obj;
        }

        public PQuestObject CreatePQuest(PQuest_Info Quest)
        {
            PQuestObject Obj = new PQuestObject(Quest);
            AddObject(Obj, Quest.ZoneId);
            return Obj;
        }

        #endregion

    }
}

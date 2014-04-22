
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
        static public int COLLECT_UPDATE_INTERVAL = 180000; // Toutes les 2 minutes;
        static public UInt16 MAX_CELL_ID = 800;
        static public UInt16 MAX_CELLS = 16;
        static public int MAX_VISIBILITY_RANGE = 300; // 300 mètre de vue ?

        public UInt16 RegionId;
        public Thread _Updater;
        public bool Running = true;
        public long NextCollect = 0;
        public List<Zone_Info> ZonesInfo;

        public RegionMgr(UInt16 RegionId, List<Zone_Info> Zones)
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
            try
            {
                Log.Success("RegionMgr", "[" + RegionId + "] Stop");
                Running = false;

                for (int i = 0; i < ZonesMgr.Count; ++i)
                    ZonesMgr[i].Stop();

                foreach (Object Obj in _Objects)
                    if (Obj != null && !Obj.IsDisposed)
                        Obj.Dispose();
            }
            catch
            {

            }
        }

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
                    WorldMgr.UpdateScripts(Start);

                    int i = 0;
                    lock (ZonesMgr)
                    {
                        for (; i < ZonesMgr.Count; ++i)
                        {
                            ZonesMgr[i].Run(Start);
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error("Erreur", e.ToString());
                }

                /*if (NextCollect <= Start)
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    NextCollect = Start + COLLECT_UPDATE_INTERVAL;
                }*/

                long Elapsed = TCPManager.GetTimeStampMS() - Start;
                if (Elapsed < REGION_UPDATE_INTERVAL)
                    Thread.Sleep((int)(REGION_UPDATE_INTERVAL - Elapsed));
                else
                {
                    Log.Debug("RegionMgr", "[" + RegionId + "] La region lag, " + GetObjects() + " objets! " + Elapsed + "ms");
                }
            }
        }

        #endregion

        #region Ranged

        public delegate void RangedObjectDelegate(Object Obj);

        public void GetRangedObject(Object Obj, int Range, RangedObjectDelegate RangeFunction)
        {
            if (!Obj.IsInWorld())
                return;

            GetCells(Obj.XOffset, Obj.YOffset, Range, (CellMgr Cell) =>
            {
                Object DistObject = null;
                for(int i=0;i<Cell._Objects.Count;++i)
                {
                    if ((DistObject = Cell._Objects[i]) == null)
                    {
                        Cell._Objects.RemoveAt(i);
                        i--;
                    }
                    else if (Obj.GetDistance(DistObject) <= MAX_VISIBILITY_RANGE)
                        RangeFunction(DistObject);
                }
            });
        }
        static public bool IsRange(int fixe, int move, int range)
        {
            int Max = fixe + range;
            int Min = fixe - range;

            if (move > Max || move < Min)
                return false;

            return true;
        }

        public bool UpdateRange(Object CurObj, bool ForceUpdate = false)
        {
            if (CurObj.IsActive == false || CurObj.IsDisposed)
                return false;

            if (CurObj.X == 0 && CurObj.Y == 0)
                return false;

            float Distance = CurObj.GetDistance(CurObj.LastRangeCheck);
            if (Distance > 100 || ForceUpdate)
            {
                CurObj.LastRangeCheck.X = CurObj.X;
                CurObj.LastRangeCheck.Y = CurObj.Y;
            }
            else 
                return false;

            CurObj.OnRangeUpdate();

            GetRangedObject(CurObj, 1, (Object DistObj) =>
                {
                    if (DistObj == null)
                        return;

                    if (IsVisibleBForA(CurObj, DistObj) && !CurObj.HasInRange(DistObj))
                    {
                        CurObj.AddInRange(DistObj);
                        DistObj.AddInRange(CurObj);

                        if (CurObj.IsPlayer())
                            DistObj.SendMeTo(CurObj.GetPlayer());

                        if (DistObj.IsPlayer())
                            CurObj.SendMeTo(DistObj.GetPlayer());
                    }
                });

            Object dist;
            for (int i = 0; i < CurObj._ObjectRanged.Count; ++i)
            {
                if ((dist = CurObj._ObjectRanged[i]) == null)
                    continue;

                if (CurObj == null || dist == null)
                    continue;

                if (dist.GetDistance(CurObj) > MAX_VISIBILITY_RANGE || !IsVisibleBForA(CurObj, dist))
                {
                    CurObj.RemoveInRange(dist);
                    dist.RemoveInRange(CurObj);
                    i--;
                }
            }

            return true;
        }

        public bool IsVisibleBForA(Object A, Object B)
        {
            if (A == null || B == null || A.IsDisposed || B.IsDisposed)
                return false;

            if (A == B || !A.IsActive || !B.IsActive || !B.IsVisible)
                return false;

            if (B.IsPlayer() && (B.GetPlayer().Client == null && !B.GetPlayer().Client.IsPlaying()))
                return false;

            return true;
        }

        #endregion

        #region Oid

        static public UInt16 MAX_OBJECTS = 65000;
        static public UInt16 MaxOID = 2;
        public Object[] _Objects = new Object[MAX_OBJECTS];
        public void GenerateOid(Object Obj)
        {
            lock (_Objects)
            {
                ushort Oid = GetOid();
                Obj.Oid = Oid;
                Obj._Loaded = false;
                _Objects[Oid] = Obj;
            }
        }

        public ushort GetOid()
        {
            for (int i = MaxOID; i < MAX_OBJECTS; ++i)
            {
                if (MaxOID >= MAX_OBJECTS - 1)
                {
                    MaxOID = 2;
                    i = 2;
                }

                if (_Objects[i] == null)
                {
                    MaxOID = (ushort)i;
                    return (ushort)i;
                }
            }

            return MaxOID;
        }

        public bool AddObject(Object Obj, UInt16 ZoneId, bool MustUpdateRange = false)
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

            Obj.ClearRange();

            if (Obj._Cell != null)
                Obj._Cell.RemoveObject(Obj);

            if (Obj.Zone != null)
                Obj.Zone.RemoveObject(Obj);
 
            if (Obj.Oid != 0)
            {
                lock (_Objects)
                    if (_Objects[Obj.Oid] != null && _Objects[Obj.Oid] == Obj)
                    {
                        _Objects[Obj.Oid] = null;
                        Obj.Oid = 0;
                        return true;
                    }
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
            {
                Object Obj = _Objects[Oid];
                if (Obj != null && Obj.IsDisposed)
                    return null;

                return Obj;
            }
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
        public delegate void GetCellDelegate(CellMgr Cell);

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
            GetCells(X, Y, Range, (CellMgr Cell) => { if (Cell != null) Cell.Load(); });
        }
        public void GetCells(UInt16 X, UInt16 Y, int Range, GetCellDelegate CellFunction)
        {
            if (CellFunction == null)
                return;

            UInt16 MinX = (ushort)Math.Max(0, (int)(X - Range));
            UInt16 MaxX = (ushort)Math.Min((int)(MAX_CELL_ID - 1), (int)(X + Range));

            UInt16 MinY = (ushort)Math.Max(0, (int)(Y - Range));
            UInt16 MaxY = (ushort)Math.Min((int)(MAX_CELL_ID - 1), (int)(Y + Range));

            for (UInt16 Ox = MinX; Ox <= MaxX; ++Ox)
                for (UInt16 Oy = MinY; Oy <= MaxY; ++Oy)
                    CellFunction(GetCell(Ox, Oy)); ;
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
            if (Spawn == null || Spawn.Proto == null)
                return null;

            Creature Crea = new Creature(Spawn);
            AddObject((Object)Crea,Spawn.ZoneId);
            return Crea;
        }
        public GameObject CreateGameObject(GameObject_spawn Spawn)
        {
            if (Spawn == null || Spawn.Proto == null)
                return null;

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

        public GameObject CreateGameObject(uint Entry, ushort ZoneId, ushort Px, ushort Py, ushort Pz)
        {
            GameObject_proto Proto = WorldMgr.GetGameObjectProto(Entry);
            if (Proto == null)
                return null;

            GameObject_spawn Spawn = new GameObject_spawn();
            Spawn.Entry = Entry;
            Spawn.Guid = (uint)WorldMgr.GenerateGameObjectSpawnGUID();
            Spawn.BuildFromProto(Proto);
            Spawn.ZoneId = ZoneId;
            ZoneMgr.CalculWorldPosition(ZoneId, Px, Py, Pz, ref Spawn.WorldX, ref Spawn.WorldY, ref Spawn.WorldZ);
            return CreateGameObject(Spawn);
        }

        public Creature CreateCreature(uint Entry, ushort ZoneId, ushort Px, ushort Py, ushort Pz)
        {
            Creature_proto Proto = WorldMgr.GetCreatureProto((uint)Entry);
            if (Proto == null)
                return null;

            Creature_spawn Spawn = new Creature_spawn();
            Spawn.Guid = (uint)WorldMgr.GenerateCreatureSpawnGUID();
            Spawn.BuildFromProto(Proto);
            Spawn.ZoneId = ZoneId;
            ZoneMgr.CalculWorldPosition(ZoneId, Px, Py, Pz, ref Spawn._WorldO, ref Spawn._WorldY, ref Spawn._WorldZ);
            return CreateCreature(Spawn);
        }

        #endregion
    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common;
using FrameWork;

namespace WorldServer
{
    public class CellMgr
    {
        public RegionMgr Region;
        public UInt16 X;
        public UInt16 Y;
        public CellSpawns Spawns;

        public CellMgr(RegionMgr Mgr, UInt16 OffX, UInt16 OffY)
        {
            Region = Mgr;
            X = OffX;
            Y = OffY;
            Spawns = Mgr.GetCellSpawn(OffX,OffY);
        }

        #region Objects

        public List<Object> _Objects = new List<Object>();
        public List<Player> _Players = new List<Player>();

        public void AddObject(Object Obj)
        {
            //Log.Success("AddObject", "[" + X + "," + Y + "] Cell Add " + Obj.Name);

            if (Obj.IsPlayer())
            {
                _Players.Add(Obj.GetPlayer());
                Region.LoadCells(X, Y, 1); // Si un joueur entre, alors on charge les cells autours sur 1 rangeen
            }

           _Objects.Add(Obj);
           Obj._Cell = this;
        }
        public void RemoveObject(Object Obj)
        {
            //Log.Success("RemoveObject", "[" + X + "," + Y + "] Cell Remove " + Obj.Name);

            if (Obj._Cell == this)
            {
                if (Obj.IsPlayer())
                    _Players.Remove(Obj.GetPlayer());

                _Objects.Remove(Obj);
                Obj._Cell = null;
            }
        }

        #endregion

        #region Spawns

        public bool _Loaded = false;
        public void Load()
        {
            if (_Loaded)
                return;

            Log.Debug(ToString(), "Loading... ");

            foreach (Creature_spawn Spawn in Spawns.CreatureSpawns)
                Region.CreateCreature(Spawn);

            foreach (GameObject_spawn Spawn in Spawns.GameObjectSpawns)
                Region.CreateGameObject(Spawn);

            foreach (Chapter_Info Spawn in Spawns.ChapterSpawns)
                Region.CreateChapter(Spawn);

            foreach (PQuest_Info Quest in Spawns.PublicQuests)
                Region.CreatePQuest(Quest);

            _Loaded = true;
        }

        public override string ToString()
        {
            return "CellMgr["+X+","+Y+"]";
        }

        #endregion
    }
}

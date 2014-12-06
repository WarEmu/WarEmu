using Common;
using FrameWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorldServer
{
    public class AIInterface : BaseInterface
    {
        static public int BrainThinkTime = 3000; // Update think every 3sec
        static public int MAX_AGGRO_RANGE = 30; // Max Range To Aggro

        private AiState _State = AiState.STANDING;
        public AiState State
        {
            get
            {
                return _State;
            }
            set
            {
                if (_State != value)
                {
                    if (value == AiState.FIGHTING)
                        OnCombatStart();
                    else if (value == AiState.STANDING && _State == AiState.FIGHTING)
                        OnCombatStop();
                    else if (value == AiState.MOVING)
                        OnWalkStart();
                    else if(_State == AiState.MOVING)
                        OnWalkEnd();

                    _State = value;
                }
            }
        }

        public CombatInterface Cbt;
        public MovementInterface Mvt;
        public EventInterface Evt;

        public AIInterface()
         {
            RangedAllies = new List<Unit>();
            RangedEnemies = new List<Unit>();
         }

        public override bool Load()
        {
            Evt = _Owner.EvtInterface;
            if (_Owner.IsUnit())
            {
                Cbt = _Owner.GetUnit().CbtInterface;
                Mvt = _Owner.GetUnit().MvtInterface;
            }

            Evt.AddEvent(UpdateThink, BrainThinkTime + RandomMgr.Next(0, 2000), 0);
            Evt.AddEventNotify(EventName.ON_TARGET_DIE, OnTargetDie);

            return base.Load();
        }

        public override void Stop()
        {
            Evt.RemoveEvent(UpdateThink);

            base.Stop();
        }

        public override void Update(long Tick)
        {
            if (_Owner.IsUnit())
            {
                if (!_Owner.GetUnit().IsDead)
                    UpdateWaypoints(Tick);
            }

            base.Update(Tick);
        }

        public bool OnTargetDie(Object Obj, object Args)
        {
            if (State == AiState.FIGHTING)
            {
                LockMovement(4000);
            }
            return false;
        }

        #region States

        public Item EquipedItem;

        public void OnCombatStart()
        {
            if (EquipedItem == null && _Owner.IsCreature())
            {
                EquipedItem = _Owner.GetCreature().ItmInterface.RemoveCreatureItem(11);
            }
        }

        public void OnCombatStop()
        {
            if (EquipedItem != null && _Owner.IsCreature())
            {
                _Owner.GetCreature().ItmInterface.AddCreatureItem(EquipedItem);
                EquipedItem = null;
            }
        }

        public void OnWalkStart()
        {

        }

        public void OnWalkEnd()
        {

        }

        #endregion

        #region Brain

        public ABrain CurrentBrain;

        public void SetBrain(ABrain NewBrain)
        {
            if (CurrentBrain != null)
                CurrentBrain.Stop();

            CurrentBrain = NewBrain;

            if (CurrentBrain != null)
                CurrentBrain.Start();
        }

        public void UpdateThink()
        {
            if (HasPlayer() || !_Owner.IsUnit())
                return;

            if (_Owner.GetUnit().IsDead || _Owner.GetUnit().IsDisposed)
                return;

            if (CurrentBrain != null && CurrentBrain.IsStart && !CurrentBrain.IsStop)
                CurrentBrain.Think();
        }

        #endregion

        #region Ranged

        public List<Unit> RangedAllies;
        public List<Unit> RangedEnemies;

        public bool AddRange(Unit Obj)
        {
            if (!HasUnit())
                return false;

            if (CombatInterface.IsEnemy(GetUnit(), Obj))
                RangedEnemies.Add(Obj.GetUnit());
            else
                RangedAllies.Add(Obj.GetUnit());

            return true;
        }

        public bool RemoveRange(Unit Obj)
        {
            if (!HasUnit())
                return false;

            RangedAllies.Remove(Obj);
            RangedEnemies.Remove(Obj);
            return true;
        }

        public void ClearRange()
        {
            RangedAllies.Clear();
            RangedEnemies.Clear();
        }

        public Unit GetAttackableUnit()
        {
            float MaxRange = AIInterface.MAX_AGGRO_RANGE;
            Unit Target = null;

            Unit Me = GetUnit();
            float Dist;
            foreach (Unit Enemy in RangedEnemies)
            {
                if (Enemy.Realm == GameData.Realms.REALMS_REALM_NEUTRAL)
                    continue;

                if (!CombatInterface.CanAttack(Me, Enemy))
                    continue;

                Dist = _Owner.GetDistanceTo(Enemy);
                if (Dist < MaxRange)
                {
                    MaxRange = Dist;
                    Target = Enemy;
                }
            }

            return Target;
        }

        #endregion

        #region Waypoints
        public List<Waypoint> Waypoints = new List<Waypoint>();
        public Waypoint CurrentWaypoint;
        public byte CurrentWaypointType = Waypoint.Loop;
        public bool IsWalkingBack = false; // False : Running on waypooints Start to End
        public int CurrentWaypointID = -1;
        public long NextAllowedMovementTime = 0;
        public bool Started = false;
        public bool Ended = false;

        // Waypoints
        private static readonly object WaypointsTableLock = new object();

        public void AddWaypoint(Waypoint AddWp)
        {
            if (_Owner.IsCreature())
            {
                if (Waypoints.Count == 0)
                {
                    Waypoint StartWp = new Waypoint();
                    StartWp.CreatureSpawnGUID = _Owner.GetCreature().Spawn.Guid;
                    StartWp.GameObjectSpawnGUID = _Owner._ObjectId;
                    StartWp.X = (ushort)_Owner.X;
                    StartWp.Y = (ushort)_Owner.Y;
                    StartWp.Z = (ushort)_Owner.Z;
                    StartWp.Speed = AddWp.Speed;
                    StartWp.WaitAtEndMS = AddWp.WaitAtEndMS;

                    lock (WaypointsTableLock)
                    {
                        StartWp.GUID = Convert.ToUInt32(WorldMgr.Database.GetNextAutoIncrement<Waypoint>());
                        Waypoints.Add(StartWp);
                        WorldMgr.DatabaseAddWaypoint(StartWp);
                    }
                    // lock (WaypointsTableLock)
                }
                AddWp.CreatureSpawnGUID = _Owner.GetCreature().Spawn.Guid;
                AddWp.GameObjectSpawnGUID = _Owner._ObjectId;
                lock (WaypointsTableLock)
                {
                    AddWp.GUID = Convert.ToUInt32(WorldMgr.Database.GetNextAutoIncrement<Waypoint>());
                    Waypoints.Add(AddWp);
                    WorldMgr.DatabaseAddWaypoint(AddWp);
                }
                // lock (WaypointsTableLock)
                Waypoint PrevWp = Waypoints[Waypoints.Count - 1];
                PrevWp.NextWaypointGUID = AddWp.GUID;
                SaveWaypoint(PrevWp);
            }
        }

        public void SaveWaypoint(Waypoint SaveWp)
        {
            WorldMgr.DatabaseSaveWaypoint(SaveWp);
        }

        public void RemoveWaypoint(Waypoint RemoveWp)
        {
            switch (Waypoints.Count)
            {
                case 0:
                case 1:
                    break;
                case 2:
                    lock (WaypointsTableLock)
                    {
                        foreach (Waypoint Wp in Waypoints)
                        {
                            WorldMgr.DatabaseDeleteWaypoint(Wp);
                        }
                        Waypoints.Clear();
                    }
                    // lock (WaypointsTableLock)
                    break;
                default:
                    lock (WaypointsTableLock)
                    {
                        int Index = -1;
                        foreach (Waypoint Wp in Waypoints)
                        {
                            if (Wp.GUID == RemoveWp.GUID)
                            {
                                Index = Waypoints.IndexOf(Wp);
                            }
                        }
                        if (Index != -1)
                        {
                            if (Index != 0)
                            {
                                if (Index == Waypoints.Count)
                                {
                                    Waypoints[Index - 1].NextWaypointGUID = 0;
                                }
                                else
                                {
                                    Waypoints[Index - 1].NextWaypointGUID = Waypoints[Index].NextWaypointGUID;
                                }
                            
                                WorldMgr.DatabaseSaveWaypoint(Waypoints[Index - 1]);
                                WorldMgr.DatabaseDeleteWaypoint(Waypoints[Index]);
                                Waypoints.RemoveAt(Index);
                            }
                        }
                    }
                    // lock (WaypointsTableLock)
                    break;
            } // switch
        }

        public void RemoveWaypoint(int WaypointGUID)
        {
            RemoveWaypoint(GetWaypoint(WaypointGUID));
        }

        public void RandomizeWaypoint(Waypoint RandomWp)
        {
            if (_Owner.GetCreature() != null)
            {
                RandomWp.X = (ushort)(_Owner.GetCreature().X + StaticRandom.Instance.Next(50) + StaticRandom.Instance.Next(100) + StaticRandom.Instance.Next(150));
                RandomWp.Y = (ushort)(_Owner.GetCreature().Y + StaticRandom.Instance.Next(50) + StaticRandom.Instance.Next(100) + StaticRandom.Instance.Next(150));
                RandomWp.Z = (ushort)_Owner.GetCreature().Z;
                RandomWp.Speed = 10;
                RandomWp.WaitAtEndMS = (uint)(5000 + StaticRandom.Instance.Next(10) * 1000);
                SaveWaypoint(RandomWp);
            }
        }

        public Waypoint GetWaypoint(int WaypointGUID)
        {
            foreach (Waypoint Wp in Waypoints)
            {
                if (Wp.GUID == WaypointGUID)
                {
                    return Wp;
                }       
            }
            return null;
        }

        public void UpdateWaypoints(long Tick)
        {
            if (State == AiState.STANDING || State == AiState.MOVING)
            {
                if (Waypoints.Count == 0)
                    return;

                if (CurrentWaypoint != null && IsAtWaypointEnd())
                    EndWaypoint(Tick);

                if (CanStartNextWaypoint(Tick))
                    SetNextWaypoint(Tick);

                if (State == AiState.MOVING && Mvt.CurrentSpeed == 0)
                    State = AiState.STANDING;

                if (State == AiState.STANDING && CurrentWaypoint != null)
                {
                    StartWaypoint(Tick);
                }
            }
        }

        public bool IsAtWaypointEnd()
        {
            if (_Owner.GetDistanceTo(CurrentWaypoint.X, CurrentWaypoint.Y, CurrentWaypoint.Z) < 3)
                return true;

            return true;
        }

        public bool CanStartNextWaypoint(long Tick)
        {
            if (CurrentWaypoint != null)
                return false;

            if (Tick <= NextAllowedMovementTime)
                return false;

            return true;
        }

        public void SetNextWaypoint(long Tick)
        {
            if (!IsWalkingBack)
                ++CurrentWaypointID;
            else
                --CurrentWaypointID;

            if (CurrentWaypointID < 0)
            {
                if (CurrentWaypointType == Waypoint.Loop)
                {
                    IsWalkingBack = false;
                    CurrentWaypointID = 0;
                }
            }
            else if (CurrentWaypointID >= Waypoints.Count)
            {
                if (CurrentWaypointType == Waypoint.Loop)
                {
                    IsWalkingBack = true;
                    CurrentWaypointID = Waypoints.Count - 2;
                }
            }

            if (CurrentWaypointID >= Waypoints.Count || CurrentWaypointID < 0)
            {
                CurrentWaypoint = null;
            }
            else
            {
                CurrentWaypoint = Waypoints[CurrentWaypointID];
                StartWaypoint(Tick);
            }
        }

        public void StartWaypoint(long Tick)
        {
            //Log.Info("Waypoints", "Starting Waypoint");

            State = AiState.MOVING;
            _Owner.GetUnit().MvtInterface.WalkTo(CurrentWaypoint.X, CurrentWaypoint.Y, CurrentWaypoint.Z, CurrentWaypoint.Speed);
            if (!Started)
            {
                // TODO : Messages,Emotes, etc

                if(CurrentWaypoint.TextOnStart != "")
                    _Owner.GetUnit().Say(CurrentWaypoint.TextOnStart, SystemData.ChatLogFilters.CHATLOGFILTERS_MONSTER_SAY);
            }

            Started = true;
            Ended = false;
        }

        public void EndWaypoint(long Tick)
        {
            //Log.Info("Waypoints", "Ending Waypoint");

            if (!Ended)
            {
                // TODO : Messages, Emote, etc
                if(CurrentWaypoint.TextOnEnd != "")
                    _Owner.GetUnit().Say(CurrentWaypoint.TextOnEnd, SystemData.ChatLogFilters.CHATLOGFILTERS_MONSTER_SAY);
            }

            NextAllowedMovementTime = Tick + CurrentWaypoint.WaitAtEndMS;
            CurrentWaypoint = null;
            Started = false;
            Ended = true;
        }

        public void LockMovement(long MSTime)
        {
            if (MSTime == 0)
                NextAllowedMovementTime = 0;
            else
                NextAllowedMovementTime = TCPManager.GetTimeStampMS() + MSTime;
        }

        #endregion
    }
}

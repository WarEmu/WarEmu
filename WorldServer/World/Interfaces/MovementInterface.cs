
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common;
using FrameWork;

namespace WorldServer
{
    public enum eFormationType
    {
        // M x x x
        Line,
        //		x
        // M x
        //		x
        Triangle,
        //		 x
        // x  M
        //		 x
        Protect,
    };

    public class MovementInterface : BaseInterface
    {
        static public readonly float METRE_SPEED_SEC = 6;
        static public readonly float METRE_SPEED_MS = 60;

        static public readonly int MIN_ALLOWED_DIST = 100;
        static public readonly int MAX_ALLOWED_DIST = 200;
        static public readonly int CONST_WALKTOTOLERANCE = 2;
        static public readonly int CREATURE_SPEED = 100;

        public Point3D TargetPosition = new Point3D(0, 0, 0);
        public Point3D StartPosition = new Point3D(0, 0, 0);
        public ushort TargetHeading = 0;

        public Mount CurrentMount;

        public bool IsMount()
        {
            return CurrentMount != null && CurrentMount.IsMount();
        }

        public void UnMount()
        {
            if(CurrentMount != null)
                CurrentMount.UnMount();
        }

        public override bool Load()
        {
            base.Load();
            if(_Owner.IsUnit())
                CurrentMount = new Mount(_Owner.GetUnit());
            return true;
        }

        public override void Update(long Tick)
        {
            UpdateFollow();

            if (CurrentSpeed != 0)
            {
                long e = MovementElapsedMs();
                int PX = (int)(StartPosition.X + e * TickSpeedX);
                int PY = (int)(StartPosition.Y + e * TickSpeedY);
                int PZ = (int)(StartPosition.Z + e * TickSpeedZ);
                _Owner.SetPosition((UInt16)PX, (UInt16)PY, (UInt16)PZ, _Owner.Heading, false);
            }

            base.Update(Tick);
        }

        public override void Stop()
        {
            StopMoving();
        }

        #region Turn

        public virtual void TurnTo(int tx, int ty)
        {
            TurnTo(tx, ty, true);
        }
        public virtual void TurnTo(int tx, int ty, bool sendUpdate)
        {
            _Owner.Heading = _Owner.GetHeading(new Point2D(tx, ty));
            TargetHeading = _Owner.Heading;

            if (!_Owner.IsPlayer() && sendUpdate)
                _Owner.GetUnit().StateDirty = true;
        }
        public virtual void TurnTo(ushort heading)
        {
            TurnTo(heading, true);
        }
        public virtual void TurnTo(ushort heading, bool sendUpdate)
        {
            _Owner.Heading = heading;
            TargetHeading = heading;

            if (!_Owner.IsPlayer() && sendUpdate)
                _Owner.GetUnit().StateDirty = true;
        }
        public virtual void TurnTo(Object target)
        {
            TurnTo(target, true);
        }
        public virtual void TurnTo(Object target, bool sendUpdate)
        {
            if (target == null || !target.IsInWorld() || target.Region != _Owner.Region)
                return;

            TurnTo(target.X, target.Y, sendUpdate);
        }

        #endregion

        #region Follow

        private Unit FollowTarget;
        private int FollowMinDist;
        private int FollowMaxDist;
        private int FollowSpeed;
        private eFormationType Formation = eFormationType.Line;

        public List<UInt16> _Followers = new List<ushort>();

        public void AddFollower(UInt16 Oid)
        {
            if (!_Followers.Contains(Oid))
                _Followers.Add(Oid);
        }
        public void RemoveFollower(UInt16 Oid)
        {
            _Followers.Remove(Oid);
        }
        public int GetFollowId(UInt16 Oid)
        {
            for (int i = 0; i < _Followers.Count; ++i)
                if (_Followers[i] == Oid)
                    return 1 + i;

            return 0;
        }
        public float GetFollowPct(UInt16 Oid)
        {
            float Id = GetFollowId(Oid);
            float Count = _Followers.Count;

            return (Id / Count) * 100;
        }

        public virtual void FollowUnit(Unit Target, int MinDist, int MaxDist,int Speed, eFormationType Form)
        {
            if (Target == null || Target == FollowTarget)
                return;

            StopFollow();

            Target.MvtInterface.AddFollower(_Owner.Oid);

            FollowTarget = Target;
            FollowMinDist = MinDist;
            FollowMaxDist = MaxDist;
            Formation = Form;
            FollowSpeed = Speed;
        }
        public virtual void StopFollow()
        {
            if (FollowTarget != null)
                FollowTarget.MvtInterface.RemoveFollower(_Owner.Oid);

            CancelWalkTo();
            FollowTarget = null;
            FollowMinDist = 0;
            FollowMaxDist = 0;
        }
        public virtual void StopFollow(Unit Unit)
        {
            if (FollowTarget == Unit)
                StopFollow();
        }
        public virtual void UpdateFollow()
        {
            if (FollowTarget == null)
                return;

            if (CurrentSpeed != 0)
                return;

            float Pct = FollowTarget.MvtInterface.GetFollowPct(_Owner.Oid);
            int Id = FollowTarget.MvtInterface.GetFollowId(_Owner.Oid);
            int Dist = _Owner.GetDistanceTo(FollowTarget);
            Point3D NewPos = new Point3D(0,0,0);
            UInt16 NewHead = 0;

            switch (Formation)
            {
                case eFormationType.Line:
                    {
                    int MinDist = Id * FollowMinDist;
                    int MaxDist = MinDist + (FollowMaxDist - FollowMinDist);

                    if (!_Owner.IsObjectInFront(FollowTarget, 10))
                        TurnTo(FollowTarget,true);

                    if (Dist >= MaxDist)
                    {
                        Point2D Tp = FollowTarget.GetBackPoint(FollowTarget.Heading, MinDist);
                        NewPos.X = Tp.X;
                        NewPos.Y = Tp.Y;
                        NewPos.Z = FollowTarget.Z;
                    }

                    break;
                    }
                case eFormationType.Protect:
                    {
                        if (!_Owner.IsObjectInFront(FollowTarget, 10))
                            TurnTo(FollowTarget, true);

                        if (Dist > FollowMaxDist)
                        {
                            Point2D Tp = FollowTarget.GetPointFromHeading((ushort)((float)4096 - ((float)4096 / (float)100 * Pct)), FollowMinDist);
                            NewPos.X = Tp.X;
                            NewPos.Y = Tp.Y;
                            NewPos.Z = FollowTarget.Z;
                        }
                    }
                    break;
            }

            if (NewPos.X != 0 || NewPos.Y != 0 || NewPos.Z != 0)
                WalkTo(NewPos, FollowSpeed);
            else if (NewHead != 0)
                TurnTo(NewHead, true);
        }
        public virtual bool IsFollowing()
        {
            return FollowTarget != null;
        }


        #endregion

        // 82mètre,speed100 = 4920ms
        public virtual int GetMsTimeToArriveAt(IPoint3D target, int speed)
        {
            return (int)(_Owner.GetDistanceTo(target) * (METRE_SPEED_MS / speed * 100f));
        }

        #region Move

        public long MovementStart;
        public int CurrentSpeed;
        public float TickSpeedX;
        public float TickSpeedY;
        public float TickSpeedZ;

        public virtual long MovementElapsedMs()
        {
            long Elapsed = TCPManager.GetTimeStampMS() - MovementStart;
            return Elapsed;
        }

        public virtual void ArriveAtTarget()
        {
            if(CurrentSpeed != 0)
                _Owner.SetPosition((UInt16)TargetPosition.X, (UInt16)TargetPosition.Y, (UInt16)TargetPosition.Z, _Owner.Heading, false);

            CancelWalkTo();
        }
        public bool IsAtTargetPosition()
        {
            return (_Owner.X == TargetPosition.X && _Owner.Y == TargetPosition.Y && _Owner.Z == TargetPosition.Z);
        }

        protected void SetTickSpeed(float dx, float dy, float dz)
        {
            TickSpeedX = dx;
            TickSpeedY = dy;
            TickSpeedZ = dz;
        }
        protected void SetTickSpeed(float dx, float dy, float dz, int speed)
        {
            // dx pour chaque mètre, un mètre met 60ms au speed *100 soit : MS/(speed/100) = 60/1 = 60ms par mètre
            float tickSpeed = (METRE_SPEED_MS / (float)speed) * 100f;
            SetTickSpeed(dx / tickSpeed, dy / tickSpeed, dz / tickSpeed);
        }
        public virtual void UpdateTickSpeed()
        {
            if (TargetPosition.X != 0 || TargetPosition.Y != 0 || TargetPosition.Z != 0)
            {
                float dist = StartPosition.GetDistanceTo(new Point3D(TargetPosition.X, TargetPosition.Y, TargetPosition.Z));

                if (dist <= 0)
                {
                    SetTickSpeed(0, 0, 0);
                }

                float dx = (float)(TargetPosition.X - StartPosition.X) / dist;
                float dy = (float)(TargetPosition.Y - StartPosition.Y) / dist;
                float dz = (float)(TargetPosition.Z - StartPosition.Z) / dist;

                SetTickSpeed(dx, dy, dz, CurrentSpeed);
                return;
            }
        }
        private void StartArriveAtTargetAction(int requiredTicks)
        {
            _Owner.EvtInterface.AddEvent(ArriveAtTarget, requiredTicks, 1);
        }

        public virtual void WalkTo(int x, int y, int z, int speed)
        {
            if (speed <= 0 || !_Owner.GetUnit().CanMove())
                return;

            TargetPosition.X = x;
            TargetPosition.Y = y;
            TargetPosition.Z = z;

            if (_Owner.IsWithinRadius(TargetPosition, CONST_WALKTOTOLERANCE))
            {
                _Owner.EvtInterface.Notify(EventName.ARRIVE_AT_TARGET, _Owner, null);
                CancelWalkTo();
                return;
            }

            TurnTo(_Owner.GetHeading(TargetPosition), false);

            MovementStart = TCPManager.GetTimeStampMS();
            CurrentSpeed = speed;

            if (!_Owner.IsPlayer())
                _Owner.IsMoving = true;
            StartPosition.X = _Owner.X;
            StartPosition.Y = _Owner.Y;
            StartPosition.Z = _Owner.Z;

            UpdateTickSpeed();
            _Owner.EvtInterface.Notify(EventName.ON_WALK_TO, _Owner, null);
            StartArriveAtTargetAction(GetMsTimeToArriveAt(TargetPosition, speed));
            _Owner.GetUnit().StateDirty = true;
        }

        public virtual void WalkTo(IPoint3D target, int speed)
        {
            WalkTo(target.X, target.Y, target.Z, speed);
        }
        public virtual void Walk(short speed)
        {
            _Owner.EvtInterface.Notify(EventName.ON_WALK, _Owner, null);

            CancelWalkTo();
            TargetPosition.Clear();

            CurrentSpeed = speed;

            MovementStart = TCPManager.GetTimeStampMS();
            UpdateTickSpeed();
        }
        public virtual void CancelWalkTo()
        {
            CurrentSpeed = 0;

            if(!_Owner.IsPlayer())
                _Owner.IsMoving = false;
            _Owner.EvtInterface.RemoveEvent(ArriveAtTarget);
            _Owner.Heading = TargetHeading;
            _Owner.GetUnit().StateDirty = true;
        }
        public virtual void StopMoving()
        {
            CancelWalkTo();
        }



        #endregion
    }
}

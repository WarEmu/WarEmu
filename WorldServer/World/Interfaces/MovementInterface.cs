
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
        static public readonly int CONST_WALKTOTOLERANCE = 3;

        private Point3D TargetPosition = new Point3D(0, 0, 0);
        private Point3D StartPosition = new Point3D(0, 0, 0);

        public MovementInterface(Object Obj)
            : base(Obj)
        {
        }

        public override void Update(long Tick)
        {
            UpdateFollow();

            if (CurrentSpeed != 0)
            {
                int PX = (int)(StartPosition.X + MovementElapsedMs() * TickSpeedX);
                int PY = (int)(StartPosition.Y + MovementElapsedMs() * TickSpeedY);
                int PZ = (int)(StartPosition.Z + MovementElapsedMs() * TickSpeedZ);
                Obj.SetPosition((UInt16)PX, (UInt16)PY, (UInt16)PZ, Obj.Heading);
            }

            base.Update(Tick);
        }

        #region Turn

        public virtual void TurnTo(int tx, int ty)
        {
            TurnTo(tx, ty, true);
        }
        public virtual void TurnTo(int tx, int ty, bool sendUpdate)
        {
            Obj.Heading = Obj.GetHeading(new Point2D(tx, ty));

            if (!Obj.IsPlayer() && sendUpdate)
                Obj.GetUnit().SendState(null);
        }
        public virtual void TurnTo(ushort heading)
        {
            TurnTo(heading, true);
        }
        public virtual void TurnTo(ushort heading, bool sendUpdate)
        {
            Obj.Heading = heading;
            if (!Obj.IsPlayer() && sendUpdate)
                Obj.GetUnit().SendState(null);
        }
        public virtual void TurnTo(Object target)
        {
            TurnTo(target, true);
        }
        public virtual void TurnTo(Object target, bool sendUpdate)
        {
            if (target == null || !target.IsInWorld() || target.Region != Obj.Region)
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

        public virtual void FollowUnit(Unit Target, int MinDist, int MaxDist,int Speed, eFormationType Form)
        {
            if (Target == null || Target == FollowTarget)
                return;

            StopFollow();

            Target.CbtInterface.AddFollower(Obj.Oid);

            FollowTarget = Target;
            FollowMinDist = MinDist;
            FollowMaxDist = MaxDist;
            Formation = Form;
            FollowSpeed = Speed;
        }
        public virtual void StopFollow()
        {
            if (FollowTarget != null)
                FollowTarget.CbtInterface.RemoveFollower(Obj.Oid);

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

            float Pct = FollowTarget.CbtInterface.GetFollowPct(Obj.Oid);
            int Id = FollowTarget.CbtInterface.GetFollowId(Obj.Oid);
            int Dist = Obj.GetDistanceTo(FollowTarget);
            Point3D NewPos = new Point3D(0,0,0);
            UInt16 NewHead = 0;

            switch (Formation)
            {
                case eFormationType.Line:
                    {
                    int MinDist = Id * FollowMinDist;
                    int MaxDist = MinDist + (FollowMaxDist - FollowMinDist);

                    if (!Obj.IsObjectInFront(FollowTarget, 10))
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
                        if (!Obj.IsObjectInFront(FollowTarget, 10))
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
            return (int)((float)(Obj.GetDistanceTo(target) / (float)speed) * METRE_SPEED_MS)*100;
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
            CancelWalkTo();
        }
        public bool IsAtTargetPosition()
        {
            return (Obj.X == TargetPosition.X && Obj.Y == TargetPosition.Y && Obj.Z == TargetPosition.Z);
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
            float tickSpeed = (METRE_SPEED_MS / (speed/100));
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
                    return;
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
            Obj.EvtInterface.AddEvent(ArriveAtTarget, requiredTicks, 1);
        }

        public virtual void WalkTo(IPoint3D target, int speed)
        {
            if (speed <= 0)
                return;

            TargetPosition.X = target.X;
            TargetPosition.Y = target.Y;
            TargetPosition.Z = target.Z;

            if (Obj.IsWithinRadius(TargetPosition, CONST_WALKTOTOLERANCE))
            {
                Obj.EvtInterface.Notify("ArriveAtTarget", Obj, null);
                return;
            }

            CancelWalkTo();

            TurnTo(Obj.GetHeading(TargetPosition), false);

            MovementStart = TCPManager.GetTimeStampMS();
            CurrentSpeed = speed;
            Obj.IsMoving = true;
            StartPosition.X = Obj.X;
            StartPosition.Y = Obj.Y;
            StartPosition.Z = Obj.Z;

            UpdateTickSpeed();
            Obj.EvtInterface.Notify("WalkTo", Obj, null);
            StartArriveAtTargetAction(GetMsTimeToArriveAt(TargetPosition, speed));
        }
        public virtual void Walk(short speed)
        {
            Obj.EvtInterface.Notify("Walk", Obj,null);

            CancelWalkTo();
            TargetPosition.Clear();

            CurrentSpeed = speed;

            MovementStart = TCPManager.GetTimeStampMS();
            UpdateTickSpeed();
        }
        public virtual void CancelWalkTo()
        {
            CurrentSpeed = 0;
            Obj.IsMoving = false;
            Obj.EvtInterface.RemoveEvent(ArriveAtTarget);
        }
        public virtual void StopMoving()
        {
            CancelWalkTo();

            if (Obj.IsMoving)
                CurrentSpeed = 0;
        }



        #endregion
    }
}

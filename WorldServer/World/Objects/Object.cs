
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common;
using FrameWork;

namespace WorldServer
{
    public class Object : Point3D
    {
        static public int RANGE_UPDATE_INTERVAL = 300; // Check des Ranged toutes les 300Ms

        public EventInterface EvtInterface;

        public UInt16 _ObjectId;
        public UInt16 Oid 
        { 
            get 
            { 
                return _ObjectId; 
            } 
            set 
            { 
                _ObjectId = value; 
            } 
        }
        private string _Name;
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public Object()
        {
            if (!IsPlayer())
                EvtInterface = new EventInterface(this);
        }

        private bool _Disposed = false;
        public bool IsDisposed { get { return _Disposed; } }
        public virtual void Dispose()
        {
            EvtInterface.Stop();

            RemoveFromWorld();

            _Disposed = true;
        }

        public virtual void Update()
        {
            EvtInterface.Update(TCPManager.GetTimeStampMS());
        }

        public bool _Loaded = false;
        public bool IsLoad() 
        { 
            return _Loaded; 
        }
        public void Load()
        {
            _Loaded = true;
            OnLoad();
        }

        public virtual void OnLoad()
        {

        }

        #region Sender

        public virtual void SendMeTo(Player Plr)
        {

        }
        public virtual void SendRemove(Player Plr)
        {
            PacketOut Out = new PacketOut((byte)Opcodes.F_REMOVE_PLAYER);
            Out.WriteUInt16(Oid);
            Out.WriteUInt16(0);
            if (Plr != null)
                Plr.SendPacket(Out);
            else
                DispatchPacket(Out, false);
        }
        public virtual void SendInteract(Player Plr,InteractMenu Menu)
        {
        }
        public virtual void Say(string Msg, SystemData.ChatLogFilters Filter)
        {
            foreach (Player Plr in _PlayerRanged.ToArray())
                Plr.SendMessage(this, Msg, Filter);

            if (IsPlayer())
                GetPlayer().SendMessage(this, Msg, Filter);
        }

        #endregion

        #region Detection

        public bool IsPlayer() { return this is Player; }
        public bool IsUnit() { return this is Unit; }
        public bool IsCreature() { return this is Creature; }
        public bool IsDoor() { return this is Door; }
        public bool IsGameObject() { return this is GameObject; }
        public bool IsChapter() { return this is ChapterObject; }

        public Unit GetUnit() { return this as Unit; }
        public Player GetPlayer() { return this as Player; }
        public Creature GetCreature() { return this as Creature; }

        #endregion

        #region Position

        public UInt16 Heading;
        public UInt16 XOffset, YOffset;
        public UInt32 XZone, YZone;

        public ZoneMgr _ZoneMgr;
        public CellMgr _Cell;
        public ZoneMgr Zone
        {
            get { return _ZoneMgr; }
            set{ _ZoneMgr = value; }
        }
        public RegionMgr Region
        {
            get { return _ZoneMgr != null ? _ZoneMgr.Region : null; }
        }
        public bool IsInWorld() { return _ZoneMgr != null; }
        public Point3D WorldPosition = new Point3D();

        public void RemoveFromWorld()
        {
            if (!IsInWorld())
                return;

            Region.RemoveObject(this);
        }

        public void CalculOffset()
        {
            if (!IsInWorld() || X==0 || Y==0)
                return;

            UInt16 Ox = (UInt16)Math.Truncate((decimal)(X / 4096 + Zone.Info.OffX));
            UInt16 Oy = (UInt16)Math.Truncate((decimal)(Y / 4096 + Zone.Info.OffY));

            if(Ox != XOffset || Oy != YOffset)
                SetOffset(Ox, Oy);
        }

        public void CalcWorldPositions()
        {
            int x = X > 32768 ? X - 32768 : X;
            int y = Y > 32768 ? Y - 32768 : Y;


            WorldPosition.X = (int)((int)XZone + ((int)((int)x) & 0x00000FFF));
            WorldPosition.Y = (int)((int)YZone + ((int)((int)y) & 0x00000FFF));
            WorldPosition.Z = Z / 2;
            if (Zone.ZoneId == 161)
                WorldPosition.Z = (32768 + Z) / 2;
        }

        public void SetOffset(UInt16 OffX, UInt16 OffY)
        {
            if (OffX <= 0 || OffY <= 0)
                return;

            XOffset = OffX;
            YOffset = OffY;
            XZone = (uint)(OffX << 12);
            YZone = (uint)(OffY << 12);

            if (IsInWorld())
                Region.CheckZone(this);
        }


        /// <summary>
		/// Returns the angle towards a target spot in degrees, clockwise
		/// </summary>
		/// <param name="tx">target x</param>
		/// <param name="ty">target y</param>
		/// <returns>the angle towards the spot</returns>
        public float GetAngle( IPoint2D point )
		{
			float headingDifference = ( GetHeading( point ) & 0xFFF ) - ( this.Heading & 0xFFF );

			if (headingDifference < 0)
				headingDifference += 4096.0f;

			return (headingDifference * 360.0f / 4096.0f);
		}

        /// <summary>
        /// Get distance to a point
        /// </summary>
        /// <remarks>
        /// If either Z-value is zero, the z-axis is ignored
        /// </remarks>
        /// <param name="point">Target point</param>
        /// <returns>Distance</returns>
        public override int GetDistanceTo( IPoint3D point )
        {
			Object obj = point as Object;

			if ( obj == null || this._ZoneMgr == obj._ZoneMgr )
			{
				return (int)(base.GetDistanceTo( point ));
			}
			else
			{
				return -1;
			}
        }
      
        /// <summary>
        /// Get distance to a point (with z-axis adjustment)
        /// </summary>
        /// <remarks>
        /// If either Z-value is zero, the z-axis is ignored
        /// </remarks>
        /// <param name="point">Target point</param>
        /// <param name="zfactor">Z-axis factor - use values between 0 and 1 to decrease the influence of Z-axis</param>
        /// <returns>Adjusted distance</returns>
        public override int GetDistanceTo( IPoint3D point, double zfactor )
        {
			Object obj = point as Object;

			if ( obj == null || this._ZoneMgr == obj._ZoneMgr )
			{
				return (int)(base.GetDistanceTo( point, zfactor ));
			}
			else
			{
				return -1;
			}
        }

        /// <summary>
        /// Checks if an object is within a given radius
        /// </summary>
        /// <param name="obj">Target object</param>
        /// <param name="radius">Radius</param>
        /// <returns>False if the object is null, in a different region, or outside the radius; otherwise true</returns>
        public bool IsWithinRadius( GameObject obj, int radius)
        {
			return IsWithinRadius(obj, radius, false);
		}

        /// <summary>
		/// Checks if an object is within a given radius, optionally ignoring z values
		/// </summary>
		/// <param name="obj">Target object</param>
		/// <param name="radius">Radius</param>
		/// <param name="ignoreZ">Ignore Z values</param>
		/// <returns>False if the object is null, in a different region, or outside the radius; otherwise true</returns>
		public bool IsWithinRadius(Object obj, int radius, bool ignoreZ)
		{
			if (obj == null)
				return false;

			if (this._ZoneMgr != obj._ZoneMgr)
				return false;

			return base.IsWithinRadius(obj, radius, ignoreZ);
		}

        /// <summary>
		/// determines wether a target object is front
		/// in front is defined as north +- viewangle/2
		/// </summary>
		/// <param name="target"></param>
		/// <param name="viewangle"></param>
		/// <returns></returns>
		public virtual bool IsObjectInFront(Object target, double viewangle)
		{
			return IsObjectInFront(target, viewangle, true);
		}

        /// <summary>
		/// determines wether a target object is front
		/// in front is defined as north +- viewangle/2
		/// </summary>
		/// <param name="target"></param>
		/// <param name="viewangle"></param>
		/// <param name="rangeCheck"></param>
		/// <returns></returns>
		public virtual bool IsObjectInFront(Object target, double viewangle, bool rangeCheck)
		{
			if (target == null)
				return false;
			float angle = this.GetAngle(target);
			if (angle >= 360 - viewangle / 2 || angle < viewangle / 2)
				return true;

			if (rangeCheck)
                return this.IsWithinRadius( target, 1 );
			else
                return false;
		}

        private bool _IsMoving = false;
        public bool IsMoving
        {
            get { return _IsMoving; }
            set
            {
                _IsMoving = value;
                if(_IsMoving)
                    EvtInterface.Notify("Moving", this, null);
            }
        }
        public Point2D LastRangeCheck = new Point2D(0,0);

        public virtual void InitPosition(UInt16 OffX, UInt16 OffY, UInt16 PinX, UInt16 PinY)
        {
            X = PinX;
            Y = PinY;
            XOffset = OffX;
            YOffset = OffY;
        }
        public virtual bool SetPosition(UInt16 PinX, UInt16 PinY, UInt16 PinZ, UInt16 Head)
        {
            bool Updated = false;
            if (PinX != X || PinY != Y || PinZ != Z || Head != Heading)
            {
                X = PinX;
                Y = PinY;
                Z = PinZ;
                Heading = Head;
                IsMoving = true;

                if(!IsPlayer())
                    CalculOffset();

                CalcWorldPositions();
                Updated = Region.UpdateRange(this);

                if (!IsPlayer())
                    GetUnit().SendState(null);
            }

            return Updated;
        }

        #endregion

        #region Range

        public List<Object> _ObjectRanged = new List<Object>();
        public List<Player> _PlayerRanged = new List<Player>();

        public virtual bool HasInRange(Object Obj)
        {
            return _ObjectRanged.Contains(Obj);
        }
        public virtual void AddInRange(Object Obj)
        {
            if (Obj == null)
                return;

            _ObjectRanged.Add(Obj);

            if (Obj.IsPlayer())
                _PlayerRanged.Add(Obj.GetPlayer());
        }
        public virtual void RemoveInRange(Object Obj)
        {
            if (Obj == null)
                return;

            if (!_ObjectRanged.Contains(Obj))
                return;

            _ObjectRanged.Remove(Obj);

            if(Obj.IsPlayer())
                _PlayerRanged.Remove(Obj.GetPlayer());

            if(IsPlayer())
                Obj.SendRemove(GetPlayer());
        }
        public virtual void ClearRange()
        {
            //SendRemove(null);

            foreach (Object Ranged in _ObjectRanged)
                Ranged.RemoveInRange(this);

            _PlayerRanged.Clear();
            _ObjectRanged.Clear();
        }

        public virtual void DispatchPacket(PacketOut Out, bool Self)
        {
            foreach (Object Player in _PlayerRanged)
                if (Player != null && Player != this)
                    Player.GetPlayer().SendCopy(Out);

            if (Self && IsPlayer())
                GetPlayer().SendPacket(Out);
                
        }

        #endregion
    }
}

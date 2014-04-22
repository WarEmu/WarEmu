
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common;
using FrameWork;

namespace WorldServer
{
    public abstract class BaseInterface
    {
        public Object _Owner;
        public bool Loaded = false;
        public bool IsLoad { get { return Loaded; } }

        public BaseInterface()
        {

        }

        public virtual void SetOwner(Object Owner)
        {
            this._Owner = Owner;
        }

        public virtual bool Load()
        {
            Loaded = true;
            return true;
        }

        public virtual void Update(long Tick)
        {

        }

        public virtual void Stop()
        {

        }

        public virtual void Save()
        {

        }

        public virtual bool HasObject()
        {
            return _Owner != null;
        }

        public virtual bool HasPlayer()
        {
            if (!HasObject())
                return false;

            return _Owner.IsPlayer();
        }

        public virtual bool HasUnit()
        {
            if (!HasObject())
                return false;

            return _Owner.IsUnit();

        }

        public virtual Unit GetUnit()
        {
            if (!HasUnit())
                return null;

            return _Owner.GetUnit();
        }

        public virtual Player GetPlayer()
        {
            if (!HasPlayer())
                return null;

            return _Owner.GetPlayer();
        }
    }
}

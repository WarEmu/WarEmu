
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common;
using FrameWork;

namespace WorldServer
{
    public class BaseInterface
    {
        public Object Obj;
        public bool Loaded = false;

        public BaseInterface()
        {

        }

        public BaseInterface(Object Obj)
        {
            this.Obj = Obj;
        }

        public virtual void Update(long Tick)
        {

        }

        public virtual bool Load()
        {
            Loaded = true;
            return true;
        }

        public bool IsLoad { get { return Loaded; } }

        public virtual bool HasObject()
        {
            return Obj != null;
        }
        public virtual bool HasPlayer()
        {
            if (!HasObject())
                return false;

            return Obj.IsPlayer();
        }
        public virtual bool HasUnit()
        {
            if (!HasObject())
                return false;

            return Obj.IsUnit();

        }
        public virtual Unit GetUnit()
        {
            if (!HasUnit())
                return null;

            return Obj.GetUnit();
        }
        public virtual Player GetPlayer()
        {
            if (!HasPlayer())
                return null;

            return Obj.GetPlayer();
        }

        public virtual void Save()
        {

        }
        public virtual void Stop()
        {
            Obj = null;
        }
    }
}

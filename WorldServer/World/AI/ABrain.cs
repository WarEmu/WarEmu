using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WorldServer
{
    public abstract class ABrain
    {
        public AIInterface Interface;
        public bool IsStart = false;
        public bool IsStop = false;

        public ABrain(AIInterface Interface)
        {
            this.Interface = Interface;
        }

        public virtual bool Start()
        {
            if (IsStart)
                return false;

            IsStart = true;
            return true;
        }

        public virtual bool Stop()
        {
            if (IsStop)
                return false;

            IsStop = true;
            return true;
        }

        public abstract void Think();
    }
}

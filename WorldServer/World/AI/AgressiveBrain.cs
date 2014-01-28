using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FrameWork;

namespace WorldServer
{
    public class AgressiveBrain : ABrain
    {
        public AgressiveBrain(AIInterface Interface)
            : base(Interface)
        {

        }

        public override void Think()
        {
            if (Interface.Cbt.IsFighting())
                return;

            Unit Target = Interface.GetAttackableUnit();
            if (Target == null)
                return;

            Interface.Cbt.CombatStart(Target);
        }
    }
}

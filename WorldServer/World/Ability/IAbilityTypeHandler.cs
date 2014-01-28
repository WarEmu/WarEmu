
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FrameWork;
using Common;

namespace WorldServer
{
    [AttributeUsage(AttributeTargets.Class)]
    public class IAbilityTypeAttribute : Attribute
    {
        public UInt16 AbilityType;
        public string TypeDescription;

        public IAbilityTypeAttribute(UInt16 AbilityType, string TypeDescription)
        {
            this.AbilityType = AbilityType;
            this.TypeDescription = TypeDescription;
        }
    }

    public abstract class IAbilityTypeHandler
    {
        public abstract void Start(Ability Ab);

        public abstract void Update();

        public abstract GameData.AbilityResult CanCast();

        public abstract void Cast();

        public abstract void Stop();

        public abstract UInt32 GetAbilityDamage();
    }
}

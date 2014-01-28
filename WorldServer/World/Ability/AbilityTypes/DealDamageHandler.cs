
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common;
using FrameWork;

namespace WorldServer
{
    [IAbilityType(1,"Deal Damage Handler")]
    public class DealDamageHandler : IAbilityTypeHandler
    {
        public Ability Ab;
        public Unit Target;

        public override void Start(Ability Ab)
        {
            this.Ab = Ab;

            if (Ab.Caster.IsUnit())
                Target = Ab.Caster.GetUnit().CbtInterface.GetTarget();

            Log.Info("DealDamage", "Target = " + Target);
        }

        public override GameData.AbilityResult CanCast()
        {
            if (Target == null)
                return GameData.AbilityResult.ABILITYRESULT_ILLEGALTARGET;
            else if (Target.IsDead)
                return GameData.AbilityResult.ABILITYRESULT_ILLEGALTARGET_DEAD;
            else if (Ab.Caster.GetDistance(Target) > Ab.Info.MaxRange)
                return GameData.AbilityResult.ABILITYRESULT_OUTOFRANGE;
            else if (Ab.Caster.GetUnit().CbtInterface.GetTargetType() != GameData.TargetTypes.TARGETTYPES_TARGET_ENEMY)
                return GameData.AbilityResult.ABILITYRESULT_ILLEGALTARGET_IN_YOUR_ALLIANCE;
            
            return GameData.AbilityResult.ABILITYRESULT_OK;
        }

        public override void Cast()
        {
            if (Target == null)
                return;

            Log.Info("DealDamage", "Cast");

            uint Damage = GetAbilityDamage();

            Ab.SendStartCasting(Target.Oid);
            Ab.SendSpellDamage(Target.Oid, Damage);
            Ab.Caster.GetUnit().SendAttackState(Target, (ushort)Damage, (ushort)Damage, Ab.Info);
            Ab.Caster.GetUnit().DealDamage(Target, (int)Damage, 0, 0);
            Ab.SendAbilityDone(Target.Oid);
        }

        public override void Stop()
        {
            
        }

        public override void Update()
        {
            
        }

        public override uint GetAbilityDamage()
        {
            uint Damage = (uint)Ab.Info.Damage;

            if(Ab.Info.MaxRange <= 5)
                Damage += (uint)(Ab.Caster.GetUnit().StsInterface.GetTotalStat((byte)GameData.Stats.STATS_STRENGTH) / 5);

            return Damage;
        }
    }
}

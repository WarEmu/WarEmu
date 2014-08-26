/*
 * Copyright (C) 2013 APS
 *	http://AllPrivateServer.com
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common;
using FrameWork;

namespace WorldServer
{
    [IAbilityType("ResurrectsTarget", "ResurrectsTarget Handler")]
    public class ResurrectsTargetHandler : IAbilityTypeHandler
    {
        public Ability Ab;
        public Unit Target;

        public override void InitAbility(Ability Ab)
        {
            this.Ab = Ab;

            if (Ab.Caster.IsUnit())
            {
                Target = Ab.Caster.GetUnit().CbtInterface.GetTarget(GameData.TargetTypes.TARGETTYPES_TARGET_ALLY);
                /*if (Target != null && !Target.IsPlayer())
                    Target = null;*/
            }
        }

        public override GameData.AbilityResult CanCast(bool IsStart)
        {
            GameData.AbilityResult Result = GameData.AbilityResult.ABILITYRESULT_OK;

            if (Target == null)
                Result = GameData.AbilityResult.ABILITYRESULT_ILLEGALTARGET;
            else if (!Target.IsDead)
                Result = GameData.AbilityResult.ABILITYRESULT_ILLEGALTARGET_DEAD;
            else if (CombatInterface.IsEnemy(Ab.Caster.GetUnit(), Target))
                Result = GameData.AbilityResult.ABILITYRESULT_ILLEGALTARGET_IN_YOUR_ALLIANCE;

            return Result;
        }

        public override IAbilityTypeHandler Cast()
        {
            if (Target == null || !Target.IsDead)
                return null;

            CallOnCast(this, 0);
            Target.RezUnit();
            Target.Health = (Target.MaxHealth * Ab.Info.GetPercent(0)) / 100;
            return null;
        }

        public override void SendDone()
        {
            if (Target != null)
            {
                Ab.SendAbilityDone(Target.Oid);
            }
        }

        public override void Stop()
        {
            SendDone();
            Target = null;
        }

        public override void Update(long Tick)
        {
            if (Target == null || !Target.IsDead)
                Ab.Interface.Cancel(false);
        }

        public override void GetTargets(OnTargetFind OnFind)
        {
            OnFind(Target);
        }
    }
}

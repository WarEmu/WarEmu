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
    [IAbilityType("SimpleHeal", "Heal Target Handler")]
    public class SimpleHealHandler : IAbilityTypeHandler
    {
        public Ability Ab;
        public Unit Target;

        public override void InitAbility(Ability Ab)
        {
            this.Ab = Ab;

            if (Ab.Caster.IsUnit())
            {
                Target = Ab.Caster.CbtInterface.GetTarget(GameData.TargetTypes.TARGETTYPES_TARGET_ALLY);

                if (Target == null)
                    Target = Ab.Caster.GetUnit();
            }
        }

        public override void Start(Ability Ab)
        {
            if (Target == null)
                return;
        }

        public override GameData.AbilityResult CanCast(bool IsStart)
        {
            GameData.AbilityResult Result = GameData.AbilityResult.ABILITYRESULT_OK;

            if (Target == null)
                Result = GameData.AbilityResult.ABILITYRESULT_ILLEGALTARGET;
            else if (Target.IsDead)
                Result = GameData.AbilityResult.ABILITYRESULT_ILLEGALTARGET_DEAD;
            else if (Ab.Caster.GetDistanceTo(Target) > Ab.Info.Info.MaxRange)
                Result = GameData.AbilityResult.ABILITYRESULT_OUTOFRANGE;
            else if (IsStart && Ab.Caster.GetDistanceTo(Target) < Ab.Info.Info.MinRange)
                Result = GameData.AbilityResult.ABILITYRESULT_TOOCLOSE;
            else if (IsStart && Ab.Info.Info.MinRange <= 5 && !Ab.Caster.IsObjectInFront(Target, 110))
                Result = GameData.AbilityResult.ABILITYRESULT_OUT_OF_ARC;
            else if (!CombatInterface.IsFriend(Ab.Caster.GetUnit(),Target))
                Result = GameData.AbilityResult.ABILITYRESULT_ILLEGALTARGET_NOT_ALLY;

            //Log.Info("SimpleHeal", Result.ToString());
            return Result;
        }

        public override IAbilityTypeHandler Cast()
        {
            if (Target == null)
                return null;

            //Log.Info("DealDamage", "Cast");

            if (CustomValue == 0 && Ab.Info.GetTime(0) != 0 && Ab.Info.Info.ApSec == 0)
                return Target.AbtInterface.AddBuff(Ab, "HealOverTime");

            if (CustomValue != 0 || Ab.Info.Info.ApSec != 0 || Ab.Info.GetTime(0) == 0 || Ab.Info.GetHeal(1) != 0)
            {
                uint Damage = GetAbilityDamage();
                CallOnCast(this, Damage);
                Ab.Caster.DealHeal(Target, Ab, Damage);
            }

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
            if (Target == null || Target.IsDead)
                Ab.Interface.Cancel(false);
        }

        public override uint GetAbilityDamage()
        {
            if (CustomValue != 0)
                return CustomValue;

            float Damage = (float)Ab.Info.GetHeal(0) + (float)Ab.Info.Level * 9f;
            Damage += ((float)Ab.Caster.ItmInterface.GetDamage() * 0.1f) * ((float)Ab.Info.Info.CastTime * 0.001f);
            if (Damage < 0) Damage = 0;
            return (uint)Damage;
        }

        public override void GetTargets(OnTargetFind OnFind)
        {
            OnFind(Target);
        }
    }
}

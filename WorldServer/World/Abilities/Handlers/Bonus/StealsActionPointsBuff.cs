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
    [IAbilityType("StealsActionPointsBuff", "StealsActionPointsBuff Handler")]
    public class StealsActionPointsBuff : IAbilityTypeHandler
    {
        public Ability Ab;
        public Unit Target;

        public override void InitAbility(Ability Ab)
        {
            this.Ab = Ab;
            this.Target = Ab.Parent.Handler.GetTarget();
        }

        public override void Reset()
        {
            this.Ab.SetBuff(Ab.Info.GetTime(0) * 1000);
        }

        public override void Start(Ability Ab)
        {
            if (!CombatInterface.CanAttack(Ab.Caster, Target))
                Stop();
        }

        public override void OnTick(int Id) // Second Tick
        {
            Cast();
        }

        public override IAbilityTypeHandler Cast()
        {
            if (!CombatInterface.CanAttack(Ab.Caster, Target))
                return null;

            //Log.Info("DealDamage", "Cast");

            uint Damage = Ab.Info.GetHeal(0) / Ab.Info.GetTime(0);
            CallOnCast(this, Damage);
            if (Target.ActionPoints >= Damage)
            {
                Target.ActionPoints -= (ushort)Damage;
                Ab.Caster.ActionPoints += (ushort)Damage;
            }
            else
            {
                Ab.Caster.ActionPoints += Target.ActionPoints;
                Target.ActionPoints -= 0;
            }

            return null;
        }

        public override void SendDone()
        {

        }

        public override void Stop()
        {
            Target = null;
        }

        public override void Update(long Tick)
        {
            if (!CombatInterface.CanAttack(Ab.Caster, Target))
                Stop();
        }
    }
}

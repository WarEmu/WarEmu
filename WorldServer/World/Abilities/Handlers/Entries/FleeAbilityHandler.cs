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
    [IAbilityType(new ushort[] { 245 }, "Flee Ability Handler")]
    public class FleeAbilityHandler : IAbilityTypeHandler
    {
        public Ability Ab;

        public override void InitAbility(Ability Ab)
        {
            this.Ab = Ab;
        }

        public override void Start(Ability Ab)
        {

        }

        public override void SendDone()
        {
            Ab.SendAbilityDone(Ab.Caster.Oid);
        }

        public override void Stop()
        {
            SendDone();
        }

        public override GameData.AbilityResult CanCast(bool IsStart)
        {
            GameData.AbilityResult Result = GameData.AbilityResult.ABILITYRESULT_OK;

            //Log.Info("SimpleHeal", Result.ToString());
            return Result;
        }

        public override IAbilityTypeHandler Cast()
        {
            Ab.Caster.ActionPoints = 0;
            Ab.Caster.ResetNextActionPoints(10000);
            Ab.Caster.AbtInterface.AddBuff(Ab, "FleeAbilityBuff");
            return null;
        }
    }
}

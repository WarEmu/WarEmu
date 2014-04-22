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
    [IAbilityType(new ushort[] { 9566 }, "DealDamagesInFront", "Deal Damages In Front Zone Handler")] // Essence Lash
    public class DealDamagesInFront : IAbilityTypeHandler
    {
        public Ability Ab;
        public DealDamagesHandler DealHandler;

        public override void InitAbility(Ability Ab)
        {
            this.Ab = Ab;
            this.DealHandler = new DealDamagesHandler();
            DealHandler.InitAbility(Ab);
        }

        public override void Start(Ability Ab)
        {
            base.Start(Ab);
            DealHandler.Start(Ab);
        }

        public override GameData.AbilityResult CanCast(bool IsStart)
        {
            GameData.AbilityResult Result = GameData.AbilityResult.ABILITYRESULT_OK;

            return Result;
        }

        public override IAbilityTypeHandler Cast()
        {
            int Count = 0;

            GetTargets((Unit Target) =>
            {
                if ((Ab.Info.Entry == 1747 || Ab.Info.Entry == 1435) && Count >= 3) // (Choppa , Lotsa) (Slayer, Flurry) , hit 3 targets
                    return;

                if (Ab.Info.Entry == 1490 & Count >= 9) // (Accuracy, Slayer)
                    return;

                DealHandler.InitTargets(Target);
                if (DealHandler.CanCast(false) == GameData.AbilityResult.ABILITYRESULT_OK)
                {
                    if (Ab.Info.Entry == 8397) // Mouth of Tzeentch
                        Target.AbtInterface.Cancel(false);

                    DealHandler.Cast();
                    if (Ab.Info.Entry == 8038) // Heaven's Fury , movement stuck 3 seconds
                        Target.DisableMovements(3000);

                    ++Count;
                }
            });

            return null;
        }

        public override void GetTargets(OnTargetFind OnFind)
        {
            Unit Target;
            foreach (Object Obj in Ab.Caster._ObjectRanged)
            {
                if (!Obj.IsUnit())
                    continue;

                Target = Obj.GetUnit();
                if (!CombatInterface.CanAttack(Ab.Caster, Target))
                    continue;

                if (Obj.GetDistanceTo(Ab.Px, Ab.Py, Ab.Pz) < Ab.Info.GetRadius(0) && Ab.Caster.IsObjectInFront(Target,90))
                {
                    OnFind(Target);
                }
            }
        }
    }
}

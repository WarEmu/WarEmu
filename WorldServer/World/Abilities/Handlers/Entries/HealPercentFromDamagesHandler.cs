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
    [IAbilityType("HealPercentFromDamage", "Heal Percent From Damages Handler")]
    public class HealPercentFromDamagesHandler : IAbilityTypeHandler
    {
        public Ability Ab;
        public DealDamagesHandler DamagesHandler;
        public IAbilityTypeHandler HealHandler;

        public override void InitAbility(Ability Ab)
        {
            this.Ab = Ab;
            this.DamagesHandler = new DealDamagesHandler();

            if (Ab.Info.Info.GroupMates)
                this.HealHandler = new SimpleHealGroupHandler();
            else
                this.HealHandler = new SimpleHealHandler();
            HealHandler.InitAbility(Ab);
            DamagesHandler.InitAbility(Ab);
            DamagesHandler.OnCastEvents += OnDealDamageTarget;
        }

        public override void Start(Ability Ab)
        {
            HealHandler.Start(Ab);
            DamagesHandler.Start(Ab);
        }

        public override void GetTargets(OnTargetFind OnFind)
        {
            DamagesHandler.GetTargets(OnFind);
        }

        public void OnDealDamageTarget(IAbilityTypeHandler Handler, uint Damage)
        {
            HealHandler.CustomValue = (uint)(((float)Damage * (float)Ab.Info.GetPercent(0)) / 100f) + Ab.Info.GetHeal(0);
            HealHandler.Cast();
        }

        public override GameData.AbilityResult CanCast(bool IsStart)
        {
            return DamagesHandler.CanCast(IsStart);
        }

        public override IAbilityTypeHandler Cast()
        {
            IAbilityTypeHandler NewHandler = DamagesHandler.Cast();
            if (NewHandler != null)
                NewHandler.OnCastEvents += OnDealDamageTarget;
            return null;
        }

        public override void SendDone()
        {
            DamagesHandler.SendDone();
            HealHandler.SendDone();
        }

        public override void Stop()
        {
            SendDone();
        }

        public override void Update(long Tick)
        {

        }
    }
}

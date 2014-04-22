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
    [IAbilityType(new ushort[] { 1907, 9557 }, "SimpleHealGroup", "Heal Group Handler")] // 
    public class SimpleHealGroupHandler : IAbilityTypeHandler
    {
        public Ability Ab;
        public Player PlrCaster;
        public GameData.TargetTypes T;

        public override void InitAbility(Ability Ab)
        {
            this.Ab = Ab;
            PlrCaster = Ab.Caster.GetPlayer();
        }

        public override void Start(Ability Ab)
        {
            if (PlrCaster == null)
                Stop();
        }

        public override GameData.AbilityResult CanCast(bool IsStart)
        {
            GameData.AbilityResult Result = GameData.AbilityResult.ABILITYRESULT_OK;

            return Result;
        }

        public override IAbilityTypeHandler Cast()
        {
            if (PlrCaster.GetGroup() != null)
            {
                uint Damage = GetAbilityDamage();
                CallOnCast(this, Damage);
                GetTargets((Unit Target) =>
                    {
                        Ab.Caster.DealHeal(Target, Ab, Damage);
                    });
            }

            return null;
        }

        public override void GetTargets(OnTargetFind OnFind)
        {
            if (OnFind == null)
                return;

            lock (PlrCaster.GetGroup().Members)
            {
                foreach (Player Plr in PlrCaster.GetGroup().Members)
                    if (Plr != PlrCaster && Plr.GetDistanceTo(PlrCaster) < Ab.Info.GetRadius(0))
                        OnFind(Plr);
            }
        }

        public override void SendDone()
        {

        }

        public override void Stop()
        {

        }

        public override void Update(long Tick)
        {
            if (PlrCaster.GetGroup() == null)
                Stop();
        }

        public override uint GetAbilityDamage()
        {
            if (CustomValue != 0)
                return CustomValue;

            float Damage = (float)Ab.Info.GetHeal(0) + (float)Ab.Info.Level * 5.82f;

            Damage += ((float)Ab.Caster.ItmInterface.GetDamage() * 0.1f) * ((float)Ab.Info.Info.CastTime * 0.001f);

            if (Damage < 0) Damage = 0;
            return (uint)Damage;
        }
    }
}

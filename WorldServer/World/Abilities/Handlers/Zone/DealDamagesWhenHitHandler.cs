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
    [IAbilityType(new ushort[]{ 1762 }, "DealDamagesWhenHit", "DealDamagesWhenHit Handler")] // Bring it on
    public class DealDamagesWhenHitHandler : IAbilityTypeHandler
    {
        public Ability Ab;
        public bool IsValid;

        public override void InitAbility(Ability Ab)
        {
            this.Ab = Ab;
            IsValid = true;
        }

        public override void Start(Ability Ab)
        {

        }

        public override void OnTick(int Id)
        {
            IsValid = true;
        }

        public override void OnReceiveDamages(Unit Attacker, Ability Spell, ref uint Damages)
        {
            if (IsValid)
            {
                if (CombatInterface.CanAttack(Ab.Caster, Attacker))
                {
                    IsValid = false;
                    Ab.Caster.DealDamages(Attacker, Ab, GetAbilityDamage());
                }
            }
        }

        public override void SendDone()
        {
            Ab.SendAbilityDone(0);
        }

        public override void Stop()
        {
            SendDone();
        }

        public override void Update(long Tick)
        {

        }

        public override uint GetAbilityDamage()
        {
            if (CustomValue != 0)
                return CustomValue;

            float Damage = (float)Ab.Info.GetDamage(0) + (float)Ab.Info.Level * 5.82f;

            if (Ab.Info.Info.MaxRange <= 5)
            {
                Damage += Ab.Caster.StsInterface.GetTotalStat((byte)GameData.Stats.STATS_STRENGTH) / 5;
                Damage += ((float)Ab.Caster.ItmInterface.GetDamage() * 0.1f) * ((float)Ab.Info.Info.CastTime * 0.001f);
            }

            if (Damage < 0) Damage = 0;
            return (uint)Damage;
        }

        public override void GetTargets(OnTargetFind OnFind)
        {

        }
    }
}

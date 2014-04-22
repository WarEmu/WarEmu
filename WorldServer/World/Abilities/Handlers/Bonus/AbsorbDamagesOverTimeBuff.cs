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
    [IAbilityType("AbsorbDamagesOverTime", "Heal Over Time Handler")]
    public class AbsorbDamagesOverTimeBuff : IAbilityTypeHandler
    {
        public Ability Ab;
        public uint AbsorbCount;
        public uint OriginalCount;

        public override void InitAbility(Ability Ab)
        {
            this.Ab = Ab;
            this.OriginalCount = GetAbilityDamage();
        }

        public override void Reset()
        {
            AbsorbCount = OriginalCount;
            this.Ab.SetBuff(Ab.Info.GetTime(0) * 1000);
        }

        public override void Start(Ability Ab)
        {

        }

        public override void OnTick(int Id) // Second Tick
        {

        }

        public override void SendDone()
        {

        }

        public override void Stop()
        {

        }

        public override void Update(long Tick)
        {

        }

        public override void OnReceiveDamages(Unit Attacker, Ability Spell, ref uint Damages)
        {
            if (Damages >= AbsorbCount)
            {
                Damages -= AbsorbCount;
                AbsorbCount = 0;
                //Attacker.SendCastEffect(Ab.Caster, (ushort)(Spell != null ? Spell.Info.Entry : 0), GameData.CombatEvent.COMBATEVENT_ABSORB, 100);

                /*PacketOut Out = new PacketOut((byte)Opcodes.F_HIT_PLAYER);
                Out.WriteUInt16(Attacker.Oid);
                Out.WriteUInt16(Ab.Caster.Oid);
                Out.WriteHexStringBytes("000008C21D000000");
                Ab.Caster.GetPlayer().SendPacket(Out);*/
            }
            else
            {
                AbsorbCount -= Damages;
                Damages = 0;
                //Attacker.SendCastEffect(Ab.Caster, (ushort)(Spell != null ? Spell.Info.Entry : 0), GameData.CombatEvent.COMBATEVENT_ABSORB, 100);
            }
        }

        public override uint GetAbilityDamage()
        {
            if (CustomValue != 0)
                return CustomValue;

            float Damage = (float)Ab.Info.GetDamage(1);
            if (Damage == 0)
                Damage = (float)Ab.Info.GetDamage(0);

            Damage += (float)Ab.Info.Level * 9f;
            Damage += ((float)Ab.Caster.ItmInterface.GetDamage() * 0.1f) * ((float)Ab.Info.Info.CastTime * 0.001f);

            if (Damage < 0) Damage = 0;
            return (uint)Damage;
        }
    }
}

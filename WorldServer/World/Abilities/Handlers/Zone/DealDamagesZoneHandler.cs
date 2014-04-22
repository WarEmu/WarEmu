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
    [IAbilityType("DealZoneDamages", "Deal Damages Zone Handler")]
    public class DealZoneDamagesHandler : IAbilityTypeHandler
    {
        public Ability Ab;
        public GameObject InvisibleObject;
        public long NextUpdate = 0;
        public DealDamagesHandler TargetHandler;

        public override void InitAbility(Ability Ab)
        {
            this.Ab = Ab;
            if (Ab.Info.RadiusAroundTarget)
                TargetHandler = new DealDamagesHandler();

            if (TargetHandler != null)
                TargetHandler.InitAbility(Ab);
        }

        public override void Start(Ability Ab)
        {
            if(TargetHandler == null)
                InvisibleObject = Ab.Caster.Region.CreateGameObject(23, Ab.ZoneId, Ab.Px, Ab.Py, Ab.Pz);
            else
                InvisibleObject = Ab.Caster.Region.CreateGameObject(23, TargetHandler.Target.Zone.ZoneId, (ushort)TargetHandler.Target.X, (ushort)TargetHandler.Target.Y, (ushort)TargetHandler.Target.Z);
            
            InvisibleObject.Load();

            if (TargetHandler != null)
                TargetHandler.Start(Ab);
        }

        public override GameData.AbilityResult CanCast(bool IsStart)
        {
            GameData.AbilityResult Result = GameData.AbilityResult.ABILITYRESULT_OK;
            if (TargetHandler != null)
                return TargetHandler.CanCast(IsStart);

            return Result;
        }

        public override IAbilityTypeHandler Cast()
        {
            if (TargetHandler != null)
                TargetHandler.Cast();

            uint Damage = GetAbilityDamage();
            if (Damage == 0)
                return null;

            CallOnCast(this, Damage);
            GetTargets((Unit Target) =>
                {
                    Ab.Caster.DealDamages(Target, Ab, Damage);
                });

            return null;
        }

        public override void GetTargets(OnTargetFind OnFind)
        {
            if (OnFind == null)
                return;

            if (Ab.Info.GetDamage(0) == 0)
                return;

            Unit Target;
            foreach (Object Obj in InvisibleObject._ObjectRanged)
            {
                if (!Obj.IsUnit())
                    continue;

                Target = Obj.GetUnit();
                if (!CombatInterface.CanAttack(Ab.Caster, Target))
                    continue;

                if (Obj.GetDistanceTo(Ab.Px, Ab.Py, Ab.Pz) < Ab.Info.GetRadius(0))
                {
                    OnFind(Target);
                }
            }
        }

        public override void SendDone()
        {
            if (TargetHandler != null)
                TargetHandler.SendDone();
        }

        public override void Stop()
        {
            if (TargetHandler != null)
                TargetHandler.Stop();

            InvisibleObject.Dispose();
            InvisibleObject = null;
        }

        public override void OnTick(int Id)
        {
            base.OnTick(Id);

            if (Ab.Info.Info.ApSec != 0)
                Cast();
        }

        public override uint GetAbilityDamage()
        {
            if (CustomValue != 0)
                return CustomValue;

            float Damage = (float)Ab.Info.GetDamage(0) + (float)Ab.Info.Level * 5.82f;


            Unit C = Ab.Caster.GetUnit();

            if (Ab.Info.Info.MaxRange <= 5)
                Damage += C.StsInterface.GetTotalStat((byte)GameData.Stats.STATS_STRENGTH) / 5;

            Damage += ((float)C.ItmInterface.GetDamage() * 0.1f) * ((float)Ab.Info.Info.CastTime * 0.001f);

            if (Damage < 0) Damage = 0;
            return (uint)Damage;
        }
    }
}

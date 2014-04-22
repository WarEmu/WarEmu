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

using FrameWork;
using Common;

namespace WorldServer
{
    [AttributeUsage(AttributeTargets.Class)]
    public class IAbilityTypeAttribute : Attribute
    {
        public ushort[] AbilityEntry;

        public string HandlerName;
        public string TypeDescription;

        public IAbilityTypeAttribute(ushort[] AbilityEntry, string TypeDescription)
        {
            this.AbilityEntry = AbilityEntry;
            this.TypeDescription = TypeDescription;
        }

        public IAbilityTypeAttribute(string HandlerName, string TypeDescription)
        {
            this.HandlerName = HandlerName;
            this.TypeDescription = TypeDescription;
        }

        public IAbilityTypeAttribute(ushort[] AbilityEntry, string HandlerName, string TypeDescription)
        {
            this.AbilityEntry = AbilityEntry;
            this.HandlerName = HandlerName;
            this.TypeDescription = TypeDescription;
        }
    }

    public abstract class IAbilityTypeHandler
    {
        public delegate void OnCastDelegate(IAbilityTypeHandler Handler, uint Count);
        public delegate void OnTargetFind(Unit Target);
        public OnCastDelegate OnCastEvents;
        public void CallOnCast(IAbilityTypeHandler Handler, uint Count)
        {
            if (OnCastEvents != null)
                OnCastEvents(Handler, Count);
        }

        public abstract void InitAbility(Ability Ab);

        public virtual void Start(Ability Ab) { }

        public virtual void Stop() { }

        public virtual void Update(long Tick) { }

        public virtual void OnTick(int Id)
        {

        }

        public virtual void SendDone() { }

        public virtual GameData.AbilityResult CanCast(bool IsStart)
        {
            return GameData.AbilityResult.ABILITYRESULT_OK;
        }

        public virtual IAbilityTypeHandler Cast() { return null; }
        public virtual uint GetAbilityDamage() { return 0; }

        public virtual void Reset()
        {

        }

        #region Targets

        public virtual void InitTargets(List<Unit> Targets)
        {

        }

        public virtual void InitTargets(Unit Target)
        {

        }

        public Unit GetTarget()
        {
            Unit T = null;
            GetTargets((Unit Target) => { T = Target; });
            return T;
        }

        public virtual void GetTargets(OnTargetFind OnFind)
        {

        }

        #endregion

        #region Events

        public virtual void OnReceiveDamages(Unit Attacker,  Ability Ab, ref uint Damages) { }

        public virtual void OnReceiveHeal(Unit Attacker, Ability Ab, ref uint Damages) { }

        public virtual void OnDealDamages(Unit Victim, Ability Ab, ref uint Damages) { }

        public virtual void OnDealHeals(Unit Victim, Ability Ab, ref uint Damages) { }

        #endregion

        public uint CustomValue;
    }
}

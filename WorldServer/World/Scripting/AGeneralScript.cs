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
    public abstract class AGeneralScript
    {
        public string ScriptName;

        public virtual void OnInitObject(Object Obj)
        {

        }

        public virtual void OnRemoveObject(Object Obj)
        {

        }

        public virtual void OnWorldUpdate(long Tick)
        {

        }

        public virtual void OnObjectLoad(Object Obj)
        {

        }

        public virtual void OnInteract(Object Obj, Player Target, InteractMenu Menu)
        {

        }

        public virtual void OnEnterWorld(Object Obj)
        {

        }

        public virtual void OnRemoveFromWorld(Object Obj)
        {

        }

        public virtual void OnEnterRange(Object Obj, Object DistObj)
        {

        }

        public virtual void OnLeaveRange(Object Obj, Object DistObj)
        {

        }

        public virtual void OnReceiveDamages(Object Obj, Object Attacker)
        {

        }

        public virtual void OnDealDamages(Object Obj, Object Target)
        {

        }

        public virtual void OnDie(Object Obj)
        {

        }

        public virtual void OnRevive(Object Obj)
        {

        }

        public virtual void OnCastAbility(Ability Ab)
        {

        }

        #region World

        public virtual bool OnPlayerCommand(Player Plr, string Text)
        {
            return true; // True if the text can be draw
        }

        public virtual void OnWorldPlayerEvent(string EventName, Player Plr, object Data)
        {

        }

        public virtual void OnWorldCreatureEvent(string EventName, Creature Creature, object Data)
        {

        }

        public virtual void OnWorldGameObjectEvent(string EventName, GameObject Obj, object Data)
        {

        }

        public virtual void OnWorldZoneEvent(string EventName, ZoneMgr Zone, object Data)
        {

        }

        #endregion
    }
}

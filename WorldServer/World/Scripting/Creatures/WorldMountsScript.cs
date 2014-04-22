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
    [GeneralScript(false, "", CreatureEntry = 116, GameObjectEntry = 0)]
    public class WorldOrderMountScript : AGeneralScript
    {
        public override void OnInteract(Object Obj, Player Target, InteractMenu Menu)
        {
            Mount(Target);
        }

        static public void Mount(Player Target)
        {
            if (Target.MvtInterface.IsMount())
                return;

            if(Target._Info.Race == (byte)GameData.Races.RACES_DWARF)
                Target.MvtInterface.CurrentMount.SetMount(8);
            else if (RandomMgr.Next(4) == 1)
                Target.MvtInterface.CurrentMount.SetMount(180);
            else
                Target.MvtInterface.CurrentMount.SetMount(1);
        }
    }

    [GeneralScript(false, "", CreatureEntry = 155, GameObjectEntry = 0)]
    public class WorldDestructionMountScript : AGeneralScript
    {
        public override void OnInteract(Object Obj, Player Target, InteractMenu Menu)
        {
            Mount(Target);
        }

        static public void Mount(Player Target)
        {
            if (Target.MvtInterface.IsMount())
                return;

            if (RandomMgr.Next(4) == 1)
                Target.MvtInterface.CurrentMount.SetMount(3);
            else
                Target.MvtInterface.CurrentMount.SetMount(12);
        }
    }

    /*[GeneralScript(true, "WorldFleeAbility")]
    public class WorldFleeAbilityMount : AGeneralScript
    {
        public override void OnCastAbility(Ability Ab)
        {
            if (Ab.Caster.IsPlayer() && Ab.Info.Entry == 245) // Flee
            {
                if (Ab.Caster.GetPlayer().MvtInterface.IsMount())
                {
                    Ab.Caster.GetPlayer().MvtInterface.UnMount();
                    return;
                }

                if (Ab.Caster.GetPlayer().Realm == GameData.Realms.REALMS_REALM_ORDER)
                    WorldOrderMountScript.Mount(Ab.Caster.GetPlayer());
                else
                    WorldDestructionMountScript.Mount(Ab.Caster.GetPlayer());
            }
        }
    }*/
}

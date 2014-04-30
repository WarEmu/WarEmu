/*
 * Copyright (C) 2014 WarEmu
 *	http://WarEmu.com
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
    [GeneralScript(false, "", 0, 99999)]
    public class DoorScript : AGeneralScript
    {
        public override void OnInteract(Object Obj, Player Target, InteractMenu Menu)
        {
            GameObject go = Obj.GetGameObject();

            float dx = go.Spawn.WorldX - Target._Value.WorldX;
            float dy = go.Spawn.WorldY - Target._Value.WorldY;

            double heading = Math.Atan2(-dx, dy);

            if (heading < 0)
                heading += 4096;

            int distance = (int)((float)5 * 13.2f);
            double angle = heading;
            double targetX = go.Spawn.WorldX - (Math.Sin(angle) * distance);
            double targetY = go.Spawn.WorldY + (Math.Cos(angle) * distance);

            int newX;
            int newY;

            if (targetX > 0)
                newX = (int)targetX;
            else
                newX = 0;

            if (targetY > 0)
                newY = (int)targetY;
            else
                newY = 0;

            Target.SafeWorldTeleport((uint)newX, (uint)newY, (ushort)go.Spawn.WorldZ, (ushort)Target._Value.WorldO);
        }
    }
}
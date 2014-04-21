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

namespace Common
{
    // Valeur Fixe d'un character
    [DataTable(PreCache = false, TableName = "waypoints", DatabaseName = "World")]
    [Serializable]
    public class Waypoint : DataObject
    {
        static public byte Loop = 0;
        static public byte StartToEnd = 1;
        static public byte Random = 2;

        [DataElement()]
        public uint GUID;

        [DataElement()]
        public uint CreatureSpawnGUID;

        [DataElement()]
        public uint GameObjectSpawnGUID;

        [DataElement()]
        public ushort X;

        [DataElement()]
        public ushort Y;

        [DataElement()]
        public ushort Z;

        [DataElement()]
        public ushort O;

        [DataElement()]
        public ushort Speed = 100;

        [DataElement()]
        public byte EmoteOnStart;

        [DataElement()]
        public byte EmoteOnEnd;

        [DataElement()]
        public uint WaitAtEndMS;

        [DataElement()]
        public ushort EquipOnStart;

        [DataElement()]
        public ushort EquipOnEnd;

        [DataElement()]
        public string TextOnStart;

        [DataElement()]
        public string TextOnEnd;

        [DataElement()]
        public UInt32 NextWaypointGUID;

        public Waypoint NextWaypoint;
    }
}

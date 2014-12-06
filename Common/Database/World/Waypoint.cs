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

        private uint _GUID;
        private uint _CreatureSpawnGUID=0;
        private uint _GameObjectSpawnGUID=0;
        private ushort _X=0;
        private ushort _Y=0;
        private ushort _Z=0;
        private ushort _O=0;
        private ushort _Speed=100;
        private byte _EmoteOnStart;
        private byte _EmoteOnEnd;
        private uint _WaitAtEndMS;
        private ushort _EquipOnStart;
        private ushort _EquipOnEnd;
        private string _TextOnStart;
        private string _TextOnEnd;
        private uint _NextWaypointGUID;

        [PrimaryKey(AutoIncrement = true)]
        public uint GUID
        {
            get { return _GUID; }
            set { _GUID = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public uint CreatureSpawnGUID
        {
            get { return _CreatureSpawnGUID; }
            set { _CreatureSpawnGUID = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public uint GameObjectSpawnGUID
        {
            get { return _GameObjectSpawnGUID; }
            set { _GameObjectSpawnGUID = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public ushort X
        {
            get { return _X; }
            set { _X = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public ushort Y
        {
            get { return _Y; }
            set { _Y = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public ushort Z
        {
            get { return _Z; }
            set { _Z = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = true)]
        public ushort O
        {
            get { return _O; }
            set { _O = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public ushort Speed
        {
            get { return _Speed; }
            set { _Speed = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = true)]
        public byte EmoteOnStart
        {
            get { return _EmoteOnStart; }
            set { _EmoteOnStart = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = true)]
        public byte EmoteOnEnd
        {
            get { return _EmoteOnEnd; }
            set { _EmoteOnEnd = value; Dirty = true; }
        }
        
        [DataElement(AllowDbNull = true)]
        public uint WaitAtEndMS
        {
            get { return _WaitAtEndMS; }
            set { _WaitAtEndMS = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = true)]
        public ushort EquipOnStart
        {
            get { return _EquipOnStart; }
            set { _EquipOnStart = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = true)]
        public ushort EquipOnEnd
        {
            get { return _EquipOnEnd; }
            set { _EquipOnEnd = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = true)]
        public string TextOnStart
        {
            get { return _TextOnStart; }
            set { _TextOnStart = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = true)]
        public string TextOnEnd
        {
            get { return _TextOnEnd; }
            set { _TextOnEnd = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public UInt32 NextWaypointGUID
        {
            get { return _NextWaypointGUID; }
            set { _NextWaypointGUID = value; Dirty = true; }
        }

        public Waypoint NextWaypoint;

        public override String ToString()
        {
            return GUID + ":" + CreatureSpawnGUID + ":" + GameObjectSpawnGUID + ":" + X + "," + Y + "," + Z + "," + O + "," + Speed + "," + EmoteOnStart + "," + EmoteOnEnd + "," + WaitAtEndMS + "," + EquipOnStart + "," + EquipOnEnd + "," + TextOnStart + "," + TextOnEnd + "," + NextWaypointGUID + "," + NextWaypoint;
        }
    }
}

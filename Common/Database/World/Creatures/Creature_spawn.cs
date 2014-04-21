/*
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
    [DataTable(PreCache = false, TableName = "creature_spawns", DatabaseName = "World")]
    [Serializable]
    public class Creature_spawn : DataObject
    {
        public Creature_proto Proto;

        private uint _Guid;
        private uint _Entry;
        private ushort _ZoneId;
        public int _WorldX;
        public int _WorldY;
        public int _WorldZ;
        public int _WorldO;
        private string _Bytes;
        private byte _Icone;
        private byte _Emote;
        private ushort _Title;
        private byte _Faction;

        [PrimaryKey(AutoIncrement = true)]
        public uint Guid
        {
            get { return _Guid; }
            set { _Guid = value; Dirty = true; }
        }

        [DataElement(AllowDbNull=false)]
        public uint Entry
        {
            get { return _Entry; }
            set { _Entry = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public ushort ZoneId
        {
            get { return _ZoneId; }
            set { _ZoneId = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public int WorldX
        {
            get { return _WorldX; }
            set { _WorldX = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public int WorldY
        {
            get { return _WorldY; }
            set { _WorldY = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public int WorldZ
        {
            get { return _WorldZ; }
            set { _WorldZ = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public int WorldO
        {
            get { return _WorldO; }
            set { _WorldO = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public string Bytes
        {
            get { return _Bytes; }
            set { _Bytes = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public byte Icone
        {
            get { return _Icone; }
            set { _Icone = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public byte Emote
        {
            get { return _Emote; }
            set { _Emote = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public ushort Title
        {
            get { return _Title; }
            set { _Title = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public byte Faction
        {
            get { return _Faction; }
            set { _Faction = value; Dirty = true; }
        }

        [DataElement()]
        public byte WaypointType = 0; // 0 = Loop Start->End->Start, 1 = Start->End, 2 = Random

        public byte[] bBytes
        {
            get
            {
                List<byte> Btes = new List<byte>();
                string[] Strs = _Bytes.Split(';');
                foreach (string Str in Strs)
                    if (Str.Length > 0)
                        Btes.Add(byte.Parse(Str));

                Btes.Remove(4);
                Btes.Remove(5);
                Btes.Remove(7);

                return Btes.ToArray();
            }
        }

        public void BuildFromProto(Creature_proto Proto)
        {
            this.Proto = Proto;
            Entry = Proto.Entry;
            Title = Proto.Title;
            Emote = Proto.Emote;
            Bytes = Proto.Bytes;
            Icone = Proto.Icone;
        }
    }
}

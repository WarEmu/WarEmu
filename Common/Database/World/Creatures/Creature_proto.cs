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
    public enum CreatureTypes
    {
        NONE = 0,
        ANIMALS = 1,
        DEAMONS = 2,
        HUMANOIDS = 3,
        MONSTERS = 4,
        PLANTS = 5,
        UNDEAD = 6,
    };

    public enum CreatureSubTypes
    {
        NONE = 0,
        BEASTS = 1,
        BIRDS = 2,
        INSECTS_ARACHNIDS = 3,
        REPTILES = 5,
        UNMARKED_DEAMONS = 7,
        DEAMONDS_OF_KHORNE = 8,
        DEAMONS_OF_TZEENTCH = 9,
        DEAMONS_OF_NURGLE = 10,
        DEAMONS_OF_SLAANESH = 11,
        BEASMEN = 12,
        DWARFS =14,
        GREENSKINS = 15,
        HUMANS = 16,
        OGRES = 17,
        SKAVEN = 18,
        CHAOS_BREEDS = 19,
    };

    // Valeur Fixe d'un character
    [DataTable(PreCache = false, TableName = "creature_protos", DatabaseName = "World")]
    [Serializable]
    public class Creature_proto : DataObject
    {
        private uint _Entry;
        private string _Name;
        private UInt16 _Model1;
        private UInt16 _Model2;
        private UInt16 _MinScale;
        private UInt16 _MaxScale;
        private byte _MinLevel;
        private byte _MaxLevel;
        private byte _Faction;
        private byte _Ranged;
        private string _Bytes;
        private byte _Icone;
        private byte _Emote;
        private UInt16 _Title;
        public UInt16[] _Unks = new UInt16[7];
        private string _Flag;
        private string _ScriptName;

        [DataElement(Unique=true,AllowDbNull = false)]
        public uint Entry
        {
            get { return _Entry; }
            set { _Entry = value; Dirty = true; }
        }

        [DataElement(Varchar=255,AllowDbNull = false)]
        public string Name
        {
            get { return _Name; }
            set { _Name = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public UInt16 Model1
        {
            get { return _Model1; }
            set { _Model1 = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public UInt16 Model2
        {
            get { return _Model2; }
            set { _Model2 = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public UInt16 MinScale
        {
            get { return _MinScale; }
            set { _MinScale = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public UInt16 MaxScale
        {
            get { return _MaxScale; }
            set { _MaxScale = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public byte MinLevel
        {
            get { return _MinLevel; }
            set { _MinLevel = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public byte MaxLevel
        {
            get { return _MaxLevel; }
            set { _MaxLevel = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public byte Faction
        {
            get { return _Faction; }
            set { _Faction = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public byte CreatureType;

        [DataElement(AllowDbNull = false)]
        public byte CreatureSubType;

        [DataElement(AllowDbNull = false)]
        public byte Ranged
        {
            get { return _Ranged; }
            set { _Ranged = value; Dirty = true; }
        }

        [DataElement(Varchar=255,AllowDbNull = false)]
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
        public UInt16 Title
        {
            get { return _Title; }
            set { _Title = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public UInt16 Unk
        {
            get { return _Unks[0]; }
            set { if (_Unks == null)_Unks = new UInt16[7]; _Unks[0] = value; }
        }

        [DataElement(AllowDbNull = false)]
        public UInt16 Unk1
        {
            get { return _Unks[1]; }
            set { if (_Unks == null)_Unks = new UInt16[7]; _Unks[1] = value; }
        }

        [DataElement(AllowDbNull = false)]
        public UInt16 Unk2
        {
            get { return _Unks[2]; }
            set { if (_Unks == null)_Unks = new UInt16[7]; _Unks[2] = value; }
        }

        [DataElement(AllowDbNull = false)]
        public UInt16 Unk3
        {
            get { return _Unks[3]; }
            set { if (_Unks == null)_Unks = new UInt16[7]; _Unks[3] = value; }
        }

        [DataElement(AllowDbNull = false)]
        public UInt16 Unk4
        {
            get { return _Unks[4]; }
            set { if (_Unks == null)_Unks = new UInt16[7]; _Unks[4] = value; }
        }

        [DataElement(AllowDbNull = false)]
        public UInt16 Unk5
        {
            get { return _Unks[5]; }
            set { if (_Unks == null)_Unks = new UInt16[7]; _Unks[5] = value; }
        }

        [DataElement(AllowDbNull = false)]
        public UInt16 Unk6
        {
            get { return _Unks[6]; }
            set { if (_Unks == null)_Unks = new UInt16[7]; _Unks[6] = value; }
        }

        [DataElement(Varchar = 255, AllowDbNull = false)]
        public string Flag
        {
            get { return _Flag; }
            set { _Flag = value; Dirty = true; }
        }

        [DataElement(Varchar = 255, AllowDbNull = false)]
        public string ScriptName
        {
            get { return _ScriptName; }
            set { _ScriptName = value; Dirty = true; }
        }

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

        public List<Quest> StartingQuests;
        public List<Quest> FinishingQuests;
    }
}

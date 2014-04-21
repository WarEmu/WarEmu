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
    [DataTable(PreCache = false, TableName = "zone_Infos", DatabaseName = "World")]
    [Serializable]
    public class Zone_Info : DataObject
    {
        private UInt16 _ZoneId;
        private string _Name;
        private byte _MinLevel;
        private byte _MaxLevel;
        private int _Type;
        private int _Tier;
        private UInt16 _Price;
        private UInt16 _Region;
        private int _OffX;
        private int _OffY;

        [DataElement(Unique = false)]
        public UInt16 ZoneId
        {
            get { return _ZoneId; }
            set { _ZoneId = value; Dirty = true; }
        }

        [DataElement(Varchar=255)]
        public string Name
        {
            get { return _Name; }
            set { _Name = value; Dirty = true; }
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
        public int Type
        {
            get { return _Type; }
            set { _Type = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public int Tier
        {
            get { return _Tier; }
            set { _Tier = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public UInt16 Price
        {
            get { return _Price; }
            set { _Price = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public UInt16 Region
        {
            get { return _Region; }
            set { _Region = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public int OffX
        {
            get { return _OffX; }
            set { _OffX = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public int OffY
        {
            get { return _OffY; }
            set { _OffY = value; Dirty = true; }
        }
    }
}

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
    [DataTable(PreCache = false, TableName = "characters_items", DatabaseName = "Characters")]
    [Serializable]
    public class Character_item : DataObject
    {
        private long _Guid;
        private UInt32 _CharacterId;
        private uint _Entry;
        private UInt16 _SlotId;
        private uint _ModelId;
        private UInt16 _Counts;
        public List<UInt32> _Talismans = new List<uint>();

        public Character_item()
            : base()
        {

        }

        [DataElement(AllowDbNull = false)]
        public long Guid
        {
            get { return _Guid; }
            set { _Guid = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public UInt32 CharacterId
        {
            get { return _CharacterId; }
            set { _CharacterId = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public uint Entry
        {
            get { return _Entry; }
            set { _Entry = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public UInt16 SlotId
        {
            get { return _SlotId; }
            set { _SlotId = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public uint ModelId
        {
            get { return _ModelId; }
            set { _ModelId = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public UInt16 Counts
        {
            get { return _Counts; }
            set { _Counts = value; Dirty = true; }
        }

        [DataElement(Varchar = 255)]
        public string Talismans
        {
            get
            {
                string Str = "";
                foreach (UInt32 Entry in _Talismans)
                    Str += Entry + ";";
                return Str;
            }
            set
            {
                string[] Split = value.Split(';');
                _Talismans.Clear();
                foreach (string Str in Split)
                {
                    if(Str.Length > 0)
                        _Talismans.Add(UInt32.Parse(Str));
                }
            }
        }
    }
}

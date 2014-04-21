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
    [DataTable(PreCache = false, TableName = "creature_vendors", DatabaseName = "World")]
    [Serializable]
    public class Creature_vendor : DataObject
    {
        public Item_Info Info;
        private uint _Entry;
        private uint _ItemId;
        private uint _Price;
        private string _ReqItems;

        [DataElement(AllowDbNull = false)]
        public uint Entry
        {
            get { return _Entry; }
            set { _Entry = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public uint ItemId
        {
            get { return _ItemId; }
            set { _ItemId = value; Dirty = true; }
        }

        [DataElement(AllowDbNull = false)]
        public uint Price
        {
            get { return _Price; }
            set { _Price = value; Dirty = true; }
        }

        [DataElement(Varchar=255,AllowDbNull = false)]
        public string ReqItems
        {
            get { return _ReqItems; }
            set 
            { 
                _ReqItems = value;
                string[] Infos = _ReqItems.Split(')');
                foreach (string Info in Infos)
                {
                    if (Info.Length <= 0)
                        continue;

                    string[] Items = Info.Split(',');
                    if (Items.Length < 2)
                        continue;

                    Items[0] = Items[0].Remove(0, 1);

                    UInt16 Count = UInt16.Parse(Items[0]);
                    uint Entry = uint.Parse(Items[1]);

                    if (!ItemsReq.ContainsKey(Entry))
                        ItemsReq.Add(Entry, Count);
                }
                Dirty = true; 
            }
        }

        public Dictionary<uint, UInt16> ItemsReq = new Dictionary<uint, ushort>();
        public Dictionary<UInt16, Item_Info> ItemsReqInfo = new Dictionary<UInt16, Item_Info>();
    }
}

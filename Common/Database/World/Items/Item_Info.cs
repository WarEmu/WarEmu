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
    [DataTable(PreCache = false, TableName = "Item_Infos", DatabaseName = "World")]
    [Serializable]
    public class Item_Info : DataObject
    {
        static public int MAX_STATS = 12;

        private uint _Entry;
        private string _Name;
        private string _Description;
        private byte _Type;
        private byte _Race;
        private uint _ModelId;
        private UInt16 _SlotId;
        private byte _Rarity;
        private uint _Career;
        private uint _Skills;
        private int _Icon;
        private byte _Bind;
        private UInt16 _Armor;
        private int _SpellId;
        private UInt16 _Dps;
        private UInt16 _Speed;
        private byte _MinRank;
        private byte _MinRenown;
        private int _StartQuest;
        public Dictionary<byte, UInt16> _Stats = new Dictionary<byte, UInt16>();
        private uint _SellPrice;
        private byte _TalismanSlots;
        private UInt16 _MaxStack;
        private string _ScriptName;

        [DataElement(Unique = true)]
        public uint Entry { get { return _Entry; } set { _Entry = value; } }
        [DataElement(Varchar = 255)]
        public string Name { get { return _Name; } set { _Name = value; } }
        [DataElement(Varchar = 255)]
        public string Description { get { return _Description; } set { _Description = value; } }
        
        [DataElement()]
        public byte Type { get { return _Type; } set { _Type = value; } }
        [DataElement()]
        public byte Race { get { return _Race; } set { _Race = value; } }
        [DataElement()]
        public uint ModelId  { get { return _ModelId; } set { _ModelId = value; } }
        [DataElement()]
        public UInt16 SlotId { get { return _SlotId; } set { _SlotId = value; } }
        [DataElement()]
        public byte Rarity { get { return _Rarity; } set { _Rarity = value; } }
        [DataElement()]
        public uint Career { get { return _Career; } set { _Career = value; } }
        [DataElement()]
        public uint Skills { get { return _Skills; } set {  _Skills= value; } }
        [DataElement()]
        public int Icon { get { return _Icon; } set { _Icon = value; } }
        [DataElement()]
        public byte Bind { get { return _Bind; } set { _Bind = value; } }
        [DataElement()]
        public UInt16 Armor { get { return _Armor; } set { _Armor = value; } }
        [DataElement()]
        public int SpellId { get { return _SpellId; } set { _SpellId = value; } }
        [DataElement()]
        public UInt16 Dps { get { return _Dps; } set { _Dps = value; } }
        [DataElement()]
        public UInt16 Speed { get { return _Speed; } set { _Speed = value; } }
        [DataElement()]
        public byte MinRank { get { return _MinRank; } set { _MinRank = value; } }
        [DataElement()]
        public byte MinRenown { get { return _MinRenown; } set { _MinRenown = value; } }
        [DataElement()]
        public int StartQuest { get { return _StartQuest; } set { _StartQuest = value; } }
        [DataElement()]
        public string Stats
        {
            get
            {
                string St = "";
                foreach (KeyValuePair<byte, UInt16> Stat in _Stats)
                    St += Stat.Key + ":" + Stat.Value + ";";

                return St;
            }
            set
            {
                string[] St = value.Split(';');
                byte Type;
                UInt16 Value;
                UInt16 LastValue;
                string[] Val;

                foreach (string Str in St)
                    if (Str.Length > 1)
                    {
                        Val = Str.Split(':');
                        if (Val.Length < 2) continue;

                        Type = byte.Parse(Val[0]);
                        Value = UInt16.Parse(Val[1]);

                        if (Type <= 0 || Value <= 0)
                            continue;

                        LastValue = 0;

                        if (!_Stats.TryGetValue(Type, out LastValue))
                            _Stats.Add(Type, Value);
                        else
                            _Stats[Type] = (ushort)(LastValue+Value);
                    }
            }
        }
        [DataElement()]
        public uint SellPrice { get { return _SellPrice; } set { _SellPrice = value; } }
        [DataElement()]
        public byte TalismanSlots { get { return _TalismanSlots; } set { _TalismanSlots = value; } }
        [DataElement()]
        public UInt16 MaxStack { get { return _MaxStack; } set { _MaxStack = value; } }
        [DataElement(Varchar=255)]
        public string ScriptName { get { return _ScriptName; } set { _ScriptName = value; } }

        public Dictionary<byte, UInt16> GetStats()
        {
            return _Stats;
        }
    }
}

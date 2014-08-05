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
    [DataTable(PreCache = false, TableName = "item_infos", DatabaseName = "World")]
    [Serializable]
    public class Item_Info : DataObject
    {
        static public int MAX_STATS = 12;

        private uint _Entry;
        private string _Name="";
        private string _Description="";
        private byte _Type=0;
        private byte _Race=0;
        private uint _ModelId=0;
        private UInt16 _SlotId=0;
        private byte _Rarity=0;
        private uint _Career=0;
        private uint _Skills=0;
        private byte _Bind=0;
        private UInt16 _Armor=0;
        private int _SpellId=0;
        private UInt16 _Dps=0;
        private UInt16 _Speed=0;
        private byte _MinRank=0;
        private byte _MinRenown=0;
        private byte _ObjectLevel=0;
        private byte _UniqueEquiped=0;
        private int _StartQuest=0;
        private uint _SellPrice=0;
        private byte _TalismanSlots=0;
        private UInt16 _MaxStack=0;
        private string _ScriptName;
        private byte[] _Unk27;
        private bool _TwoHanded;
        public byte Realm;

        public Dictionary<byte, UInt16> _Stats = new Dictionary<byte, UInt16>();
        public Dictionary<UInt32, UInt32> _Spells = new Dictionary<uint, uint>();
        public List<KeyValuePair<byte, UInt16>> _Crafts = new List<KeyValuePair<byte, ushort>>();
        public List<KeyValuePair<UInt32, UInt16>> _SellRequiredItems = new List<KeyValuePair<UInt32, UInt16>>();
        public List<KeyValuePair<Item_Info, UInt16>> RequiredItems = new List<KeyValuePair<Item_Info, UInt16>>();

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
        public byte ObjectLevel { get { return _ObjectLevel; } set { _ObjectLevel = value; } }
        [DataElement()]
        public byte UniqueEquiped { get { return _UniqueEquiped; } set { _UniqueEquiped = value; } }
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
        public string Spells
        {
            get
            {
                string St = "";
                foreach (KeyValuePair<UInt32, UInt32> Spell in _Spells)
                    St += Spell.Key + ":" + Spell.Value + ";";

                return St;
            }
            set
            {
                string[] St = value.Split(';');
                UInt32 Type;
                UInt32 Value;
                UInt32 LastValue;
                string[] Val;

                foreach (string Str in St)
                {
                    if (Str.Length > 1)
                    {
                        Val = Str.Split(':');
                        if (Val.Length < 2) continue;

                        Type = UInt32.Parse(Val[0]);
                        Value = UInt32.Parse(Val[1]);

                        if (Type <= 0 || Value <= 0)
                            continue;

                        LastValue = 0;

                        if (!_Spells.TryGetValue(Type, out LastValue))
                            _Spells.Add(Type, Value);
                        else
                            _Spells[Type] = (ushort)(LastValue + Value);
                    }
                }
            }
        }

        [DataElement()]
        public string Crafts
        {
            get
            {
                string St = "";
                foreach (KeyValuePair<byte, UInt16> Craft in _Crafts)
                    St += Craft.Key + ":" + Craft.Value + ";";

                return St;
            }
            set
            {
                string[] St = value.Split(';');
                byte Type;
                UInt16 Value;
                string[] Val;
                _Crafts.Clear();

                foreach (string Str in St)
                {
                    if (Str.Length > 1)
                    {
                        Val = Str.Split(':');
                        if (Val.Length < 2) continue;

                        Type = byte.Parse(Val[0]);
                        Value = UInt16.Parse(Val[1]);

                        if (Type <= 0 || Value <= 0)
                            continue;

                        _Crafts.Add(new KeyValuePair<byte, UInt16>(Type, Value));
                    }
                }
            }
        }

        [DataElement()]
        public uint SellPrice { get { return _SellPrice; } set { _SellPrice = value; } }
        [DataElement()]
        public string SellRequiredItems
        {
            get
            {
                string St = "";
                foreach (KeyValuePair<UInt32, UInt16> SRI in _SellRequiredItems)
                    St += SRI.Key + ":" + SRI.Value + ";";

                return St;
            }
            set
            {
                string[] St = value.Split(';');
                UInt32 Type;
                UInt16 Value;
                string[] Val;
                _SellRequiredItems.Clear();

                foreach (string Str in St)
                {
                    if (Str.Length > 1)
                    {
                        Val = Str.Split(':');
                        if (Val.Length < 2) continue;

                        Type = UInt32.Parse(Val[0]);
                        Value = UInt16.Parse(Val[1]);

                        if (Type <= 0 || Value <= 0)
                            continue;

                        _SellRequiredItems.Add(new KeyValuePair<UInt32, UInt16>(Type, Value));
                    }
                }
            }
        }

        [DataElement()]
        public byte TalismanSlots { get { return _TalismanSlots; } set { _TalismanSlots = value; } }
        [DataElement()]
        public UInt16 MaxStack { get { return _MaxStack; } set { _MaxStack = value; } }
        [DataElement()]
        public byte[] Unk27 
        {
            get
            {
                if (_Unk27 == null || _Unk27.Length < 27)
                {
                    _Unk27 = new byte[27];
                    _Unk27[4] = 0x03;
                    _Unk27[5] = 0x02;
                }
                return _Unk27;
            }
            set
            {
                _Unk27 = value;
                if (_Unk27 == null || _Unk27.Length < 27)
                {
                    _Unk27 = new byte[27];
                    _Unk27[4] = 0x03;
                    _Unk27[5] = 0x02;
                }
            }
        }

        [DataElement(Varchar=255)]
        public string ScriptName { get { return _ScriptName; } set { _ScriptName = value; } }

        [DataElement()]
        public bool TwoHanded { get { return _TwoHanded; } set { _TwoHanded = value; } }

        public Dictionary<byte, UInt16> GetStats()
        {
            return _Stats;
        }
    }
}

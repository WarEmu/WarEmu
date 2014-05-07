/*
 * Copyright (C) 2014 WarEmu
 *	http://WarEmu.com
 * 
 * Copyright (C) 2011-2013 APS
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

using Common;
using FrameWork;

namespace WorldServer
{
    public enum EquipSlot
    {
        NONE = 0,
        MAIN_DROITE = 10,
        MAIN_GAUCHE = 11,
        ARME_DISTANCE = 12,
        MAIN = 13,
        ETENDARD = 14,

        TROPHEE_1 = 15,
        TROPHEE_2 = 16,
        TROPHEE_3 = 17,
        TROPHEE_4 = 18,
        TROPHEE_5 = 19,

        CORPS = 20,
        GANTS = 21,
        CHAUSSURE = 22,
        HEAUME = 23,
        EPAULE = 24,
        POCHE_1 = 25,
        POCHE_2 = 26,
        DOS = 27,
        CEINTURE = 28,

        BIJOUX_1 = 31,
        BIJOUX_2 = 32,
        BIJOUX_3 = 33,
        BIJOUX_4 = 34,
    };

    public class Item
    {
        // Créature ou Player
        public Object Owner;
        public UInt32 _ModelId=0;
        public UInt16 _SlotId=0;
        public UInt16 _Count=1;
        public UInt32 _EffectId;
        public long Cooldown;

        // Player Uniquement
        public Item_Info Info;
        public Character_item CharItem = null;

        public Item(Object Owner)
        {
            this.Owner = Owner;
        }
        public Item(Creature_item CItem)
        {
            _SlotId = CItem.SlotId;
            _ModelId = CItem.ModelId;
            _EffectId = CItem.EffectId;
            _Count = 1;
        }

        public bool Load(Character_item Item)
        {
            if (Item == null)
                return false;

            Info = WorldMgr.GetItem_Info(Item.Entry);
            if (Info == null)
            {
                Log.Error("ItemInterface", "Load : Info==null,Entry=" + Item.Entry);
                return false;
            }

            CharItem = Item;
            return true;
        }
        public bool Load(uint Entry, UInt16 SlotId, UInt16 Count)
        {
            Info = WorldMgr.GetItem_Info(Entry);
            if (Info == null)
                return false;

            if (Count <= 0)
                Count = 1;

            _SlotId = SlotId;
            _ModelId = Info.ModelId;
            _Count = Count;
            return true;
        }
        public bool Load(Item_Info Info, UInt16 SlotId, UInt16 Count)
        {
            this.Info = Info;
            if (Info == null)
                return false;

            if (Count <= 0)
                Count = 1;

            _SlotId = SlotId;
            _ModelId = Info.ModelId;
            _Count = Count;
            return true;
        }

        public void Delete()
        {
            if (CharItem != null)
                CharMgr.DeleteItem(CharItem);
        }

        public Character_item Create(UInt32 CharacterId)
        {
            CharItem = new Character_item();
            CharItem.CharacterId = CharacterId;
            CharItem.Counts = _Count;
            CharItem.Entry = Info.Entry;
            CharItem.ModelId = _ModelId;
            CharItem.SlotId = _SlotId;
            CharMgr.CreateItem(CharItem);

            return CharItem;
        }
        public Character_item Save(UInt32 CharacterId)
        {
            if (CharItem != null)
            {
                CharItem.CharacterId = CharacterId;
                CharMgr.Database.SaveObject(CharItem);
            }
            else 
                Create(CharacterId);

            return CharItem;
        }

        public Item_Info GetTalisman(byte SlotId)
        {
            if (CharItem == null)
                return null;

            if (CharItem._Talismans.Count <= SlotId)
                return null;

            return WorldMgr.GetItem_Info(CharItem._Talismans[SlotId]);
        }

        static public void BuildItem(ref PacketOut Out,Item Itm,Item_Info Info,ushort SlotId,ushort Count)
        {
            SlotId = SlotId == 0 ? (Itm == null ? SlotId : Itm.SlotId ) : SlotId;
            Count = Count == 0 ? (Itm == null ? Count : Itm.Count) : Count;
            Info = Info == null ? (Itm == null ? null : Itm.Info) : Info;
            

            if(SlotId != 0)
                Out.WriteUInt16(SlotId);

            Out.WriteByte(0);
            Out.WriteUInt32((uint)(Info != null ? Info.Entry : 0));

            if (Info == null)
                return;

            Out.WriteUInt16((ushort)Info.ModelId);  // Valid 1.4.8
            Out.Fill(0, 7);  // Valid 1.4.8
            Out.WriteUInt16(Info.SlotId);  // Valid 1.4.8
            Out.WriteByte(Info.Type);  // Valid 1.4.8

            Out.WriteByte(Info.MinRank); // Min Level
            Out.WriteByte(Info.ObjectLevel); // 1.3.5, Object Level
            Out.WriteByte(Info.MinRenown); // 1.3.5, Min Renown
            Out.WriteByte(Info.MinRenown); // ?
            Out.WriteByte(Info.UniqueEquiped); // Unique - Equiped

            Out.WriteByte(Info.Rarity);
            Out.WriteByte(Info.Bind);
            Out.WriteByte(Info.Race);

            // Trophys have some extra bytes
            if (Info.Type == (byte)GameData.ItemTypes.ITEMTYPES_TROPHY)
            {
                Out.WriteUInt32(0);
                Out.WriteUInt16(0x0080);
            }

            Out.WriteUInt32(Info.Career);
            Out.WriteUInt32(0);
            Out.WriteUInt32(Info.SellPrice);

            Out.WriteUInt16((UInt16)(Count > 0 ? Count : 1));
            Out.WriteUInt16((UInt16)(Count > 0 ? Count : 1));

            Out.WriteUInt32(0);

            Out.WriteUInt32(Info.Skills);  // Valid 1.4.8
            Out.WriteUInt16(Info.Dps > 0 ? Info.Dps : Info.Armor);  // Valid 1.4.8
            Out.WriteUInt16(Info.Speed);  // Valid 1.4.8
            Out.WritePascalString(Info.Name);  // Valid 1.4.8

            Out.WriteByte((byte)Info._Stats.Count);  // Valid 1.4.8
            foreach (KeyValuePair<byte, UInt16> Key in Info._Stats)
            {
                Out.WriteByte(Key.Key);
                Out.WriteUInt16(Key.Value);
                Out.Fill(0, 5);
            }

            Out.WriteByte(0); // Equip Effects

            Out.WriteByte((byte)Info._Spells.Count); // OK
            foreach (KeyValuePair<UInt32, UInt32> Kp in Info._Spells)
            {
                Out.WriteUInt32(Kp.Key);
                Out.WriteUInt32(Kp.Value);
            }
            // (uint32)Entry, uint16 X, uint16 Y

            Out.WriteByte((byte)Info._Crafts.Count); // OK
            foreach (KeyValuePair<byte, ushort> Kp in Info._Crafts)
            {
                Out.WriteByte(Kp.Key);
                Out.WriteUInt16(Kp.Value);
            }

            Out.WriteByte(0); // ??

            Out.WriteByte(Info.TalismanSlots);
            Item_Info TalismanInfo = null;
            for (int i = 0; i < Info.TalismanSlots; ++i)
            {
                if (Itm != null)
                    TalismanInfo = Itm.GetTalisman((byte)i);
 
                if (TalismanInfo == null)
                    Out.WriteUInt32(0); // Entry;
                else
                {
                    Out.WriteUInt32(TalismanInfo.Entry);
                    Out.WritePascalString(TalismanInfo.Name);
                    Out.Fill(0, 15);
                }
            }

            Out.WritePascalString(Info.Description);

            Out.Write(Info.Unk27);

            /*Out.WriteByte(0);
            Out.WriteByte(0);
            Out.WriteByte(0);
            Out.WriteByte(0);

            Out.WriteUInt16(0x0302);

            Out.Fill(0, 8);
            Out.WriteByte(0); // Type , Culture, etc etc
            Out.WriteByte(0); // Type, Recipian , Soil , etc etc
            Out.Fill(0, 11);*/
        }

        #region Accessor

        public UInt16 SlotId
        {
            get
            {
                if (CharItem != null)
                    return CharItem.SlotId;
                return _SlotId;
            }
            set
            {
                if (CharItem != null)
                    CharItem.SlotId = value;
                else
                    _SlotId = value;
            }
        }

        public UInt16 Count
        {
            get
            {
                if (CharItem != null)
                    return CharItem.Counts;
                else
                    return _Count;
            }
            set
            {
                if (CharItem != null)
                    CharItem.Counts = value;
                else
                    _Count = value;
            }
        }

        public UInt32 ModelId
        {
            get 
            { 
                if (Info != null) 
                    return Info.ModelId; 
                else
                    return _ModelId; 
            }
            set{ _ModelId = value; }
        }

        #endregion
    }

    /*
     * public struct ItemSpells
{
    public UInt32 Entry;
    public UInt16 X,Y;
}

public struct ItemArtisana
{
    public byte Unk1,Unk2,Unk3;
}

public struct ItemStat
{
    public byte Type;
    public UInt16 Count;
}

public struct ItemTalisman
{
    public UInt32 Entry;
    public UInt16 Unk;
    public string Name;
}
     * 
     * static public ItemInformation DecodeItem(PacketIn Packet)
    {
        ItemInformation Info = new ItemInformation();

        Packet.GetUint16(); // SlotId
        Packet.GetUint8();

        Info.Entry = Packet.GetUint32();

        if (Info.Entry == 0)
            return Info;

        Info.ModelId = Packet.GetUint16();
        Info.UnknownId = Packet.GetUint16();
        Info.UnknownEntry = Packet.GetUint32();
        Info.UnknownText = Packet.GetPascalString();

        Info.SlotId = Packet.GetUint16();
        Info.Type = Packet.GetUint8();
        Info.MinRank = Packet.GetUint8();
        Packet.GetUint8();
        Info.MinRenown = Packet.GetUint8();
        Packet.Skip(2);
        Info.Rarity = Packet.GetUint8();
        Info.Bind = Packet.GetUint8();
        Info.Race = Packet.GetUint8();
        Info.Career = Packet.GetUint32();

        UInt32 ObjectPrice = Packet.GetUint32(); /// ?
        if (ObjectPrice != 0)
            Packet.Skip(2);

        Info.Price = Packet.GetUint32();
        Packet.GetUint32(); // Count / MaxCount
        Packet.GetUint32(); // ?
        Info.Skill = Packet.GetUint32();
        Info.Dps = Packet.GetUint16();
        Info.Speed = Packet.GetUint16();
        Info.Name = Packet.GetPascalString();

        int TempCount = Packet.GetUint8();
        Info.Stats = new List<ItemStat>(TempCount);
        for (int i = 0; i < TempCount; ++i)
        {
            ItemStat Stat = new ItemStat();
            Stat.Type = Packet.GetUint8();
            Stat.Count = Packet.GetUint16();
            Packet.Skip(5);
            Info.Stats.Add(Stat);
        }

        TempCount = Packet.GetUint8(); // Equip Effects
        for (int i = 0; i < TempCount; ++i)
        {
            Packet.GetUint16(); // Effect Id
            Packet.GetUint32(); // ?
            Packet.GetUint32(); // ?

            string Txt = Packet.GetPascalString();
            if (Txt.Length == 0)
                Packet.Skip(3);
        }

        TempCount = Packet.GetUint8();
        Info.Spells = new List<ItemSpells>(TempCount);
        for (int i = 0; i < TempCount; ++i)
        {
            ItemSpells Spell = new ItemSpells();
            Spell.Entry = Packet.GetUint32();
            Spell.X = Packet.GetUint16();
            Spell.Y = Packet.GetUint16();
            Info.Spells.Add(Spell);
            
        }

        TempCount = Packet.GetUint8();
        Info.Artisanas = new List<ItemArtisana>(TempCount);
        for (int i = 0; i < TempCount; ++i)
        {
            ItemArtisana Art = new ItemArtisana();
            Art.Unk1 = Packet.GetUint8();
            Art.Unk2 = Packet.GetUint8();
            Art.Unk3 = Packet.GetUint8();
            Info.Artisanas.Add(Art);
        }

        Packet.GetUint8();

        TempCount = Packet.GetUint8();
        Info.TalismanSlots = new List<ItemTalisman>(TempCount);
        for (int i = 0; i < TempCount; ++i)
        {
            ItemTalisman Talisman = new ItemTalisman();
            Talisman.Entry = Packet.GetUint32();
            if (Talisman.Entry != 0)
            {
                Talisman.Unk = Packet.GetUint16();
                Talisman.Name = Packet.GetPascalString();
                Packet.Skip(15);
            }
            else
                Talisman.Name = "";

            Info.TalismanSlots.Add(Talisman);
        }

        Info.Description = Packet.GetPascalString();

        Packet.Skip(27); // Culture, recipian , soil, etc...
        return Info;
    }*/
}

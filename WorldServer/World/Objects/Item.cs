
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

        // Player Uniquement
        public Item_Info Info;
        public Character_items CharItem = null;

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

        public bool Load(Character_items Item)
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

        public Character_items Create(int CharacterId)
        {
            CharItem = new Character_items();
            CharItem.CharacterId = CharacterId;
            CharItem.Counts = _Count;
            CharItem.Entry = Info.Entry;
            CharItem.ModelId = _ModelId;
            CharItem.SlotId = _SlotId;
            CharMgr.CreateItem(CharItem);

            return CharItem;
        }
        public Character_items Save(int CharacterId)
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

            Out.WriteUInt32(Info.ModelId);
            Out.WriteUInt16(Info.SlotId);
            Out.WriteByte(Info.Type);
            Out.WriteByte(Info.MinRank);

            Out.WriteByte((byte)(Info.MinRank + 1)); // 1.3.5
            Out.WriteByte(Info.MinRenown); // 1.3.5

            Out.WriteByte(Info.MinRenown);
            Out.WriteByte(Info.MinRenown);
            Out.WriteByte(Info.Rarity);
            Out.WriteByte(Info.Bind);
            Out.WriteByte(Info.Race);
            Out.WriteUInt32(Info.Career);

            Out.WriteUInt32(0);
            Out.WriteUInt32(Info.SellPrice);

            Out.WriteUInt16((UInt16)(Count > 0 ? Count : 1));

            Out.WriteUInt32(0);

            Out.WriteUInt32(Info.Skills);
            Out.WriteUInt16(Info.Dps > 0 ? Info.Dps : Info.Armor);
            Out.WriteUInt16(Info.Speed);
            Out.WritePascalString(Info.Name);

            Out.WriteByte((byte)Info._Stats.Count);
            foreach (KeyValuePair<byte, UInt16> Key in Info._Stats)
            {
                Out.WriteByte(Key.Key);
                Out.WriteUInt16(Key.Value);
                Out.Fill(0, 5);
            }

            Out.WriteByte(0);
            Out.WriteByte(0); // SpellCounts
            // (uint32)Entry, uint16 X, uint16 Y

            Out.WriteByte(0); // Artisana Info , 3 bytes pour chaque artisana
            Out.WriteByte(0);

            Out.WriteByte(Info.TalismanSlots);
            for (int i = 0; i < Info.TalismanSlots; ++i)
                Out.WriteUInt32(0); // Entry;

            Out.WritePascalString(Info.Description);

            Out.WriteByte(0);
            Out.WriteByte(0);
            Out.WriteByte(0);
            Out.WriteByte(0);

            Out.WriteUInt16(0x0302);

            Out.Fill(0, 8);
            Out.WriteByte(0); // Type , Culture, etc etc
            Out.WriteByte(0); // Type, Recipian , Soil , etc etc
            Out.Fill(0, 11);
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
}

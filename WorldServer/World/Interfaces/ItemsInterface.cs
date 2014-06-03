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
    public enum ItemError
    {
        RESULT_OK = 0,
        RESULT_MAX_BAG = 1, // Sac Plein
        RESULT_ITEMID_INVALID = 2,// Item incorrect
    };

    public class ItemsInterface : BaseInterface
    {
        #region Define

        static public UInt16 MY_TRADE_SLOT          = 232;
        static public UInt16 OTHER_TRADE_SLOT       = 241;
        static public UInt16 BUY_BACK_SLOT          = 20;
        static public UInt16 BASE_BAG_PRICE = 100;
        static public UInt16 MAX_TRADE_SLOT         = 9;
        static public UInt16 INVENTORY_SLOT_COUNT   = 16;
        static public UInt16 MAX_EQUIPED_SLOT       = 40;
        static public UInt16 INVENTORY_START_SLOT   = 40;
        static public UInt16 ARTISANAT_START_SLOT   = 400;
        static public UInt16 DEVISE_START_SLOT      = 500;
        static public UInt16 AUTO_EQUIP_SLOT        = 600;
        static public UInt16 BANK_START_SLOT        = 800;
        static public UInt16 BANK_END_SLOT          = 1039;
        static public UInt16 DELETE_SLOT            = 1040;

        #endregion

        public Item[] Items = new Item[DELETE_SLOT];

        public byte _BagBuy=0;
        public byte BagBuy
        {
            get
            {
                if (_Owner.IsPlayer())
                    return _Owner.GetPlayer()._Value.BagBuy;
                else
                    return _BagBuy;
            }
            set
            {
                if (_Owner.IsPlayer())
                    _Owner.GetPlayer()._Value.BagBuy = value;
                else
                    _BagBuy = value;
            }

        }

        public void Load(Item[] NItems)
        {
            if (IsLoad)
                return;

            foreach (Item Itm in NItems)
            {
                if (Itm != null && Itm.SlotId >= 0 && Itm.SlotId < Items.Length)
                {
                    if (Itm.SlotId == 0)
                    {
                        BuyBack.Add(Itm);
                        continue;
                    }

                    if (Items[Itm.SlotId] == null)
                    {
                        Items[Itm.SlotId] = Itm;
                        if (IsEquipedSlot(Itm.SlotId))
                            EquipItem(Itm);
                    }
                    else
                        DuplicatedSlot.Add(Itm);
                }
            }

            CheckDuplicated();

            base.Load();
        }
        public void Load(List<Character_item> NItems)
        {
            if (IsLoad)
                return;

            foreach (Character_item Item in NItems)
                if (Item.SlotId < Items.Length && Item.SlotId >= 0)
                {
                    Item Itm = new Item(_Owner);
                    if (!Itm.Load(Item))
                        continue;

                    if (Itm.SlotId == 0)
                    {
                        BuyBack.Add(Itm);
                        continue;
                    }

                    if (Items[Itm.SlotId] == null)
                    {
                        Items[Itm.SlotId] = Itm;
                        if (IsEquipedSlot(Itm.SlotId))
                            EquipItem(Itm);
                    }
                    else
                        DuplicatedSlot.Add(Itm);
                }

            CheckDuplicated();

            base.Load();
        }
        public void Load(List<Creature_item> CItems)
        {
            if (IsLoad)
                return;

            foreach (Creature_item Item in CItems)
            {
                if (Item.SlotId < Items.Length && Item.SlotId >= 0)
                {
                    Item Itm = new Item(Item);

                    if (Itm.SlotId == 0)
                    {
                        BuyBack.Add(Itm);
                        continue;
                    }

                    Items[Itm.SlotId] = Itm;
                }
            }
        }

        public void AddCreatureItem(Creature_item Item)
        {
            if (Item.SlotId < Items.Length && Item.SlotId >= 0)
            {
                Item Itm = new Item(Item);
                AddCreatureItem(Itm);
            }
        }

        public void AddCreatureItem(Item Item)
        {
            if (Item.SlotId < Items.Length && Item.SlotId >= 0)
            {
                if (Item.SlotId == 0)
                {
                    BuyBack.Add(Item);
                    return;
                }

                Items[Item.SlotId] = Item;
            }

            if(IsEquipedSlot(Item.SlotId))
                SendEquiped(null);
        }

        public Item RemoveCreatureItem(ushort Slot)
        {
            Item Itm = RemoveItem(Slot);

            if (Itm != null)
            {
                if(IsEquipedSlot(Slot))
                    SendEquiped(null, Slot);
            }

            return Itm;
        }

        public override void Update(long Tick)
        {

        }

        public List<Item> DuplicatedSlot = new List<Item>();
        public void CheckDuplicated()
        {
            foreach (Item Itm in DuplicatedSlot)
            {
                Log.Success("CheckDuplicated", "Item dupliqué : " + Itm.Info.Name);
                Itm.SlotId = GetFreeInventorySlot();
                Items[Itm.SlotId] = Itm;
            }
        }
        public override void Save()
        {
            List<Item> Itms = Items.ToList();
            Itms.AddRange(BuyBack.ToList());

            if (_Owner.IsPlayer())
                CharMgr.SaveItems(GetPlayer()._Info.CharacterId, Itms);
        }

        #region Stats

        public void SetCooldown(ushort Slot, long Tick)
        {
            Item Itm = GetItemInSlot(Slot);
            if (Itm != null)
                Itm.Cooldown = TCPManager.GetTimeStampMS() + Tick;
        }

        public bool CanUseItem(ushort Slot, long Tick)
        {
            Item Itm = GetItemInSlot(Slot);
            if (Itm != null && Itm.Info.Type != (byte)GameData.ItemTypes.ITEMTYPES_SHIELD)
            {
                return Itm.Cooldown <= Tick;
            }

            return false;
        }

        public ushort GetAttackTime(EquipSlot Slot)
        {
            Item Itm = Items[(UInt16)Slot];
            if (Itm == null)
            {
                return 200;
            }
            else
                return Itm.Info.Speed;
        }
        public ushort GetAttackDamage(EquipSlot Slot)
        {
            Item Itm = Items[(UInt16)Slot];
            if(Itm == null)
                return 0;
            else
                return Itm.Info.Dps;

        }
        public byte GetAttackSpeed()
        {
            Item Gauche = GetItemInSlot((UInt16)EquipSlot.MAIN_GAUCHE);
            Item Droite = GetItemInSlot((UInt16)EquipSlot.MAIN_DROITE);
            Item Distance = GetItemInSlot((UInt16)EquipSlot.ARME_DISTANCE);

            byte Speed = 0;
            if (Gauche != null) Speed += (byte)(Gauche.Info.Speed * 0.1);
            if (Droite != null) Speed += (byte)(Droite.Info.Speed * 0.1);
            if (Distance != null) Speed += (byte)(Distance.Info.Speed * 0.1);

            return Speed;
        }
        public UInt16 GetDamage()
        {
            Item Gauche = GetItemInSlot((UInt16)EquipSlot.MAIN_GAUCHE);
            Item Droite = GetItemInSlot((UInt16)EquipSlot.MAIN_DROITE);
            Item Distance = GetItemInSlot((UInt16)EquipSlot.ARME_DISTANCE);

            UInt16 Damage = 0;
            if (Gauche != null) Damage += Gauche.Info.Dps;
            if (Droite != null) Damage += Droite.Info.Dps;
            if (Distance != null) Damage += Distance.Info.Dps;

            return Damage;
        }
        public UInt16 GetEquipedArmor()
        {
            UInt16 Armor = 0;
            for (UInt16 i = 0; i < MAX_EQUIPED_SLOT; ++i)
                if (Items[i] != null)
                    Armor += Items[i].Info.Armor;

            return Armor;
        }
        public void EquipItem(Item Itm)
        {
            if (Itm == null || !_Owner.IsPlayer())
                return;

            Player Plr = _Owner.GetPlayer();

            foreach (KeyValuePair<byte, UInt16> Stats in Itm.Info._Stats)
                Plr.StsInterface.AddBonusStat(Stats.Key, Stats.Value);

            Plr.StsInterface.ApplyStats();
        }
        public void UnEquipItem(Item Itm)
        {
            if (Itm == null || !_Owner.IsPlayer())
                return;

            Player Plr = _Owner.GetPlayer();

            foreach (KeyValuePair<byte, UInt16> Stats in Itm.Info._Stats)
                Plr.StsInterface.RemoveBonusStat(Stats.Key, Stats.Value);

            Plr.StsInterface.ApplyStats();
        }

        #endregion

        #region Accessor

        public Item GetItemInSlot(UInt16 SlotID)
        {
            if (SlotID >= Items.Length)
                return null;

            return Items[SlotID];
        }
        public UInt16 GetItemCount()
        {
            UInt16 Count = 0;
            for (int i = 0; i < Items.Length; ++i)
                if (Items[i] != null)
                    ++Count;

            return Count;
        }
        public UInt16 GetItemCount(uint Entry)
        {
            UInt16 Count = 0;
            foreach (Item Itm in Items)
            {
                if (Itm != null && Itm.Info != null && Itm.Info.Entry == Entry)
                    Count += Itm.Count;
            }
            return Count;
        }

        public bool HasItemCount(uint Entry, UInt16 Count)
        {
            return GetItemCount(Entry) >= Count;
        }

        public bool HasMaxBag()
        {
            if (BagBuy < 3)
                return false;

            return true;
        }
        public byte GetTotalSlot()
        {
            return (byte)(GetMaxInventorySlot() - INVENTORY_START_SLOT);
        }        
        public byte GetMaxInventorySlot()
        {
            return (byte)(INVENTORY_START_SLOT + 32 + BagBuy * INVENTORY_SLOT_COUNT);
        }

        public UInt16 GetBagPrice()
        {
            double Bag = (double)(BagBuy);
            double Price = BASE_BAG_PRICE;

            Price *= Math.Pow(10, Bag);
            return (UInt16)Price;
        }
        public UInt16 GetTotalFreeInventorySlot()
        {
            UInt16 Count = 0;
            for (UInt16 Slot = MAX_EQUIPED_SLOT; Slot < GetMaxInventorySlot(); ++Slot)
                if (Items[Slot] == null)
                    ++Count;

            return Count;
        }
        public UInt16 GetFreeInventorySlot()
        {
            for (UInt16 Slot = MAX_EQUIPED_SLOT; Slot < GetMaxInventorySlot(); ++Slot)
                if (Items[Slot] == null)
                    return Slot;
            return 0;
        }

        public List<UInt16> GetStackItem(uint Entry)
        {
            List<UInt16> Infos = new List<ushort>(); 

            for (UInt16 Slot = MAX_EQUIPED_SLOT; Slot < GetMaxInventorySlot(); ++Slot)
            {
                if (Items[Slot] != null && Items[Slot].Info.Entry == Entry)
                {
                    if (Items[Slot].Count < Items[Slot].Info.MaxStack)
                        Infos.Add(Slot);
                }
            }

            return Infos;
        }
        public UInt16 GetStackableCount(List<UInt16> Infos)
        {
            UInt16 Count = 0;
            foreach (UInt16 Slot in Infos)
            {
                if (Items[Slot] != null)
                {
                    if (Items[Slot].Count < Items[Slot].Info.MaxStack)
                        Count += (ushort)(Items[Slot].Info.MaxStack - Items[Slot].Count);
                }
            }
            return Count;
        }

        #endregion

        #region Packets

        public void BuildStats(ref PacketOut Out)
        {
            Out.WriteByte((byte)GameData.Stats.STATS_COUNT);
            Out.WriteByte(GetAttackSpeed());
            Out.WriteByte(01);
            Out.WriteByte(0xF4);
        }
        public void SendMaxInventory(Player Plr) // 1.3.5
        {
            PacketOut Out = new PacketOut((byte)Opcodes.F_BAG_INFO);
            Out.WriteByte(0x0F);
            Out.WriteByte(GetTotalSlot()); // Nombre de slots disponibles
            Out.WriteUInt16((UInt16)INVENTORY_SLOT_COUNT);
            Out.WriteByte(0);
            Out.WriteUInt32R(GetBagPrice());

            Out.WriteUInt16(2);
            Out.WriteByte(0x50);
            Out.WriteUInt16(0x08);
            Out.WriteUInt16(0x60);
            Out.WriteByte(0xEA);
            Out.WriteUInt16(0);
            Plr.SendPacket(Out);
        }
        public void SendAllItems(Player Plr)
        {
            SendMaxInventory(Plr); // 1.3.5

            // On Envoi les items 16 par 16
            byte Count = 0;

            PacketOut Buffer = new PacketOut(0);
            Buffer.Position = 0;

            for (int i = 0; i < Items.Length; ++i)
            {
                if (Count >= 8)
                    SendBuffer(Plr, ref Buffer,ref Count);

                if (Items[i] != null)
                {
                    Item.BuildItem(ref Buffer, Items[i], null, 0, 0);
                    ++Count;
                }
            }

            if (Count > 0)
                SendBuffer(Plr, ref Buffer, ref Count);
        }
        private void SendBuffer(Player Plr,ref PacketOut Buffer,ref byte Count)
        {
            // On Envoi le Packet des items
            byte[] ArrayBuf = Buffer.ToArray();
            PacketOut Packet = new PacketOut((byte)Opcodes.F_GET_ITEM);
            Packet.WriteByte(Count);
            Packet.Fill(0, 3);
            Packet.Write(ArrayBuf, 0, ArrayBuf.Length);
            Packet.WritePacketLength();

            Plr.SendPacket(Packet);

            // On Remet le compteur a zero
            Count = 0;

            // On Initalise un nouveau buffer
            Buffer = new PacketOut(0);
            Buffer.Position = 0;
        }
        private void SendItems(Player Plr, List<UInt16> Slots)
        {
            if (Slots.Count <= 0)
                return;

            PacketOut Out = new PacketOut((byte)Opcodes.F_GET_ITEM);
            Out.WriteByte((byte)Slots.Count);
            Out.Fill(0, 3);
            for (int i = 0; i < Slots.Count; ++i)
                Item.BuildItem(ref Out, GetItemInSlot(Slots[i]), null, Slots[i], 0);
            Plr.SendPacket(Out);
        }
        private void SendItems(Player Plr, UInt16 Slot)
        {
            PacketOut Out = new PacketOut((byte)Opcodes.F_GET_ITEM);
            Out.WriteByte(1);
            Out.Fill(0, 3);
            Item.BuildItem(ref Out, GetItemInSlot(Slot), null, Slot, 0);
            Plr.SendPacket(Out);
        }
        private void SendItem(Player Plr, UInt16 Slot, Item_Info Info)
        {
            PacketOut Out = new PacketOut((byte)Opcodes.F_GET_ITEM);
            Out.WriteByte(1);
            Out.Fill(0, 3);
            Item.BuildItem(ref Out, null, Info, Slot, 1);
            Plr.SendPacket(Out);
        }
        private void SendItems(Player Plr, UInt16 To, UInt16 From)
        {
            PacketOut Out = new PacketOut((byte)Opcodes.F_GET_ITEM);
            Out.WriteByte(2);
            Out.Fill(0, 3);
            Item.BuildItem(ref Out, GetItemInSlot(To), null, To, 0);
            Item.BuildItem(ref Out, GetItemInSlot(From), null, From, 0);
            Plr.SendPacket(Out);
        }
        public void SendEquiped(Player Plr)
        {
            List<UInt16> Itms = new List<UInt16>();
            for (UInt16 i = 0; i < MAX_EQUIPED_SLOT; ++i)
                if (Items[i] != null)
                    Itms.Add(i);

            SendEquiped(Plr, Itms);
        }

        public void SendEquiped(Player Plr, UInt16 Slot)
        {
            if (!IsEquipedSlot(Slot))
                return;

            PacketOut Out = new PacketOut((byte)Opcodes.F_PLAYER_INVENTORY);
            Out.WriteUInt16(_Owner.Oid);
            Out.WriteUInt16(1); // Count
            Out.WriteUInt16(Slot);
            Out.WriteUInt16((UInt16)(Items[Slot] != null ? Items[Slot].ModelId : 0));
            Out.WriteByte(0);

            if (Plr != null)
                Plr.SendPacket(Out);
            else
                _Owner.DispatchPacket(Out, false);
        }

        public void SendEquiped(Player Plr, UInt16 To, UInt16 From)
        {
            int Invalide = !IsEquipedSlot(To) ? 1 : 0;
            Invalide += !IsEquipedSlot(From) ? 1 : 0;

            if (Invalide == 2)
                return;

            PacketOut Out = new PacketOut((byte)Opcodes.F_PLAYER_INVENTORY);
            Out.WriteUInt16(_Owner.Oid);
            Out.WriteUInt16((UInt16)(2 - Invalide)); // Count

            if (IsEquipedSlot(To))
            {
                Out.WriteUInt16(To);
                Out.WriteUInt16((UInt16)(Items[To] != null ? Items[To].ModelId : 0));

            }
            if (!IsEquipedSlot(From))
            {
                Out.WriteUInt16(From);
                Out.WriteUInt16((UInt16)(Items[To] != null ? Items[To].ModelId : 0));
            }

            Out.WriteByte(0);

            if (Plr != null)
                Plr.SendPacket(Out);
            else
                _Owner.DispatchPacket(Out, false);
        }
        public void SendEquiped(Player Plr, List<UInt16> Itms)
        {
            int Invalide = Itms.Count(slot => !IsEquipedSlot(slot));
            if (Invalide >= Itms.Count)
                return;

            PacketOut Out = new PacketOut((byte)Opcodes.F_PLAYER_INVENTORY);
            Out.WriteUInt16(_Owner.Oid);
            Out.WriteUInt16((UInt16)(Itms.Count - Invalide)); // Count
            foreach (UInt16 Slot in Itms)
            {
                if (!IsEquipedSlot(Slot))
                    continue;

                Out.WriteUInt16(Slot);
                Out.WriteUInt16((UInt16)(Items[Slot] != null ? Items[Slot].ModelId : 0));
            }
            Out.WriteByte(0);

            if (Plr != null)
                Plr.SendPacket(Out);
            else
                _Owner.DispatchPacket(Out, false);
        }
        public void SendInspect(Player Plr)
        {
            byte Count = 0;
            for (UInt16 i = 0; i < MAX_EQUIPED_SLOT; ++i)
                if (Items[i] != null)
                    ++Count;

            PacketOut Out = new PacketOut((byte)Opcodes.F_SOCIAL_NETWORK);
            Out.WriteUInt16(_Owner.Oid);
            Out.WriteByte(7); // Inspect Code
            Out.WriteByte(Count);

            for (UInt16 i = 0; i < MAX_EQUIPED_SLOT; ++i)
                if (Items[i] != null)
                    Item.BuildItem(ref Out, Items[i], null, 0, 0);
            Out.WriteByte(0);
            Plr.SendPacket(Out);
        }

        #endregion

        #region Moving

        static public bool IsEquipedSlot(UInt16 Slot)
        {
            if (Slot < MAX_EQUIPED_SLOT)
                return true;

            return false;
        }
        static public bool CanUse(Item_Info Info, Player Plr, bool IgnoreLevel, bool IgnoreSkills, bool IgnoreRace, bool IgnoreCareer, bool IgnoreRenown)
        {
            if (!IgnoreSkills)
            {
                uint Skills = Info.Skills << 1;
                long PlayerSkill = Plr._Value.Skills;


                if (Skills != 0 && (PlayerSkill &= Skills) != Skills)
                    return false;
            }

            if (!IgnoreCareer)
            {
                int Career = (int)Info.Career;
                int PlayerCareer = 1 << (Plr._Info.CareerLine - 1);

                if (Career != 0 && (Career & PlayerCareer) == 0)
                    return false;

            }

            if (!IgnoreRace)
            {
                int Race = (int)Info.Race;
                int PlayerRace = 1 << (Plr._Info.Race - 1);

                if (Race != 0 && (Race & PlayerRace) == 0)
                    return false;
            }

            if (!IgnoreLevel)
            {
                if (Plr.Level < Info.MinRank)
                    return false;
            }

            if (!IgnoreRenown)
            {
                if (Plr.Renown < Info.MinRenown)
                    return false;
            }

            return true;
        }

        public bool CanMove(Item Itm, UInt16 Slot)
        {
            if (Itm == null)
                return true;

            if (!_Owner.IsPlayer())
                return true;

            if (IsEquipedSlot(Slot)) // Si le slot est équipé alors on check qu'on ai les skills et autre
            {
                Player Plr = _Owner.GetPlayer();

                if (!CanUse(Itm.Info, Plr, false, false, false, false, false))
                    return false;

                EquipSlot ESlot = (EquipSlot)Slot;
                EquipSlot ISlot = (EquipSlot)Itm.Info.SlotId;

                if (ESlot == EquipSlot.TROPHEE_2 && Plr.Level < 10) // Level 10 pour le trophee_2
                    return false;
                else if (ESlot == EquipSlot.TROPHEE_3 && Plr.Level < 20) // Level 20 pour le trophee_3
                    return false;
                else if (ESlot == EquipSlot.TROPHEE_4 && Plr.Level < 30) // Level 30 pour le trophee_4
                    return false;
                else if (ESlot == EquipSlot.TROPHEE_5 && Plr.Level < 40) // Level 40 pour le trophee_5
                    return false;

                if (Slot != Itm.Info.SlotId)
                {
                    if (ISlot == EquipSlot.MAIN && (ESlot == EquipSlot.MAIN_DROITE || ESlot == EquipSlot.MAIN_GAUCHE))
                        return true;
                    else if ((ISlot == EquipSlot.POCHE_1 || ISlot == EquipSlot.POCHE_2) && (ESlot == EquipSlot.POCHE_1 && ESlot == EquipSlot.POCHE_2))
                        return true;
                    else if ((ISlot >= EquipSlot.TROPHEE_1 && ISlot <= EquipSlot.TROPHEE_5) && (ESlot >= EquipSlot.TROPHEE_1 && ESlot <= EquipSlot.TROPHEE_5))
                        return true;
                    else if ((ISlot >= EquipSlot.BIJOUX_1 && ISlot <= EquipSlot.BIJOUX_4) && (ESlot >= EquipSlot.BIJOUX_1 && ESlot <= EquipSlot.BIJOUX_4))
                        return true;
                    else
                        return false;
                }
            }
            else
            {
                if (Slot >= BANK_START_SLOT && Slot <= BANK_END_SLOT)
                    return true;
                if (Slot > GetMaxInventorySlot())
                    return false;
            }

            return true;
        }
        public bool MoveSlot(UInt16 From, UInt16 To)
        {
            Item IFrom = GetItemInSlot(From);
            Item ITo = GetItemInSlot(To);

            if (CanMove(ITo, From) && CanMove(IFrom, To))
            {
                Items[From] = ITo;
                if(ITo != null)
                    ITo.SlotId = From;

                Items[To] = IFrom;
                if(IFrom != null)
                    IFrom.SlotId = To;

                if (IsEquipedSlot(From) && !IsEquipedSlot(To))
                {
                    UnEquipItem(IFrom);
                    EquipItem(ITo);
                }

                if (IsEquipedSlot(To) && !IsEquipedSlot(From))
                {
                    UnEquipItem(ITo);
                    EquipItem(IFrom);
                }

                SendEquiped(null, To, From);

                if (_Owner.IsPlayer())
                    SendItems(_Owner.GetPlayer(), To, From);
                return true;
            }

            //Log.Error("MoveSlot", "From=" + From + ",To=" + To);
            if (_Owner.IsPlayer() && IFrom != null && IFrom.Info != null)
                if (IsEquipedSlot(To))
                    _Owner.GetPlayer().SendLocalizeString(IFrom.Info.Name, GameData.Localized_text.TEXT_ITEM_ERR_CANT_EQUIP_X);
                else
                    _Owner.GetPlayer().SendLocalizeString(IFrom.Info.Name, GameData.Localized_text.TEXT_ITEM_ERR_CANT_MOVE_X);


            return false;
        }
        public bool MoveSlot(UInt16 From,UInt16 To,UInt16 Count)
        {
            if (To == DELETE_SLOT)
            {
                DeleteItem(From, Count, true);
                return true;
            }
            
            if (To == AUTO_EQUIP_SLOT)
                To = GenerateAutoSlot(From);

            //Log.Success("MoveSlot", "From=" + From + ",To=" + To +",Count="+Count);

            if (To == 0)
                return false;

            Item IFrom = GetItemInSlot(From);
            Item ITo = GetItemInSlot(To);

            if( (IFrom == null && ITo != null) // Si le slot que je déplace est vide
                || (IFrom != null && ITo == null && IFrom.Info.MaxStack <= 1) // Si l'objet n'est pas stackable
                || (IFrom != null && ITo != null && IFrom.Info != ITo.Info) // Si l'objet est différent de l'objet de destination
                )
                return MoveSlot(From, To);
            else if(IFrom != null)
            {
                if (ITo != null) // Si il y a un objet dans lequel je veux aller
                {
                    if (IFrom.Info.MaxStack < Count + ITo.Count) // et si il n'a pas la place de prendre
                        return MoveSlot(From, To);
                    else
                    {
                        DeleteItem(From, Count, true);
                        ITo.Count += Count;

                        if (_Owner.IsPlayer())
                            SendItems(_Owner.GetPlayer(), From, To);

                        return true;
                    }
                }
                else // Si je déplace l'objet dans un Slot vide
                {
                    if (Count >= IFrom.Count)
                        return MoveSlot(From, To);
                    else
                    {
                        DeleteItem(From, Count, true);

                        Item New = new Item(_Owner);
                        New.Load(IFrom.Info, To, Count);
                        Items[To] = New;

                        if (_Owner.IsPlayer())
                            SendItems(_Owner.GetPlayer(), From, To);

                        return true;
                    }
                }
            }
            return false;
        }
        public Item RemoveItem(UInt16 Slot)
        {
            Item ITo = Items[Slot];
            Items[Slot] = null;
            return ITo;
        }
        public bool RemoveItem(uint Entry, UInt16 Count)
        {
            List<UInt16> Removed = new List<ushort>();
            int Result = RemoveItem(Entry, Count, ref Removed);
            SendItems(GetPlayer(), Removed);
            return Result != 0;
        }
        public int RemoveAllItems(uint Entry)
        {
            ushort Count = GetItemCount(Entry);
            List<UInt16> Removed = new List<ushort>();
            int Result = RemoveItem(Entry, Count, ref Removed, false);
            SendItems(GetPlayer(), Removed);
            return Result;
        }
        public int RemoveItem(uint Entry, UInt16 Count, ref List<UInt16> Removed, bool CheckCount = true)
        {
            if (CheckCount && GetItemCount(Entry) < Count)
                return 0;

            UInt16 OriginalCount = Count;
            for (UInt16 Slot = MAX_EQUIPED_SLOT; Slot < GetMaxInventorySlot() && Count > 0; ++Slot)
            {
                if (Items[Slot] != null && Items[Slot].Info.Entry == Entry)
                {
                    if (Items[Slot].Count > Count)
                    {
                        Removed.Add(Slot);
                        DeleteItem(Slot, Count, true);
                        Items[Slot].Count -= Count;
                        Count = 0;
                        return Count;
                    }
                    else
                    {
                        Removed.Add(Slot);
                        Count -= Items[Slot].Count;
                        DeleteItem(Slot, Count, true);
                    }
                }
            }

            return OriginalCount - Count;
        }
        public UInt16 GenerateAutoSlot(UInt16 From)
        {
            UInt16 SlotId = 0;

            Item IFrom = GetItemInSlot(From);
            if (IFrom == null)
                return SlotId;

            if (IsEquipedSlot(From)) // Si l'item est équipe , on le place dans l'inventaire
                SlotId = GetFreeInventorySlot();
            else
            {
                EquipSlot ISlot = (EquipSlot)IFrom.Info.SlotId;

                if (ISlot >= EquipSlot.BIJOUX_1 && ISlot <= EquipSlot.BIJOUX_4) // Un bijoux a placer, on prend un emplacement de bijoux vide
                {
                    if (Items[(int)EquipSlot.BIJOUX_1] == null)
                        SlotId = (ushort)EquipSlot.BIJOUX_1;
                    else if (Items[(int)EquipSlot.BIJOUX_2] == null)
                        SlotId = (ushort)EquipSlot.BIJOUX_2;
                    else if (Items[(int)EquipSlot.BIJOUX_3] == null)
                        SlotId = (ushort)EquipSlot.BIJOUX_3;
                    else if (Items[(int)EquipSlot.BIJOUX_4] == null)
                        SlotId = (ushort)EquipSlot.BIJOUX_4;
                    else
                        SlotId = (ushort)ISlot;
                }
                else if (ISlot == EquipSlot.MAIN) // Si c'est la main , alors prend un emplacement d'une main disponible
                {
                    if(Items[(int)EquipSlot.MAIN_DROITE] == null)
                        SlotId = (ushort)EquipSlot.MAIN_DROITE;
                    else if(Items[(int)EquipSlot.MAIN_GAUCHE] == null)
                        SlotId = (ushort)EquipSlot.MAIN_GAUCHE;
                    else
                        SlotId = (ushort)EquipSlot.MAIN_DROITE;
                }
                else if (ISlot >= EquipSlot.POCHE_1 && ISlot <= EquipSlot.POCHE_2)
                {
                    if (Items[(int)EquipSlot.POCHE_1] == null)
                        SlotId = (ushort)EquipSlot.POCHE_1;
                    else if (Items[(int)EquipSlot.POCHE_2] == null)
                        SlotId = (ushort)EquipSlot.POCHE_2;
                    else
                        SlotId = (ushort)ISlot;
                }
                else if (ISlot >= EquipSlot.TROPHEE_1 && ISlot <= EquipSlot.TROPHEE_5)
                {
                    if (Items[(int)EquipSlot.TROPHEE_1] == null)
                        SlotId = (ushort)EquipSlot.TROPHEE_1;
                    else if (Items[(int)EquipSlot.TROPHEE_2] == null)
                        SlotId = (ushort)EquipSlot.TROPHEE_2;
                    else if (Items[(int)EquipSlot.TROPHEE_3] == null)
                        SlotId = (ushort)EquipSlot.TROPHEE_3;
                    else if (Items[(int)EquipSlot.TROPHEE_4] == null)
                        SlotId = (ushort)EquipSlot.TROPHEE_4;
                    else if (Items[(int)EquipSlot.TROPHEE_5] == null)
                        SlotId = (ushort)EquipSlot.TROPHEE_5;
                    else
                        SlotId = (ushort)ISlot;
                }
                else
                    SlotId = IFrom.Info.SlotId;
            }

            //Log.Success("Generate", "ItemSlot=" + IFrom.Info.SlotId + ",generated=" + SlotId);
            return SlotId;
        }
        public void DeleteItem(UInt16 SlotId,UInt16 Count,bool Delete)
        {
            //Log.Success("DeleteItem", "SlotId=" + SlotId);
            Item IFrom = GetItemInSlot(SlotId);
            if (IFrom != null)
            {
                IFrom.Count -= Count;
                if (IFrom.Count <= 0)
                {
                    Items[SlotId] = null;

                    if (Delete)
                        IFrom.Delete();
                }

                if (_Owner.IsPlayer())
                {
                    SendItems(_Owner.GetPlayer(), SlotId);
                    _Owner.GetPlayer().QtsInterface.HandleEvent(Objective_Type.QUEST_GET_ITEM, IFrom.Info.Entry, Count, false);
                }

                if (IsEquipedSlot(SlotId))
                    SendEquiped(null, SlotId);
            }
        }
        public ItemError CreateItem(UInt32 ItemId, UInt16 Count)
        {
            Item_Info Info = WorldMgr.GetItem_Info(ItemId);

            return CreateItem(Info, Count);
        }

        List<UInt16> Stacked = new List<ushort>(); // List des Objets stackable
        List<UInt16> ToSend = new List<ushort>(); // List des Objets mis a jours
        public ItemError CreateItem(Item_Info Info, UInt16 Count)
        {
            if(Info == null)
                return ItemError.RESULT_ITEMID_INVALID;

            lock (ToSend)
            {
                Stacked.Clear();
                ToSend.Clear();

                UInt16 CanStack = 0; // Nombre d'objet qui peuvent être stacker
                UInt16 ToCreate = (UInt16)Math.Ceiling((decimal)(Count / Info.MaxStack) + 1); // Nombre d'objet qui doit être créé Count/MaxStack

                if (Info.MaxStack > 1) // Si l'objet a créer est stackable on recherche les objets déja dans l'inventaire
                {
                    Stacked = GetStackItem(Info.Entry);
                    CanStack = GetStackableCount(Stacked);

                    if (CanStack >= Count) // Si on a + de place pour le stack que pour le créer alors on n'en créer aucun
                    {
                        ToCreate = 0; // Nombre de slots a créé
                        CanStack = Count;
                    }
                    else
                    {
                        Count -= CanStack;
                        ToCreate = (UInt16)Math.Ceiling((decimal)(Count / Info.MaxStack) + 1); // On supprime le nombre stackable et on recalcul le nombre de slot necessaire
                    }
                }

                UInt16 TotalFreeSlot = GetTotalFreeInventorySlot(); // Nombre de slots total dont je dispose

                //Log.Info("ItemsInterface", "Count=" + Count + ",FreeSlot=" + TotalFreeSlot + ",ToCreate=" + ToCreate+",CanStack="+CanStack);


                if (TotalFreeSlot < ToCreate) // Je n'ai pas assez de slots disponible pour créer ces objets
                    return ItemError.RESULT_MAX_BAG;

                foreach (UInt16 StackableSlot in Stacked)
                {
                    Item Itm = Items[StackableSlot];

                    if (Itm == null || Itm.Count >= Itm.Info.MaxStack)
                        continue;

                    UInt16 ToAdd = (UInt16)Math.Min(Itm.Info.MaxStack - Itm.Count, CanStack);

                    //Log.Info("ItemsInterface", "StackableSlot Add : " + ToAdd);

                    Itm.Count += ToAdd;
                    CanStack -= ToAdd;
                    Count -= ToAdd;

                    ToSend.Add(StackableSlot);

                    if (CanStack <= 0)
                        break;
                }

                for (int i = 0; i < ToCreate && Count > 0; ++i)
                {
                    UInt16 FreeSlot = GetFreeInventorySlot();
                    if (FreeSlot == 0)
                        return ItemError.RESULT_MAX_BAG;

                    UInt16 ToAdd = Math.Min(Count, Info.MaxStack);
                    Count -= ToAdd;

                    Item Itm = new Item(_Owner);
                    if (!Itm.Load(Info, FreeSlot, ToAdd))
                        return ItemError.RESULT_ITEMID_INVALID;

                    //Log.Info("ItemsInterface", "New Item ToAdd : " + ToAdd + ",Count=" + Count);
                    Items[FreeSlot] = Itm;
                    ToSend.Add(FreeSlot);
                }

                if (_Owner.IsPlayer())
                    _Owner.GetPlayer().QtsInterface.HandleEvent(Objective_Type.QUEST_GET_ITEM, Info.Entry, Count, false);
                
                SendItems(GetPlayer(), ToSend);
            }

            return ItemError.RESULT_OK;
        }

        #endregion

        #region Trading

        public Player Trading = null;
        public UInt32 TradingMoney = 0;
        public byte TradingAccepted = 0;
        public bool TradingUpdated = false;
        public byte TradingUpdate = 0;

        public void HandleTrade(PacketIn packet)
        {
            TradingUpdated = false;

            byte Status = packet.GetUint8();
            byte Unk = packet.GetUint8();
            UInt16 Oid = packet.GetUint16();

            if (!_Owner.IsInWorld())
                return;

            if (!_Owner.IsPlayer())
                return;

            Player Plr = _Owner.GetPlayer();

            if (Oid <= 0)
            {
                Plr.SendLocalizeString("", GameData.Localized_text.TEXT_TRADE_ERR_NO_TARGET);
                SendTradeClose(Oid);
                return;
            }

            if (Oid == _Owner.Oid)
            {
                Plr.SendLocalizeString("", GameData.Localized_text.TEXT_TRADE_ERR_CANT_TRADE_WITH_YOURSELF);
                SendTradeClose(Oid);
                return;
            }

            Log.Success("HandleTrade", "Status=" + Status + ",Oid=" + Oid);

            Trading = Plr.Region.GetPlayer(Oid);

            if (Trading == null)
            {
                SendTradeClose(Oid);
                return;
            }

            if (Status == 0 && TradingAccepted == 0) // Nouveau Trade
            {
                if (!CanTrading(Trading))
                {
                    Plr.SendLocalizeString("", GameData.Localized_text.TEXT_TRADE_ERR_TARGET_ALREADY_TRADING);
                    CloseTrade();
                    return;
                }

                SendTradeInfo(this);
                Trading.ItmInterface.SendTradeInfo(this);
                TradingAccepted = 1;
            }
            else if (Status == 1 && IsTrading()) // Trade mis a jours
            {
                uint Money = packet.GetUint32();
                byte Update = packet.GetUint8();
                byte ItemCounts = packet.GetUint8();

                //Log.Info("Trade", "Money=" + Money + ",Update=" + Update + ",Items=" + ItemCounts);

                Trading.ItmInterface.TradingAccepted = 1;
                TradingAccepted = 1;

                TradingMoney = Money;
                if (TradingMoney > Plr.GetMoney())
                {
                    TradingMoney = Plr.GetMoney();
                    Plr.SendLocalizeString("", GameData.Localized_text.TEXT_TRADE_ERR_INSUFFICIENT_MONEY);
                    SendTradeInfo(this);
                    Trading.ItmInterface.SendTradeInfo(this);
                    return;
                }

                SendTradeInfo(Trading.ItmInterface);
                Trading.ItmInterface.SendTradeInfo(this);
            }
            else if (Status == 2 && IsTrading()) // J'accept le trade
            {
                TradingAccepted = 2;

                Trading.ItmInterface.SendTradeInfo(this);

                if (TradingAccepted == 2 && Trading.ItmInterface.TradingAccepted == 2)
                    Trade(Trading.ItmInterface);
            }
            else if (Status == 3 && IsTrading()) // Je Ferme le Trade
            {
                Trading.ItmInterface.SendTradeClose(_Owner.Oid);
                SendTradeClose(Oid);
            }
        }
        public void Trade(ItemsInterface DistInter)
        {
            Log.Success("Trade", "TRADE !");

            Player Me = GetPlayer();
            Player Other = DistInter.GetPlayer();

            bool AllOk = true;

            if (DistInter.TradingMoney > 0)
                if (!Other.HaveMoney(DistInter.TradingMoney))
                    AllOk = false;

            if (TradingMoney > 0)
                if (!Me.HaveMoney(TradingMoney))
                    AllOk = false;

            // TODO : CheckItem

            if (AllOk)
            {
                if (Other.RemoveMoney(DistInter.TradingMoney))
                    Me.AddMoney(DistInter.TradingMoney);

                if (Me.RemoveMoney(TradingMoney))
                    Other.AddMoney(TradingMoney);
            }

            SendTradeClose(Other.Oid);
            DistInter.SendTradeClose(Me.Oid);

            CloseTrade();
        }
        public void SendTradeInfo(ItemsInterface DistInterface)
        {
            PacketOut Out = new PacketOut((byte)Opcodes.F_TRADE_STATUS);
            Out.WriteByte(DistInterface.TradingAccepted);
            Out.WriteByte(0);
            Out.WriteUInt16(DistInterface != this ? DistInterface._Owner.Oid : (ushort)0);

            if (DistInterface.TradingAccepted == 2)
                Out.Fill(0, 24);
            else
            {
                Out.WriteUInt32(DistInterface.TradingMoney);
                Out.WriteByte(TradingUpdate);
                Out.Fill(0, 2 * MAX_TRADE_SLOT);
            }

            _Owner.GetPlayer().SendPacket(Out);
        }
        public bool IsTrading()
        {
            return Trading != null;
        }
        public bool CanTrading(Player Plr)
        {
            return Plr.ItmInterface.Trading == null || Plr.ItmInterface.Trading == _Owner.GetPlayer();
        }
        public void CloseTrade()
        {
            Trading = null;
            TradingMoney = 0;
            TradingAccepted = 0;
            TradingUpdate = 0;
        }
        public void SendTradeClose(UInt16 Oid)
        {   
            PacketOut Out = new PacketOut((byte)Opcodes.F_TRADE_STATUS);
            Out.WriteByte(3);
            Out.WriteByte(0);
            Out.WriteUInt16(Oid);
            Out.Fill(0, 24);
            _Owner.GetPlayer().SendPacket(Out);

            CloseTrade();
        }

        #endregion

        #region BuyBack

        public List<Item> BuyBack = new List<Item>();
        public void SendBuyBack()
        {
            if (!HasPlayer())
                return;

            Player Plr = GetPlayer();

            PacketOut Out = new PacketOut((byte)Opcodes.F_STORE_BUY_BACK);
            Out.WriteByte((byte)BuyBack.Count); // Count
            for (int i = BuyBack.Count - 1; i >= 0; --i)
            {
                Out.WriteUInt32(BuyBack[i].Info.SellPrice);
                Item.BuildItem(ref Out, BuyBack[i], null, 0, 0);
            }
            Out.WriteByte(0);
            Plr.SendPacket(Out);
        }
        public void SellItem(InteractMenu Menu)
        {
            UInt16 SlotId = (UInt16)(Menu.Num + (Menu.Page * 256));
            UInt16 Count = Menu.Count;

            List<UInt16> ToSend = new List<ushort>();
            Item Itm = GetItemInSlot(SlotId);
            if (Itm == null || Itm.Info.SellPrice <= 0)
                return;

            if (Count <= 0 || Count > Itm.Count)
                Count = Itm.Count;

            ToSend.Add(SlotId);

            if (Count == Itm.Count)
            {
                Items[SlotId] = null;
                AddBuyBack(Itm);
            }
            else if (Count < Itm.Count)
            {
                Itm.Count -= Count;

                Item New = new Item(_Owner);
                if (!New.Load(Itm.Info, 0, Count))
                    return;
                AddBuyBack(New);
            }

            GetPlayer().AddMoney((uint)Itm.Info.SellPrice * Count);

            SendItems(GetPlayer(), ToSend);
            SendBuyBack();
        }
        public void BuyBackItem(InteractMenu Menu)
        {
            UInt16 SlotId = (ushort)(BuyBack.Count - 1 - (Menu.Num + (Menu.Page * 256)));

            UInt16 FreeSlot = GetFreeInventorySlot();
            if (FreeSlot <= 0)
            {
                GetPlayer().SendLocalizeString("", GameData.Localized_text.TEXT_MERCHANT_INSUFFICIENT_SPACE_TO_BUY);
                return;
            }

            Item Itm = GetBuyBack(SlotId, Menu.Count);
            if (Itm == null || !GetPlayer().RemoveMoney(Itm.Count*Itm.Info.SellPrice) )
            {
                GetPlayer().SendLocalizeString("", GameData.Localized_text.TEXT_MERCHANT_INSUFFICIENT_MONEY_TO_BUY);
                return;
            }

            Items[FreeSlot] = Itm;
            Itm.SlotId = FreeSlot;

            SendItems(GetPlayer(), FreeSlot);
            SendBuyBack();
        }
        public void AddBuyBack(Item Itm)
        {
            Itm.SlotId = 0;
            if (BuyBack.Count >= BUY_BACK_SLOT)
            {
                Item ToDel = BuyBack[0];
                BuyBack.Remove(ToDel);
                if (ToDel != null)
                    ToDel.Delete();
            }
            BuyBack.Add(Itm);
        }
        public Item GetBuyBack(UInt16 Num,UInt16 Count)
        {
            if (Num >= BuyBack.Count)
                return null;

            Item Itm = BuyBack[Num];

            if (Itm != null && !GetPlayer().HaveMoney(Itm.Info.SellPrice * Count))
                return null;

            BuyBack.Remove(Itm);

            if (Itm.Count == Count)
                return Itm;
            else
            {
                Itm.Count -= Count;
                Item New = new Item(_Owner);
                New.Load(Itm.Info, 0, Count);
                return New;
            }
        }

        #endregion

    }
}

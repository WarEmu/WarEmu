
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common;
using FrameWork;

namespace WorldServer
{
    public class LootInfo
    {
        public LootInfo(Item_Info Item)
        {
            this.Item = Item;
        }

        public Item_Info Item;
    }

    public class Loot
    {
        public UInt32 Money = 0;
        public LootInfo[] Loots;
        public bool IsLootable()
        {
            if (GetLootCount() > 0 || Money > 0)
                return true;

            return false;
        }
        public byte GetLootCount()
        {
            return (byte)Loots.Count(info => { return info != null; });
        }
        public void TakeLoot(Player Plr, byte Id)
        {
            if (Id >= Loots.Length || Loots[Id] == null)
                Plr.SendLocalizeString("", GameData.Localized_text.TEXT_CANT_LOOT_THAT);
            else
            {
                ItemError Error = Plr.ItmInterface.CreateItem(Loots[Id].Item, 1);
                if (Error == ItemError.RESULT_OK)
                    Loots[Id] = null;
                else
                    Plr.SendLocalizeString("", GameData.Localized_text.TEXT_OVERAGE_CANT_LOOT);
            }
        }
        public void SendInteract(Player Plr, InteractMenu Menu)
        {
            if (Money > 0)
            {
                Plr.AddMoney(Money);
                Money = 0;
            }

            switch (Menu.Menu)
            {
                case 15: // Fermeture du loot

                    return;

                case 13: // Récupération de tous les items
                    if (Plr.ItmInterface.GetTotalFreeInventorySlot() < GetLootCount())
                        Plr.SendLocalizeString("", GameData.Localized_text.TEXT_OVERAGE_CANT_LOOT);
                    else
                    {
                        for (byte i = 0; i < Loots.Length; ++i)
                        {
                            if (Loots[i] == null)
                                continue;

                            TakeLoot(Plr, i);
                        }
                    }

                    break;

                case 12: // Récupération d'un item
                    TakeLoot(Plr, Menu.Num);
                    break;
            }
                

            PacketOut Out = new PacketOut((byte)Opcodes.F_INTERACT_RESPONSE);
            Out.WriteByte(4);
            Out.WriteByte(GetLootCount());

            for (byte i = 0; i < Loots.Length; ++i)
            {
                if (Loots[i] == null)
                    continue;

                Out.WriteByte(i);
                Item.BuildItem(ref Out, null, Loots[i].Item, 0, 1);
            }

            Plr.SendPacket(Out);
        }
    }

    static public class LootsMgr
    {
        static public Loot GenerateLoot(Unit Corps, Unit Looter)
        {
            if (!Looter.IsPlayer())
                return null;

            if (Corps.IsCreature())
            {
                Creature Crea = Corps.GetCreature();

                List<Creature_loot> CreatureLoots = WorldMgr.GetLoots(Crea.Entry);
                if (CreatureLoots.Count <= 0)
                    return null;

                List<LootInfo> Loots = new List<LootInfo>();
                foreach (Creature_loot Loot in CreatureLoots)
                {
                    float Pct = Loot.Pct * Program.Config.GlobalLootRate;
                    if (Pct <= 0)
                        Pct = 0.01f;

                    switch ((SystemData.ItemRarity)Loot.Info.Rarity)
                    {
                        case SystemData.ItemRarity.ITEMRARITY_COMMON:
                            Pct *= Program.Config.CommonLootRate;
                            break;
                        case SystemData.ItemRarity.ITEMRARITY_UNCOMMON:
                            Pct *= Program.Config.UncommonLootRate;
                            break;
                        case SystemData.ItemRarity.ITEMRARITY_RARE:
                            Pct *= Program.Config.RareLootRate;
                            break;
                        case SystemData.ItemRarity.ITEMRARITY_VERY_RARE:
                            Pct *= Program.Config.VeryRareLootRate;
                            break;
                        case SystemData.ItemRarity.ITEMRARITY_ARTIFACT:
                            Pct *= Program.Config.ArtifactLootRate;
                            break;
                    };

                    if(Pct  > 100.0f || RandomMgr.Next(10000) < (Pct*100))
                      Loots.Add(new LootInfo(Loot.Info));
                }

                UInt32 Money = (UInt32)(Corps.Level * (UInt32)7) + (Corps.Rank * (UInt32)50);

                if (Loots.Count > 0 || Money > 0)
                {
                    Log.Success("LootMgr", "Generate Loot : " + Loots.Count);
                    Loot Lt = new Loot();
                    Lt.Money = Money;
                    Lt.Loots = Loots.ToArray();
                    return Lt;
                }
              
            }

            return null;
        }
    }
}

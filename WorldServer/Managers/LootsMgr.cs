
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
        public List<LootInfo> Loots;
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
            if (Id >= Loots.Count || Loots[Id] == null)
                Plr.SendLocalizeString("", GameData.Localized_text.TEXT_CANT_LOOT_THAT);
            else
            {
                ItemError Error = Plr.ItmInterface.CreateItem(Loots[Id].Item, 1);
                if (Error == ItemError.RESULT_OK)
                    Loots[Id] = null;
                else if (Error == ItemError.RESULT_MAX_BAG)
                    Plr.SendLocalizeString("", GameData.Localized_text.TEXT_OVERAGE_CANT_LOOT);
            }
        }
        public void SendInteract(Player Plr, InteractMenu Menu)
        {
            if (Money > 0)
            {
                if (Plr.GetGroup() == null)
                {
                    Plr.AddMoney(Money);
                }
                else
                {
                    Plr.GetGroup().AddMoney(Money / (uint)Plr.GetGroup().Size());
                }
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
                        for (byte i = 0; i < Loots.Count; ++i)
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

            for (byte i = 0; i < Loots.Count; ++i)
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

            Player Plr = Looter.GetPlayer();
            if (Corps.IsCreature())
            {
                Creature Crea = Corps.GetCreature();

                List<Creature_loot> CreatureLoots = WorldMgr.GetLoots(Crea.Entry);
                if (CreatureLoots.Count <= 0)
                    return null;

                QuestsInterface Interface = Plr.QtsInterface;

                List<LootInfo> Loots = new List<LootInfo>();
                float Pct;
                foreach (Creature_loot Loot in CreatureLoots)
                {
                    if (Loot.Info.MinRank > Corps.Level + 4 || Loot.Info.MinRenown > (Corps.Level + 4) * 2)
                        continue;

                    if (Loot.Info.Realm != 0 && Loot.Info.Realm != (byte)Plr.Realm)
                        continue;

                    Pct = Loot.Pct * Program.Config.GlobalLootRate;
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

                    if (Interface != null && Pct != 100.0f)
                    {
                        foreach (KeyValuePair<ushort, Character_quest> Kp in Interface._Quests)
                        {
                            if (!Kp.Value.Done && !Kp.Value.IsDone())
                            {
                                foreach (Character_Objectives Obj in Kp.Value._Objectives)
                                {
                                    if (!Obj.IsDone() && Obj.Objective.Item != null)
                                    {
                                        if (Obj.Objective.Item.Entry == Loot.ItemId)
                                        {
                                            Pct = 100;
                                            break;
                                        }
                                    }
                                }
                            }

                            if (Pct >= 100)
                                break;
                        }
                    }

                    if(Pct >= 100f || RandomMgr.Next(10000) < (Pct*100))
                      Loots.Add(new LootInfo(Loot.Info));
                }

                UInt32 Money = (UInt32)(Corps.Level * (UInt32)7) + (Corps.Rank * (UInt32)50);

                if (Loots.Count > 0 || Money > 0)
                {
                    Loot Lt = new Loot();
                    Lt.Money = Money;
                    Lt.Loots = Loots;
                    Corps.EvtInterface.Notify(EventName.ON_GENERATE_LOOT, Looter, Lt);
                    return Lt;
                }
              
            }
            else if (Corps.IsGameObject())
            {
                // This will generate gameobject loot. Currently this only shows loot
                // if a player needs an item it holds for a quest. If an object has
                // been looted already or has no loot this will return null.
                // Todo: Currently object loot always is 100%. Make this support
                // non quest related loot.
                GameObject GameObj = Corps.GetGameObject();
                List<GameObject_loot> GameObjectLoots = WorldMgr.GetGameObjectLoots(GameObj.Spawn.Entry);
                if (GameObjectLoots.Count <= 0 || GameObj.Looted)
                    return null;

                QuestsInterface Interface = Plr.QtsInterface;
                List<LootInfo> Loots = new List<LootInfo>();
                foreach (GameObject_loot Loot in GameObjectLoots)
                {
                    if (Interface != null)
                    {
                        foreach (KeyValuePair<ushort, Character_quest> Kp in Interface._Quests)
                        {
                            if (!Kp.Value.Done && !Kp.Value.IsDone())
                            {
                                foreach (Character_Objectives Obj in Kp.Value._Objectives)
                                {
                                    if (!Obj.IsDone() && Obj.Objective.Item != null)
                                    {
                                        if (Obj.Objective.Item.Entry == Loot.ItemId)
                                        {
                                            Loots.Add(new LootInfo(Loot.Info));
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    
                }

                Loot Lt = new Loot();
                Lt.Money = 0;
                Lt.Loots = Loots;
                return Lt;
            }

            return null;
        }
    }
}

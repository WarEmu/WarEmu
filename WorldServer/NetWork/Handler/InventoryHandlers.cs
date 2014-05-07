
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common;
using FrameWork;

namespace WorldServer
{
    public class InventoryHandlers : IPacketHandler
    {
        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.F_BAG_INFO, (int)eClientState.Playing, "onBagInfo")]
        static public void F_BAG_INFO(BaseClient client, PacketIn packet)
        {
            GameClient cclient = client as GameClient;

            if (!cclient.IsPlaying())
                return;

            byte Type = packet.GetUint8();
            
            Player Plr = cclient.Plr;

            switch (Type)
            {
                case 3: // Toggle Pvp
                    Plr.CbtInterface.TurnPvp();
                    break;

                case 16: // Buy more bag space
                    byte Price = packet.GetUint8();
                    if (!Plr.ItmInterface.HasMaxBag())
                    {
                        if (Plr.HaveMoney(Plr.ItmInterface.GetBagPrice()))
                        {
                            if (Plr.RemoveMoney(Plr.ItmInterface.GetBagPrice()))
                            {
                                ++Plr.ItmInterface.BagBuy;
                                Plr.ItmInterface.SendMaxInventory(Plr);
                            }
                        }
                    }
                    break;
            }
        }

        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.F_TRANSFER_ITEM, (int)eClientState.Playing, "onTransferItem")]
        static public void F_TRANSFER_ITEM(BaseClient client, PacketIn packet)
        {
            GameClient cclient = client as GameClient;

            if (cclient.Plr == null || !cclient.Plr.IsInWorld())
                return;

            ushort Oid = packet.GetUint16();
            ushort To = packet.GetUint16();
            ushort From = packet.GetUint16();
            ushort Count = packet.GetUint16();

            cclient.Plr.ItmInterface.MoveSlot(From, To, Count);
        }

        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.F_TRADE_STATUS, (int)eClientState.Playing, "onTradeStatus")]
        static public void F_TRADE_STATUS(BaseClient client, PacketIn packet)
        {
            GameClient cclient = client as GameClient;
            if (!cclient.IsPlaying())
                return;

            cclient.Plr.ItmInterface.HandleTrade(packet);
        }

        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.F_USE_ITEM, (int)eClientState.Playing, "onUseItem")]
        static public void F_USE_ITEM(BaseClient client, PacketIn packet)
        {
            GameClient cclient = client as GameClient;
            if (!cclient.IsPlaying())
                return;

            Player Plr = cclient.Plr;

            ushort slot = packet.GetUint16();

            Item item = Plr.ItmInterface.GetItemInSlot(slot);

            if (Plr.Level < item.Info.MinRank)
            {
                Plr.SendLocalizeString("", GameData.Localized_text.TEST_ITEM_PLAYER_LEVEL_TOO_LOW);
                return;
            }
            else if (Plr.Rank < item.Info.MinRenown)
            {
                Plr.SendLocalizeString("", GameData.Localized_text.TEST_ITEM_PLAYER_RENOWN_TOO_LOW);
                return;
            }

            if (item.Info.Type == 31 && item.Info.ScriptName == "HealthPotion") // Potion
            {
                Plr.DealHeal(Plr, (uint)item.Info.MinRank * 100);
                Plr.ItmInterface.DeleteItem(slot, 1, true);
            }
            
        }
    }
}

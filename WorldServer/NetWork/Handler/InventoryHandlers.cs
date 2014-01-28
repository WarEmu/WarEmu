
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
        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.F_BAG_INFO, "onBagInfo")]
        static public void F_BAG_INFO(BaseClient client, PacketIn packet)
        {
            GameClient cclient = client as GameClient;

            if (!cclient.IsPlaying())
                return;

            byte SlotCount = packet.GetUint8();
            byte Price = packet.GetUint8();

            Player Plr = cclient.Plr;

            if (!Plr.ItmInterface.HasMaxBag())
                if (Plr.HaveMoney(Plr.ItmInterface.GetBagPrice()))
                {
                    if (Plr.RemoveMoney(Plr.ItmInterface.GetBagPrice()))
                    {
                        ++Plr.ItmInterface.BagBuy;
                        Plr.ItmInterface.SendMaxInventory(Plr);
                    }
                }
        }

        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.F_TRANSFER_ITEM, "onTransferItem")]
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

        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.F_TRADE_STATUS, "onTradeStatus")]
        static public void F_TRADE_STATUS(BaseClient client, PacketIn packet)
        {
            GameClient cclient = client as GameClient;
            if (!cclient.IsPlaying())
                return;

            cclient.Plr.ItmInterface.HandleTrade(packet);
        }
    }
}

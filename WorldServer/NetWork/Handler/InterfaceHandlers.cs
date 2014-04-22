using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common;
using FrameWork;

namespace WorldServer
{
    public class InterfaceHandlers : IPacketHandler
    {
        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.F_INTERFACE_COMMAND, (int)eClientState.WorldEnter, "onInterfaceCommand")]
        static public void F_INTERFACE_COMMAND(BaseClient client, PacketIn packet)
        {
            GameClient cclient = client as GameClient;

            if (cclient.Plr == null)
                return;

            byte CommandId = packet.GetUint8();

            switch (CommandId)
            {

                case 1: // ????
                    {
                    } break;

                case 2: // Resurrect Button
                    {
                        cclient.Plr.PreRespawnPlayer();
                    } break;

                case 10: // Talisman Fuse
                    {
                    } break;

            };

        }

    }
}

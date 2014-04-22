using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common;
using FrameWork;

namespace WorldServer
{
    public class GtoupHandlers : IPacketHandler
    {
        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.F_GROUP_COMMAND, (int)eClientState.WorldEnter, "onGroupCommand")]
        static public void F_GROUP_COMMAND(BaseClient client, PacketIn packet)
        {
            GameClient cclient = client as GameClient;

            if (!cclient.IsPlaying() || !cclient.Plr.IsInWorld())
                return;

            Player Plr = cclient.Plr;

            packet.GetUint32(); // UNK
            byte State = packet.GetUint8();

            switch (State)
            {
                case 2: // Accept invitation
                    if (Plr.Invitation == null)
                        return;
                    Plr.Invitation.AcceptInvitation();
                    break;
                case 6: // Decline invitation
                    if (Plr.Invitation == null)
                        return;
                    Plr.Invitation.DeclineInvitation();
                    break;
                case 3: // Leave group
                    if (Plr.GetGroup() == null)
                        return;
                    Plr.GetGroup().RemoveMember(Plr);
                    Plr.SetGroup(null);
                    break;
                case 17: // Make main
                    break;
                default:
                    Log.Error("GroupHandler", "Unsupported type: " + State);
                    break;
            }
        }
    }
}

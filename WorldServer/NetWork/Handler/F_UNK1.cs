
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common;
using FrameWork;

namespace WorldServer
{
    public class Unk1 : IPacketHandler
    {
        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.F_UNK1, (int)eClientState.WorldEnter, "onUnk1")]
        static public void F_UNK1(BaseClient client, PacketIn packet)
        {

        }
    }
}

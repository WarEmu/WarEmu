
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common;
using FrameWork;

namespace WorldServer
{
    public class TchatHandlers : IPacketHandler
    {
        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.F_TEXT, "onText")]
        static public void F_TEXT(BaseClient client, PacketIn packet)
        {
            GameClient cclient = client as GameClient;

            if (cclient.Plr == null)
                return;

            byte Unk = packet.GetUint8();
            string Text = packet.GetString((int)(packet.Length - packet.Position));

            Log.Success("Text", "Unk = " + Unk + ",String=" + Text);
            CommandMgr.HandleText(cclient.Plr, Text);
        }
    }
}

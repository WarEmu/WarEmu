
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
        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.F_TEXT, (int)eClientState.Playing, "onText")]
        static public void F_TEXT(BaseClient client, PacketIn packet)
        {
            GameClient cclient = client as GameClient;

            if (cclient.Plr == null)
                return;

            byte Unk = packet.GetUint8();
            string Text = packet.GetString((int)(packet.Length - packet.Position));

            CommandMgr.HandleText(cclient.Plr, Text);
        }

        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.F_EMOTE, (int)eClientState.Playing, "onEmote")]
        static public void F_EMOTE(BaseClient client, PacketIn packet)
        {
            GameClient cclient = client as GameClient;

            if (cclient.Plr == null)
                return;

            UInt32 emote = packet.GetUint32();

            PacketOut Out = new PacketOut((byte)Opcodes.F_EMOTE);
            Out.WriteUInt16(cclient.Plr.Oid);
            Out.WriteUInt16((UInt16)emote);
            if (cclient.Plr.CbtInterface.HasTarget(GameData.TargetTypes.TARGETTYPES_TARGET_ALLY))
                Out.WriteUInt16(cclient.Plr.CbtInterface.Targets[(int)GameData.TargetTypes.TARGETTYPES_TARGET_ALLY]);
            cclient.Plr.DispatchPacket(Out, false);
            cclient.Plr.SendPacket(Out);
        }
    }
}

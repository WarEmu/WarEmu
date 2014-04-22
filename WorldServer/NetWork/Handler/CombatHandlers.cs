
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common;
using FrameWork;

namespace WorldServer
{
    public struct InteractMenu
    {
        public UInt16 Oid;
        public UInt16 Menu;
        public byte Page;
        public byte Num;
        public UInt16 Unk;
        public UInt16 Count;

        public PacketIn Packet;
    }

    public class CombatHandlers : IPacketHandler
    {
        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.F_PLAYER_INFO, (int)eClientState.Playing, "onPlayerInfo")]
        static public void F_PLAYER_INFO(BaseClient client, PacketIn packet)
        {
            GameClient cclient = client as GameClient;

            if (cclient.Plr == null || !cclient.Plr.IsInWorld())
                return;

            packet.GetUint16();
            UInt16 Oid = packet.GetUint16();
            byte Unk = packet.GetUint8();
            byte TargetType = packet.GetUint8();
            cclient.Plr.CbtInterface.SetTarget(Oid, (GameData.TargetTypes)TargetType);
        }

        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.F_SWITCH_ATTACK_MODE, (int)eClientState.Playing, "onSwitchAttackMode")]
        static public void F_SWITCH_ATTACK_MODE(BaseClient client, PacketIn packet)
        {
            GameClient cclient = client as GameClient;
            if (!cclient.HasPlayer())
                return;

            cclient.Plr.CbtInterface.Attacking = true;
        }

        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.F_INTERACT, (int)eClientState.Playing, "onInteract")]
        static public void F_INTERACT(BaseClient client, PacketIn packet)
        {
            GameClient cclient = client as GameClient;

            if (cclient.Plr == null || !cclient.Plr.IsInWorld())
                return;

            if (cclient.Plr.IsDead)
                return;

            InteractMenu Menu = new InteractMenu();
            Menu.Unk = packet.GetUint16();
            Menu.Oid = packet.GetUint16();
            Menu.Menu = packet.GetUint16();
            Menu.Page = packet.GetUint8();
            Menu.Num = packet.GetUint8();
            Menu.Count = packet.GetUint16();

            Object Obj = cclient.Plr.Region.GetObject(Menu.Oid);
            if (Obj == null)
                return;

            if (Obj.GetDistanceTo(cclient.Plr) > 20)
            {
                //Log.Error("F_INTERACT", "Distance = " + Obj.GetDistanceTo(cclient.Plr));
                return;
            }

            Menu.Packet = packet;
            Obj.SendInteract(cclient.Plr, Menu);
        }

        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.F_DO_ABILITY, (int)eClientState.Playing, "F_DO_ABILITY")]
        static public void F_DO_ABILITY(BaseClient client, PacketIn packet)
        {
            GameClient cclient = client as GameClient;

            if (cclient.Plr == null)
                return;

            UInt32 Unk, Unk2, Unk3 = 0;
            UInt16 AbilityID = 0;

            Unk = packet.GetUint32();
            Unk2 = packet.GetUint32();
            Unk3 = packet.GetUint32();
            AbilityID = packet.GetUint16();

            cclient.Plr.AbtInterface.StartCast(AbilityID);
        }

        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.F_DO_ABILITY_AT_POS, (int)eClientState.Playing, "F_DO_ABILITY_AT_POS")]
        static public void F_DO_ABILITY_AT_POS(BaseClient client, PacketIn packet)
        {
            GameClient cclient = client as GameClient;

            if (cclient.Plr == null)
                return;

            ushort unk = packet.GetUint16();
            ushort Oid = packet.GetUint16();
            ushort CastPx = packet.GetUint16();
            ushort CastPy = packet.GetUint16();
            ushort CastZoneId = packet.GetUint16();

            ushort unk2 = packet.GetUint16();
            ushort AbilityId = packet.GetUint16();
            ushort Px = packet.GetUint16();
            ushort Py = packet.GetUint16();
            ushort ZoneId = packet.GetUint16();

            //Log.Info("Ability", AbilityId + " Cast Ability At position : " + Px + "," + Py);
            cclient.Plr.AbtInterface.StartCast(AbilityId);
        }

        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.F_INTERRUPT, (int)eClientState.Playing, "F_INTERRUPT")]
        static public void F_INTERRUPT(BaseClient client, PacketIn packet)
        {
            GameClient cclient = client as GameClient;

            if (cclient.Plr == null)
                return;

            cclient.Plr.AbtInterface.Cancel(false);
        }
    }
}

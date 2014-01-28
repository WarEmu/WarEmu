
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
        public UInt16 SellCount;
    }

    public class CombatHandlers : IPacketHandler
    {
        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.F_PLAYER_INFO, "onPlayerInfo")]
        static public void F_PLAYER_INFO(BaseClient client, PacketIn packet)
        {
            GameClient cclient = client as GameClient;

            if (cclient.Plr == null)
                return;

            if (!cclient.Plr.IsInWorld())
                return;

            packet.Skip(2);
            UInt16 Oid = packet.GetUint16();
            packet.Skip(1);
            byte Faction = packet.GetUint8();

            cclient.Plr.CbtInterface.SetTarget(cclient.Plr.Region.GetObject(Oid) as Unit);
        }

        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.F_SWITCH_ATTACK_MODE, "onSwitchAttackMode")]
        static public void F_SWITCH_ATTACK_MODE(BaseClient client, PacketIn packet)
        {
            GameClient cclient = client as GameClient;
            if (!cclient.HasPlayer())
                return;

            Player Plr = cclient.Plr;

            if (!Plr.CbtInterface.HasTarget())
            {
                Log.Error("F_SWITCH_ATTACK_MODE", "Target == null");
                return;
            }

            if (Plr.CbtInterface.GetTargetType() != GameData.TargetTypes.TARGETTYPES_TARGET_ENEMY)
            {
                Log.Error("F_SWITCH_ATTACK_MODE", "Invalide target !");
                return;
            }

            Plr.CbtInterface.Attacking = true;
        }

        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.F_INTERACT, "onInteract")]
        static public void F_INTERACT(BaseClient client, PacketIn packet)
        {
            GameClient cclient = client as GameClient;

            if (cclient.Plr == null || !cclient.Plr.IsInWorld())
                return;

            Log.Dump("F_INTERACT", packet.ToArray(), 0, packet.ToArray().Length);

            InteractMenu Menu = new InteractMenu();
            Menu.Oid = packet.GetUint16();
            Menu.Menu = packet.GetUint16();
            Menu.Page = packet.GetUint8();
            Menu.Num = packet.GetUint8();
            Menu.Unk = packet.GetUint16();
            Menu.SellCount = packet.GetUint16();
            Menu.Count = packet.GetUint16();

            Object Obj = cclient.Plr.Region.GetObject(Menu.Oid);
            if (Obj == null)
                return;

            if (Obj.GetDistanceTo(cclient.Plr) > 20)
            {
                Log.Error("F_INTERACT", "Distance = " + Obj.GetDistanceTo(cclient.Plr));
                return;
            }

            Obj.SendInteract(cclient.Plr, Menu);
        }

        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.F_DO_ABILITY, "F_DO_ABILITY")]
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
    }
}

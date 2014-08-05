/*
 * Copyright (C) 2013 APS
 *	http://AllPrivateServer.com
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common;
using FrameWork;

namespace WorldServer
{
    public class MovementHandlers : IPacketHandler
    {
        public enum MovementFlag : byte
        {
            RECULE = 84,            // 001010100
            AVANCE = 192,           // 011000000
            IMMOBILE = 254,         // 111111100

            SAUTE = 4,
            GDBOUGE = 8,            // 000001000
            GAUCHE = 32,            // 000010000
            DROITE = 64,            // 000100000

            HORS_COMBAT = 31,   // 000011111
            COMBAT = 95,        // 001011111
        }

        enum MovementTypes
        {
            GroundForward = 0xC0,
            GroundBackward = 0x54,
            FlyModeForward = 0x88,
            FlyModeBackward = 0x00,
            NotMoving = 0xFE,
        }

        enum Strafe  // 
        {
            left1 = 0x20,
            left2 = 0xa0,

            leftforward1 = 0x30,
            leftforward2 = 0xb0,

            right1 = 0x40,
            right2 = 0xc0,

            rightforward1 = 0x50,
            rightforward2 = 0xd0,

            jumpright = 0x44,
            jumpleft = 0x24,
        }

        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.F_ZONEJUMP, (int)eClientState.Playing, "F_JUMPZONE")]
        static public void F_ZONEJUMP(BaseClient client, PacketIn packet)
        {
            GameClient cclient = client as GameClient;

            if (cclient.Plr == null || !cclient.Plr.IsInWorld())
                return;

            //Log.Dump("Jump", packet, true);
            UInt32 Id = packet.GetUint32();
            Log.Info("Jump", "Jump to :" + Id);

            Zone_jump Jump = WorldMgr.GetZoneJump(Id);
            if (Jump != null)
            {
                cclient.Plr.Teleport(Jump.ZoneID, Jump.WorldX, Jump.WorldY, Jump.WorldZ, Jump.WorldO);
            }
            else
                Log.Error("Jump", "Invalid Jump");
        }


        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.F_PLAYER_STATE2, (int)eClientState.WorldEnter, "F_PLAYER_STATE2")]
        static public void F_PLAYER_STATE2(BaseClient client, PacketIn packet)
        {
            GameClient cclient = client as GameClient;

            if (cclient.Plr == null || !cclient.Plr.IsInWorld())
                return;

            Player Plr = cclient.Plr;

            long Pos = packet.Position;

            PacketOut Out = new PacketOut((byte)Opcodes.F_PLAYER_STATE2);
            Out.Write(packet.ToArray(), (int)packet.Position, (int)packet.Size);
            Out.WriteByte(0);
            Plr.DispatchPacket(Out, false);

            packet.Position = Pos;

            UInt16 Key = packet.GetUint16();

            byte Type = packet.GetUint8();
            byte MoveingState = packet.GetUint8();
            byte CombatByte = packet.GetUint8();
            byte Strafe = packet.GetUint8();

            UInt16 Heading = packet.GetUint16R();
            UInt16 X = packet.GetUint16R();
            UInt16 Y = packet.GetUint16R();
            byte Unk1 = packet.GetUint8();
            UInt16 Z = packet.GetUint16R();
            byte zmod = packet.GetUint8();

            //Log.Success("zMod ",""+zmod);
            // zmod is somewhat strange what i found out so far
            // z mod is 255 while standing
            // z mod is 0 while running and z is below 65535
            // z mod is 1 while running and z is above 65535
            // z mod can be  113 / 97 / 115 / 99 while running and z is below 65535 and enemy in target
            // z mod is 99 / 100 / 116  while running and z is above 65535 and enemy in target

            // z mod is 4 while running in water
            // z mod is 68 while swimming in water
            // z mod is ticking with 255 7 times then 68 while standing in deep water
            // z mod is 12 while running in water (that should lower your health / kill you and reduce movement speed)
            // z mod is ticking with 255 7 times then 12 while standing in lava

            if (packet.Size < 10)
                return;

            int z_temp = Z;

            Heading /= 8;
            X /= 2;
            Y /= 2;

            // z update if z is higher then 65535
            if (zmod != 0 && zmod != 97 && zmod != 113 && zmod != 99 && zmod != 115)
                z_temp += 65535;

            if (Type != (byte)MovementTypes.NotMoving)
                Plr.IsMoving = true;
            else
                Plr.IsMoving = false;

            //Log.Success("Movement Before ", X + "," + Y + "," + Z);
            if (CombatByte >= 50 && CombatByte < 0x92 || CombatByte == 0xDF)
            {
                if (Plr.LastCX != 0 && Plr.LastCY != 0)
                {
                    if (Plr.LastCX > 12288 && X < 4096)
                        Plr.SetOffset((ushort)(Plr.XOffset + 1), Plr.YOffset);
                    else if (X > 12288 && Plr.LastCX < 4096)
                        Plr.SetOffset((ushort)(Plr.XOffset - 1), Plr.YOffset);

                    if (Plr.LastCY > 24576 && Y < 8192)
                        Plr.SetOffset(Plr.XOffset, (ushort)(Plr.YOffset + 1));
                    else if (Y > 24576 && Plr.LastCY < 8192)
                        Plr.SetOffset(Plr.XOffset, (ushort)(Plr.YOffset - 1));
                }

                Plr.LastCX = X;
                Plr.LastCY = Y;

                X = Plr.Zone.CalculCombat(X, Plr.XOffset, true);
                Y = Plr.Zone.CalculCombat(Y, Plr.YOffset, false);

                Heading /= 2;
                z_temp /= 16;

                // combat offset z
                if (Plr._ZoneMgr.ZoneId == 161 || Plr._ZoneMgr.ZoneId == 162)
                    z_temp += 12288;
                else
                    z_temp += 4096;
            }
            else
            {
                if (Plr.LastX != 0 && Plr.LastY != 0)
                {
                    if (Plr.LastX > 24576 && X < 4096)
                        Plr.SetOffset((ushort)(Plr.XOffset + 1), Plr.YOffset);
                    else if (Plr.LastX < 4096 && X > 24576)
                        Plr.SetOffset((ushort)(Plr.XOffset - 1), Plr.YOffset);

                    if (Plr.LastY > 24576 && Y < 4096)
                        Plr.SetOffset(Plr.XOffset, (ushort)(Plr.YOffset + 1));
                    else if (Plr.LastY < 4096 && Y > 24576)
                        Plr.SetOffset(Plr.XOffset, (ushort)(Plr.YOffset - 1));

                }

                Plr.LastX = X;
                Plr.LastY = Y;

                X = Plr.Zone.CalculPin(X, Plr.XOffset, true);
                Y = Plr.Zone.CalculPin(Y, Plr.YOffset, false);
                z_temp /= 4;
            }

            //  if (Plr.IsInWorld() && Plr.Zone.ZoneId == 161)
            //      Z += 16384;

            //Log.Success("Movement after", "X=" + X + ",Y=" + Y + ",Z=" + Z + ",ztemp = " + z_temp + "," + Type + "," + Unk1 + "," + CombatByte);
            Plr.SetPosition(X, Y, (ushort)z_temp, Heading);
        }

        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.F_DUMP_STATICS, (int)eClientState.WorldEnter, "onDumpStatics")]
        static public void F_DUMP_STATICS(BaseClient client, PacketIn packet)
        {
            GameClient cclient = client as GameClient;

            if (cclient.Plr == null || !cclient.Plr.IsInWorld())
                return;

            UInt32 Unk1 = packet.GetUint32();
            UInt16 Unk2 = packet.GetUint16();
            UInt16 OffX = packet.GetUint16();
            UInt16 Unk3 = packet.GetUint16();
            UInt16 OffY = packet.GetUint16();

            cclient.Plr.SetOffset(OffX, OffY);

            if (!cclient.Plr.IsActive)
            {
                cclient.Plr.IsActive = true;
            }
        }

        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.F_OPEN_GAME, (int)eClientState.CharScreen, "onOpenGame")]
        static public void F_OPEN_GAME(BaseClient client, PacketIn packet)
        {
            GameClient cclient = client as GameClient;

            PacketOut Out = new PacketOut((byte)Opcodes.S_GAME_OPENED);

            if (cclient.Plr == null)
                Out.WriteByte(1);
            else
                Out.WriteByte(0);

            cclient.SendPacket(Out);
        }

        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.F_INIT_PLAYER, (int)eClientState.CharScreen, "onInitPlayer")]
        static public void F_INIT_PLAYER(BaseClient client, PacketIn packet)
        {
            GameClient cclient = client as GameClient;

            Player Plr = cclient.Plr;
            if (Plr == null)
                return;

            if (!Plr.IsInWorld()) // Si le joueur n'est pas sur une map, alors on l'ajoute a la map
            {
                UInt16 ZoneId = Plr._Info.Value.ZoneId;
                ushort RegionId = (ushort)Plr._Info.Value.RegionId;
                RegionMgr Region = WorldMgr.GetRegion(RegionId, true);
                Region.AddObject(Plr, ZoneId, false);
            }
            else
                Plr._Loaded = false;
        }

        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.F_PLAYER_ENTER_FULL, (int)eClientState.CharScreen, "onPlayerEnterFull")]
        static public void F_PLAYER_ENTER_FULL(BaseClient client, PacketIn packet)
        {
            GameClient cclient = client as GameClient;

            UInt16 SID;
            byte unk1, serverID, characterSlot;

            SID = packet.GetUint16();
            unk1 = packet.GetUint8();
            serverID = packet.GetUint8();
            string CharName = packet.GetString(24);
            packet.Skip(2);
            string Language = packet.GetString(2);
            packet.Skip(4);
            characterSlot = packet.GetUint8();

            Log.Debug("F_PLAYER_ENTER_FULL", "Entrer en jeu de : " + CharName + ",Slot=" + characterSlot);

            if (Program.Rm.RealmId != serverID)
                cclient.Disconnect();
            else
            {
                PacketOut Out = new PacketOut((byte)Opcodes.S_PID_ASSIGN);
                Out.WriteUInt16R((ushort)cclient.Id);
                cclient.SendPacket(Out);
            }
        }

        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.F_REQUEST_WORLD_LARGE, (int)eClientState.CharScreen, "onRequestWorldLarge")]
        static public void F_REQUEST_WORLD_LARGE(BaseClient client, PacketIn packet)
        {
            GameClient cclient = client as GameClient;

            if (cclient.Plr == null)
                return;

            if (!cclient.Plr.IsLoad())
                return;

            PacketOut Out = new PacketOut((byte)Opcodes.F_SET_TIME);
            Out.WriteUInt32((uint)TCPServer.GetTimeStamp());
            Out.WriteUInt32(7);
            cclient.Plr.SendPacket(Out);

            Out = new PacketOut((byte)Opcodes.S_WORLD_SENT);
            Out.WriteByte(0);
            cclient.Plr.SendPacket(Out);
        }

        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.F_REQUEST_INIT_PLAYER, (int)eClientState.WorldEnter, "onRequestInitPlayer")]
        static public void F_REQUEST_INIT_PLAYER(BaseClient client, PacketIn packet)
        {
            GameClient cclient = client as GameClient;

            if (cclient.Plr == null)
                return;

            ushort Oid = packet.GetUint16();

            foreach (Player Obj in cclient.Plr._PlayerRanged)
                if (Obj != null && !Obj.IsDisposed && Obj.Oid == Oid)
                    Obj.SendMeTo(cclient.Plr);
        }

        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.F_REQUEST_INIT_OBJECT, (int)eClientState.WorldEnter, "onRequestInitObject")]
        static public void F_REQUEST_INIT_OBJECT(BaseClient client, PacketIn packet)
        {
            GameClient cclient = client as GameClient;

            if (cclient.Plr == null)
                return;

            ushort Oid = packet.GetUint16();

            foreach (Object Obj in cclient.Plr._ObjectRanged)
                if (Obj != null && !Obj.IsDisposed && Obj.Oid == Oid)
                    Obj.SendMeTo(cclient.Plr);
        }

        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.F_FLIGHT, "F_FLIGHT")]
        static public void F_FLIGHT(BaseClient client, PacketIn packet)
        {
            GameClient cclient = client as GameClient;

            UInt16 TargetOID = packet.GetUint16();
            UInt16 State = packet.GetUint16();

            //Log.Info("F_FLIGHT", "TargetOid = " + TargetOID + ",State=" + State);

            if (State == 20) // Flight Master
            {
                Object Obj = cclient.Plr.Zone.GetObject(TargetOID);
                if (Obj == null || !Obj.IsCreature())
                {
                    Log.Error("F_FLIGHT", "Invalid Creature OID : " + TargetOID);
                    return;
                }

                UInt16 FlyID = packet.GetUint16();

                List<Zone_Taxi> Taxis = WorldMgr.GetTaxis(cclient.Plr);
                if (Taxis.Count <= FlyID - 1)
                    return;

                if (!cclient.Plr.RemoveMoney(Taxis[FlyID - 1].Info.Price))
                {
                    cclient.Plr.SendLocalizeString("", GameData.Localized_text.TEXT_MERCHANT_INSUFFICIENT_MONEY_TO_BUY);
                    return;
                }

                cclient.Plr.Teleport(Taxis[FlyID - 1].ZoneID, Taxis[FlyID - 1].WorldX, Taxis[FlyID - 1].WorldY, Taxis[FlyID - 1].WorldZ, Taxis[FlyID - 1].WorldO);
            }
        }
    }
}

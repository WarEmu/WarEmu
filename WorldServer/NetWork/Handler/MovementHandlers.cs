
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

        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.F_PLAYER_STATE2, "onPlayerState2")]
        static public void F_PLAYER_STATE2(BaseClient client, PacketIn packet)
        {
            GameClient cclient = client as GameClient;

            if (cclient.Plr == null || !cclient.Plr.IsInWorld())
                return;

            Player Plr = cclient.Plr;

            try
            {
                long Pos = packet.Position;

                PacketOut Out = new PacketOut((byte)Opcodes.F_PLAYER_STATE2);
                Out.Write(packet.ToArray(), (int)packet.Position, (int)packet.Size);
                Out.WriteByte(0);
                Plr.DispatchPacket(Out, false);

                packet.Position = Pos;
            }
            catch (Exception e)
            {
                Log.Error("F_PLAYER_STATE2", e.ToString());
            }

            if (packet.Size < 18)
            {
                Plr.IsMoving = false;
                return;
            }

            UInt16 Key = packet.GetUint16();

            byte MoveByte = packet.GetUint8();
            byte UnkByte = packet.GetUint8();
            byte CombatByte = packet.GetUint8();
            byte RotateByte = packet.GetUint8();

            UInt16 Heading = packet.GetUint16R();
            UInt16 X = packet.GetUint16R();
            UInt16 Y = packet.GetUint16R();
            byte Unk1 = packet.GetUint8();
            UInt16 Z = packet.GetUint16R();
            byte Unk2 = packet.GetUint8();

            Heading /= 8;
            X /= 2;
            Y /= 2;
            Z /= 2;

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
                Z += 32768;
                Z /= 4;
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
            }

            Plr.SetPosition(X, Y, Z, Heading);
        }

        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.F_DUMP_STATICS, "onDumpStatics")]
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

            Log.Success("F_DUMP_STATIC", "X=" + OffX + ",Y=" + OffY);

            cclient.Plr.SetOffset(OffX, OffY);
        }

        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.F_OPEN_GAME, "onOpenGame")]
        static public void F_OPEN_GAME(BaseClient client, PacketIn packet)
        {
            GameClient cclient = client as GameClient;

            PacketOut Out = new PacketOut((byte)Opcodes.S_GAME_OPENED);

            if (cclient.Plr == null)
                Out.WriteByte(1);
            else
                Out.WriteByte(0);

            cclient.SendTCP(Out);
        }

        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.F_INIT_PLAYER, "onInitPlayer")]
        static public void F_INIT_PLAYER(BaseClient client, PacketIn packet)
        {
            GameClient cclient = client as GameClient;

            Log.Success("F_INIT_PLAYER", "Lancement !");

            Player Plr = cclient.Plr;

            if (!Plr.IsInWorld()) // Si le joueur n'est pas sur une map, alors on l'ajoute a la map
            {
                UInt16 ZoneId = Plr._Info.Value[0].ZoneId;
                ushort RegionId = (ushort)Plr._Info.Value[0].RegionId;
                RegionMgr Region = WorldMgr.GetRegion(RegionId, true);
                Region.AddObject(Plr, ZoneId);
            }
            else
                Plr._Loaded = false;
        }

        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.F_PLAYER_ENTER_FULL, "onPlayerEnterFull")]
        static public void F_PLAYER_ENTER_FULL(BaseClient client, PacketIn packet)
        {
            GameClient cclient = client as GameClient;

          /*  UInt16 SID;
            byte unk1, serverID, characterSlot;

            SID = packet.GetUint16();
            unk1 = packet.GetUint8();
            serverID = packet.GetUint8();
            string CharName = packet.GetString(24);
            packet.Skip(2);
            string Language = packet.GetString(2);
            packet.Skip(4);
            characterSlot = packet.GetUint8();

            Log.Success("F_PLAYER_ENTER_FULL", "Entrer en jeu de : " + CharName + ",Slot=" + characterSlot);

            if (Program.Rm.RealmId != serverID)
                cclient.Disconnect();
            else
            {*/
                PacketOut Out = new PacketOut((byte)Opcodes.S_PID_ASSIGN);
                Out.WriteUInt16R((ushort)cclient.Id);
                cclient.SendTCP(Out);
            //}
        }

        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.F_REQUEST_WORLD_LARGE, "onRequestWorldLarge")]
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

        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.F_REQUEST_INIT_PLAYER, "onRequestInitPlayer")]
        static public void F_REQUEST_INIT_PLAYER(BaseClient client, PacketIn packet)
        {
            GameClient cclient = client as GameClient;

            Log.Dump("F_REQUEST_INIT_PLAYER", packet.ToArray(), 0, packet.ToArray().Length);

            if (cclient.Plr == null)
                return;

            foreach (Player Obj in cclient.Plr._PlayerRanged)
                Obj.SendMeTo(cclient.Plr);
        }

        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.F_REQUEST_INIT_OBJECT, "onRequestInitObject")]
        static public void F_REQUEST_INIT_OBJECT(BaseClient client, PacketIn packet)
        {
            GameClient cclient = client as GameClient;
        }

        [PacketHandlerAttribute(PacketHandlerType.TCP, (int)Opcodes.F_FLIGHT, "F_FLIGHT")]
        static public void F_FLIGHT(BaseClient client, PacketIn packet)
        {
            GameClient cclient = client as GameClient;

            UInt16 TargetOID = packet.GetUint16();
            UInt16 State = packet.GetUint16();

            Log.Info("F_FLIGHT", "TargetOid = " + TargetOID + ",State=" + State);

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

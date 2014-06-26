/*
 * Copyright (C) 2014 WarEmu
 *	http://WarEmu.com
 * 
 * Copyright (C) 2011-2013 APS
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
    public class Creature : Unit
    {
        public Creature_spawn Spawn;
        public uint Entry
        {
            get
            {
                if (Spawn == null)
                    return 0;

                return Spawn.Entry;
            }
        }

        public Creature()
            : base()
        {
        }

        public Creature(Creature_spawn Spawn) : this()
        {
            this.Spawn = Spawn;
            Name = Spawn.Proto.Name;
        }

        public override void Update(long Tick)
        {
            base.Update(Tick);
        }

        static public ushort GenerateWounds(byte Level, byte Rank)
        {
            float Wounds = 0;
            Wounds += 95 * (Level + 1);
            Wounds += Level * 5.5f;
            if (Rank > 0)
                Wounds += Rank * (5.85f * Level * 52);
            return (ushort)(Wounds/10);
        }
        public override void OnLoad()
        {
            InteractType = GenerateInteractType(Spawn.Title != 0 ? Spawn.Title : Spawn.Proto.Title);

            SetFaction(Spawn.Faction != 0 ? Spawn.Faction : Spawn.Proto.Faction);

            ItmInterface.Load(WorldMgr.GetCreatureItems(Spawn.Entry));
            if (Spawn.Proto.MinLevel > Spawn.Proto.MaxLevel)
                Spawn.Proto.MinLevel = Spawn.Proto.MaxLevel;

            if (Spawn.Proto.MaxLevel <= Spawn.Proto.MinLevel)
                Spawn.Proto.MaxLevel = Spawn.Proto.MinLevel;

            if (Spawn.Proto.MaxLevel == 0) Spawn.Proto.MaxLevel = 1;
            if (Spawn.Proto.MinLevel == 0) Spawn.Proto.MinLevel = 1;

            Level = (byte)RandomMgr.Next((int)Spawn.Proto.MinLevel, (int)Spawn.Proto.MaxLevel+1);
            StsInterface.SetBaseStat((byte)GameData.Stats.STATS_WOUNDS, GenerateWounds(Level,Rank));
            StsInterface.ApplyStats();
            Health = TotalHealth;

            X = Zone.CalculPin((uint)(Spawn.WorldX), true);
            Y = Zone.CalculPin((uint)(Spawn.WorldY), false);
            Z = (ushort)(Spawn.WorldZ);
           // if (Zone.ZoneId == 161)
           // {
               // Z += 16384;
               // X += 16384;
               // Y += 16384;
         //   }

            // TODO : Bad Height Formula
            /*int HeightMap = HeightMapMgr.GetHeight(Zone.ZoneId, X, Y);
            if (Z < HeightMap)
            {
                Log.Error("Creature", "["+Spawn.Entry+"] Invalid Height : Min=" + HeightMap + ",Z=" + Z);
                return;
            }*/

            Heading = (ushort)Spawn.WorldO;
            WorldPosition.X = Spawn.WorldX;
            WorldPosition.Y = Spawn.WorldY;
            WorldPosition.Z = Spawn.WorldZ;

            SetOffset((ushort)(Spawn.WorldX >> 12), (ushort)(Spawn.WorldY >> 12));
            ScrInterface.AddScript(Spawn.Proto.ScriptName);
            base.OnLoad();

            if (Spawn.Title == 0 && Spawn.Icone == 0 && Spawn.Proto.Title == 0 && Spawn.Icone == 0 && Spawn.Emote == 0 && Spawn.Proto.FinishingQuests == null && Spawn.Proto.StartingQuests == null)
            {
                if (Faction <= 1 || Faction == 128 || Faction == 129)
                {
                    SFastRandom Random = new SFastRandom(X ^ Y ^ Z);

                    for (int i = 0; i < 3; ++i)
                    {
                        Waypoint Wp = new Waypoint();
                        Wp.X = (ushort)(X + Random.randomInt(50) + Random.randomInt(100) + Random.randomInt(150));
                        Wp.Y = (ushort)(Y + Random.randomInt(50) + Random.randomInt(100) + Random.randomInt(150));
                        Wp.Z = (ushort)Z;
                        Wp.Speed = 10;
                        Wp.WaitAtEndMS = (uint)(5000 + Random.randomIntAbs(10) * 1000);
                        AiInterface.AddWaypoint(Wp);
                    }
                }
            }

            IsActive = true;
        }
        public override void SendMeTo(Player Plr)
        {
            //Log.Success("Creature", "SendMe " + Name);

            PacketOut Out = new PacketOut((byte)Opcodes.F_CREATE_MONSTER);
            Out.WriteUInt16(Oid);
            Out.WriteUInt16(0);

            Out.WriteUInt16((UInt16)Heading);
            Out.WriteUInt16((UInt16)WorldPosition.Z);
            Out.WriteUInt32((UInt32)WorldPosition.X);
            Out.WriteUInt32((UInt32)WorldPosition.Y);
            Out.WriteUInt16(0); // Speed Z
            // 18
            Out.WriteUInt16(Spawn.Proto.Model1);
            Out.WriteByte((byte)Spawn.Proto.MinScale);
            Out.WriteByte(Level);
            Out.WriteByte(Faction);

            Out.Fill(0, 4);
            Out.WriteByte(Spawn.Emote);
            Out.WriteByte(0); // ?
            Out.WriteUInt16(Spawn.Proto._Unks[1]);
            Out.WriteByte(0);
            Out.WriteUInt16(Spawn.Proto._Unks[2]);
            Out.WriteUInt16(Spawn.Proto._Unks[3]);
            Out.WriteUInt16(Spawn.Proto._Unks[4]);
            Out.WriteUInt16(Spawn.Proto._Unks[5]);
            Out.WriteUInt16(Spawn.Proto._Unks[6]);
            Out.WriteUInt16(Spawn.Title);

            long TempPos = Out.Position;
            byte TempLen = (byte)(Spawn.bBytes.Length + States.Count);
            Out.WriteByte(TempLen);
            Out.Write(Spawn.bBytes, 0, Spawn.bBytes.Length);
            Out.Write(States.ToArray(), 0, States.Count);
            if (QtsInterface.CreatureHasStartQuest(Plr))
            {
                Out.WriteByte(5);
                Out.Position = TempPos;
                Out.WriteByte((byte)(TempLen + 1));
            }
            else if (QtsInterface.CreatureHasQuestToAchieve(Plr))
            {
                Out.WriteByte(4);
                Out.Position = TempPos;
                Out.WriteByte((byte)(TempLen + 1));
            }
            else if (QtsInterface.CreatureHasQuestToComplete(Plr))
            {
                Out.WriteByte(7);
                Out.Position = TempPos;
                Out.WriteByte((byte)(TempLen + 1));
            }

            Out.Position = Out.Length;

            Out.WriteByte(0);

            Out.WriteStringBytes(Name);

            Out.Fill(0, 48);
            Plr.SendPacket(Out);

            base.SendMeTo(Plr);
        }
        public override void SendInteract(Player Plr, InteractMenu Menu)
        {
            Plr.QtsInterface.HandleEvent(Objective_Type.QUEST_SPEACK_TO, Spawn.Entry, 1);

            if (!IsDead)
            {
                switch (InteractType)
                {
                    case GameData.InteractType.INTERACTTYPE_DYEMERCHANT:
                        {
                            string Text = WorldMgr.GetCreatureText(Spawn.Entry);

                            if (Menu.Menu == 9) // List des objets a vendre
                                WorldMgr.SendVendor(Plr, Spawn.Entry);
                            else if (Menu.Menu == 11) // Achat d'un item
                                WorldMgr.BuyItemVendor(Plr, Menu, Spawn.Entry);
                            else if (Menu.Menu == 14) // Vend un Item
                                Plr.ItmInterface.SellItem(Menu);
                            else if (Menu.Menu == 36) // Rachette un item
                                Plr.ItmInterface.BuyBackItem(Menu);
                            else
                            {
                                PacketOut Out = new PacketOut((byte)Opcodes.F_INTERACT_RESPONSE);
                                Out.WriteByte(0);
                                Out.WriteUInt16(Oid);
                                Out.WriteUInt16(0);
                                Out.WriteByte(0x40); // Dye
                                Out.WriteByte(0x22); // Vendors
                                Out.WriteByte(0);
                                Out.WritePascalString(Text);
                                Out.WriteByte(0);
                                Plr.SendPacket(Out);
                            }
                        } break;

                    case GameData.InteractType.INTERACTTYPE_FLIGHT_MASTER:
                        {
                            byte[] data = new byte[62]
		                    {
			                    0x01,0xF4,0x00,0x00,0x00,0x00,0x00,0x00,0x64,0x42,0x39,0x00,0x00,0x00,0xC0,0xE3,
			                    0x03,0x39,0xA0,0xD1,0x6F,0x00,0xC8,0xA8,0x1D,0x37,0x28,0x94,0x79,0x33,0xB2,0x24,
			                    0x32,0x44,0xDB,0xD7,0x1C,0x5D,0x18,0x5D,0xDD,0x1C,0xA4,0x0D,0x00,0x00,0xA8,0x6B,
			                    0x21,0x36,0x11,0x00,0x00,0x00,0xC8,0xD0,0xAF,0x3A,0x78,0xD1,0x6F,0x00
		                    };

                            UInt16 Counts = 1;

                            PacketOut Out = new PacketOut((byte)Opcodes.F_INTERACT_RESPONSE);
                            Out.WriteUInt16(0x0A12);
                            foreach (Zone_Taxi Taxi in WorldMgr.GetTaxis(Plr))
                            {
                                Out.WriteUInt16(Counts);
                                Out.WriteByte(2);
                                Out.WriteUInt16(Taxi.Info.Price);
                                Out.WriteUInt16(Taxi.Info.ZoneId);
                                Out.WriteByte(1);
                                ++Counts;
                            }
                            Out.Write(data);
                            Plr.SendPacket(Out);
                        }break;

                    case GameData.InteractType.INTERACTTYPE_TRAINER:
                        {
                            if (Menu.Menu == 7)
                            {
                                PacketOut Out = new PacketOut((byte)Opcodes.F_INTERACT_RESPONSE);
                                Out.WriteByte(5);
                                Out.WriteByte(0x0F);
                                Out.WriteByte(6);
                                Out.WriteUInt16(0);
                                Plr.SendPacket(Out);
                            }
                            else
                            {
                                PacketOut Out = new PacketOut((byte)Opcodes.F_INTERACT_RESPONSE);
                                Out.WriteByte(0);
                                Out.WriteUInt16(Oid);

                                if (Plr.Realm == GameData.Realms.REALMS_REALM_ORDER)
                                {
                                    Out.WritePacketString(@"|00 00 00 21 00 94 48 61 69 6C |.........!..Hail|
|20 64 65 66 65 6E 64 65 72 20 6F 66 20 74 68 65 | defender of the|
|20 45 6D 70 69 72 65 21 20 20 59 6F 75 72 20 70 | Empire!  Your p|
|65 72 66 6F 72 6D 61 6E 63 65 20 69 6E 20 62 61 |erformance in ba|
|74 74 6C 65 20 69 73 20 74 68 65 20 6F 6E 6C 79 |ttle is the only|
|20 74 68 69 6E 67 20 74 68 61 74 20 6B 65 65 70 | thing that keep|
|73 20 74 68 65 20 68 6F 72 64 65 73 20 6F 66 20 |s the hordes of |
|43 68 61 6F 73 20 61 74 20 62 61 79 2E 20 4C 65 |Chaos at bay. Le|
|74 27 73 20 62 65 67 69 6E 20 79 6F 75 72 20 74 |t's begin your t|
|72 61 69 6E 69 6E 67 20 61 74 20 6F 6E 63 65 21 |raining at once!|
|00                                              |.               |");
                                }
                                else
                                {
                                    Out.WritePacketString(@"|00 00 00 21 00 AA 4C 65 61 72 |.........!..Lear|
|6E 20 74 68 65 73 65 20 6C 65 73 73 6F 6E 73 20 |n these lessons |
|77 65 6C 6C 2C 20 66 6F 72 20 67 61 69 6E 69 6E |well, for gainin|
|67 20 74 68 65 20 66 61 76 6F 72 20 6F 66 20 74 |g the favor of t|
|68 65 20 52 61 76 65 6E 20 67 6F 64 20 73 68 6F |he Raven god sho|
|75 6C 64 20 62 65 20 6F 66 20 75 74 6D 6F 73 74 |uld be of utmost|
|20 69 6D 70 6F 72 74 61 6E 63 65 20 74 6F 20 79 | importance to y|
|6F 75 2E 20 4F 74 68 65 72 77 69 73 65 2E 2E 2E |ou. Otherwise...|
|20 54 68 65 72 65 20 69 73 20 61 6C 77 61 79 73 | There is always|
|20 72 6F 6F 6D 20 66 6F 72 20 6D 6F 72 65 20 53 | room for more S|
|70 61 77 6E 20 77 69 74 68 69 6E 20 6F 75 72 20 |pawn within our |
|72 61 6E 6B 73 2E 00                            |.......         |");
                                }
                                Plr.SendPacket(Out);
                            }
                        } break;
                    case GameData.InteractType.INTERACTTYPE_BANKER:
                        {
                            PacketOut Out = new PacketOut((byte)Opcodes.F_INTERACT_RESPONSE);
                            Out.WriteByte(0x1D);
                            Out.WriteByte(0);
                            Plr.SendPacket(Out);
                        } break;
                    default:
                        QtsInterface.HandleInteract(Plr, this, Menu);
                        break;
                };
            }

            base.SendInteract(Plr, Menu);
        }

 
        /* // Group LOOT : Pass , accept, cancel
         * Out.WritePacketString(@"|07 19 0A 00 00 00 00 03 2E 56 22 B9 00 |............V..|
|00 00 00 00 00 00 00 00 24 00 00 00 00 00 01 00 |........$.......|
|00 00 00 00 00 00 00 00 00 00 00 00 00 09 C4 00 |................|
|01 00 00 00 00 00 00 00 00 00 00 00 00 09 57 61 |..............Wa|
|72 20 43 72 65 73 74 00 00 00 00 00 00 71 50 72 |r Crest......qPr|
|6F 6F 66 20 6F 66 20 79 6F 75 72 20 76 61 6C 6F |oof of your valo|
|72 20 6F 6E 20 74 68 65 20 66 69 65 6C 64 20 6F |r on the field o|
|66 20 62 61 74 74 6C 65 2E 20 54 68 65 73 65 20 |f battle. These |
|6D 61 79 20 62 65 20 75 73 65 64 20 74 6F 20 74 |may be used to t|
|72 61 64 65 20 66 6F 72 20 65 71 75 69 70 6D 65 |rade for equipme|
|6E 74 20 66 72 6F 6D 20 76 61 72 69 6F 75 73 20 |nt from various |
|51 75 61 72 74 65 72 6D 61 73 74 65 72 73 2E 01 |Quartermasters..|
|00 00 00 03 06 00 08 00 00 00 00 00 00 00 00 00 |................|
|00 00 00 00 00 00 00 00 00 00                   |..........      |");*/

        public override void SetDeath(Unit Killer)
        {
            Killer.QtsInterface.HandleEvent(Objective_Type.QUEST_KILL_MOB, Spawn.Entry,1);
            base.SetDeath(Killer);
            EvtInterface.AddEvent(RezUnit, 30000 + Level * 1000, 1); // 30 seconde Rez
        }

        public override void RezUnit()
        {
            Region.CreateCreature(Spawn);
            Dispose();
        }

        public override string ToString()
        {
            return "SpawnId=" + Spawn.Entry + ",Entry=" + Spawn.Entry + ",Name=" + Name + ",Level=" + Level + ",Faction=" + Faction + ",Emote=" + Spawn.Emote + "AI:" + AiInterface.State + ",Position :" + base.ToString();
        }
    }
}

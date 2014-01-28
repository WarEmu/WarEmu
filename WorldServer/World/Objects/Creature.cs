
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

        public override void Update()
        {
            base.Update();
        }

        static public ushort GenerateWounds(byte Level, byte Rank)
        {
            float Wounds = 0;
            Wounds += 92 * (Level + 1);
            Wounds += Level * 2.5f;
            if (Rank > 0)
                Wounds += Rank * (5.85f * Level * 92);
            return (ushort)(Wounds/10);
        }
        public override void OnLoad()
        {
            InteractType = GenerateInteractType(Spawn.Title);

            SetFaction(Spawn.Faction != 0 ? Spawn.Faction : Spawn.Proto.Faction);

            ItmInterface.Load(WorldMgr.GetCreatureItems(Spawn.Entry));
            Level = (byte)RandomMgr.Next((int)Spawn.Proto.MinLevel, (int)Spawn.Proto.MaxLevel);
            StsInterface.SetBaseStat((byte)GameData.Stats.STATS_WOUNDS, GenerateWounds(Level,Rank));
            StsInterface.ApplyStats();
            Health = TotalHealth;

            X = Zone.CalculPin((uint)(Spawn.WorldX), true);
            Y = Zone.CalculPin((uint)(Spawn.WorldY), false);
            Z = (ushort)(Spawn.WorldZ * 2);


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
            Region.UpdateRange(this);
                
            base.OnLoad();
        }
        public override void SendMeTo(Player Plr)
        {
            List<byte> TmpState = new List<byte>();

            if (QtsInterface.CreatureHasStartQuest(Plr))
                TmpState.Add(5);
            else if (QtsInterface.CreatureHasQuestToComplete(Plr))
                TmpState.Add(7);

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
            Out.WriteByte(Spawn.Proto.MinLevel);
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

            Out.WriteByte((byte)(Spawn.bBytes.Length + States.Count + TmpState.Count));
            Out.Write(Spawn.bBytes, 0, Spawn.bBytes.Length);
            Out.Write(States.ToArray(), 0, States.Count);
            Out.Write(TmpState.ToArray(), 0, TmpState.Count);

            Out.WriteByte(0);

            Out.WriteStringBytes(Name);

            Out.WriteByte(0); // ?
            Out.WriteByte(1); // ?
            Out.WriteByte(10); // ?

            Out.WriteByte(0); // ?

            Out.WriteUInt16(0); // ?
            Out.WriteByte(Spawn.Icone);
            Out.WriteByte((byte)Spawn.Proto._Unks[0]);

            Out.WriteByte(0);

            Out.Fill(0, 8); // Flags;

            Out.WriteByte(100); // Health %

            Out.WriteUInt16(Zone.ZoneId);

            Out.Fill(0, 48);

            Plr.SendPacket(Out);

            base.SendMeTo(Plr);
        }
        public override void SendInteract(Player Plr, InteractMenu Menu)
        {
            Log.Success("SendInteract", "" + Name + " -> " + Plr.Name + ",Type="+InteractType);

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
                            Zone_Info Info;

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

                    default:
                        QtsInterface.HandleInteract(Plr, Menu);
                        break;
                };
            }

            base.SendInteract(Plr, Menu);
        }

        public override void SetDeath(Unit Killer)
        {
            Killer.QtsInterface.HandleEvent(Objective_Type.QUEST_KILL_MOB, Spawn.Entry,1);
            base.SetDeath(Killer);
            EvtInterface.AddEvent(RezUnit, 60000, 1); // 60 seconde Rez
        }

        public override void RezUnit()
        {
            Region.CreateCreature(Spawn);
            Dispose();
        }

        public override string ToString()
        {
            return "SpawnId="+Spawn.Entry+",Entry="+Spawn.Entry+",Name="+Name+",Level="+Level+",Faction="+Faction+",Emote="+Spawn.Emote+",Position :" +base.ToString();
        }
    }
}

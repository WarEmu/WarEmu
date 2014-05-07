
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common;
using FrameWork;

namespace WorldServer
{
    public class GameObject : Unit
    {
        public GameObject_spawn Spawn;
        public bool Looted;
        static public int RELOOTABLE_TIME = 120000; // 2 Mins

        public GameObject()
            : base()
        {

        }

        public GameObject(GameObject_spawn Spawn)
            : this()
        {
            this.Spawn = Spawn;
            Name = Spawn.Proto.Name;
        }

        public override void OnLoad()
        {
            Faction = Spawn.Proto.Faction;
            while (Faction >= 8) Faction -= 8;
            if (Faction < 2) Rank = 0;
            else if (Faction < 4) Rank = 1;
            else if (Faction < 6) Rank = 2;
            else if (Faction < 9) Rank = 3;
            Faction = Spawn.Proto.Faction;

            Level = Spawn.Proto.Level;
            MaxHealth = Math.Min(1,Spawn.Proto.HealthPoints);
            Health = TotalHealth;

            X = Zone.CalculPin((uint)(Spawn.WorldX), true);
            Y = Zone.CalculPin((uint)(Spawn.WorldY), false);
            Z = (ushort)(Spawn.WorldZ);

            Heading = (ushort)Spawn.WorldO;
            SetOffset((ushort)(Spawn.WorldX >> 12), (ushort)(Spawn.WorldY >> 12));
            ScrInterface.AddScript(Spawn.Proto.ScriptName);
            base.OnLoad();
            IsActive = true;
            Looted = false;
        }

        /*
         * |00 3D 71 
06 90 
00 00 
00 00 
1F E3 
00 0C E3 29 
00 0E 59 90 
FF FF 
1E 00 00 00 35 F3 00 03 04 00 00
|00 00 64 8B 76 09 CD 00 00 00 00 0F 45 6D 70 69 |..d.v.......Empi|
|72 65 20 42 61 72 20 44 6F 6F 72 04 06 AB DD 00 |re Bar Door.....|                                                
-------------------------------------------------------------------
         * */

        public override void SendMeTo(Player Plr)
        {
            PacketOut Out = new PacketOut((byte)Opcodes.F_CREATE_STATIC);
            Out.WriteUInt16(Oid);
            Out.WriteUInt16(0);

            Out.WriteUInt16((UInt16)Spawn.WorldO);
            Out.WriteUInt16((UInt16)Spawn.WorldZ);
            Out.WriteUInt32((UInt32)Spawn.WorldX);
            Out.WriteUInt32((UInt32)Spawn.WorldY);
            Out.WriteUInt16((ushort)Spawn.DisplayID);

            Out.WriteUInt16(Spawn.GetUnk(0));
            Out.WriteUInt16(Spawn.GetUnk(1));
            Out.WriteUInt16(Spawn.GetUnk(2));
            Out.WriteByte(Spawn.Unk1);

            int flags = Spawn.GetUnk(3);
            Loot Loots = LootsMgr.GenerateLoot(this, Plr);
            if (Loots != null && Loots.IsLootable())
            {
                flags = flags | 4;
            }

            Out.WriteUInt16((ushort)flags);
            Out.WriteByte(Spawn.Unk2);
            Out.WriteUInt32(Spawn.Unk3);
            Out.WriteUInt16(Spawn.GetUnk(4));
            Out.WriteUInt16(Spawn.GetUnk(5));
            Out.WriteUInt32(Spawn.Unk4);

            Out.WritePascalString(Name);
            Out.WriteByte(0);

            Plr.SendPacket(Out);

            base.SendMeTo(Plr);
        }

        public override void SendInteract(Player Plr, InteractMenu Menu)
        {
            Tok_Info Info = WorldMgr.GetTok(Spawn.Proto.TokUnlock);

            if (!IsDead)
            {
                Plr.QtsInterface.HandleEvent(Objective_Type.QUEST_USE_GO, Spawn.Entry, 1);
            }

            if (Spawn.Proto.TokUnlock != 0)
                Plr.TokInterface.AddTok(Info);

            Loot Loots = LootsMgr.GenerateLoot(this, Plr);

            if (Loots != null)
            {
                Loots.SendInteract(Plr, Menu);
                // If object has been looted, make it unlootable
                // and then Reset its lootable staus in XX seconds
                if (!Loots.IsLootable())
                {
                    Looted = true;
                    foreach (Object Obj in this._ObjectRanged)
                    {
                        if (Obj.IsPlayer())
                        {
                            this.SendMeTo(Obj.GetPlayer());
                        }
                    }
                    EvtInterface.AddEvent(ResetLoot, RELOOTABLE_TIME, 1);   
                }
            }

            base.SendInteract(Plr, Menu);
        }

        // This will reset the GameObject loot after it has
        // been looted. Allowing it to be looted again.
        public void ResetLoot()
        {
            Looted = false;
            foreach (Object Obj in this._ObjectRanged)
            {
                if (Obj.IsPlayer())
                {
                    this.SendMeTo(Obj.GetPlayer());
                }
            }
        }

        public override string ToString()
        {
            return "SpawnId=" + Spawn.Guid + ",Entry=" + Spawn.Entry + ",Name=" + Name + ",Level=" + Level + ",Faction=" + Faction + ",Position :" + base.ToString();
        }
    }
}

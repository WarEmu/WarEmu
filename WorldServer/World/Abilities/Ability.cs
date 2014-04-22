using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FrameWork;
using Common;

namespace WorldServer
{
    public class Ability
    {
        public byte BuffId;
        public Ability Parent;
        public Unit Caster;
        public AbilityInterface Interface;
        public Ability_Stats Info;
        public IAbilityTypeHandler Handler;
        public ushort Px;
        public ushort Py;
        public ushort Pz;
        public ushort ZoneId;

        public bool IsStarted = false;
        public bool IsDone = false;
        public bool IsBuff = false;

        private long StartTime = 0;
        private long EndTime = 0;
        private long DoneTime = 0;

        public Ability(AbilityInterface Interface, Ability Parent, Ability_Stats Info, Unit Caster, bool IsBuff, ushort Px, ushort Py, ushort Pz, ushort ZoneId, string OverrideHandler = "")
            : this(Interface, Parent, Info, Caster, IsBuff, OverrideHandler)
        {
            this.Px = Px;
            this.Py = Py;
            this.Pz = Pz;
            this.ZoneId = ZoneId;
        }

        public Ability(AbilityInterface Interface, Ability Parent, Ability_Stats Info, Unit Caster, bool IsBuff, string OverrideHandler = "")
        {
            this.Parent = Parent;
            this.Info = Info;
            this.Caster = Caster;
            this.Interface = Interface;
            if (OverrideHandler == "")
                this.Handler = AbilityMgr.GetAbilityHandler(Info.Entry, Info.Info.HandlerName);
            else
                this.Handler = AbilityMgr.GetAbilityHandler(0, OverrideHandler);

            this.IsBuff = IsBuff;

            if (Handler != null)
                Handler.InitAbility(this);
        }

        public void Start()
        {
            //Log.Info("Ability", "Start : " + Caster.Name);
            IsStarted = true;
            StartTime = TCPServer.GetTimeStampMS();
            NextTick = StartTime + 1000;
            Caster.EvtInterface.Notify(EventName.ON_START_CASTING, Caster, this);

            if (Handler != null)
                Handler.Start(this);

            if(!IsBuff)
                SendStart();
        }

        public void Update(long Tick)
        {
            if (Handler != null)
                Handler.Update(Tick);

            if (NextTick < Tick)
            {
                NextTick = Tick + 1000;
                OnTick(TickCount);
                ++TickCount;
            }

            if (!IsBuff)
            {
                if (StartTime + Info.Info.CastTime <= TCPServer.GetTimeStampMS())
                {
                    if (Info.Info.ApSec == 0)
                        Cast();
                    else
                        OnTick(TickCount);

                    Stop();
                }
            }

            if (EndTime != 0 && EndTime < Tick)
                Stop();
        }

        public void Stop()
        {
            IsDone = true;

            if (Handler != null)
            {
                Handler.Stop();
                Handler = null;
            }

            SendStop();
            if (Info.Info.ApSec != 0)
                SendAbilityDone(0);
        }

        public bool Cast()
        {
            if (IsDone)
                return false;

            bool CanCast = true;

            if (Caster.AbtInterface.CanCast(Info, false) == GameData.AbilityResult.ABILITYRESULT_OK)
            {
                Caster.ActionPoints -= Info.Info.ApCost;

                if (Info.Info.ApSec == 0)
                    DoneTime = TCPServer.GetTimeStampMS();

                if (Handler != null)
                {
                    if (CanCast && Handler.CanCast(false) == GameData.AbilityResult.ABILITYRESULT_OK)
                    {
                        Caster.EvtInterface.Notify(EventName.ON_CAST, Caster, this);
                        Handler.Cast();
                    }
                    else
                        Handler.SendDone();
                }
                else
                    SendAbilityDone(0);

                WorldMgr.GeneralScripts.OnCastAbility(this);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void Cancel()
        {
            Stop();
        }

        public void SendStart()
        {
            //Log.Info("Ability", "SendStart " + Handler);

            PacketOut Out = new PacketOut((byte)Opcodes.F_USE_ABILITY);
            Out.WriteUInt16(0);
            Out.WriteUInt16(Info.Entry);
            Out.WriteUInt16(Caster.Oid);
            Out.WriteUInt16(Info != null ? Info.Info.EffectID : (ushort)0);
            Out.WriteUInt16(0); // Target Oid
            Out.WriteByte(1); // Ability Type

            Out.WriteByte(1);
            Out.WriteUInt32((uint)Info.Info.CastTime); // Cast Time
            Out.WriteByte(1);
            Out.WriteUInt16(0);
            Out.WriteByte(0);
            Interface._Owner.DispatchPacket(Out, true);
        }

        public void SendStop()
        {
            //Log.Info("Ability", "SendStop");

            /*{
                PacketOut Out = new PacketOut((byte)Opcodes.F_USE_ABILITY); // This Stop the bar
                Out.WriteUInt16(0);
                Out.WriteUInt16(Info != null ? Info.Entry : (ushort)0);
                Out.WriteUInt16(Interface._Owner.Oid);
                Out.WriteUInt16(Info != null ? Info.Info.EffectID : (ushort)0);
                Out.WriteUInt16(0);
                Out.WriteByte(2);

                Out.WriteByte(1);
                Out.WriteUInt32(0);
                Out.WriteByte(1);
                Out.Fill(0, 3);
                Interface._Owner.DispatchPacket(Out, true);
            }

            {
                PacketOut Out = new PacketOut((byte)Opcodes.F_USE_ABILITY); // This Stop the bar
                Out.WriteUInt16(0);
                Out.WriteUInt16(Info != null ? Info.Entry : (ushort)0);
                Out.WriteUInt16(Interface._Owner.Oid);
                Out.WriteUInt16(Info != null ? Info.Info.EffectID : (ushort)0);
                Out.WriteUInt16(0x2273);
                Out.WriteByte(2);
                Out.WriteByte(1);
                Out.WriteUInt32(0);
                Out.WriteByte(1);
                Out.WriteUInt16(0);
                Out.WriteByte(0);
                //Out.WriteHexStringBytes("227302010000000001000000");
                Interface._Owner.DispatchPacket(Out, true);
            }*/

            if (Interface._Owner.IsPlayer())
            {
                /*{
                    PacketOut Out = new PacketOut((byte)Opcodes.F_SWITCH_ATTACK_MODE);
                    Out.WriteByte(1);
                    Out.Fill(0, 3);
                    Interface._Owner.GetPlayer().SendPacket(Out);
                }*/

                /*{
                    PacketOut Out = new PacketOut((byte)Opcodes.F_SET_ABILITY_TIMER);
                    Out.WriteUInt16(Info != null ? Info.Entry : (ushort)0);
                    Out.Fill(0, 10);
                    Interface._Owner.GetPlayer().SendPacket(Out);
                }*/
            }
        }

        public void SendStartCasting(ushort TargetOID)
        {
            /*{
                PacketOut Out = new PacketOut((byte)Opcodes.F_UPDATE_STATE);
                Out.WriteUInt16(TargetOID);
                Out.WriteByte(0x1A);
                Out.Fill(0, 7);
                Caster.DispatchPacket(Out, true);
            }*/

            /*if(Info.CastTime != 0)
            {
                PacketOut Out = new PacketOut((byte)Opcodes.F_USE_ABILITY);
                Out.WriteUInt16(0);
                Out.WriteUInt16(Info.Entry);
                Out.WriteUInt16(Caster.Oid);
                Out.WriteUInt16(0x0610); // Ability ?
                Out.WriteUInt16(0); // Target Oid
                Out.WriteByte(1); // Ability Type

                Out.WriteByte(1);
                Out.WriteUInt16(0x0000);
                Out.WriteUInt16((ushort)Info.CastTime); // Cast Time
                Out.WriteByte(1);
                Out.Fill(0, 3);
                Caster.DispatchPacket(Out, true);
            }*/
        }

        // Draw Damages Text
        public void SendSpellDamage(Unit Target, uint Damage, bool Heal)
        {
            if (Caster.IsPlayer())
            {
                PacketOut Out = new PacketOut((byte)Opcodes.F_CAST_PLAYER_EFFECT);
                Out.WriteUInt16(Caster.Oid);
                Out.WriteUInt16(Target.Oid);
                Out.WriteUInt16(Info.Entry); // 00 00 07 D D
                Out.WriteByte(0);
                Out.WriteByte(0);
                Out.WriteByte(7);
                Out.WriteByte((byte)((128 + (Damage % 64) * 2) + (Heal ? 0 : 1)));
                Out.WriteByte((byte)(Damage / 64));
                Out.WriteByte(0xCE);
                Out.WriteByte(0x07);
                Caster.DispatchGroup(Out);
            }

            if (Caster != Target && Target.IsPlayer())
            {
                PacketOut Out = new PacketOut((byte)Opcodes.F_CAST_PLAYER_EFFECT);
                Out.WriteUInt16(Caster.Oid);
                Out.WriteUInt16(Target.Oid);
                Out.WriteUInt16(Info.Entry); // 00 00 07 D D
                Out.WriteByte(0);
                Out.WriteByte(0);
                Out.WriteByte(7);
                Out.WriteByte((byte)((128 + (Damage % 64) * 2) + (Heal ? 0 : 1)));
                Out.WriteByte((byte)(Damage / 64));
                Out.WriteByte(0xCE);
                Out.WriteByte(0x07);
                Target.DispatchGroup(Out);
            }
        }

        public void SendSpellEffect(Object Caster, Unit Target, UInt16 RealDamage, UInt16 Damage, Ability_Stats Ability = null)
        {
            if (Target == null)
                return;

            // Frappe
            /*{
                PacketOut Out = new PacketOut((byte)Opcodes.F_USE_ABILITY);
                Out.WriteUInt16(Target.Oid);
                Out.WriteUInt16(Ability.Entry);
                Out.WriteUInt16(Caster.Oid);
                Out.WriteUInt16(Ability.EffectID);

                Out.WriteUInt16(Target.Oid);
                Out.WriteByte(6);
                Out.WriteByte(1);
                Out.WriteUInt16(0);
                Out.WriteUInt32(0x023F0C00);
                Out.WriteUInt16(0);

                Caster.DispatchPacket(Out, true);
            }

            {
                PacketOut Out = new PacketOut((byte)Opcodes.F_CAST_PLAYER_EFFECT);
                Out.WriteUInt16(Caster.Oid);
                Out.WriteUInt16(Target.Oid);
                Out.WriteUInt16(Ability.Entry);
                Out.WriteByte(2);
                Out.WriteByte((byte)GameData.CombatEvent.COMBATEVENT_ABILITY_HIT);
                Out.WriteByte(7);
                Out.WriteUInt16(0x0799);
                Out.WriteByte(0);
                Caster.DispatchPacket(Out, true);
            }*/
        }

        public void SendAbilityDone(ushort TargetOID)
        {
            //Log.Info("SendAbilityDone", TargetOID.ToString());

            Player Plr = Caster.GetPlayer();

            {
                PacketOut Out = new PacketOut((byte)Opcodes.F_USE_ABILITY);
                Out.WriteUInt16(TargetOID);
                Out.WriteUInt16(Info.Entry);
                Out.WriteUInt16(Caster.Oid);
                Out.WriteUInt16(Info.Info.EffectID);
                Out.WriteUInt16(0x2273);
                Out.WriteByte(6);
                Out.WriteByte(1);
                Out.WriteUInt16(0);
                Out.WriteUInt16(0x01E6);
                Out.WriteUInt16(0);
                Out.WriteByte(0);
                //Out.WriteHexStringBytes("22730601000001E601000000");
                Plr.DispatchPacket(Out, true);
            }

            {
                PacketOut Out = new PacketOut((byte)Opcodes.F_USE_ABILITY); // This Stop the bar
                Out.WriteUInt16(TargetOID);
                Out.WriteUInt16(Info.Entry);
                Out.WriteUInt16(Caster.Oid);
                Out.WriteUInt16(Info.Info.EffectID);
                Out.WriteUInt16(0x2273);
                Out.WriteByte(2);
                Out.WriteByte(1);
                Out.WriteUInt16(0);
                Out.WriteUInt16(0);
                Out.WriteByte(1);
                Out.WriteUInt16(0);
                Out.WriteByte(0);
                //Out.WriteHexStringBytes("227302010000000001000000");
                Plr.DispatchPacket(Out, true);
            }

            /*{
                PacketOut Out = new PacketOut((byte)Opcodes.F_SWITCH_ATTACK_MODE);
                Out.WriteByte(1);
                Out.Fill(0, 3);
                Plr.SendPacket(Out);
            }

            {
                PacketOut Out = new PacketOut((byte)Opcodes.F_SET_ABILITY_TIMER);
                Out.WriteUInt16(Info.Entry);
                Out.Fill(0, 10);
                Plr.SendPacket(Out);
            }*/
        }

        public void SetBuff(long MSEndTime)
        {
            IsBuff = true;
            EndTime = TCPServer.GetTimeStampMS() + MSEndTime;
            InitEffect((byte)(MSEndTime / 1000));
        }

        public void Reset()
        {
            if (Handler != null)
                Handler.Reset();
        }

        public void InitEffect(byte Seconds, Player Plr = null)
        {
            PacketOut Out = new PacketOut((byte)Opcodes.F_INIT_EFFECTS);
            Out.WriteByte(1);
            Out.WriteByte(1);
            Out.WriteUInt16(0);
            Out.WriteUInt16(Interface._Owner.Oid);
            Out.WriteByte(BuffId);
            Out.WriteByte(0);
            Out.WriteUInt16R(Info.Entry); // Entry
            Out.WriteByte((byte)(Seconds * 2)); // Time * 2 ?
            Out.WriteByte(0x37);
            Out.WriteUInt16(0x81);
            Out.WriteByte(0x02);
            Out.WriteByte(01);
            Out.WriteUInt16(0xA4);
            Out.WriteByte(1);
            Out.WriteByte(00);

            if (Plr == null)
                Interface._Owner.DispatchPacket(Out, true);
            else
                Plr.SendPacket(Out);
        }

        public void SendEffect(Player Plr)
        {
            byte Time = (byte)((EndTime - TCPManager.GetTimeStampMS()) / 1000);
            InitEffect(Time, Plr);
        }

        public void RemoveEffect()
        {
            PacketOut Out = new PacketOut((byte)Opcodes.F_INIT_EFFECTS);
            Out.WriteByte(1);
            Out.WriteByte(2);
            Out.WriteUInt16(0);
            Out.WriteUInt16(Interface._Owner.Oid);
            Out.WriteByte(BuffId);
            Out.WriteByte(0);
            Out.WriteUInt16R(Info.Entry); // Entry
            Out.WriteByte(0);
            Interface._Owner.DispatchPacket(Out, true);
        }

        public int TickCount = 0;
        public long NextTick = 0;
        public virtual void OnTick(int Id)
        {
            if (Handler != null)
                Handler.OnTick(Id);

            if (Info.Info.ApSec != 0)
                Cast();
        }
    }
}

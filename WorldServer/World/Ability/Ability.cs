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
        public Ability_Info Info;
        public Object Caster;
        public IAbilityTypeHandler Handler;

        public bool IsStarted = false;
        private long StartTime = 0;

        public bool IsDone = false;
        private long DoneTime = 0;

        public Ability(Ability_Info Info, Object Caster)
        {
            this.Info = Info;
            this.Caster = Caster;
            Handler = AbilityMgr.GetAbilityHandler(Info.AbilityType);
        }

        public void Update()
        {
            Log.Info("Ability", "Update Start=" + StartTime + ",Cur=" + TCPServer.GetTimeStampMS() + ",Info=" + Info.CastTime);
            if (Handler != null)
                Handler.Update();

            if (StartTime + Info.CastTime < TCPServer.GetTimeStampMS())
                Cast();
        }

        public void Start()
        {
            Log.Info("Ability", "Start : " + Caster.Name);
            IsStarted = true;
            StartTime = TCPServer.GetTimeStampMS();

            if(Handler != null)
                Handler.Start(this);
        }

        public void Stop()
        {
            IsDone = true;

            if (Handler != null)
                Handler.Stop();
        }

        public void Cast()
        {
            Log.Info("Ability", "Cast");

            if (IsDone)
                return;

            IsDone = true;
            DoneTime = TCPServer.GetTimeStampMS();

            if (Handler != null)
            {
                GameData.AbilityResult Result = Handler.CanCast();

                if(Result == GameData.AbilityResult.ABILITYRESULT_OK)
                    Handler.Cast();
            }
        }

        public void SendStartCasting(ushort TargetOID)
        {
            PacketOut Out1 = new PacketOut((byte)Opcodes.F_UPDATE_STATE);
            Out1.WriteUInt16(TargetOID);
            Out1.WriteByte(0x1A);
            Out1.Fill(0, 7);
            Caster.DispatchPacket(Out1, true);

            if (Info.CastTime == 0)
                return;

            PacketOut Out2 = new PacketOut((byte)Opcodes.F_USE_ABILITY);
            Out2.WriteUInt16(0);
            Out2.WriteUInt16(Info.Entry);
            Out2.WriteUInt16(Caster.Oid);
            Out2.WriteByte(0x06);
            Out2.WriteByte(0x10);
            Out2.WriteUInt16(0x2273);
            Out2.WriteUInt32(0x1010000);
            Out2.WriteUInt16((ushort)this.Info.CastTime);
            Out2.WriteByte(1);
            Out2.Fill(0, 3);
            Caster.GetPlayer().DispatchPacket(Out2, true);
        }

        public void SendSpellDamage(ushort TargetOID, UInt32 Damage)
        {
            PacketOut Out = new PacketOut((byte)Opcodes.F_CAST_PLAYER_EFFECT);
            Out.WriteUInt16(Caster.Oid);
            Out.WriteUInt16(TargetOID);
            Out.WriteUInt16(Info.Entry);
            Out.WriteByte(2);
            Out.WriteUInt16(7);
            Out.WriteByte((byte)((Damage * 2) - 1));
            Out.WriteByte(1);
            Out.WriteUInt16(0x3E18);
            Caster.DispatchPacket(Out, true);
        }

        public void SendAbilityDone(ushort TargetOID)
        {
            Log.Success("Ability", "Send Done :" + Info.Entry);

            Player player = this.Caster.GetPlayer();
            PacketOut Out1 = new PacketOut((byte)Opcodes.F_USE_ABILITY);
            Out1.WriteUInt16(0);
            Out1.WriteUInt16(Info.Entry);
            Out1.WriteUInt16(Caster.Oid);
            Out1.WriteHexStringBytes("061022730601000001E601000000");
            player.DispatchPacket(Out1, true);

            PacketOut Out2 = new PacketOut((byte)Opcodes.F_USE_ABILITY);
            Out2.WriteUInt16(0);
            Out2.WriteUInt16(Info.Entry);
            Out2.WriteUInt16(Caster.Oid);
            Out2.WriteHexStringBytes("0610227302010000000001000000");
            player.DispatchPacket(Out2, true);

            PacketOut Out3 = new PacketOut((byte)Opcodes.F_SWITCH_ATTACK_MODE);
            Out3.WriteByte(1);
            Out3.Fill(0, 3);
            player.SendPacket(Out3);

            PacketOut Out4 = new PacketOut((byte)Opcodes.F_SET_ABILITY_TIMER);
            Out4.WriteUInt16(Info.Entry);
            Out4.Fill(0, 10);
            player.SendPacket(Out4);
        }
    }
}

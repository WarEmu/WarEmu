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
            PacketOut Out = new PacketOut((byte)Opcodes.F_UPDATE_STATE);
            Out.WriteUInt16(TargetOID);
            Out.WriteByte(0x1A);
            Out.Fill(0, 7);
            Caster.DispatchPacket(Out, true);
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
            // ???
        }
    }
}

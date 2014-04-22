using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common;
using FrameWork;

namespace WorldServer
{
    public class Mount
    {
        public Unit Owner;
        public Mount_Info CurrentMountInfo;

        public Mount(Unit Owner)
        {
            this.Owner = Owner;
            Owner.EvtInterface.AddEventNotify(EventName.ON_RECEIVE_DAMAGE, this.OnTakeDamage);
            Owner.EvtInterface.AddEventNotify(EventName.ON_DEAL_DAMAGE, this.OnDealDamage);
            Owner.EvtInterface.AddEventNotify(EventName.ON_START_CASTING, this.OnStartCast);
        }

        public void Stop()
        {

        }

        public bool IsMount()
        {
            return CurrentMountInfo != null;
        }

        public void SetMount(uint Id)
        {
            Mount_Info Info = WorldMgr.GetMount(Id);
            SetMount(Info);
        }

        public void SetMount(Mount_Info Info)
        {
            UnMount();
            if (Info == null)
                return;

            CurrentMountInfo = Info;
            Owner.StsInterface.AddBonusSpeed(CurrentMountInfo.Speed);
            SendMount(null);
        }

        public void UnMount()
        {
            if (CurrentMountInfo == null)
                return;

            Owner.StsInterface.RemoveBonusSpeed(CurrentMountInfo.Speed);
            CurrentMountInfo = null;
            SendMount(null);
        }

        public void SendMount(Player Plr)
        {
            PacketOut Out = new PacketOut((byte)Opcodes.F_MOUNT_UPDATE);
            Out.WriteUInt16(Owner.Oid);

            if (CurrentMountInfo == null)
                Out.WriteUInt32(0);
            else
                Out.WriteUInt32(CurrentMountInfo.Entry);

            Out.Fill(0, 14);

            if (Plr == null)
                Owner.DispatchPacket(Out, true);
            else
                Plr.SendPacket(Out);
        }

        public bool OnStartCast(Object Obj, object Args)
        {
            if ((Args as Ability).Info.Entry == 245)
                return false;

            UnMount();
            return false;
        }

        public bool OnTakeDamage(Object Obj, object Args)
        {
            UnMount();
            return false;
        }

        public bool OnDealDamage(Object Obj, object Args)
        {
            UnMount();
            return false;
        }
    }
}

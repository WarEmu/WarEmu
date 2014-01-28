
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common;
using FrameWork;

namespace WorldServer
{
    public class AbilityInterface : BaseInterface
    {
        static public long GlobalMSCoolDown = 1500;

        public List<Ability_Info> Abilities = new List<Ability_Info>();
        public Ability CurrentAbility = null;
        public long LastCast = 0;

        public AbilityInterface(Object Owner)
            : base(Owner)
        {

        }

        public override bool Load()
        {
            UpdateAbilities();
            return base.Load();
        }

        public void UpdateAbilities()
        {
            if (HasPlayer())
                Abilities = AbilityMgr.GetCareerAbility(GetPlayer()._Info.CareerLine, GetPlayer().Level);
        }

        public bool HasAbility(ushort AbilityID)
        {
            return GetAbility(AbilityID) != null;
        }

        public Ability_Info GetAbility(ushort AbilityID)
        {
            return Abilities.Find(info => info.Entry == AbilityID);
        }

        public void SendAbilities()
        {
            if (!HasPlayer())
                return;

            Log.Info("AbilityInterface", "Sending " + Abilities.Count + " Abilities");

            PacketOut Out = new PacketOut((byte)Opcodes.F_CHARACTER_INFO);
            Out.WriteByte(1); // Action
            Out.WriteByte((byte)Abilities.Count);
            Out.WriteUInt16(0x300);

            foreach (Ability_Info Info in Abilities)
            {
                Out.WriteUInt16(Info.Entry);
                Out.WriteByte(Info.Level);
            }

            GetPlayer().SendPacket(Out);

            PacketOut AutoAttack = new PacketOut((byte)Opcodes.F_CHARACTER_INFO);
            AutoAttack.WriteByte(1); // Action
            AutoAttack.WriteByte(1); // Count
            AutoAttack.WriteUInt16(0x300);
            AutoAttack.WriteUInt16(245);
            AutoAttack.WriteByte(1);
            GetPlayer().SendPacket(AutoAttack);

        }

        public bool IsCasting()
        {
            return CurrentAbility != null;
        }

        public override void Update(long Tick)
        {
            if (CurrentAbility != null)
            {
                if (CurrentAbility.IsStarted)
                    CurrentAbility.Update();

                if (CurrentAbility.IsDone)
                    CurrentAbility = null;
            }

            base.Update(Tick);
        }

        public void StartCast(ushort AbilityID)
        {
            Ability_Info Info = GetAbility(AbilityID);
            if (Info == null)
                return;

            if (IsCasting() && CurrentAbility.Info == Info)
                return;

            Log.Info("AbilityInterface","StartCast : " + AbilityID);

            if (CurrentAbility != null)
            {
                CurrentAbility.Stop();
                CurrentAbility = null;
            }

            GameData.AbilityResult Result = CanCast(Info);

            if (Result == GameData.AbilityResult.ABILITYRESULT_OK)
            {
                LastCast = TCPServer.GetTimeStampMS();

                CurrentAbility = new Ability(Info, Obj);
                CurrentAbility.Start();

                if (CurrentAbility.Handler != null)
                    Result = CurrentAbility.Handler.CanCast();

                Log.Info("Cast", "CastResult = " + Result);

                if (CurrentAbility.Handler == null || Result == GameData.AbilityResult.ABILITYRESULT_OK)
                {
                    Obj.GetUnit().ActionPoints -= Info.ActionPoints;
                    if (Obj.IsPlayer())
                        Obj.GetPlayer().SendHealh();
                }
                else
                    CurrentAbility.Stop();
            }
        }

        public GameData.AbilityResult CanCast(Ability_Info Info)
        {
            if (LastCast + GlobalMSCoolDown > TCPServer.GetTimeStampMS())
                return GameData.AbilityResult.ABILITYRESULT_NOTREADY;
            else if (Info.ActionPoints > GetPlayer().ActionPoints)
                return GameData.AbilityResult.ABILITYRESULT_AP;

            return GameData.AbilityResult.ABILITYRESULT_OK;
        }

        public void StopCast()
        {
            if (CurrentAbility != null)
                CurrentAbility.Stop();
        }
    }
}

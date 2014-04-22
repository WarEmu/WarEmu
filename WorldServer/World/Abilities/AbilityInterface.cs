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
    public class AbilityInterface : BaseInterface
    {
        static public ushort GlobalMSCoolDown = 1500;

        public Dictionary<UInt16, Ability_Stats> Abilities = new Dictionary<UInt16, Ability_Stats>();
        public Ability CurrentAbility = null;

        public override bool Load()
        {
            UpdateAbilities();
            _Owner.EvtInterface.AddEventNotify(EventName.ON_MOVE, OnMove);
            _Owner.EvtInterface.AddEventNotify(EventName.ON_LEVEL_UP, OnLevelUp);
            return base.Load();
        }

        public bool OnLevelUp(object Obj, object Args)
        {
            UpdateAbilities();
            SendAbilities();
            return false;
        }

        public bool OnMove(Object Sender, object Args)
        {
            if(IsCasting() && CurrentAbility.Info.Info.CastTime > 1000)
                Cancel(false);
            return false;
        }

        public bool IsValidAbility(Ability_Info Info)
        {
            if (!HasPlayer())
                return true;

            Player Plr = GetPlayer();
            if (Info.MinimumRank > Plr.Level)
            {
                //Log.Info(Info.Name, "Rank :" + Info.MinimumRank + ">" + Plr.Level);
                return false;
            }

            if (Info.MinimumRenown > Plr.Renown)
            {
                //Log.Info(Info.Name, "Renown :" + Info.MinimumRenown + ">" + Plr.Renown);
                return false;
            }

            if (Info.MinimumCategoryPoints != 0)
                return false;

            if (Info.PointCost != 0)
                return false;

            return true;
        }

        public void UpdateAbilities()
        {
            if (HasPlayer())
            {
                List<Ability_Info> Ab = AbilityMgr.GetCareerAbility(GetPlayer()._Info.CareerLine);
                if (Ab != null)
                {
                    //Log.Info("Abilities", "Selected :" + Ab.Count);

                    foreach (Ability_Info Info in Ab)
                    {
                        if (IsValidAbility(Info))
                        {
                            byte LastLevel = 0;
                            foreach (Ability_Stats Stat in Info.Stats)
                            {
                                if (Stat.Level <= GetPlayer().Level && Stat.Level > LastLevel)
                                {
                                    LastLevel = Stat.Level;

                                    if (!Abilities.ContainsKey(Stat.Entry))
                                        Abilities.Add(Stat.Entry, Stat);
                                    else
                                        Abilities[Stat.Entry] = Stat;
                                }
                            }
                        }
                    }
                }
            }
        }

        public bool HasAbility(ushort AbilityID)
        {
            return GetAbility(AbilityID) != null;
        }

        public Ability_Stats GetAbility(ushort AbilityID)
        {
            Ability_Stats Stat;
            Abilities.TryGetValue(AbilityID, out Stat);
            return Stat;
        }

        public void SendAbilities()
        {
            if (!HasPlayer())
                return;

            //Log.Info("AbilityInterface", "Sending " + Abilities.Count + " Abilities");

            PacketOut Out = new PacketOut((byte)Opcodes.F_CHARACTER_INFO);
            Out.WriteByte(1); // Action
            Out.WriteByte((byte)Abilities.Count);
            Out.WriteUInt16(0x300);

            foreach (KeyValuePair<UInt16,Ability_Stats> Kp in Abilities)
            {
                Out.WriteUInt16(Kp.Value.Entry);
                Out.WriteByte(Kp.Value.Level);
            }

            GetPlayer().SendPacket(Out);

            /*PacketOut AutoAttack = new PacketOut((byte)Opcodes.F_CHARACTER_INFO);
            AutoAttack.WriteByte(1); // Action
            AutoAttack.WriteByte(1); // Count
            AutoAttack.WriteUInt16(0x300);
            AutoAttack.WriteUInt16(245);
            AutoAttack.WriteByte(1);
            GetPlayer().SendPacket(AutoAttack);*/

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
                    CurrentAbility.Update(Tick);
            }

            base.Update(Tick);

            if (CurrentAbility != null && CurrentAbility.IsDone)
            {
                CurrentAbility = null;
            }

            UpdateBuff(Tick);
        }

        public void StartCast(ushort AbilityID)
        {
            StartCast(AbilityID, (ushort)_Owner.X, (ushort)_Owner.Y, (ushort)_Owner.Z, _Owner.Zone.ZoneId);
        }

        public void StartCast(ushort AbilityID, ushort Px, ushort Py,ushort Pz, ushort ZoneId)
        {
            Ability_Stats Info = GetAbility(AbilityID);

            if (Info == null)
                return;

            if (IsCasting() && CurrentAbility.Info == Info)
                return;

            Cancel(false);

            Cast(Info, Px, Py, Pz, ZoneId);
        }

        public void Cast(Ability_Stats Info, ushort Px, ushort Py,ushort Pz, ushort ZoneId)
        {
            GameData.AbilityResult Result = CanCast(Info, true);

            if (Result == GameData.AbilityResult.ABILITYRESULT_OK)
            {
                Ability NewAbility = new Ability(this, null, Info, _Owner.GetUnit(),false, Px, Py, Pz, ZoneId);

                if (NewAbility.Handler != null)
                    Result = NewAbility.Handler.CanCast(true);

                if (NewAbility.Handler == null || Result == GameData.AbilityResult.ABILITYRESULT_OK)
                {
                    //Log.Info("Cast", Info.Entry.ToString() + ":" + Info.Description + " : " + Info.Damages1);
                    SetCooldown(0, GlobalMSCoolDown);
                    CurrentAbility = NewAbility;
                    CurrentAbility.Start();
                    SetCooldown(Info.Entry, Info.Info.Cooldown * 1000);
                }
                else
                    Cancel(false);
            }
            else
                Cancel(false);
        }

        public void Cancel(bool Force)
        {
            if (CurrentAbility != null)
            {
                Ability Ab = CurrentAbility;
                CurrentAbility = null;
                Ab.Cancel();
            }
        }

        public GameData.AbilityResult CanCast(Ability_Stats Info, bool CheckCoolDown)
        {
            if (Info.Info.ApCost > GetPlayer().ActionPoints)
                return GameData.AbilityResult.ABILITYRESULT_AP;
            else if (CheckCoolDown && !CanCastCooldown(0))
                return GameData.AbilityResult.ABILITYRESULT_NOTREADY;
            else if (CheckCoolDown && !CanCastCooldown(Info.Entry))
                return GameData.AbilityResult.ABILITYRESULT_NOTREADY;

            return GameData.AbilityResult.ABILITYRESULT_OK;
        }

        #region Cooldowns

        public Dictionary<ushort, long> Cooldowns = new Dictionary<ushort, long>();

        public void SetCooldown(ushort Id, long Time)
        {
            if (!Cooldowns.ContainsKey(Id))
                Cooldowns.Add(Id, Time + TCPManager.GetTimeStampMS());
            else
                Cooldowns[Id] = Time + TCPManager.GetTimeStampMS();
        }

        public bool CanCastCooldown(ushort Id)
        {
            long Time;
            if (!Cooldowns.TryGetValue(Id, out Time))
                return true;

            return Time <= TCPManager.GetTimeStampMS();
        }

        public bool IsOnGlobalCooldown()
        {
            return !CanCastCooldown(0);
        }

        #endregion

        #region Buff

        public List<Ability> ActiveBuff = new List<Ability>();

        public void UpdateBuff(long Tick)
        {
            Ability Buff;
            for (int i = 0; i < ActiveBuff.Count; ++i)
            {
                Buff = ActiveBuff[i];
                if (Buff == null)
                    continue;

                Buff.Update(Tick);
                if (Buff.IsDone)
                {
                    Buff.RemoveEffect();
                    ActiveBuff[i] = null;
                }
            }
        }

        public Ability GetBuff(ushort Entry)
        {
            foreach (Ability Buff in ActiveBuff)
                if (Buff != null && Buff.Info.Entry == Entry)
                    return Buff;
            return null;
        }

        public IAbilityTypeHandler AddBuff(Ability Parent, string NewHandler)
        {
            Ability Current = GetBuff(Parent.Info.Entry);
            if (Current != null && Current.Info.Level == Parent.Info.Level)
            {
                Current.Reset();
                return Current.Handler;
            }
            else if (Current != null && Current.Info.Level > Parent.Info.Level)
                return null;
            else
            {
                if (Current != null && Current.Info.Level < Parent.Info.Level)
                    Current.Stop();

                byte Id = (byte)GetBuffId();
                Ability Ab = new Ability(this, Parent, Parent.Info, Parent.Caster, true, NewHandler);
                Ab.BuffId = (byte)(Id + 1);
                Ab.Start();
                Ab.Reset();

                if (Id >= ActiveBuff.Count)
                    ActiveBuff.Add(Ab);
                else
                    ActiveBuff[Id] = Ab;

                Log.Info("AddBuff", _Owner.Name + "," + Ab.Handler);
                return Ab.Handler;
            }
        }

        public void SendAllBuff(Player Plr)
        {
            foreach (Ability Ab in ActiveBuff)
                if (Ab != null)
                    Ab.SendEffect(Plr);
        }

        public int GetBuffId()
        {
            for (int i = 0; i < ActiveBuff.Count; ++i)
                if (ActiveBuff[i] == null)
                    return i;

            return ActiveBuff.Count;
        }

        #endregion

        #region Events

        public void OnReceiveDamages(Unit Attacker, Ability Spell, ref uint Damages)
        {
            if(CurrentAbility != null && CurrentAbility.Handler != null)
                CurrentAbility.Handler.OnReceiveDamages(Attacker,Spell,ref Damages);

            foreach (Ability Ab in ActiveBuff)
                if (Ab != null && Ab.Handler != null)
                    Ab.Handler.OnReceiveDamages(Attacker, Spell, ref Damages);
        }

        public void OnReceiveHeal(Unit Attacker, Ability Spell, ref uint Damages)
        {
            if (CurrentAbility != null && CurrentAbility.Handler != null)
                CurrentAbility.Handler.OnReceiveHeal(Attacker, Spell, ref Damages);

            foreach (Ability Ab in ActiveBuff)
                if (Ab != null && Ab.Handler != null)
                    Ab.Handler.OnReceiveHeal(Attacker, Spell, ref Damages);
        }

        public void OnDealDamages(Unit Victim, Ability Spell, ref uint Damages)
        {
            if (CurrentAbility != null && CurrentAbility.Handler != null)
                CurrentAbility.Handler.OnDealDamages(Victim, Spell, ref Damages);

            foreach (Ability Ab in ActiveBuff)
                if (Ab != null && Ab.Handler != null)
                    Ab.Handler.OnDealDamages(Victim, Spell, ref Damages);
        }

        public void OnDealHeals(Unit Victim, Ability Spell, ref uint Damages)
        {
            if (CurrentAbility != null && CurrentAbility.Handler != null)
                CurrentAbility.Handler.OnDealHeals(Victim, Spell, ref Damages);

            foreach (Ability Ab in ActiveBuff)
                if (Ab != null && Ab.Handler != null)
                    Ab.Handler.OnDealHeals(Victim, Spell, ref Damages);
        }

        #endregion
    }
}

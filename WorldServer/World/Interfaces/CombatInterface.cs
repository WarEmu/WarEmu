
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Common;
using FrameWork;

namespace WorldServer
{
    public class AggroInfo
    {
        public AggroInfo(UInt16 Oid, bool IsPlayer)
        {
            this.Oid = Oid;
            this.IsPlayer = IsPlayer;
        }

        public UInt16 Oid;
        public bool IsPlayer;

        public UInt64 DamagesDeal;
        public UInt64 DamagesReceive;

        public UInt64 HealsDeal;
        public UInt64 HealsReceive;

        public float GetHate(UInt64 TotalDamage)
        {
            if (TotalDamage <= 0)
                return 0;

            float Hate = 0.0f;

            Hate = (float)((float)DamagesDeal / (float)TotalDamage) * 100.0f;

            return Hate;
        }

        public void ResetHeals()
        {
            HealsDeal = 0;
            HealsReceive = 0;
        }
        public void ResetDamages()
        {
            DamagesDeal = 0;
            DamagesReceive = 0;
        }
        public void Reset()
        {
            ResetDamages();
            ResetHeals();
        }
    }

    public enum AiState
    {
        STANDING = 0,
        MOVING = 1,
        FIGHTING = 2,
        BACK = 3,
    }

    public class CombatInterface : BaseInterface
    {
        public AIInterface AI;

        public bool Attacking = false;
        public long NextAttackTime = 0;
        public long FightingStateTime = 0;
        public ushort[] Targets = new ushort[7];

        public override bool Load()
        {
            AI = _Owner.GetUnit().AiInterface;
            ClearTargets();
            return base.Load();
        }

        public void ResetFightingState(long Tick)
        {
            if (FightingStateTime == 0)
                _Owner.EvtInterface.Notify(EventName.ON_ENTER_COMBAT, _Owner, null);
 
            FightingStateTime = Tick + 2000; // 2 Sec
        }

        public override void Update(long Tick)
        {
            if (FightingStateTime != 0 && FightingStateTime < Tick)
            {
                FightingStateTime = 0;
                _Owner.EvtInterface.Notify(EventName.ON_LEAVE_COMBAT, _Owner, null);
            }

            if (!HasUnit())
                return;

            Unit Me = GetUnit();
            Unit Target = GetNextTarget();

            if (HasPlayer())
            {
                if (Target == null)
                    return;

                if (!CanAttack(Me, Target))
                    return;

                if (Attacking)
                {
                    if (NextAttackTime < Tick)
                    {
                        Player Plr = _Owner.GetPlayer();
                        if (Plr.AbtInterface.IsCasting())
                        {
                            NextAttackTime += 100;
                        }
                        else
                        {
                            NextAttackTime += 100;

                            if (!Plr.IsObjectInFront(Target, 110))
                                return;

                            EquipSlot SlotToUse = EquipSlot.NONE;

                            if(Plr.ItmInterface.GetItemInSlot((ushort)EquipSlot.MAIN_DROITE) == null 
                                && Plr.ItmInterface.GetItemInSlot((ushort)EquipSlot.MAIN_GAUCHE) == null
                                && Plr.ItmInterface.GetItemInSlot((ushort)EquipSlot.ARME_DISTANCE) == null)
                            {
                                if(Plr.GetDistance(Target) < 5)
                                {
                                    Me.Strike(Target);
                                    NextAttackTime = Tick + Me.ItmInterface.GetAttackTime(EquipSlot.MAIN_DROITE) * 10;
                                    return;
                                }
                            }

                            if (Plr.GetDistance(Target) < 5) // Melee
                            {
                                if (!Plr.ItmInterface.CanUseItem((ushort)EquipSlot.MAIN_DROITE, Tick))
                                {
                                    if (Plr.ItmInterface.CanUseItem((ushort)EquipSlot.MAIN_GAUCHE, Tick))
                                        SlotToUse = EquipSlot.MAIN_GAUCHE;
                                    else
                                        SlotToUse = EquipSlot.NONE;
                                }
                                else
                                    SlotToUse = EquipSlot.MAIN_DROITE;
                            }
                            else if(Plr.ItmInterface.CanUseItem((ushort)EquipSlot.ARME_DISTANCE, Tick) && Plr.GetDistance(Target) < 90)// Ranged
                            {
                                SlotToUse = EquipSlot.ARME_DISTANCE;
                            }
                            
                            if(SlotToUse != EquipSlot.NONE)
                            {
                                ushort Time = Me.ItmInterface.GetAttackTime(SlotToUse);
                                Me.Strike(Target, SlotToUse);
                                NextAttackTime = Tick + Time * 5;
                                Me.ItmInterface.SetCooldown((ushort)SlotToUse, Time * 10);
                            }
                        }
                    }
                }

                DisablePvp(Tick, false);
            }
            else
                AttackAI(Tick, Target);
        }

        public void AttackAI(long Tick, Unit Target)
        {
            if (Target == null || Target.IsDead || _Owner.GetUnit().IsDead)
            {
                if (AI.State == AiState.FIGHTING)
                    CombatStop();

                return;
            }

            if (AI.State == AiState.FIGHTING)
            {
                SetTarget(Target.Oid, GameData.TargetTypes.TARGETTYPES_TARGET_ENEMY);
                LookAt(Target);

                if (NextAttackTime < Tick)
                {
                    if (_Owner.IsWithinRadius(Target, 5))
                    {
                        _Owner.GetUnit().Strike(Target);
                        NextAttackTime = Tick + 2000;
                    }
                }
            }
        }

        public void LookAt(Unit Target)
        {
            if (HasPlayer())
                return;

            if (!_Owner.IsCreature())
                return;

            if (CheckSpawnRange())
                return;

            Creature Crea = _Owner.GetCreature();
            Crea.MvtInterface.FollowUnit(Target, 7, 3, 100, eFormationType.Protect);
        }

        #region IASystem

        public bool IsFighting()
        {
            if (_Owner.IsPlayer())
            {
                if (FightingStateTime != 0)
                    return true;
            }
            else
            {
                if (AI.State == AiState.FIGHTING || FightingStateTime != 0)
                    return true;
            }

            return false;
        }

        public bool IsFightingWithFriend(Unit Me)
        {
            if (!HasTarget(GameData.TargetTypes.TARGETTYPES_TARGET_ENEMY))
                return false;

            if (!IsFighting())
                return false;

            if (IsFriend(Me, GetTarget(GameData.TargetTypes.TARGETTYPES_TARGET_ENEMY)))
                return true;
            else
                return false;
        }

        public void CombatStart(Unit Fighter)
        {
            //Log.Success("CombatStart", Obj.Name + " Start combat with " + Fighter.Name);
            AI.State = AiState.FIGHTING;
            GetAggro(Fighter.Oid, Fighter.IsPlayer()).DamagesReceive += 100;
        }

        public void CombatStop()
        {
            //Log.Success("CombatStop", Obj.Name + " Stop combat ");
            AI.State = AiState.STANDING;
            ClearTargets();

            if (_Owner.IsCreature())
                _Owner.GetCreature().MvtInterface.StopFollow();

            if (_Owner.IsCreature() && !_Owner.GetCreature().IsDead)
                ReturnToSpawn();

            _Owner.EvtInterface.Notify(EventName.ON_LEAVE_COMBAT, null, null);
        }

        public void Evade()
        {
            if (_Owner == null || _Aggros == null)
                return;

            Object Target;
            AggroInfo Info;
            foreach (KeyValuePair<ushort,AggroInfo> Kp in _Aggros.ToArray())
            {
                Info = Kp.Value;
                Target = _Owner.Zone.GetObject(Info.Oid);
                if (Target == null || !Target.IsUnit())
                   continue;

                Target.GetUnit().CbtInterface.RemoveAggro(_Owner.Oid);
            }

            CombatStop();
        }

        public bool CheckSpawnRange()
        {
            if (!_Owner.IsCreature())
                return false;

            Creature Crea = _Owner.GetCreature();

            if (Crea.GetDistance(Crea.SpawnPoint) > 200)
            {
                Evade();
                ReturnToSpawn();
                return true;
            }

            return false;
        }
        public void ReturnToSpawn()
        {
            if (!_Owner.IsCreature())
                return;

            Creature Crea = _Owner.GetCreature();
            Crea.MvtInterface.CancelWalkTo();
            Crea.SetPosition((UInt16)Crea.SpawnPoint.X, (UInt16)Crea.SpawnPoint.Y, (UInt16)Crea.SpawnPoint.Z, Crea.SpawnHeading, true);
        }

        #endregion

        #region AggroSystem

        public Dictionary<UInt16, AggroInfo> _Aggros = new Dictionary<ushort, AggroInfo>();

        public AggroInfo GetAggro(UInt16 Oid, bool IsPlayer, bool Create = true)
        {
            AggroInfo Info = null;
            if (!_Aggros.TryGetValue(Oid, out Info) && Create)
            {
                Info = new AggroInfo(Oid, IsPlayer);
                _Aggros.Add(Oid, Info);
            }

            return Info;
        }
        public void RemoveAggro(UInt16 Oid)
        {
            _Aggros.Remove(Oid);
        }
        public UInt32 GetAggroCount()
        {
            return (UInt32)_Aggros.Count;
        }

        public AggroInfo GetMaxAggroHate(bool Player)
        {
            UInt64 TotalDamages = TotalDamageReceive();
            AggroInfo Aggro = null;
            float MaxHate = -1;

            AggroInfo Info;
            foreach (KeyValuePair<ushort, AggroInfo> Kp in _Aggros)
            {
                Info = Kp.Value;
                float Hate = Info.GetHate(TotalDamages);

                if (Hate > MaxHate && Info.IsPlayer == Player)
                {
                    MaxHate = Hate;
                    Aggro = Info;
                }
            }

            return Aggro;
        }

        public Player GetMaxAggroPlayerHate()
        {
            UInt64 TotalDamages = TotalDamageReceive();
            AggroInfo Aggro = null;
            float MaxHate = -1;

            AggroInfo Info;
            foreach (KeyValuePair<ushort, AggroInfo> Kp in _Aggros)
            {
                Info = Kp.Value;
                if (!Info.IsPlayer)
                    continue;

                float Hate = Info.GetHate(TotalDamages);

                if (Hate > MaxHate)
                {
                    MaxHate = Hate;
                    Aggro = Info;
                }
            }

            if (Aggro != null)
                return _Owner.Region.GetPlayer(Aggro.Oid);

            return null;
        }

        public UInt64 TotalDamageReceive()
        {
            UInt64 Damages = 0;

            AggroInfo Info;
            foreach (KeyValuePair<ushort, AggroInfo> Kp in _Aggros)
            {
                Info = Kp.Value;
                Damages += Info.DamagesDeal;
            }

            return Damages;
        }

        public void ClearTargets()
        {
            _Aggros.Clear();
            Targets[0] = 0;
            Targets[1] = 0;
            Targets[2] = 0;
            Targets[3] = 0;
            Targets[4] = 0;
            Targets[5] = 0;
            Targets[6] = 0;
        }
        public void AddDamageReceive(UInt16 Oid, bool IsPlayer, UInt32 Damages)
        {
            AggroInfo Info = GetAggro(Oid, IsPlayer);
            Info.DamagesDeal = Damages;
        }
        public void AddHealReceive(UInt16 Oid, bool IsPlayer, UInt32 Count)
        {
            AggroInfo Info = GetAggro(Oid, IsPlayer);
            Info.HealsReceive = Count;
        }

        public Unit GetNextTarget()
        {
            Unit Me = GetUnit();
            Unit Target = null;

            if (!Me.IsPlayer())
            {
                AggroInfo Info = GetMaxAggroHate(true);
                if (Info == null)
                    Info = GetMaxAggroHate(false);

                if(Info != null)
                    Target = _Owner.Region.GetObject(Info.Oid) as Unit;

            }
            else
            {
                Target = GetTarget(GameData.TargetTypes.TARGETTYPES_TARGET_ENEMY);
            }

            if (Target == null)
                return null;

            if (!Target.IsInWorld())
                return null;

            if (Target.IsDead)
            {
                if (AI.State == AiState.FIGHTING)
                    OnTargetDie(Target);

                return null;
            }

            return Target;
        }

        #endregion

        #region Targets

        /*public TargetInfo FriendlyTarget = new TargetInfo(null, GameData.TargetTypes.TARGETTYPES_TARGET_NONE);
        public TargetInfo EnemyTarget = new TargetInfo(null, GameData.TargetTypes.TARGETTYPES_TARGET_NONE);

        public void SetTarget(ushort Oid)
        {
            if (CurrentTarget.Target != Target)
            {
                CurrentTarget.Target = Target;
                CurrentTarget.Type = GetTargetType(GetUnit(), Target);
                Attacking = false;
            }
        }*/

        public void SetTarget(ushort Oid, GameData.TargetTypes Type)
        {
            Targets[(int)Type] = Oid;
            if (Oid != 0 && HasPlayer())

            {
                Object Obj = _Owner.Zone.GetObject(Oid);
                if (Obj != null && Obj.IsUnit())
                    Obj.GetUnit().AbtInterface.SendAllBuff(_Owner.GetPlayer());
            }
         }

        public bool HasTarget(GameData.TargetTypes Type)
        {
            return Targets[(int)Type] != 0;
        }
        public Unit GetTarget(GameData.TargetTypes Type)
        {
            Unit U = null;
            ushort Oid = Targets[(int)Type];
            if (Oid != 0)
                U = _Owner.Region.GetObject(Oid) as Unit;
            return U;
        }

        public Unit GetCurrentTarget()
        {
            Unit U = null;
            ushort Oid;

            for (int i = 0; i < 7; ++i)
            {
                Oid = Targets[(int)i];
                if (Oid != 0)
                {
                    U = _Owner.Region.GetObject(Oid) as Unit;

                    if (U != null)
                        return U;
                }
            }

        return U;
        }

        #endregion

        #region Events

        public void OnTakeDamage(Unit Fighter, UInt32 DamageCount)
        {
            switch (AI.State)
            {
                case AiState.STANDING:
                    CombatStart(Fighter);
                    break;
                case AiState.MOVING:
                    CombatStart(Fighter);
                    break;
            };

            ResetFightingState(TCPManager.GetTimeStampMS());
            AddDamageReceive(Fighter.Oid, Fighter.IsPlayer(), DamageCount);

            _Owner.EvtInterface.Notify(EventName.ON_RECEIVE_DAMAGE, Fighter, null);

            if (_Owner.IsPlayer() && Fighter.IsPlayer())
                ResetPvpTime();
        }

        public void OnDealDamage(Unit Victim, UInt32 DamageCount)
        {
            switch (AI.State)
            {
                case AiState.STANDING:
                    CombatStart(Victim);
                    break;
                case AiState.MOVING:
                    CombatStart(Victim);
                    break;
            };
            
            ResetFightingState(TCPManager.GetTimeStampMS());
            _Owner.EvtInterface.Notify(EventName.ON_DEAL_DAMAGE, Victim, null);

            if (_Owner.IsPlayer() && Victim.IsPlayer())
                ResetPvpTime();
        }

        public void OnDealHeal(Unit Target, UInt32 DamageCount)
        {
            ResetFightingState(TCPManager.GetTimeStampMS());
            _Owner.EvtInterface.Notify(EventName.ON_DEAL_HEAL, Target, null);

            if (_Owner.IsPlayer() && Target.IsPlayer() && Target.GetPlayer().CbtInterface.IsPvp)
                ResetPvpTime();
        }

        public void OnTakeHeal(Unit Caster, UInt32 DamageCount)
        {
            ResetFightingState(TCPManager.GetTimeStampMS());
            AddHealReceive(Caster.Oid, Caster.IsPlayer(), DamageCount);
            _Owner.EvtInterface.Notify(EventName.ON_RECEIVE_HEAL, Caster, null);

            if (_Owner.IsPlayer() && Caster.IsPlayer())
                ResetPvpTime();
        }

        public void OnTargetDie(Unit Victim)
        {
            RemoveAggro(Victim.Oid);

            if (GetAggroCount() <= 0)
                CombatStop();

            _Owner.EvtInterface.Notify(EventName.ON_TARGET_DIE, Victim, null);
        }

        #endregion

        #region PVP

        public bool IsPvp = false;
        public long NextAllowedDisable = 0;

        public void TurnPvp()
        {
            if (IsPvp)
            {
                if (NextAllowedDisable == 0)
                    ResetPvpTime();

                _Owner.GetPlayer().SendLocalizeString("", GameData.Localized_text.TEXT_RVR_UNFLAG);
            }
            else
                EnablePvp();
        }

        public void EnablePvp()
        {
            if (IsPvp)
                return;

            NextAllowedDisable = 0;
            IsPvp = true;
            _Owner.GetPlayer().SetPVPFlag(true);
            _Owner.GetPlayer().SendLocalizeString("", GameData.Localized_text.TEXT_RVR_FLAG);
            _Owner.GetPlayer().SendLocalizeString("", GameData.Localized_text.TEXT_YOU_ARE_NOW_RVR_FLAGGED);
        }

        public void DisablePvp(long Tick, bool Force)
        {
            if (!IsPvp || NextAllowedDisable == 0)
                return;

            if (Force || NextAllowedDisable < Tick)
            {
                NextAllowedDisable = 0;
                IsPvp = false;
                _Owner.GetPlayer().SetPVPFlag(false);
                _Owner.GetPlayer().SendLocalizeString("", GameData.Localized_text.TEXT_YOU_ARE_NO_LONGER_RVR_FLAGGED);
            }
        }

        public void ResetPvpTime()
        {
            EnablePvp();

            if (IsPvp)
            {
                NextAllowedDisable = TCPManager.GetTimeStampMS() + 60000 * 10;
            }
        }

        #endregion

        static public GameData.TargetTypes GetTargetType(Unit A, Unit B)
        {
            if (B == null)
                return GameData.TargetTypes.TARGETTYPES_TARGET_NONE;

            if (A == B)
                return GameData.TargetTypes.TARGETTYPES_TARGET_SELF;

            if (IsEnemy(A, B))
                return GameData.TargetTypes.TARGETTYPES_TARGET_ENEMY;

            if (IsFriend(A, B))
                return GameData.TargetTypes.TARGETTYPES_TARGET_ALLY;

            return GameData.TargetTypes.TARGETTYPES_TARGET_NONE;
        }
        static public bool IsEnemy(Unit A, Unit B)
        {            
            if (A == null || B == null)
                return false;

            if (A.Realm == GameData.Realms.REALMS_REALM_NEUTRAL && B.Realm == GameData.Realms.REALMS_REALM_NEUTRAL)
                return false;

            return A.Realm != B.Realm;
        }
        static public bool IsFriend(Unit A, Unit B)
        {
            return A.Realm == B.Realm;
        }

        static public bool CanAttack(Unit A, Unit Victim)
        {
            /*if (A.IsCreature() && B.IsCreature())
                return false;*/

            if (A == null || Victim == null)
                return false;

            if (A.IsDisposed || Victim.IsDisposed)
                return false;

            if (A == Victim)
                return false;

            if (A.IsDead || Victim.IsDead)
                return false;

            if (!IsEnemy(A, Victim))
                return false;

            if (!A.IsInWorld() || !Victim.IsInWorld())
                return false;

            if (!A.IsVisible || A.IsInvinsible)
                return false;

            if (!Victim.IsVisible || Victim.IsInvinsible)
                return false;

            if (A.IsPlayer() && Victim.IsPlayer())
            {
                if (!Victim.CbtInterface.IsPvp)
                    return false;
            }

            return true;
        }
    }
}

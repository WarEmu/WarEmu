
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
        public AggroInfo(UInt16 Oid)
        {
            this.Oid = Oid;
        }

        public UInt16 Oid;

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

    public class TargetInfo
    {
        public TargetInfo(Unit Target, GameData.TargetTypes Type)
        {
            this.Target = Target;
            this.Type = Type;
        }

        public Unit Target;
        public GameData.TargetTypes Type;
    };

    public enum AiState
    {
        STANDING = 0,
        MOVING = 1,
        FIGHTING = 2,
        BACK = 3,
    }

    public class CombatInterface : BaseInterface
    {
        public bool Attacking = false;
        public long NextAttackTime = 0;

        public AiState State = AiState.STANDING;

        public CombatInterface(Object Obj)
            : base(Obj)
        {

        }

        public override void Update(long Tick)
        {
            if (!HasUnit())
                return;

            Unit Me = GetUnit();
            Unit Target = GetNextTarget();

            if (Target == null)
                Attacking = false;

            if (HasPlayer())
            {
                if (Attacking)
                {
                    if (NextAttackTime < Tick)
                    {
                        if (!Obj.IsWithinRadius(Target, 10))
                        {
                            NextAttackTime += 200;
                        }
                        else if (!Obj.IsObjectInFront(Target, 90))
                        {
                            NextAttackTime += 200;
                        }
                        else
                        {
                            Me.Strike(Target);
                            NextAttackTime = Tick + Me.ItmInterface.GetAttackTime(EquipSlot.MAIN_DROITE) * 10;
                        }
                    }
                }
            }
            else
                AttackAI(Target);
        }

        public void AttackAI(Unit Target)
        {
            if (Target == null)
            {
                if (State == AiState.FIGHTING)
                    CombatStop();

                return;
            }

            long Tick = TCPManager.GetTimeStampMS();

            SetTarget(Target);
            LookAt(Target);

            if (NextAttackTime < Tick)
            {
                if (Obj.IsWithinRadius(Target, 10))
                {
                    Obj.GetUnit().Strike(Target);
                    NextAttackTime = Tick + 2000;
                }
            }
        }

        public void LookAt(Unit Target)
        {
            if (HasPlayer())
                return;

            if (!Obj.IsCreature())
                return;

            if (CheckSpawnRange())
                return;

            Creature Crea = Obj.GetCreature();
            Crea.MvtInterface.FollowUnit(Target, 7, 10,200, eFormationType.Protect);
        }

        #region IASystem

        public bool IsFighting()
        {
            if (State == AiState.FIGHTING)
                return true;

            return false;
        }

        public bool IsFightingWithFriend(Unit Me)
        {
            if (!HasTarget())
                return false;

            if (!IsFighting())
                return false;

            if(IsFriend(Me, CurrentTarget.Target))
                return true;
            else
                return false;
        }

        public void CombatStart(Unit Fighter)
        {
            Log.Success("CombatStart", Obj.Name + " Start combat with " + Fighter.Name);
            State = AiState.FIGHTING;
            GetAggro(Fighter.Oid).DamagesReceive+=100;
        }

        public void CombatStop()
        {
            //Log.Success("CombatStop", Obj.Name + " Stop combat ");
            State = AiState.STANDING;
            ClearTargets();

            if (Obj.IsCreature())
                Obj.GetCreature().MvtInterface.StopFollow();

            if(Obj.IsCreature() && !Obj.GetCreature().IsDead)
                ReturnToSpawn();

            Obj.EvtInterface.Notify("CombatStop", null, null);
        }

        public void Evade()
        {
            foreach (AggroInfo Info in _Aggros.Values.ToArray())
            {
                Object Target = Obj.Zone.GetObject(Info.Oid);
                if (Target == null || !Target.IsUnit())
                   continue;

                Target.GetUnit().CbtInterface.RemoveAggro(Obj.Oid);
            }

            CombatStop();
        }

        public bool CheckSpawnRange()
        {
            if (!Obj.IsCreature())
                return false;

            Creature Crea = Obj.GetCreature();

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
            if (!Obj.IsCreature())
                return;

            Creature Crea = Obj.GetCreature();
            Crea.MvtInterface.CancelWalkTo();
            Crea.SetPosition((UInt16)Crea.SpawnPoint.X, (UInt16)Crea.SpawnPoint.Y, (UInt16)Crea.SpawnPoint.Z, Crea.SpawnHeading);
        }

        #endregion

        #region AggroSystem

        public Dictionary<UInt16, AggroInfo> _Aggros = new Dictionary<ushort, AggroInfo>();

        public AggroInfo GetAggro(UInt16 Oid, bool Create=true)
        {
            AggroInfo Info = null;
            if (!_Aggros.TryGetValue(Oid, out Info) && Create)
            {
                Info = new AggroInfo(Oid);
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
        public AggroInfo GetMaxAggroHate()
        {
            UInt64 TotalDamages = TotalDamageReceive();
            AggroInfo Aggro = null;
            float MaxHate = -1;

            foreach (AggroInfo Info in _Aggros.Values.ToArray())
            {
                float Hate = Info.GetHate(TotalDamages);

                if (Hate > MaxHate)
                {
                    MaxHate = Hate;
                    Aggro = Info;
                }
            }

            return Aggro;
        }
        public UInt64 TotalDamageReceive()
        {
            UInt64 Damages = 0;

            foreach (AggroInfo Info in _Aggros.Values.ToArray())
                Damages += Info.DamagesDeal;

            return Damages;
        }

        public void ClearTargets()
        {
            _Aggros.Clear();
            SetTarget(null);
        }
        public void AddDamageReceive(UInt16 Oid, UInt32 Damages)
        {
            AggroInfo Info = GetAggro(Oid);
            Info.DamagesDeal = Damages;
        }

        public Unit GetNextTarget()
        {
            Unit Me = GetUnit();
            Unit Target = null;

            if (!Me.IsPlayer())
            {
                AggroInfo Info = GetMaxAggroHate();
                if(Info != null)
                    Target = Obj.Region.GetObject(Info.Oid) as Unit;

            }
            else
            {
                if (CurrentTarget.Type != GameData.TargetTypes.TARGETTYPES_TARGET_ALLY)
                    Target = CurrentTarget.Target;
            }

            if (Target == null)
                return null;

            if (!Target.IsInWorld())
                return null;

            if (Target.IsDead)
            {
                if(State == AiState.FIGHTING)
                    OnTargetDie(Target);

                return null;
            }

            return Target;
        }

        #endregion

        #region Follower

        public List<UInt16> _Followers = new List<ushort>();

        public void AddFollower(UInt16 Oid)
        {
            if (!_Followers.Contains(Oid))
                _Followers.Add(Oid);
        }
        public void RemoveFollower(UInt16 Oid)
        {
            _Followers.Remove(Oid);
        }
        public int GetFollowId(UInt16 Oid)
        {
            for (int i = 0; i < _Followers.Count; ++i)
                if (_Followers[i] == Oid)
                    return 1+i;

            return 0;
        }
        public float GetFollowPct(UInt16 Oid)
        {
            float Id = GetFollowId(Oid);
            float Count = _Followers.Count;

            return (Id / Count) * 100;
        }

        #endregion

        #region Targets

        public TargetInfo CurrentTarget = new TargetInfo(null, GameData.TargetTypes.TARGETTYPES_TARGET_NONE);

        public void SetTarget(Unit Target)
        {
            CurrentTarget.Target = Target;
            CurrentTarget.Type = GetTargetType(GetUnit(), Target);
        }
        public Unit GetTarget()
        {
            return CurrentTarget.Target;
        }
        public bool HasTarget()
        {
            return CurrentTarget.Type != GameData.TargetTypes.TARGETTYPES_TARGET_NONE;
        }
        public GameData.TargetTypes GetTargetType()
        {
            return CurrentTarget.Type;
        }

        #endregion

        #region Events

        public void OnTakeDamage(Unit Fighter, UInt32 DamageCount)
        {
            switch (State)
            {
                case AiState.STANDING:
                    CombatStart(Fighter);
                    break;
            };

            AddDamageReceive(Fighter.Oid, DamageCount);
            Obj.EvtInterface.Notify("OnTakeDamage", Fighter, null);
        }

        public void OnDealDamage(Unit Victim, UInt32 DamageCount)
        {
            switch (State)
            {
                case AiState.STANDING:
                    CombatStart(Victim);
                    break;
            };
            Obj.EvtInterface.Notify("OnDealDamage", Victim, null);
        }

        public void OnTargetDie(Unit Victim)
        {
            RemoveAggro(Victim.Oid);

            if (GetAggroCount() <= 0)
                CombatStop();

            Obj.EvtInterface.Notify("OnTargetDie", Victim, null);
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
            if (A.Realm == GameData.Realms.REALMS_REALM_NEUTRAL && B.Realm == GameData.Realms.REALMS_REALM_NEUTRAL)
                return false;

            return A.Realm != B.Realm;
        }
        static public bool IsFriend(Unit A, Unit B)
        {
            return A.Realm == B.Realm;
        }

        static public bool CanAttack(Unit A, Unit B)
        {
            if (A.IsDead || B.IsDead)
                return false;

            if (A == B)
                return false;

            if (!IsEnemy(A, B))
                return false;

            if (!A.IsInWorld() || !B.IsInWorld())
                return false;

            return true;
        }
    }
}

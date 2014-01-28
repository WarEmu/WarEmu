using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using FrameWork;

namespace WorldServer
{
    public class AIInterface : BaseInterface
    {
        static public int BrainThinkTime = 3000; // Update think every 3sec
        static public int MAX_AGGRO_RANGE = 50; // Max Range To Aggro

        public CombatInterface Cbt;
        public MovementInterface Mvt;
        public EventInterface Evt;

        public AIInterface(Object Obj)
            : base(Obj)
        {
        }

        public override bool Load()
        {
            Evt = Obj.EvtInterface;
            if (Obj.IsUnit())
            {
                Cbt = Obj.GetUnit().CbtInterface;
                Mvt = Obj.GetUnit().MvtInterface;
            }

            if(Evt != null)
                Evt.AddEvent(UpdateThink, BrainThinkTime, 0);

            return base.Load();
        }

        public override void Stop()
        {
            if (Evt != null)
                Evt.RemoveEvent(UpdateThink);

            base.Stop();
        }

        #region Brain

        public ABrain CurrentBrain;

        public void SetBrain(ABrain NewBrain)
        {
            if (CurrentBrain != null)
                CurrentBrain.Stop();

            CurrentBrain = NewBrain;

            if (CurrentBrain != null)
                CurrentBrain.Start();
        }

        public void UpdateThink()
        {
            if (HasPlayer())
                return;

            if (Obj.GetUnit().IsDead || Obj.GetUnit().IsDisposed)
                return;

            if (CurrentBrain != null && CurrentBrain.IsStart && !CurrentBrain.IsStop)
                CurrentBrain.Think();
        }

        #endregion

        #region Ranged

        public List<Unit> RangedAllies = new List<Unit>();
        public List<Unit> RangedEnemies = new List<Unit>();

        public bool AddRange(Unit Obj)
        {
            if (!HasUnit())
                return false;

            if (CombatInterface.IsEnemy(GetUnit(), Obj))
                RangedEnemies.Add(Obj.GetUnit());
            else
                RangedAllies.Add(Obj.GetUnit());

            return true;
        }

        public bool RemoveRange(Unit Obj)
        {
            if (!HasUnit())
                return false;

            RangedAllies.Remove(Obj);
            RangedEnemies.Remove(Obj);
            return true;
        }

        public void ClearRange()
        {
            RangedAllies.Clear();
            RangedEnemies.Clear();
        }

        public Unit GetAttackableUnit()
        {
            float MaxRange = AIInterface.MAX_AGGRO_RANGE;
            Unit Target = null;

            foreach (Unit Enemy in RangedEnemies)
            {
                if (!CombatInterface.CanAttack(GetUnit(), Enemy))
                    continue;

                if (Enemy.Realm == GameData.Realms.REALMS_REALM_NEUTRAL)
                    continue;

                float Dist = Obj.GetDistance(Enemy);
                if (Dist < MaxRange)
                {
                    MaxRange = Dist;
                    Target = Enemy;
                }
            }

            return Target;
        }

        #endregion
    }
}

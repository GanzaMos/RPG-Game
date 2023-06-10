using UnityEngine;

namespace RPG.Control
{
    public class EnemyStateAttacking: EnemyStateChasing
    {
        public EnemyStateAttacking(EnemyAIStateManager stateManager) : base(stateManager) { }
        
        
        //===================================================================
        //General methods
        //===================================================================
        public override void InitializeState()
        {
            base.InitializeState();
        }

        public override void EnterState()
        {
            base.EnterState();
        }

        public override void UpdateState()
        {
            base.UpdateState();
            
            if (!Fighter.IsInWeaponRange())
                sm.SwitchState(sm.EnemyStateChasing);
            
            sm.transform.LookAt(Fighter.TargetPosition);

            if (IsEnoughTimeSinceLastAttack()) return;

            //this will trigger animation Hit() or Shoot() event in Fighter
            Fighter.StartAttack();
        }

        public override void LateUpdateState()
        {
            base.LateUpdateState();
        }

        public override void ExitState()
        {
            base.ExitState();
        }
        
        
        //===================================================================
        //Specific methods
        //===================================================================  
        bool IsEnoughTimeSinceLastAttack()
        {
            return Fighter.TimeSinceLastAttack >= Fighter.currentWeaponConfig.TimeBetweenAttacks;
        }
    }
}
using UnityEngine;

namespace RPG.Control
{
    public class EnemyStateChasing : EnemyBaseState
    {
        public EnemyStateChasing(EnemyAIStateManager stateManager) : base(stateManager) { }
        
        //===================================================================
        //General methods
        //===================================================================
        public override void InitializeState()
        {
            base.InitializeState();
        }
        
        public override void EnterState()
        {
        }

        public override void UpdateState()
        {
            //Timer DistanceToPlayer is 0 if Player is in vision range
            if (sm.DistanceToPlayer <= sm.VisionRadius)    
                sm.TimeSinceLastSawPlayer = 0;
            
            //Out of chasing time? Go back to your duties
            if(sm.TimeSinceLastSawPlayer >= sm.MaxSuspicionTime)
                GoBackToStartState();
            
            //Target dead? Go back to Start state
            if (Fighter.TargetCharacter.data.health.IsDead)
                GoBackToStartState();
            
            //Trying to get closer and start attack if in weapon range
            if (Fighter.IsInWeaponRange())                       
                sm.SwitchState(sm.EnemyStateAttacking);
            else
                Mover.MoveToAttack(Fighter.TargetPosition.position);
        }

        public override void LateUpdateState()
        {
            
        }

        public override void ExitState()
        {
            Mover.Cancel();
        }
        
        
        //===================================================================
        //Specific methods
        //===================================================================
        void GoBackToStartState()
        {
            Fighter.Cancel();
            sm.SwitchState(sm.GetStartState());
        }
    }
}
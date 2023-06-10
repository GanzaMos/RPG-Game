using UnityEngine;

namespace RPG.Control
{
    public class EnemyStateIdle : EnemyBaseState
    {
        public EnemyStateIdle(EnemyAIStateManager stateManager) : base(stateManager) {
        }
        
        
        //===================================================================
        //General methods
        //===================================================================
        public override void InitializeState()
        {
            base.InitializeState();
        }
        
        public override void EnterState()
        {
            Animator.Play("Locomotion");
        }

        public override void UpdateState()
        {
            if (Player == null) return; //TODO it works, but after Players dead enemies don't go back to their duties

            ClearingFighterData();  //TODO need to clear after target is killed
            
            //have seen the Player? Attack him!
            if (sm.DistanceToPlayer <= sm.VisionRadius || Ch.data.ai.IsProvoked)                
            {
                Fighter.SetTarget(Player.gameObject);
                sm.TimeSinceLastSawPlayer = 0f;
                sm.TimeSinceBeenProvoked = 0f;
                sm.SwitchState(sm.EnemyStateChasing);
            }
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
        void ClearingFighterData()
        {
            if (Fighter.TargetHealth && Fighter.TargetCharacter.data.health.IsDead)
            {
                Fighter.TargetHealth = null;
                Fighter.TargetPosition = null;
            }
        }
    }
}
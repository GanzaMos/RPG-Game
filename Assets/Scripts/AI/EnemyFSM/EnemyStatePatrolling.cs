using UnityEngine;

namespace RPG.Control
{
    public class EnemyStatePatrolling : EnemyStateIdle
    {
        float _dwellingTimeAtCurrentWaypoint = 0;
        
        public EnemyStatePatrolling(EnemyAIStateManager stateManager) : base(stateManager) { }
        
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

            _dwellingTimeAtCurrentWaypoint += Time.deltaTime;
                
            if (AtWaypoint())
            {
                CycleWaypoint();
                _dwellingTimeAtCurrentWaypoint = 0;
            }
            else if (_dwellingTimeAtCurrentWaypoint < sm.MaxDwellingTimeAtWaypoint)
            {
                Mover.Cancel();
            }
            else
            {
                Mover.MoveTo(GetCurrentWaypointPosition(), sm.CalmSpeedFraction);
            }
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
        bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(sm.transform.position, GetCurrentWaypointPosition());
            return distanceToWaypoint < sm.WaypointTolerance;
        }

        void CycleWaypoint()
        {
            if (sm.CurrentWaypointIndex == sm.PatrolPath.transform.childCount - 1)
            {
                sm.CurrentWaypointIndex = 0;
            }
            else
            {
                sm.CurrentWaypointIndex++;
            }
        }

        Vector3 GetCurrentWaypointPosition()
        {
            return sm.PatrolPath.GetWaypointPosition(sm.CurrentWaypointIndex);
        }
    }
}
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Control
{
    public class EnemyStateSearching : EnemyStateIdle
    {
        public EnemyStateSearching(EnemyAIStateManager stateManager) : base(stateManager) { }

        Vector3 _positionToSearch;
        NavMeshAgent _navMeshAgent;

        int _currentPointToCheck;
        float _timer;
        bool _canCheckNewPoint;
        
        //===================================================================
        //General methods
        //===================================================================
        public override void InitializeState()
        {
            base.InitializeState();
            _navMeshAgent = sm.gameObject.GetComponent<NavMeshAgent>() ?? InstError<NavMeshAgent>();
        }
        
        public override void EnterState()
        {
            base.EnterState();
            _currentPointToCheck = 0;
            _canCheckNewPoint = true;
        }

        public override void UpdateState()
        {
            base.UpdateState();
        }

        public override void LateUpdateState()
        {
            base.LateUpdateState();
            
            GetPointAndGoCheckIt();
            WaitAtThePoint();
        }

        public override void ExitState()
        {
            base.ExitState();
        }
        
        
        //===================================================================
        //Specific methods
        //===================================================================
        void GetPointAndGoCheckIt()
        {
            if (_canCheckNewPoint)
            {
                _canCheckNewPoint = false;
                Mover.RandomPoint(sm.gameObject.transform.position, 5f, out _positionToSearch);
                Mover.MoveTo(_positionToSearch);
                _currentPointToCheck++;
            }
        }
        
        
        void WaitAtThePoint()
        {
            if (_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
            {
                _timer += Time.deltaTime;
                if (_timer < sm.TimeToSearch) return;
                _timer = 0;

                _canCheckNewPoint = true;

                StopSearchingAtLastPoint();
            }
        }

        
        void StopSearchingAtLastPoint()
        {
            if (_currentPointToCheck == sm.PointsToSearch)
            {
                sm.SwitchState(sm.GetStartState());
            }
        }
    }
}
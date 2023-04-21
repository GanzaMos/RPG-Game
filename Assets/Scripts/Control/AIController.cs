using System;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using RPG.Attributes;
using UnityEngine;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        //Serialized
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float maxSuspicionTime = 3f;
        
        [SerializeField] float maxProvokedTime = 5f;
        [SerializeField] float neighboursProvokingRadius = 10f;
        
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float waypointTolerance = 1f;
        [SerializeField] float maxDwellingTimeAtWaypoint = 3f;
        [Range(0,2)]
        
        [SerializeField] float calmSpeedFraction = 0.5f;
        
        
        //Cashed
        Transform _player;
        Fighter _fighter;
        Mover _mover;
        Health _health;
        ActionScheduler _actionScheduler;
        
        //Local variables
        bool _isProvoked = false;
        public bool IsProvoked => _isProvoked;

        float _distanceToPlayer;
        Vector3 _guardPosition;
        float _timeSinceLastSawPlayer = Mathf.Infinity;
        float _timeSinceBeenProvoked = Mathf.Infinity;
        float _dwellingTimeAtCurrentWaypoint = 0; 
        int _currentWaypointIndex = 0;
        
        
        //Starting setup
        
        void Awake()
        {
            _player = GameObject.FindWithTag("Player").transform;
            _fighter = GetComponent<Fighter>();
            _mover = GetComponent<Mover>();
            _health = GetComponent<Health>();
            _actionScheduler = GetComponent<ActionScheduler>();
            _guardPosition = transform.position;
        }

        
        //Main AI behavior sequence
        
        void Update()
        {
            if (_health.IsDead) return;
            
            ChasingCheck();
        }

        void ChasingCheck()
        {
            if (_player == null) return;
            _distanceToPlayer = Vector3.Distance(transform.position, _player.position);
            
            if (_distanceToPlayer <= chaseDistance || _isProvoked)                //have seen the Player? Attack him!
            {
                _fighter.SetTarget(_player.gameObject);
                _timeSinceLastSawPlayer = 0f;
                _timeSinceBeenProvoked = 0f;
            }
            else if(maxSuspicionTime >= _timeSinceLastSawPlayer)   //Player escaped you? Wait for a while
            {
                _actionScheduler.CancelCurrentAction();
            }
            else                                                   //Waited a bit? Go back to your normal duties
            {
                PatrolBehavior();
            }


            if (_timeSinceBeenProvoked >= maxProvokedTime)
            {
                _isProvoked = false;
            }

            _timeSinceBeenProvoked += Time.deltaTime;
            _timeSinceLastSawPlayer += Time.deltaTime;
        }
        
        void PatrolBehavior()
        {
            if (!patrolPath)                                 //Don't have patrol path? Go back to the starting point
            {
                _mover.MoveTo(_guardPosition, calmSpeedFraction);
            }
            else                                                    //Have a patrol path? Go back to patrolling
            {
                _dwellingTimeAtCurrentWaypoint += Time.deltaTime;
                
                if (AtWaypoint())
                {
                    CycleWaypoint();
                    _dwellingTimeAtCurrentWaypoint = 0;
                }
                else if (_dwellingTimeAtCurrentWaypoint < maxDwellingTimeAtWaypoint)
                {
                    _mover.Cancel();
                }
                else
                {
                _mover.MoveTo(GetCurrentWaypointPosition(), calmSpeedFraction);
                }
            }
        }

        
        //Public methods

        public void ProvokeEnemy(bool provokeEnemy, bool provokeEnemyNeighbours = true)
        {
            _isProvoked = provokeEnemy;
            _timeSinceBeenProvoked = 0;
            if (provokeEnemyNeighbours)
            {
                ProvokeNearbyEnemies();
            }
        }
        

        void ProvokeNearbyEnemies()
        {
            RaycastHit[] hits = Physics.SphereCastAll(transform.position, neighboursProvokingRadius, transform.forward, 0f);
            
            foreach (var hit in hits)
            {
                AIController aiControllerObject = hit.transform.GetComponent<AIController>();
                if (!aiControllerObject) continue;
                aiControllerObject.ProvokeEnemy(true, false);
            }
        }

        //Waypoints processing
        
        bool AtWaypoint()
        {
            float distanceToWaypoint = Vector3.Distance(transform.position, GetCurrentWaypointPosition());
            return distanceToWaypoint < waypointTolerance;
        }

        void CycleWaypoint()
        {
            if (_currentWaypointIndex == patrolPath.transform.childCount - 1)
            {
                _currentWaypointIndex = 0;
            }
            else
            {
                _currentWaypointIndex++;
            }
        }

        Vector3 GetCurrentWaypointPosition()
        {
            return patrolPath.GetWaypointPosition(_currentWaypointIndex);
        }

        
        //Draw a sphere of a chasing radius
        
        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}
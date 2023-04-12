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
        [SerializeField] float chaseDistance = 5f;
        [SerializeField] float maxSuspicionTime = 3f;
        [SerializeField] PatrolPath patrolPath;
        [SerializeField] float waypointTolerance = 1f;
        [SerializeField] float maxDwellingTimeAtWaypoint = 3f;
        [Range(0,2)]
        [SerializeField] float calmSpeedFraction = 0.5f;

        Transform _player;
        Fighter _fighter;
        Mover _mover;
        Health _health;
        ActionScheduler _actionScheduler;
        
        float _distanceToPlayer;
        Vector3 _guardPosition;
        float _timeSinceLastSawPlayer = Mathf.Infinity;
        float _dwellingTimeAtCurrentWaypoint = 0; 
        int _currentWaypointIndex = 0;
        
        
        
        void Start()
        {
            _player = GameObject.FindWithTag("Player").transform;
            _fighter = GetComponent<Fighter>();
            _mover = GetComponent<Mover>();
            _health = GetComponent<Health>();
            _actionScheduler = GetComponent<ActionScheduler>();
            _guardPosition = transform.position;
        }

        void Update()
        {
            if (_health.IsDead) return;
            
            ChasingCheck();
        }

        void ChasingCheck()
        {
            if (_player == null) return;
            _distanceToPlayer = Vector3.Distance(transform.position, _player.position);
            
            if (_distanceToPlayer <= chaseDistance)                //see the Player? Attack him!
            {
                _fighter.Attack(_player.gameObject);
                _timeSinceLastSawPlayer = 0f;
            }
            else if(maxSuspicionTime >= _timeSinceLastSawPlayer)   //Player escaped you? Wait for a while
            {
                _actionScheduler.CancelCurrentAction();
            }
            else                                                   //Waited a bit? Go back to your normal duties
            {
                PatrolBehavior();
            }
            _timeSinceLastSawPlayer += Time.deltaTime;
        }

        void PatrolBehavior()
        {
            if (patrolPath == null)                                 //Don't have patrol path? Go back to the starting point
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

        void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(transform.position, chaseDistance);
        }
    }
}
using System;
using RPG.Combat;
using RPG.Core;
using RPG.Core.RPG.Core;
using RPG.Movement;
using Unity.VisualScripting;
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
            if (_health.isDead) return;
            
            ChasingCheck();
        }

        void ChasingCheck()
        {
            if (_player == null) return;
            _distanceToPlayer = Vector3.Distance(transform.position, _player.position);
            
            if (_distanceToPlayer <= chaseDistance)
            {
                _fighter.Attack(_player.gameObject);
                _timeSinceLastSawPlayer = 0f;
            }
            else if(maxSuspicionTime >= _timeSinceLastSawPlayer)
            {
                _actionScheduler.CancelCurrentAction();
            }
            else
            {
                PatrolBehavior();
            }
            _timeSinceLastSawPlayer += Time.deltaTime;
        }

        void PatrolBehavior()
        {
            if (patrolPath == null)
            {
                _mover.MoveTo(_guardPosition);
            }
            else
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
                _mover.MoveTo(GetCurrentWaypointPosition());
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
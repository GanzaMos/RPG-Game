using RPG.Core;
using RPG.Core.RPG.Core;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
    [RequireComponent(typeof(ActionScheduler))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class Mover : MonoBehaviour, IAction
    {
        [SerializeField] float maxSpeed;
        
        NavMeshAgent _navMeshAgent;
        Animator _animator;
        ActionScheduler _actionScheduler;
        Health _health;

        float _speedFraction;

        void Start()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _animator = GetComponent<Animator>();
            _actionScheduler = GetComponent<ActionScheduler>();
            _health = GetComponent<Health>();
            maxSpeed = GetComponent<NavMeshAgent>().speed;
        }

        void Update()
        {
            _navMeshAgent.enabled = !_health.isDead;
            UpdateAnimator();
        }
        
        public void MoveTo(Vector3 destination)
        {
            _actionScheduler.StartAction(this);            //canceling all previous actions
            _navMeshAgent.isStopped = false;               
            _navMeshAgent.speed = maxSpeed;
            _navMeshAgent.SetDestination(destination);
        }
        
        public void MoveTo(Vector3 destination, float speedFraction)
        {
            _actionScheduler.StartAction(this);
            _navMeshAgent.isStopped = false;
            _navMeshAgent.speed = maxSpeed * speedFraction;
            _navMeshAgent.SetDestination(destination);
        }
        
        public void MoveToAttack(Vector3 destination)
        {
            _navMeshAgent.isStopped = false;
            _navMeshAgent.speed = maxSpeed;
            _navMeshAgent.SetDestination(destination);
        }
        
        public void MoveToAttack(Vector3 destination, float speedFraction)
        {
            _navMeshAgent.isStopped = false;
            _navMeshAgent.speed = maxSpeed * speedFraction;
            _navMeshAgent.SetDestination(destination);
        }

        public void Cancel()
        {
            _navMeshAgent.isStopped = true;
        }
        
    
        void UpdateAnimator()
        {
            Vector3 velocity = _navMeshAgent.velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            _animator.SetFloat("forwardSpeed", speed);
        }
    }
}

using RPG.Core;
using RPG.Core.RPG.Core;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
    [RequireComponent(typeof(ActionScheduler))]
    public class Mover : MonoBehaviour, IAction
    {
        NavMeshAgent _navMeshAgent;
        Animator _animator;
        ActionScheduler _actionScheduler;
        Health _health;

        void Start()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _animator = GetComponent<Animator>();
            _actionScheduler = GetComponent<ActionScheduler>();
            _health = GetComponent<Health>();
        }

        void Update()
        {
            _navMeshAgent.enabled = !_health.isDead;
            UpdateAnimator();
        }
        
        public void MoveTo(Vector3 destination)
        {
            _actionScheduler.StartAction(this);
            _navMeshAgent.isStopped = false;
            _navMeshAgent.SetDestination(destination);
        }
        
        public void MoveToAttack(Vector3 destination)
        {
            _navMeshAgent.isStopped = false;
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

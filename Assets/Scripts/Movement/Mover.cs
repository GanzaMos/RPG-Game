using System.Collections.Generic;
using RPG.Core;
using RPG.Core.RPG.Core;
using UnityEngine;
using UnityEngine.AI;
using RPG.Saving;

namespace RPG.Movement
{
    [RequireComponent(typeof(ActionScheduler))]
    [RequireComponent(typeof(NavMeshAgent))]
    public class Mover : MonoBehaviour, IAction, ISaveable
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

        public object CaptureState()
        {
            Dictionary<string, SerializableVector3> data = new Dictionary<string, SerializableVector3>();
            data["position"] = new SerializableVector3(transform.position);
            data["rotation"] = new SerializableVector3(transform.eulerAngles);
            return data;
        }

        public void RestoreState(object state)
        {
            Dictionary<string, SerializableVector3> data = (Dictionary<string, SerializableVector3>) state;
            GetComponent<NavMeshAgent>().Warp(data["position"].ToVector());
            transform.eulerAngles = data["rotation"].ToVector();
        }
    }
}

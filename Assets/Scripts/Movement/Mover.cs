using System;
using System.Collections.Generic;
using RPG.Core;
using RPG.Attributes;
using RPG.Combat;
using UnityEngine;
using UnityEngine.AI;
using RPG.Saving;
using Random = UnityEngine.Random;

namespace RPG.Movement
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class Mover : MonoBehaviour, ISaveable
    {
        float _maxSpeed;
        
        NavMeshAgent _navMeshAgent;
        Animator _animator;
        Character _ch;

        void Awake()
        {
            _ch = GetComponentInParent<Character>() ?? InstError<Character>();
            _navMeshAgent = GetComponent<NavMeshAgent>() ?? InstError<NavMeshAgent>();
            _animator = GetComponent<Animator>() ?? InstError<Animator>();
            _maxSpeed = GetComponent<NavMeshAgent>().speed;
        }
        
        T InstError<T>()
        {
            string className = typeof(T).Name;
            if (typeof(T) is Character) throw new Exception($"Missing {className} component for {name}, ID {GetInstanceID()}");
            else throw new Exception($"Missing {className} component for {name} in {_ch?.gameObject.name}, ID {GetInstanceID()}");
        }

        void Update()
        {
            _navMeshAgent.enabled = !_ch.data.health.IsDead;
            UpdateAnimator();
        }
        
        public void MoveTo(Vector3 destination)
        {
            // _actionScheduler.StartAction(this);            //canceling all previous actions
            GetComponent<Fighter>().playerIsAttacking = false; //todo temporary solution, need to implement FSM for Player
            _navMeshAgent.isStopped = false;               
            _navMeshAgent.speed = _maxSpeed;
            _navMeshAgent.SetDestination(destination);
        }
        
        public void MoveTo(Vector3 destination, float speedFraction)
        {
            //_actionScheduler.StartAction(this);
            _navMeshAgent.isStopped = false;
            _navMeshAgent.speed = _maxSpeed * speedFraction;
            _navMeshAgent.SetDestination(destination);
        }
        
        public void MoveToAttack(Vector3 destination)
        {
            _navMeshAgent.isStopped = false;
            _navMeshAgent.speed = _maxSpeed;
            _navMeshAgent.SetDestination(destination);
        }
        
        public void MoveToAttack(Vector3 destination, float speedFraction)
        {
            _navMeshAgent.isStopped = false;
            _navMeshAgent.speed = _maxSpeed * speedFraction;
            _navMeshAgent.SetDestination(destination);
        }

        public bool RandomPoint(Vector3 center, float range, out Vector3 result)
        {
            Vector3 randomPoint = new Vector3();

            //todo need a script with ray casting so it will guarantee find a point on NavMesh; 
            for (int i = 0; i < 30; i++)
            {
                randomPoint = center + Random.insideUnitSphere * range;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas))
                {
                    result = hit.position;
                    return true;
                }
            }
            result = center;
            return false;
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

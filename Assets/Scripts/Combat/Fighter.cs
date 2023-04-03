using System;
using RPG.Core;
using RPG.Core.RPG.Core;
using RPG.Movement;
using Unity.VisualScripting;
using UnityEngine;

namespace RPG.Combat
{
    [RequireComponent(typeof(ActionScheduler))]
    public class Fighter : MonoBehaviour, IAction
    {
        [SerializeField] float weaponRange = 2f;
        [SerializeField] float timeBetweenAttacks = 1f;
        [SerializeField] float weaponDamage = 10f;

        float _timeSinceLastAttack = Mathf.Infinity;
        
        Mover _mover;
        Transform _targetPosition;
        Health _health;
        ActionScheduler _actionScheduler;
        Animator _animator;
        
        void Start()
        {
            _mover = GetComponent<Mover>();
            _actionScheduler = GetComponent<ActionScheduler>();
            _animator = GetComponent<Animator>();
        }

        public void Attack(GameObject combatTarget)
        {
            _actionScheduler.StartAction(this);
            _targetPosition = combatTarget.transform;
            _health = _targetPosition.GetComponent<Health>();
        }

        void AttackBehavior()
        {
            transform.LookAt(_targetPosition);
            if (_timeSinceLastAttack >= timeBetweenAttacks && !_health.isDead)
            {
                //this will trigger animation Hit() effect
                _animator.ResetTrigger("stopAttacking");
                _animator.SetTrigger("attack");
            }

        }

        //animation effect
        void Hit()
        {
            if (_health == null) return;
            _health.TakeDamage(weaponDamage); 
            _timeSinceLastAttack = 0f;
        }

        public void Cancel()
        {
            _targetPosition = null;
            _animator.ResetTrigger("attack");
            _animator.SetTrigger("stopAttacking");
        }

        void Update()
        {
            _timeSinceLastAttack += Time.deltaTime;
            
            if (_targetPosition == null) return;

            if (!IsNear())
            {
                _mover.MoveToAttack(_targetPosition.position);
            }
            else 
            {
                _mover.Cancel();
                AttackBehavior();
            }
        }

        bool IsNear()
        {
            return Vector3.Distance(transform.position, _targetPosition.position) <= weaponRange;
        }
    }
}
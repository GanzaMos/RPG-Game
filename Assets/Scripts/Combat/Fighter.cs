using System;
using RPG.Core;
using RPG.Core.RPG.Core;
using RPG.Movement;
using RPG.Saving;
using Unity.VisualScripting;
using UnityEngine;

namespace RPG.Combat
{
    [RequireComponent(typeof(ActionScheduler))]
    public class Fighter : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] Weapon defaultWeapon = null;
        [SerializeField] Weapon currentWeapon = null;

        float _timeSinceLastAttack = Mathf.Infinity;
        
        GameObject _currentWeaponPrefab;
        Mover _mover;
        ActionScheduler _actionScheduler;
        Animator _animator;
        Transform _targetPosition;
        Health _targetHealth;
        
        void Start()
        {
            _mover = GetComponent<Mover>();
            _actionScheduler = GetComponent<ActionScheduler>();
            _animator = GetComponent<Animator>();
            if (currentWeapon == null)
            {
                EquipWeapon(defaultWeapon);
            }
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

        public void EquipWeapon(Weapon weapon)
        {
            if (weapon == null) return;
            if (weapon == currentWeapon) return;
            if (_currentWeaponPrefab != null) Destroy(_currentWeaponPrefab);
            
            currentWeapon = weapon;
            _currentWeaponPrefab = weapon.Spawn(rightHandTransform, leftHandTransform, _animator);
        }

        public void Attack(GameObject combatTarget)
        {
            _actionScheduler.StartAction(this);
            _targetPosition = combatTarget.transform;
            _targetHealth = _targetPosition.GetComponent<Health>();
        }

        void AttackBehavior()
        {
            transform.LookAt(_targetPosition);
            if (_timeSinceLastAttack >= currentWeapon.TimeBetweenAttacks && !_targetHealth.isDead)
            {
                //this will trigger animation Hit() effect
                _animator.ResetTrigger("stopAttacking");
                _animator.SetTrigger("attack");
                _timeSinceLastAttack = 0f;
            }
        }

        //animation effect
        void Hit()
        {
            if (_targetHealth == null) return;
            _targetHealth.TakeDamage(currentWeapon.WeaponDamage);
        }
        
        void Shoot()
        {
            if (_targetHealth == null) return;
            
            if (currentWeapon.HasProjectile())
            {
                currentWeapon.LaunchProjectile(rightHandTransform, leftHandTransform, _targetHealth);
            }
            
        }

        public void Cancel()
        {
            _targetPosition = null;
            _mover.Cancel();
            _animator.ResetTrigger("attack");
            _animator.SetTrigger("stopAttacking");
        }


        bool IsNear()
        {
            return Vector3.Distance(transform.position, _targetPosition.position) <= currentWeapon.WeaponRange;
        }

        public object CaptureState()
        {
            if (currentWeapon != null)
            {
                return currentWeapon.name;
            }
            else
            {
                return defaultWeapon.name;
            }

        }

        public void RestoreState(object state)
        {
            string savedWeapon = (string) state;
            Weapon weapon = Resources.Load<Weapon>(savedWeapon);
            EquipWeapon(weapon);
        }
    }
}
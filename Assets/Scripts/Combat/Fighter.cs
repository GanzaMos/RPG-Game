using System;
using RPG.Core;
using RPG.Attributes;
using RPG.Movement;
using RPG.Saving;
using UnityEngine;

namespace RPG.Combat
{
    [RequireComponent(typeof(ActionScheduler))]
    public class Fighter : MonoBehaviour, IAction, ISaveable
    {
        //Serializable
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] Weapon defaultWeapon = null;
        [SerializeField] Weapon currentWeapon = null;

        //local variables
        float _timeSinceLastAttack = Mathf.Infinity;
        
        //Cashed
        GameObject _currentWeaponPrefab;
        Mover _mover;
        ActionScheduler _actionScheduler;
        Animator _animator;
        Transform _targetPosition;
        Health _targetHealth;
        
        
        //Starting setup
        
        void Start()
        {
            _mover = GetComponent<Mover>()                     ?? throw new Exception($"Missing Mover for Fighter in {gameObject.name}!");
            _actionScheduler = GetComponent<ActionScheduler>() ?? throw new Exception($"Missing ActionScheduler for Fighter in {gameObject.name}!");
            _animator = GetComponent<Animator>()               ?? throw new Exception($"Missing Animator for Fighter in {gameObject.name}!");
            
            if (!currentWeapon)
            {
                EquipWeapon(defaultWeapon);
            }
        }
        
        
        //Main methods for attacking
        
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
            return Vector3.Distance(transform.position, _targetPosition.position) <= currentWeapon.WeaponRange;
        }

        void AttackBehavior()
        {
            transform.LookAt(_targetPosition);
            if (_timeSinceLastAttack >= currentWeapon.TimeBetweenAttacks && !_targetHealth.IsDead)
            {
                //this will trigger animation Hit() or Shoot() effect
                _animator.ResetTrigger("stopAttacking");
                _animator.SetTrigger("attack");
                _timeSinceLastAttack = 0f;
            }
        }
        
        void Hit()   //animation called method
        {
            if (_targetHealth == null) return;
            _targetHealth.ReduceHealth(currentWeapon.WeaponDamage, gameObject);
        }
        
        void Shoot()  //animation called method
        {
            if (_targetHealth == null) return;
            
            if (currentWeapon.ProjectilePrefab)
            {
                currentWeapon.LaunchProjectile(rightHandTransform, leftHandTransform, _targetHealth, gameObject);
            }
        }
        
        
        //Set target for attack 
        
        public void SetTarget(GameObject combatTarget)
        {
            _actionScheduler.StartAction(this);
            _targetPosition = combatTarget.transform;
            _targetHealth = _targetPosition.GetComponent<Health>();
        }

        
        //Equipping weapon from Serialized field 
        
        public void EquipWeapon(Weapon weapon)
        {
            if (weapon == null) return;
            if (weapon == currentWeapon) return;
            if (_currentWeaponPrefab) Destroy(_currentWeaponPrefab);
            
            currentWeapon = weapon;
            _currentWeaponPrefab = weapon.Spawn(rightHandTransform, leftHandTransform, _animator);
        }
        

        //IAction interface method for correct work with Mover
        
        public void Cancel()
        {
            _targetPosition = null;
            _mover.Cancel();
            _animator.ResetTrigger("attack");
            _animator.SetTrigger("stopAttacking");
        }


        //Saving system methods

        public object CaptureState()
        {
            if (currentWeapon) return currentWeapon.name;
            else return defaultWeapon.name;
        }

        public void RestoreState(object state)
        {
            string savedWeapon = (string) state;
            Weapon weapon = Resources.Load<Weapon>(savedWeapon);
            EquipWeapon(weapon);
        }
    }
}
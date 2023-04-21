using System;
using System.Collections.Generic;
using RPG.Core;
using RPG.Attributes;
using RPG.Control;
using RPG.Movement;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;

namespace RPG.Combat
{
    [RequireComponent(typeof(ActionScheduler))]
    public class Fighter : MonoBehaviour, IAction, ISaveable, IModifierProvider, ILevelUpdate
    {
        //Serializable
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] WeaponConfig defaultWeaponConfig = null;
        [SerializeField] WeaponConfig currentWeaponConfig = null;
        [SerializeField] float weaponAdditionDamageBonus;
        [SerializeField] float weaponPercentageDamageBonus;

        //local variables
        float _timeSinceLastAttack = Mathf.Infinity;
        
        //Cashed
        Weapon _currentWeaponPrefab;
        Mover _mover;
        ActionScheduler _actionScheduler;
        Animator _animator;
        Transform _targetPosition;
        Health _targetHealth;
        BaseStats _baseStats;
        
        
        //Starting setup
        
        void Awake()
        {
            _mover = GetComponent<Mover>()                     ?? throw new Exception($"Missing Mover for Fighter in {gameObject.name}!");
            _actionScheduler = GetComponent<ActionScheduler>() ?? throw new Exception($"Missing ActionScheduler for Fighter in {gameObject.name}!");
            _animator = GetComponent<Animator>()               ?? throw new Exception($"Missing Animator for Fighter in {gameObject.name}!");
            _baseStats = GetComponent<BaseStats>()             ?? throw new Exception($"Missing BaseStats for Fighter in {gameObject.name}!");
            
            if (!currentWeaponConfig) EquipWeapon(defaultWeaponConfig); //TODO method in Awake - not good
        }


        //Main methods for attacking
        
        void Update()
        {
            _timeSinceLastAttack += Time.deltaTime;

            CheckIfLastTargetDead();

            if (!_targetPosition)
            {
                return;
            }
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

        void CheckIfLastTargetDead()
        {
            if (_targetHealth && _targetHealth.IsDead)
            {
                _targetHealth = null;
                _targetPosition = null;
            }
        }

        bool IsNear()
        {
            return Vector3.Distance(transform.position, _targetPosition.position) <= currentWeaponConfig.WeaponRange;
        }

        void AttackBehavior()
        {
            transform.LookAt(_targetPosition);
            if (_timeSinceLastAttack >= currentWeaponConfig.TimeBetweenAttacks && !_targetHealth.IsDead)
            {
                //this will trigger animation Hit() or Shoot() effect
                _animator.ResetTrigger("stopAttacking");
                _animator.SetTrigger("attack");
            }
        }
        
        void Hit()   //animation called method
        {
            if (!_targetHealth) return;
            _timeSinceLastAttack = 0f;
            _targetHealth.ReduceHealth(GetTotalDamage(), gameObject);
            _targetPosition?.GetComponent<AIController>().ProvokeEnemy(true);
            _currentWeaponPrefab?.OnHit();
        }

        void Shoot()  //animation called method
        {
            if (!_targetHealth) return;
            _timeSinceLastAttack = 0f;
            
            if (currentWeaponConfig.ProjectilePrefab)
            {
                currentWeaponConfig.LaunchProjectile(rightHandTransform, leftHandTransform, _targetHealth, gameObject, GetTotalDamage());
            }
        }
        
        float GetTotalDamage()
        {
            float baseDamage = _baseStats.GetAdditiveModifier(Stat.AdditionalDamageBonus);
            float percentageDamageBonus = _baseStats.GetAdditiveModifier(Stat.AdditionalDamageBonus);
            return baseDamage + baseDamage * percentageDamageBonus / 100;
        }
        
        //Set target for attack 
        
        public void SetTarget(GameObject combatTarget)
        {
            _actionScheduler.StartAction(this);
            _targetPosition = combatTarget.transform;
            _targetHealth = _targetPosition.GetComponent<Health>();
        }

        
        //Equipping weapon from Serialized field 
        
        public void EquipWeapon(WeaponConfig weaponConfig)
        {
            if (weaponConfig == null) return;
            //if (weaponConfig == currentWeaponConfig) return;
            if (_currentWeaponPrefab) Destroy(_currentWeaponPrefab);
            
            currentWeaponConfig = weaponConfig;
            _currentWeaponPrefab = weaponConfig.Spawn(rightHandTransform, leftHandTransform, _animator);
        }
        

        //IAction interface method for correct work with Mover
        
        public void Cancel()
        {
            _targetPosition = null;
            _mover.Cancel();
            _animator.ResetTrigger("attack");
            _animator.SetTrigger("stopAttacking");
        }

        
        //IModifierProvider interface method
        
        public IEnumerable<float> GetAdditiveModifier(Stat stat)
        {
            if (stat == Stat.AdditionalDamageBonus)
            {
                yield return currentWeaponConfig.WeaponDamage;
                yield return weaponAdditionDamageBonus;
            }
        }

        public IEnumerable<float> GetPercentageModifier(Stat stat)
        {
            if (stat == Stat.PercentageDamageBonus)
            {
                yield return weaponPercentageDamageBonus;
            }
        }


        //ILevelUpdate interface method

        public void SetStartLevelChanges()
        {
            weaponAdditionDamageBonus = _baseStats.GetStat(Stat.AdditionalDamageBonus);
            weaponPercentageDamageBonus = _baseStats.GetStat(Stat.PercentageDamageBonus);
        }

        public void SetNewLevelChanges()
        {
            weaponAdditionDamageBonus = _baseStats.GetStat(Stat.AdditionalDamageBonus);
            weaponPercentageDamageBonus = _baseStats.GetStat(Stat.PercentageDamageBonus);
        }

        
        //Saving system methods

        public object CaptureState()
        {
            Dictionary<string, object> fighterSave = new Dictionary<string, object>();


            if (currentWeaponConfig)
            {
                fighterSave["savedWeapon"] = currentWeaponConfig.name;
            }
            else
            {
                fighterSave["savedWeapon"] = defaultWeaponConfig.name;
            }

            if (_targetPosition == null)
            {
                fighterSave["targetID"] = "null";
            }
            else
            {
                fighterSave["targetID"] = _targetPosition.gameObject.GetComponent<SaveableEntity>().GetUniqueIdentifier();
            }

           
            return fighterSave;
        }

        public void RestoreState(object state)
        {
            Dictionary<string, object> fighterSave = (Dictionary<string, object>)state;

            //restoring _targetPosition 
            string targetID = (string)fighterSave["targetID"];

            if (targetID == "null")
            {
                _targetPosition = null;
                Cancel();
            }
            else
            {
                foreach (SaveableEntity saveable in FindObjectsOfType<SaveableEntity>())
                {
                    if (saveable.GetUniqueIdentifier() != targetID) continue;
                    _targetPosition = saveable.transform;
                }
            }

            //restoring saved weapon 
            string savedWeapon = (string)fighterSave["savedWeapon"];
            currentWeaponConfig = Resources.Load<WeaponConfig>(savedWeapon);
            EquipWeapon(currentWeaponConfig);
        }


    }
}
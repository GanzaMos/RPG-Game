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
    public class Fighter : MonoBehaviour, ISaveable, IModifierProvider, ILevelUpdate
    {
        //Serializable
        [SerializeField] Transform rightHandTransform = null;
        [SerializeField] Transform leftHandTransform = null;
        [SerializeField] public WeaponConfig defaultWeaponConfig = null;
        [SerializeField] public WeaponConfig currentWeaponConfig = null;
        [SerializeField] float weaponAdditionDamageBonus;
        [SerializeField] float weaponPercentageDamageBonus;

        //local variables
        public bool playerIsAttacking = false;
        float _timeSinceLastAttack = Mathf.Infinity;
        public float TimeSinceLastAttack
        {
            get => _timeSinceLastAttack;
            set => _timeSinceLastAttack = value;
        }

        //cashed
        Weapon _currentWeaponPrefab;
        Mover _mover;
        Animator _animator;
        BaseStats _baseStats;
        
        //target info
        Character _targetCharacter;
        public Character TargetCharacter
        {
            get => _targetCharacter;
            set => _targetCharacter = value;
        }
        
        Health _targetHealth;
        public Health TargetHealth
        {
            get => _targetHealth;
            set => _targetHealth = value;
        }

        Transform _targetPosition;
        public Transform TargetPosition
        {
            get => _targetPosition;
            set => _targetPosition = value;
        }
        
        
        //Starting setup
        
        void Awake()
        {
            _mover = GetComponent<Mover>() ?? InstError<Mover>();
            _animator = GetComponent<Animator>() ?? InstError<Animator>();
            _baseStats = GetComponent<BaseStats>() ?? InstError<BaseStats>();
        }
        
        T InstError<T>()
        {
            string className = typeof(T).Name;
            throw new Exception($"Missing {className} component for {name}, ID {GetInstanceID()}");
        }
        
        
        void Start()
        {
            if (!currentWeaponConfig) 
                EquipWeapon(defaultWeaponConfig);
        }

        
        //Main methods for attacking
        void Update()
        {
            _timeSinceLastAttack += Time.deltaTime;

            PlayerAttack();
        }


        public void StartAttack()
        {
            _animator.ResetTrigger("stopAttacking");
            _animator.SetTrigger("attack");
        }

        //Animation called method
        void Hit()   
        {
            //check if target is set
            if (!_targetHealth) return;
            
            //reset timer between attacks
            _timeSinceLastAttack = 0f;
            
            //deal damage and set Instigator;
            _targetHealth.ReduceHealth(GetTotalDamage(), gameObject);
            
            //provoke this Enemy and all Enemies nearby
            var enemyComponent = _targetPosition?.GetComponent<EnemyAIStateManager>();
            if (enemyComponent != null)
                enemyComponent.ProvokeEnemy(true);

            //cause all visual and sound effect associated with the weapon prefab
            _currentWeaponPrefab?.OnHit();
        }

        
        //Animation called method
        void Shoot()  
        {
            //Check if target is set
            if (!_targetHealth) return;
            
            //Reset timer between attacks
            _timeSinceLastAttack = 0f;
            
            //Create a projectile from Weapon prefab
            if (currentWeaponConfig.ProjectilePrefab)
                currentWeaponConfig.LaunchProjectile(
                    rightHandTransform, 
                    leftHandTransform, 
                    _targetHealth, 
                    gameObject, 
                    GetTotalDamage());
        }
        
        
        //Set target for attack 
        public void SetTarget(GameObject combatTarget)
        {
            // _actionScheduler.StartAction(this);
            _targetPosition = combatTarget.transform;
            _targetHealth = _targetPosition.GetComponent<Health>();
            _targetCharacter = _targetPosition.GetComponent<Character>();

            if (gameObject.tag == "Player")
                playerIsAttacking = true;
        }

        
        //todo temporary decision, need to make FSM for player
        void PlayerAttack()
        {
            if(!playerIsAttacking) return;

            if (!IsInWeaponRange())                       
                _mover.MoveToAttack(TargetPosition.position);
            else
            {
                transform.LookAt(TargetPosition);
                if (TimeSinceLastAttack >= currentWeaponConfig.TimeBetweenAttacks && !TargetCharacter.data.health.IsDead)
                {
                    //this will trigger animation Hit() or Shoot() event in Fighter
                    _timeSinceLastAttack = 0;
                    _animator.ResetTrigger("stopAttacking");
                    _animator.SetTrigger("attack");
                }
            }
        }
        
        
        public bool IsInWeaponRange()
        {
            return Vector3.Distance(transform.position, _targetPosition.position) <= currentWeaponConfig.WeaponRange;
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
        
        
        //IModifierProvider interface method
        float GetTotalDamage()
        {
            float baseDamage = _baseStats.GetAdditiveModifier(Stat.AdditionalDamageBonus);
            float percentageDamageBonus = _baseStats.GetAdditiveModifier(Stat.AdditionalDamageBonus);
            return baseDamage + baseDamage * percentageDamageBonus / 100;
        }

        
        //IAction interface method for correct work with Mover
        public void Cancel()
        {
            _targetPosition = null;
            _targetHealth = null;
            _mover.Cancel();
            _animator.ResetTrigger("attack");
            _animator.SetTrigger("stopAttacking");
            playerIsAttacking = false;
        }

        
        //IModifierProvider interface methods
        
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


        //ILevelUpdate interface methods

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
                fighterSave["savedWeapon"] = currentWeaponConfig.name;
            else
                fighterSave["savedWeapon"] = defaultWeaponConfig.name;

            if (_targetPosition == null)
                fighterSave["targetID"] = "null";
            else
                fighterSave["targetID"] = _targetPosition.gameObject.GetComponent<SaveableEntity>().GetUniqueIdentifier();
            
            
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
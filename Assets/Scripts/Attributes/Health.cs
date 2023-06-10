using System;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;
using UnityEngine.Events;

namespace RPG.Attributes
{
    [System.Serializable]
    public class Health : MonoBehaviour, ISaveable, ILevelUpdate
    {
        //Serializable
        [SerializeField] UnityEvent<float> reduceHealth;
        [SerializeField] UnityEvent onDie;
        
        //Cashed
        Animator _animator;
        BaseStats _baseStats;
        CapsuleCollider _capsuleCollider;
        HealthBarHandler _healthBarHandler;
        Character _ch;

       
        //Starting setup
        
        void Awake()
        {
            _ch = GetComponentInParent<Character>() ?? InstError<Character>();
            _baseStats = GetComponent<BaseStats>() ?? InstError<BaseStats>();
            _animator = GetComponent<Animator>() ?? InstError<Animator>();
            _capsuleCollider = GetComponent<CapsuleCollider>() ?? InstError<CapsuleCollider>();
            _baseStats = GetComponent<BaseStats>() ?? InstError<BaseStats>();
            _healthBarHandler = GetComponentInChildren<HealthBarHandler>() ?? InstError<HealthBarHandler>();
        }
        
        T InstError<T>()
        {
            string className = typeof(T).Name;
            if (typeof(T) is Character) throw new Exception($"Missing {className} component for {name}, ID {GetInstanceID()}");
            else throw new Exception($"Missing {className} component for {name} in {_ch?.gameObject.name}, ID {GetInstanceID()}");
        }
        

        //Methods to operate health amount

        public void ReduceHealth(float healthAmountToReduce, GameObject instigator = null)
        {
            if (_ch.data.health.IsDead) 
                return;
            
            _ch.data.health.CurrentHealth = Mathf.Max(_ch.data.health.CurrentHealth - healthAmountToReduce, 0);
            
            if (_ch.data.health.CurrentHealth <= 0)
            {
                ExperienceAward(instigator);
                onDie?.Invoke();
                UpdateUI();
                Die();
            }
            else
            {
                reduceHealth?.Invoke(healthAmountToReduce);
                UpdateUI();
            }
        }

        public void RestoreHealth(float healthPointsToRestore)
        {
            _ch.data.health.CurrentHealth = Mathf.Min(
                _ch.data.health.CurrentHealth + healthPointsToRestore,
                _ch.data.health.MaxHealth);
            UpdateUI();
        }

        public void RestoreHealthPercentage(float healthPointsPercentageToRestore)
        {
            _ch.data.health.CurrentHealth = Mathf.Min(
                _ch.data.health.MaxHealth * healthPointsPercentageToRestore + _ch.data.health.CurrentHealth,
                _ch.data.health.MaxHealth);
            UpdateUI();
        }

        public void SetCurrentHealth(float healthPointsToSet)
        {
            _ch.data.health.CurrentHealth = Mathf.Min(
                healthPointsToSet, 
                _ch.data.health.MaxHealth);
            
            if (_ch.data.health.CurrentHealth <= 0) 
                Die();
            
            UpdateUI();
        }

        public void SetMaxLevelHealth()
        {
            _ch.data.health.MaxHealth = _baseStats.GetStat(Stat.Health);
            UpdateUI();
        }


        //Private methods
        
        void ExperienceAward(GameObject instigator)
        {
            float expPerKill = _baseStats.GetStat(Stat.ExperiencePointPerKill);

            //todo need to move instigator to Data base;
            Experience instigatorExperience;  
            instigatorExperience = instigator.GetComponent<Experience>();
            if (!instigatorExperience)
            {
                print($"Missing instigators Experience in {instigator.gameObject.name} to give its ExpCost to ({gameObject.name})");
                return;
            }
            instigatorExperience.GetComponent<Experience>().GetExperience(expPerKill);
        }

        void Die(string deathTriggerName = "death")
        {
            _ch.data.health.IsDead = true;
            _animator.SetTrigger(deathTriggerName);
            GetComponent<Mover>()?.Cancel();
            GetComponent<Fighter>()?.Cancel();
            _capsuleCollider.enabled = false;
        }

        void Resurrect(string resurrectTriggerName = "instantResurrection")
        {
            _ch.data.health.IsDead = false;
            _animator.SetTrigger(resurrectTriggerName);
            GetComponent<Mover>()?.Cancel();
            GetComponent<Fighter>()?.Cancel();
            _capsuleCollider.enabled = true;
        }

        
        //Level update interface 

        public void SetStartLevelChanges()
        {
            SetMaxLevelHealth();
            SetCurrentHealth(_baseStats.GetStat(Stat.Health));
        }


        public void SetNewLevelChanges()
        {
            float oldMaxHealth = _ch.data.health.MaxHealth;
            SetMaxLevelHealth();
            
            //if character had a max health before leveling up - it should stay at the max level
            if (_ch.data.health.CurrentHealth == oldMaxHealth)
                _ch.data.health.CurrentHealth = _ch.data.health.MaxHealth;
            else
                RestoreHealthPercentage(_ch.data.health.LevelUpPercentageRestore);
            
            UpdateUI();
        }
        
        //Display event method
        
        void UpdateUI()
        {
            _healthBarHandler.SetHealthPercentage(_ch.data.health.CurrentHealth / _ch.data.health.MaxHealth);
            if (gameObject.tag != "Player") return;
            EventBusUI.OnHealthUpdated(_ch.data.health.CurrentHealth, _ch.data.health.MaxHealth);
        }


        //Saving system methods
        
        public object CaptureState()
        {
            return _ch.data.health.CurrentHealth;
        }
        
        public void RestoreState(object state)
        {
            float savedHealthPoint = (float) state;
            
            if (_ch.data.health.CurrentHealth <= 0 && savedHealthPoint > 0)
            {
                Resurrect();
                _ch.data.health.CurrentHealth = savedHealthPoint;
            }
            else if (_ch.data.health.CurrentHealth > 0 && savedHealthPoint <= 0)
            {
                Die();
                _ch.data.health.CurrentHealth = savedHealthPoint;
            }
            else
            {
                _ch.data.health.CurrentHealth = savedHealthPoint;
            }
        }
    }
}
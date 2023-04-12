using System;
using RPG.Core;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;

namespace RPG.Attributes
{
    [System.Serializable]
    public class Health : MonoBehaviour, ISaveable
    {
        //Serializable
        [SerializeField] float maxHealthPoints = 99f;
        public float MaxHealthPoints => maxHealthPoints;

        [SerializeField] float currentHealthPoints;
        public float CurrentHealthPoints => currentHealthPoints;
        
        
        //Cashed
        bool _isDead;
        public bool IsDead => _isDead;
        Animator _animator;
        BaseStats _baseStats;
        ActionScheduler _actionScheduler;
        CapsuleCollider _capsuleCollider;

       
        //Starting setup
        
        void Start()
        {
            _baseStats = GetComponent<BaseStats>() ?? throw new Exception($"Missing BaseStat for Health in {gameObject.name}!");
            if (_baseStats != null) 
            { 
                float currentLevelHealth = _baseStats.GetStat(Stat.Health);
                SetMaxHealth(currentLevelHealth);
                SetCurrentHealth(currentLevelHealth);
            }

            _animator = GetComponent<Animator>()               ?? throw new Exception($"Missing Animator for Health in {gameObject.name}!");
            _actionScheduler = GetComponent<ActionScheduler>() ?? throw new Exception($"Missing ActionScheduler for Health in {gameObject.name}!");
            _capsuleCollider = GetComponent<CapsuleCollider>() ?? throw new Exception($"Missing CapsuleCollider for Health in {gameObject.name}!");
            _baseStats = GetComponent<BaseStats>()             ?? throw new Exception($"Missing BaseStats for Health in in {gameObject.name}");
        }

        
        //Public methods to operate health amount

        public void ReduceHealth(float healthAmountToReduce, GameObject instigator = null)
        {
            if (_isDead) return;
            
            print(transform.name + " health = " + currentHealthPoints);
            currentHealthPoints = Mathf.Max(currentHealthPoints - healthAmountToReduce, 0);
            
            if (currentHealthPoints <= 0)
            {
                ExperienceAward(instigator);
                Die();
            }
        }

        public void RestoreHealth(float healthPointsToRestore)
        {
            currentHealthPoints = Mathf.Min(currentHealthPoints + healthPointsToRestore, maxHealthPoints);
        }

        public void SetCurrentHealth(float healthPointsToSet)
        {
            currentHealthPoints = Mathf.Min(healthPointsToSet, maxHealthPoints);
            if (currentHealthPoints <= 0) Die();
        }

        public void SetMaxHealth(float maxHealthPoints)
        {
            this.maxHealthPoints = maxHealthPoints;
        }


        //Private methods
        
        void ExperienceAward(GameObject instigator)
        {
            float expPerKill = _baseStats.GetStat(Stat.ExperiencePointPerKill);

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
            _isDead = true;
            _animator.SetTrigger(deathTriggerName);
            _actionScheduler.CancelCurrentAction();
            _capsuleCollider.enabled = false;
        }
        
        
        //Saving system methods
        
        public object CaptureState()
        {
            return currentHealthPoints;
        }
        
        public void RestoreState(object state)
        {
            float savedHealthPoint = (float) state;
            currentHealthPoints = savedHealthPoint;

            if (currentHealthPoints <= 0) Die("loadSave");
            else _isDead = false;
        }
    }
}
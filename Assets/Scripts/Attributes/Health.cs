using System;
using RPG.Core;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;

namespace RPG.Attributes
{
    [System.Serializable]
    public class Health : MonoBehaviour, ISaveable, ILevelUpdate
    {
        //Serializable
        [SerializeField] float maxHealthPoints = 99f;
        public float MaxHealthPoints => maxHealthPoints;

        [SerializeField] float currentHealthPoints;
        public float CurrentHealthPoints => currentHealthPoints;

        [SerializeField] const float LevelUpHealthPercentageRestore = 0.3f;
        
        //Public variables
        bool _isDead;
        public bool IsDead => _isDead;
        
        //Cashed
        Animator _animator;
        BaseStats _baseStats;
        ActionScheduler _actionScheduler;
        CapsuleCollider _capsuleCollider;

       
        //Starting setup
        
        void Awake()
        {
            _baseStats = GetComponent<BaseStats>()             ?? throw new Exception($"Missing BaseStat for Health in {gameObject.name}!");
            _animator = GetComponent<Animator>()               ?? throw new Exception($"Missing Animator for Health in {gameObject.name}!");
            _actionScheduler = GetComponent<ActionScheduler>() ?? throw new Exception($"Missing ActionScheduler for Health in {gameObject.name}!");
            _capsuleCollider = GetComponent<CapsuleCollider>() ?? throw new Exception($"Missing CapsuleCollider for Health in {gameObject.name}!");
            _baseStats = GetComponent<BaseStats>()             ?? throw new Exception($"Missing BaseStats for Health in in {gameObject.name}");
        }
        

        //Methods to operate health amount

        public void ReduceHealth(float healthAmountToReduce, GameObject instigator = null)
        {
            if (_isDead) return;
            
            print(transform.name + " health = " + currentHealthPoints);
            currentHealthPoints = Mathf.Max(currentHealthPoints - healthAmountToReduce, 0);
            SendDisplayEvent();
            
            if (currentHealthPoints <= 0)
            {
                ExperienceAward(instigator);
                Die();
            }
        }

        public void RestoreHealth(float healthPointsToRestore)
        {
            currentHealthPoints = Mathf.Min(currentHealthPoints + healthPointsToRestore, maxHealthPoints);
            SendDisplayEvent();
        }

        public void RestoreHealthPercentage(float healthPointsPercentageToRestore)
        {
            currentHealthPoints = Mathf.Min(maxHealthPoints * healthPointsPercentageToRestore + currentHealthPoints,maxHealthPoints);
            SendDisplayEvent();
        }

        public void SetCurrentHealth(float healthPointsToSet)
        {
            currentHealthPoints = Mathf.Min(healthPointsToSet, maxHealthPoints);
            if (currentHealthPoints <= 0) Die();
            SendDisplayEvent();
        }

        public void SetMaxLevelHealth()
        {
            maxHealthPoints = _baseStats.GetStat(Stat.Health);
            SendDisplayEvent();
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

        void Resurrect(string resurrectTriggerName = "instantResurrection")
        {
            _isDead = false;
            _animator.SetTrigger(resurrectTriggerName);
            _actionScheduler.CancelCurrentAction();
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
            float oldMaxHealth = maxHealthPoints;
            SetMaxLevelHealth();
            if (currentHealthPoints == oldMaxHealth)
            {
                currentHealthPoints = maxHealthPoints;
            }
            else
            {
                RestoreHealthPercentage(LevelUpHealthPercentageRestore);
            }
            SendDisplayEvent();
        }
        
        //Display event method
        
        void SendDisplayEvent()
        {
            if (gameObject.tag != "Player") return;
            EventBus.OnHealthUpdated(currentHealthPoints, maxHealthPoints);
        }


        //Saving system methods
        
        public object CaptureState()
        {
            return currentHealthPoints;
        }
        
        public void RestoreState(object state)
        {
            float savedHealthPoint = (float) state;
            
            if (currentHealthPoints <= 0 && savedHealthPoint > 0)
            {
                Resurrect();
                currentHealthPoints = savedHealthPoint;
            }
            else if (currentHealthPoints > 0 && savedHealthPoint <= 0)
            {
                Die();
                currentHealthPoints = savedHealthPoint;
            }
            else
            {
                currentHealthPoints = savedHealthPoint;
            }
        }
    }
}
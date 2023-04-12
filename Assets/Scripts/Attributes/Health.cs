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
        [SerializeField] float maxHealthPoint = 99f;
        public float MaxHealthPoint => maxHealthPoint;

        [SerializeField] float currentHealthPoint;
        public float CurrentHealthPoint => currentHealthPoint;
        
        bool _isDead;
        public bool IsDead => _isDead;

        GameObject _instigator = null;
        public GameObject Instigator
        {
            get => _instigator;
            set => _instigator = value;
        }

        void Start()
        {
            if (GetComponent<BaseStats>() == null) print("Can't find BaseStats component for Health!");
            else
            {
                float currentLevelHealth = GetComponent<BaseStats>().GetStat(Stat.Health);
                SetMaxHealth(currentLevelHealth);
                SetCurrentHealth(currentLevelHealth);
            }
        }
        
        //Public methods to operate health amount
        
        public void ReduceHealth(float healthAmountToReduce, GameObject instigator = null)
        {
            if (_isDead) return;
            
            print(transform.name + " health = " + currentHealthPoint);
            currentHealthPoint = Mathf.Max(currentHealthPoint - healthAmountToReduce, 0);
            
            if (currentHealthPoint <= 0)
            {
                ExperienceAward(instigator);
                Die();
            }
        }

        public void RestoreHealth(float healthAmountToRestore)
        {
            currentHealthPoint = Mathf.Min(currentHealthPoint + healthAmountToRestore, maxHealthPoint);
        }

        public void SetCurrentHealth(float currentHealthPoint)
        {
            this.currentHealthPoint = Mathf.Min(currentHealthPoint, maxHealthPoint);
            if (this.currentHealthPoint <= 0) Die();
        }

        public void SetMaxHealth(float maxHealthPoint)
        {
            this.maxHealthPoint = maxHealthPoint;
        }


        //Private methods

        void ExperienceAward(GameObject instigator)
        {
            float expPerKill;

            BaseStats baseStats = GetComponent<BaseStats>();
            if (baseStats)
            {
                expPerKill = GetComponent<BaseStats>().GetStat(Stat.ExperiencePointPerKill);
            }
            else
            {
                print($"Can't find BaseStat in {gameObject.name} to take Exp cost");
                return;
            }

            Experience instigatorExperience = instigator.GetComponent<Experience>();
            if (instigatorExperience)
            {
                instigatorExperience.GetComponent<Experience>().GetExperience(expPerKill);
            }
            else
            {
                print($"Can't find Experience Component in instigator ({gameObject.name}) to give it Exp cost");
                return;
            }
        }

        void Die(string deathTriggerName = "death")
        {
            _isDead = true;
            GetComponent<Animator>().SetTrigger(deathTriggerName);
            GetComponent<ActionScheduler>().CancelCurrentAction();
            GetComponent<CapsuleCollider>().enabled = false;
        }
        
        
        //Saving system components
        
        public object CaptureState()
        {
            return currentHealthPoint;
        }
        
        public void RestoreState(object state)
        {
            float savedHealthPoint = (float) state;
            currentHealthPoint = savedHealthPoint;

            if (currentHealthPoint <= 0) Die("loadSave");
            else _isDead = false;
        }
    }
}
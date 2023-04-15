using System;
using RPG.Attributes;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using RPG.Saving;
using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour, ISaveable
    {
        //Serializable
        [Range(1, 99)] [SerializeField] int currentLevel = 1;
        public int CurrentLevel => currentLevel;
        
        [SerializeField] CharacterClass characterClass;
        [SerializeField] Progression progression = null;

        //Cached
        VFXContainer _vfxContainerContainer;


        //Start setting
        
        void Awake()
        {
            _vfxContainerContainer = GetComponent<VFXContainer>();
        }

        void Start()
        {
            SendDisplayEvent();
            UpdateAllComponentsOnStart();
        }

        
        //Main work methods
        
        public float GetStat(Stat stat)
        {
            return progression.GetStat(characterClass, currentLevel, stat);
        }

        public float GetAdditiveModifier(Stat stat)
        {
            float total = 0;
            
            foreach (var provider in GetComponents<IModifierProvider>())
            {
                foreach (var modifier in provider.GetAdditiveModifier(stat))
                {
                    total += modifier;
                }
            }

            return total;
        }

        public void LevelUpCheck(float currentExp)
        {
            float nextLevelExp;

            do
            {
                //Check if the next level Exp even exist in Dictionary. It's not? Break the cycle!
                nextLevelExp = progression.GetStat(characterClass, currentLevel + 1, Stat.ExperienceToLevelUp);
                {
                    if (nextLevelExp == 0)
                    {
                        //print("Next level doesn't exist, you reached the maximum!");
                        return;
                    }
                }
                
                //Existing? Good, are we >= than it? Yes? Give the character a +1 level!
                if (currentExp >= nextLevelExp) 
                {
                    currentLevel += 1;
                    SendDisplayEvent();
                    UpdateAllComponents();
                    LevelUpEffects();
                }
                
                //Ok, now let's check, maybe we've got so many Exp, so we need to level up one more time
                nextLevelExp = progression.GetStat(characterClass, currentLevel + 1, Stat.ExperienceToLevelUp);
                
                //Double check if the next level even exist. No? Break the cycle!
                if (nextLevelExp == 0)
                {
                    //print("We gave you next level, but that's all, it's maximum!");
                    return;
                }
                
                //Next level exist? Are we still have Exp to reach it? Let's go to another level up!
            } while (currentExp >= nextLevelExp);
        }

        
        //Private methods
        
        void UpdateAllComponents()
        {
            foreach (var component in GetComponents<ILevelUpdate>())
            {
                component.SetNewLevelChanges();
            }
        }
        
        void UpdateAllComponentsOnStart()
        {
            foreach (var component in GetComponents<ILevelUpdate>())
            {
                component.SetStartLevelChanges();
            }
        }
        
        void LevelUpEffects()
        {
            print($"Level Up! Your current level is {currentLevel}!");
            Instantiate(_vfxContainerContainer.levelUpVFX, transform.position, Quaternion.Euler(-90f, 0f, 0f), transform);
        }

        
        //Updating UI methods
        
        void SendDisplayEvent()
        {
            if (gameObject.tag != "Player") return;
            EventBus.OnLevelUpdated.Invoke(currentLevel);
        }


        //Saving system
        
        public object CaptureState()
        {
            return currentLevel;
        }

        public void RestoreState(object state)
        {
            currentLevel = (int)state;
        }
    }
}
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
        [SerializeField] Progression progression = null;

        //Cached
        VFXContainer _vfxContainerContainer;
        Character _ch;


        //Start setting
        void Awake()
        {
            _ch = GetComponentInParent<Character>() ?? InstError<Character>();
            _vfxContainerContainer = GetComponent<VFXContainer>() ?? InstError<VFXContainer>();
        }

        
        T InstError<T>()
        {
            string className = typeof(T).Name;
            if (typeof(T) is Character) throw new Exception($"Missing {className} component for {name}, ID {GetInstanceID()}");
            else throw new Exception($"Missing {className} component for {name} in {_ch?.gameObject.name}, ID {GetInstanceID()}");
        }
        
        
        void Start()
        {
            SendDisplayEvent();
            UpdateAllComponentsOnStart();
        }

        
        //Main work methods
        public float GetStat(Stat stat)
        {
            return progression.GetStat(_ch.data.exp.CharacterClass, _ch.data.exp.CurrentLevel, stat);
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
                nextLevelExp = progression.GetStat(
                    _ch.data.exp.CharacterClass, 
                    _ch.data.exp.CurrentLevel + 1, 
                    Stat.ExperienceToLevelUp);

                if (nextLevelExp == 0)
                    return;
                
                //Existing? Good, are we >= than it? Yes? Give the character a +1 level!
                if (currentExp >= nextLevelExp) 
                {
                    _ch.data.exp.CurrentLevel += 1;
                    SendDisplayEvent();
                    UpdateAllComponents();
                    LevelUpEffects();
                }
                
                //Ok, now let's check, maybe we've got so many Exp, so we need to level up one more time
                nextLevelExp = progression.GetStat(
                    _ch.data.exp.CharacterClass, 
                    _ch.data.exp.CurrentLevel + 1, 
                    Stat.ExperienceToLevelUp);
                
                //Double check if the next level even exist. No? Break the cycle!
                if (nextLevelExp == 0)
                    return;

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
            print($"Level Up! Your current level is {_ch.data.exp.CurrentLevel}!");
            Instantiate(_vfxContainerContainer.levelUpVFX, transform.position, Quaternion.Euler(-90f, 0f, 0f), transform);
        }

        
        //Updating UI methods
        void SendDisplayEvent()
        {
            if (gameObject.tag != "Player") return;
            EventBusUI.OnLevelUpdated.Invoke(_ch.data.exp.CurrentLevel);
        }


        //Saving system
        public object CaptureState()
        {
            return _ch.data.exp.CurrentLevel;
        }

        public void RestoreState(object state)
        {
            _ch.data.exp.CurrentLevel = (int)state;
        }
    }
}
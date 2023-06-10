using System;
using RPG.Core;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;

namespace RPG.Attributes
{
    public class Experience : MonoBehaviour, ISaveable
    {
        //Cashed
        BaseStats _baseStats;
        Character _ch;
        
        
        //Starting setup
        
        void Awake()
        {
            _ch = GetComponentInParent<Character>() ?? InstError<Character>();
            _baseStats = GetComponent<BaseStats>() ?? InstError<BaseStats>();
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
        }

        
        //Main Methods
        
        public void GetExperience(float experienceAmount)
        {
            _ch.data.exp.CurrentExperiencePoints += experienceAmount;
            _baseStats.LevelUpCheck(_ch.data.exp.CurrentExperiencePoints);
            SendDisplayEvent();
        }

        void SendDisplayEvent()
        {
            if (gameObject.tag != "Player") return;
            EventBusUI.OnExpUpdated(_ch.data.exp.CurrentExperiencePoints);
        }


        //Saving System
        
        public object CaptureState()
        {
            return _ch.data.exp.CurrentExperiencePoints;
        }

        public void RestoreState(object state)
        {
            _ch.data.exp.CurrentExperiencePoints = (float) state;
        }
    }
}
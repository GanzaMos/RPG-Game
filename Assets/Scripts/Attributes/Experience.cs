using System;
using RPG.Saving;
using RPG.Stats;
using UnityEngine;

namespace RPG.Attributes
{
    public class Experience : MonoBehaviour, ISaveable
    {
        //Public variables
        [SerializeField] float currentExperiencePoints = 0f;
        public float CurrentExperiencePoints => currentExperiencePoints;
        
        //Cashed
        BaseStats _baseStats;
        
        
        //Starting setup
        
        void Awake()
        {
            _baseStats = GetComponent<BaseStats>() ?? throw new Exception($"Missing BaseStats for Experience in {gameObject.name}");
        }

        void Start()
        {
            SendDisplayEvent();
        }

        
        //Main Methods
        
        public void GetExperience(float experienceAmount)
        {
            currentExperiencePoints += experienceAmount;
            _baseStats.LevelUpCheck(currentExperiencePoints);
            SendDisplayEvent();
        }

        void SendDisplayEvent()
        {
            if (gameObject.tag != "Player") return;
            EventBus.OnExpUpdated(currentExperiencePoints);
        }


        //Saving System
        
        public object CaptureState()
        {
            return currentExperiencePoints;
        }

        public void RestoreState(object state)
        {
            currentExperiencePoints = (float) state;
        }
    }
}
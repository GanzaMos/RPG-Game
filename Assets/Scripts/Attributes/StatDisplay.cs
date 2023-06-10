using System;
using RPG.Combat;
using RPG.Stats;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace RPG.Attributes
{
    public class StatDisplay : MonoBehaviour
    {
        //Serializable
        [SerializeField] TMP_Text healthValue;
        [SerializeField] TMP_Text experienceValue;
        [SerializeField] TMP_Text levelValue;
        

        //event subscription

        void OnEnable()
        {
            EventBusUI.OnHealthUpdated += SetHealthValueDisplay;
            EventBusUI.OnExpUpdated += SetExperienceValueDisplay;
            EventBusUI.OnLevelUpdated += SetLevelValueDisplay;
        }

        void OnDisable()
        {
            EventBusUI.OnHealthUpdated -= SetHealthValueDisplay;
            EventBusUI.OnExpUpdated -= SetExperienceValueDisplay;
            EventBusUI.OnLevelUpdated -= SetLevelValueDisplay;
        }


        //Main methods
        void SetHealthValueDisplay(float currentHealthPoints, float maxHealthPoints)
        {
            if (healthValue == null) print("Can't find TMP component for StatDisplay.cs to display players health!");
            else healthValue.text = $"{currentHealthPoints} / {maxHealthPoints}";
        }
        
        void SetExperienceValueDisplay(float experience)
        {
            if (experienceValue == null) print("Can't find TMP component for StatDisplay.cs to display players experience!");
            else experienceValue.text = experience.ToString();
        }        
        
        void SetLevelValueDisplay(float currentLevel)
        {
            if (levelValue == null) print("Can't find TMP component for StatDisplay.cs to display players experience!");
            else levelValue.text = currentLevel.ToString();
        }
        
    }
}
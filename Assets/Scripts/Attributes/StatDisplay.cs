using System;
using RPG.Combat;
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
        
        //Cashed
        GameObject _player;
        Health _health;
        Experience _experience;
        
        
        //Setup methods
        
        void Awake()
        {
            _player = GameObject.FindGameObjectWithTag("Player") ?? throw new Exception($"Missing Player GO for {gameObject.name}!");
            _health = _player.GetComponent<Health>()             ?? throw new Exception($"Missing Health component for {gameObject.name}!");
            _experience = _player.GetComponent<Experience>()     ?? throw new Exception($"Missing Experience component for {gameObject.name}!");
        }


        //Main methods

        void Update()
        {
            SetHealthValueDisplay();
            SetExperienceValueDisplay();
        }
        
        void SetHealthValueDisplay()
        {
            if (healthValue == null)     print("Can't find TMP component for StatDisplay.cs to display players health!");
            else healthValue.text = (_health.CurrentHealthPoints / _health.MaxHealthPoints).ToString("P0");
        }
        
        void SetExperienceValueDisplay()
        {
            if (experienceValue == null) print("Can't find TMP component for StatDisplay.cs to display players experience!");
            else experienceValue.text = _experience.CurrentExperiencePoints.ToString();
        }
        
    }
}
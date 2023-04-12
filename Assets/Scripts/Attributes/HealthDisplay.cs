using System;
using TMPro;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace RPG.Attributes
{
    public class HealthDisplay : MonoBehaviour
    {
        [SerializeField] TMP_Text healthValue;
        
        Health _health;

        
        //Setup methods
        
        void Awake()
        {
            _health = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
            
            if (_health == null) print("Can't find Health component for healthDisplay!");
            else SetHealthValueDisplay(_health.MaxHealthPoint);
        }


        //Main methods

        void Update()
        {
            SetHealthValueDisplay(_health.CurrentHealthPoint);
        }

        void SetHealthValueDisplay(float currentHealthPoint)
        {
            if (healthValue == null) print("Can't find TMP component to display health!");
            else healthValue.text = (currentHealthPoint / _health.MaxHealthPoint).ToString("P0");
        }
    }
}
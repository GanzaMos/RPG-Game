using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using RPG.Control;
using UnityEngine;

namespace RPG.Combat
{
    public class HealthPickup : BasePickup, IRaycastable
    {
        [SerializeField] float healthHealAmount = 30f;

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag != "Player") return;

            var health = other.GetComponent<Health>();
            if (health != null)
            {
                health.RestoreHealth(healthHealAmount);
                StartCoroutine(HideForSeconds(secondsToRestore));
            }
            else
            {
                Debug.LogError($"Can't find Health for HealthPickup OnTriggerEnter in ID {GetInstanceID()}");
            }
        }
    }
}
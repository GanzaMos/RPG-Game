using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using RPG.Control;
using UnityEngine;

namespace RPG.Combat
{
    public class HealthPickup : MonoBehaviour, IRaycastable
    {
        [SerializeField] float healthHealAmount = 30f;
        [SerializeField] float secondsToRestore = 10f;
        
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
        
        IEnumerator HideForSeconds(float secondsToRespawn)
        {
            SetPrefabPickupActiveStatus(false);
            yield return new WaitForSeconds(secondsToRespawn);
            SetPrefabPickupActiveStatus(true);
        }

        void SetPrefabPickupActiveStatus(bool isActive)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(isActive);
            }

            var collider = GetComponent<Collider>();
            if (collider != null)
                collider.enabled = isActive;
            else
                Debug.LogError($"Can't find Collider for HealthPickup SetPrefabPickupActiveStatus in ID {GetInstanceID()}");
        }

        public EInteractType HandleRaycast()
        {
            return EInteractType.Pickup;
        }
    }
}
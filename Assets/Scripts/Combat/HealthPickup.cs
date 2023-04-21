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
            
            other.GetComponent<Health>().RestoreHealth(healthHealAmount);
            StartCoroutine(HideForSeconds(secondsToRestore));
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

            GetComponent<Collider>().enabled = isActive;
        }

        public InteractType HandleRaycast()
        {
            return InteractType.Pickup;
        }
    }
}
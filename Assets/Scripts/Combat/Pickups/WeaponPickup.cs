using System;
using System.Collections;
using RPG.Control;
using UnityEngine;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour, IRaycastable
    {
        [SerializeField] WeaponConfig pickupWeaponConfig;
        [SerializeField] float secondsToRespawn = 5f;

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag != "Player") return;

            var fighter = GetComponent<Fighter>();

            if (fighter != null)
            {
                fighter.EquipWeapon(pickupWeaponConfig);
                StartCoroutine(HideForSeconds(secondsToRespawn));
            }
            else
            {
                Debug.LogError($"Missing Fighter in {other.name} for WeaponPickup, ID {GetInstanceID()}");
            }
        }   

        IEnumerator HideForSeconds(float secondsToRespawn)
        {
            SetWeaponPickupActiveStatus(false);
            yield return new WaitForSeconds(secondsToRespawn);
            SetWeaponPickupActiveStatus(true);
        }

        void SetWeaponPickupActiveStatus(bool isActive)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(isActive);
            }

            GetComponent<Collider>().enabled = isActive;
        }

        public EInteractType HandleRaycast()
        {
            return EInteractType.Pickup;
        }
    }
    
}
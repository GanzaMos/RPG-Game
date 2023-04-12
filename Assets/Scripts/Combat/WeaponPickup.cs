using System;
using System.Collections;
using UnityEngine;

namespace RPG.Combat
{
    public class WeaponPickup : MonoBehaviour
    {
        [SerializeField] Weapon pickupWeapon;
        [SerializeField] float secondsToRespawn = 5f;

        void OnTriggerEnter(Collider other)
        {
            //print("collided");
            if (other.gameObject.tag == "Player")
            {
                //print("it's player");
                other.GetComponent<Fighter>().EquipWeapon(pickupWeapon);
                StartCoroutine(HideForSeconds(secondsToRespawn));
            }
        }

        IEnumerator HideForSeconds(float secondsToRespawn)
        {
            StatusSwitcher(false);
            yield return new WaitForSeconds(secondsToRespawn);
            StatusSwitcher(true);
        }

        void StatusSwitcher(bool isActive)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(isActive);
            }

            GetComponent<Collider>().enabled = isActive;
        }
    }
    
}
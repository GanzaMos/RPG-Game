using System;
using System.Collections;
using RPG.Control;
using UnityEngine;

namespace RPG.Combat
{
    public class WeaponPickup : BasePickup, IRaycastable
    {
        [SerializeField] WeaponConfig pickupWeaponConfig;

        void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.tag != "Player") return;

            var fighter = GetComponent<Fighter>();

            if (fighter != null)
            {
                fighter.EquipWeapon(pickupWeaponConfig);
                StartCoroutine(HideForSeconds(secondsToRestore));
            }
            else
            {
                Debug.LogError($"Missing Fighter in {other.name} for WeaponPickup, ID {GetInstanceID()}");
            }
        }
    }
    
}
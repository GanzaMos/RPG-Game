using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{
    public abstract class BasePickup : MonoBehaviour
    {
        [SerializeField] protected float secondsToRestore = 10f;
        
        protected IEnumerator HideForSeconds(float secondsToRespawn)
        {
            SetPrefabPickupActiveStatus(false);
            yield return new WaitForSeconds(secondsToRespawn);
            SetPrefabPickupActiveStatus(true);
        }
        
        protected void SetPrefabPickupActiveStatus(bool isActive)
        {
            foreach (Transform child in transform)
            {
                child.gameObject.SetActive(isActive);
            }

            var collider = GetComponent<Collider>();
            if (collider != null)
                collider.enabled = isActive;
            else
                Debug.LogError($"Can't find Collider for pickup in ID {GetInstanceID()}");
        }
        
        public virtual EInteractType HandleRaycast()
        {
            return EInteractType.Pickup;
        }
    }
}
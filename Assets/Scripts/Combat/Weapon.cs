using RPG.Core;
using Unity.VisualScripting;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.XR;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "RPG Project/Weapon", order = 0)]
    public class Weapon : ScriptableObject

    {
        [SerializeField] AnimatorOverrideController animatorOverrideController = null;
        
        [SerializeField] GameObject weaponPrefab = null;
        [SerializeField] HandType handType = HandType.RightHand;
        
        [SerializeField] Projectile projectile = null;
        [SerializeField] bool projectileIsHoming = false;
        
        [SerializeField] float weaponRange = 2f;
        public float WeaponRange => weaponRange;
        
        [SerializeField] float timeBetweenAttacks = 1f;
        public float TimeBetweenAttacks => timeBetweenAttacks;
        
        [SerializeField] float weaponDamage = 10f;
        public float WeaponDamage => weaponDamage;

        
        public GameObject Spawn(Transform rightHandTransform,Transform leftHandTransform, Animator animator)
        {
            // AnimatorOverrideController overrideController;
            //
            // if (animatorOverrideController != null)
            // {
            //     overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            //     animator.runtimeAnimatorController = animatorOverrideController; 
            // }
            // else if (overrideController != null)
            // {
            //     animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            // }
            
            if (animator != null)
            {
                animator.runtimeAnimatorController = animatorOverrideController;
            }
            
            if (weaponPrefab != null)
            {
                switch (handType)
                {
                    case HandType.RightHand: return Instantiate(weaponPrefab, rightHandTransform);
                    case HandType.LeftHand:  return Instantiate(weaponPrefab, leftHandTransform);
                }
            }
            return null;
        }

        public bool HasProjectile()
        {
            return projectile;
        }

        public void LaunchProjectile(Transform rightHandTransform, Transform leftHandTransform, Health health)
        {
            Projectile projectileInstance = projectileInstance = Instantiate(projectile, leftHandTransform.position, Quaternion.identity);
            projectileInstance.SetTarget(health);
            projectileInstance.SetRotation();
            projectileInstance.projectileDamage = weaponDamage;
            projectileInstance.projectileIsHoming = projectileIsHoming;
        }

        enum HandType
        {
            LeftHand,
            RightHand
        }
    }
}
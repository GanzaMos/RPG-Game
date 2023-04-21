using System;
using RPG.Core;
using RPG.Attributes;
using UnityEngine;
using UnityEngine.XR;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "Weapon", menuName = "RPG Project/Weapon", order = 0)]
    public class WeaponConfig : ScriptableObject

    {
        //Serializable
        [SerializeField] AnimatorOverrideController animatorOverrideController = null;
        
        [SerializeField] Weapon weaponPrefab = null;
        [SerializeField] WeaponHand weaponHand = WeaponHand.RightHand;
        
        [SerializeField] Projectile projectilePrefab = null;
        public Projectile ProjectilePrefab => projectilePrefab;
        
        [SerializeField] bool projectileIsHoming = false;
        public bool ProjectileIsHoming => projectileIsHoming;
        
        [SerializeField] float weaponRange = 2f;
        public float WeaponRange => weaponRange;

        [SerializeField] float timeBetweenAttacks = 1f;
        public float TimeBetweenAttacks => timeBetweenAttacks;
        
        [SerializeField] float weaponDamage = 10f;
        public float WeaponDamage => weaponDamage;
        
        
        //Spawning Weapon SO instance
        
        public Weapon Spawn(Transform rightHandTransform,Transform leftHandTransform, Animator animator)
        {
            animator.runtimeAnimatorController = animatorOverrideController ?? throw new Exception($"Missing default character Animator from {animator.gameObject.name} for Weapon spawn!");

            if (weaponPrefab)
            {
                switch (weaponHand)
                {
                    case WeaponHand.RightHand: return Instantiate(weaponPrefab.gameObject, rightHandTransform).GetComponent<Weapon>();
                    case WeaponHand.LeftHand:  return Instantiate(weaponPrefab.gameObject, leftHandTransform).GetComponent<Weapon>();
                }
            }
            return null;
        }

        
        //Spawning Weapon projectiles when attacking
        
        public void LaunchProjectile(Transform rightHandTransform, Transform leftHandTransform, Health health, GameObject instigator, float damage)
        {
            Projectile projectileInstance = projectileInstance = Instantiate(projectilePrefab, leftHandTransform.position, Quaternion.identity);
            projectileInstance.SetTarget(health, instigator);
            projectileInstance.ProjectileDamage = damage;
            projectileInstance.ProjectileIsHoming = projectileIsHoming;
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Core;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    [SerializeField] Transform target;
    [SerializeField] float speed = 20f;
    [SerializeField] GameObject hitEffect = null;
    
    [SerializeField] GameObject[] objectsToDestroyInstantly;
    [SerializeField] float timeToDestroyAllProjectile;

    public float projectileDamage;
    public bool projectileIsHoming;

    Health _health;
    CapsuleCollider _targetCapsuleCollider;
    Vector3 _targetMiddlePoint;

    void Update()
    {
        if (_health == null) return;
        transform.Translate(speed * Time.deltaTime * Vector3.forward);

        if (projectileIsHoming) SetRotation();
    }
    
    public void SetTarget(Health health)
    {
        _health = health;
        _targetCapsuleCollider = _health.transform.GetComponent<CapsuleCollider>();
    }   
    
    public void SetRotation()
    {
        _targetMiddlePoint = _health.transform.position + Vector3.up * (_targetCapsuleCollider.height / 3 * 2);
        transform.LookAt(_targetMiddlePoint);
    }
    
    
    void OnTriggerEnter(Collider other)
    {
        Health otherHealthComponent = other.GetComponent<Health>();
        
        if (otherHealthComponent)
        {
            print("Arrow has damage " + projectileDamage);
            otherHealthComponent.TakeDamage(projectileDamage);
        }

        if (hitEffect != null)
        {
            Instantiate(hitEffect, _targetMiddlePoint, Quaternion.identity);
        }

        foreach (var objectToDestroy in objectsToDestroyInstantly)
        {
            Destroy(objectToDestroy);
        }
        
        Destroy(gameObject, timeToDestroyAllProjectile);
    }
}

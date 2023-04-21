using System;
using System.Collections;
using System.Collections.Generic;
using RPG.Attributes;
using RPG.Combat;
using RPG.Control;
using RPG.Core;
using UnityEngine;
using UnityEngine.Events;

public class Projectile : MonoBehaviour
{
    //Serializable
    [SerializeField] Transform target;
    [SerializeField] float speed = 20f;
    [SerializeField] GameObject hitEffect = null;
    [SerializeField] GameObject[] objectsToDestroyInstantly;
    [SerializeField] float timeToDestroyAllProjectile;
    [SerializeField] UnityEvent onHit;

    //Public variables
    float _projectileDamage;
    public float ProjectileDamage
    {
        get => _projectileDamage;
        set => _projectileDamage = value;
    }
    
    bool _projectileIsHoming;
    public bool ProjectileIsHoming
    {
        get => _projectileIsHoming;
        set => _projectileIsHoming = value;
    }

    //Cashed
    Health _health;
    CapsuleCollider _targetCapsuleCollider;
    Vector3 _targetMiddlePoint;
    GameObject _instigator;

    
    //Main work methods
    
    void Update()
    {
        if (_health == null) return;
        transform.Translate(speed * Time.deltaTime * Vector3.forward);

        if (_projectileIsHoming) SetRotation();
    }
    
    void SetRotation()
    {
        _targetMiddlePoint = _health.transform.position + Vector3.up * (_targetCapsuleCollider.height / 3 * 2);
        transform.LookAt(_targetMiddlePoint);
    }
    
    void OnTriggerEnter(Collider other)
    {
        Health targetHealth = other.GetComponent<Health>();
        if (!targetHealth)
        {
            print($"Missing target Health for {gameObject.name} Projectile!");
            return;
        }
        
        if (targetHealth)
        {
            targetHealth.ReduceHealth(_projectileDamage, _instigator);
            targetHealth.transform.gameObject.GetComponent<AIController>().ProvokeEnemy(true);
            onHit?.Invoke();
        }

        if (hitEffect)
        {
            Instantiate(hitEffect, _targetMiddlePoint, Quaternion.identity);
        }

        foreach (var objectToDestroy in objectsToDestroyInstantly)
        {
            Destroy(objectToDestroy);
        }
        
        Destroy(gameObject, timeToDestroyAllProjectile);
    }
    
    
    //Public methods for setup Projectile instance
    
    public void SetTarget(Health health, GameObject instigator)
    {
        _health = health;
        _instigator = instigator;
        _targetCapsuleCollider = _health.transform.GetComponent<CapsuleCollider>() ?? throw new Exception($"Missing CapsuleCollider for Projectile in {gameObject.name}!");
        SetRotation();
    }

}

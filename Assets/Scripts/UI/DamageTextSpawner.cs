using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using TMPro;
using UnityEngine;

public class DamageTextSpawner : MonoBehaviour
{

    [SerializeField] GameObject damageTextPrefab;

    public void SpawnDamageText(float damageAmount)
    {
        var damageTextInstance = Instantiate(damageTextPrefab, transform);
        damageTextInstance.GetComponentInChildren<TMP_Text>().text = Mathf.Ceil(damageAmount).ToString();
    }
}

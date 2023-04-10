using System;
using UnityEngine;

namespace RPG.Core
{
    public class DestroyAfterTime : MonoBehaviour
    {
        [SerializeField] float timeToDestroy;

        void Awake()
        {
            Destroy(gameObject, timeToDestroy);
        }
    }
}
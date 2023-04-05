using System;
using UnityEngine;

namespace RPG.Core
{
    public class PersistantObjectSpawner : MonoBehaviour
    {
        [SerializeField] GameObject persistantObjectPrefab;

        static bool hasSpawned = false;

        void Awake()
        {
            if (hasSpawned) return;

            SpawnPersistentObject();
            
            hasSpawned = true;
        }

        void SpawnPersistentObject()
        {
            DontDestroyOnLoad(Instantiate(persistantObjectPrefab));
        }
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;

namespace AI.Sensors
{
    public class DetectableTargetManager : MonoBehaviour
    {
        public static DetectableTargetManager Instance { get; private set; } = null;

        public List<DetectableTarget> AllTargets { get; private set; } = new List<DetectableTarget>();

        void Awake()
        {
            if (Instance != null)
            {
                Debug.LogError("Already exist another DetectableTargetManager instance! Destroying " + gameObject.name);
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public void Register(DetectableTarget target)
        {
            AllTargets.Add(target);
        }
        
        public void Deregister(DetectableTarget target)
        {
            AllTargets.Remove(target);
        }
    }
}
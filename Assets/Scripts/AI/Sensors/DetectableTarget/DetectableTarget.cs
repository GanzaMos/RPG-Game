using System;
using RPG.Attributes;
using RPG.Control;
using RPG.Core;
using RPG.Stats;
using UnityEngine;
using UnityEngine.UIElements.Experimental;

namespace AI.Sensors
{
    public class DetectableTarget : MonoBehaviour
    {
        
        //Public variables
        public float AwarenessRatePerSecond { get; set; }
        public bool IsDead { get; set; }
        public bool IsProvoked { get; set; }
        public EStartState StartState { get; set; }
        public CharacterClass CharacterClass { get; set; }
        public Vector3 StartPosition { get; set; }
        public int ChID { get; set; }
        
        //Cached
        public Character ch;
        EnemyAIStateManager _stateManager;
        
        //Main methods
        void Awake()
        {
            ch = GetComponentInParent<Character>() ?? InstError<Character>();
            ChID = ch.GetInstanceID();
        }
        
        T InstError<T>()
        {
            string className = typeof(T).Name;
            if (typeof(T) is Character) throw new Exception($"Missing {className} component for {name}, ID {GetInstanceID()}");
            else throw new Exception($"Missing {className} component for {name} in {ch?.gameObject.name}, ID {GetInstanceID()}");
        }

        void Start()
        {
            _stateManager = GetComponent<EnemyAIStateManager>();
            
            AwarenessRatePerSecond = ch.data.awareness.RatePerSecond;
            IsDead = ch.data.health.IsDead;
            IsProvoked = ch.data.ai.IsProvoked;
            StartPosition = transform.position;
            CharacterClass = ch.data.exp.CharacterClass;
            
            if (_stateManager != null)
            {
                if (_stateManager.PatrolPath != null)
                    StartState = EStartState.Patrolling;
                else
                    StartState = EStartState.Idling;
            }
            else
            {
                StartState = EStartState.None;
            }

            RegisterItself();
        }
        
        void OnDisable()
        {
            DeregisterItself();
        }
        
        void RegisterItself()
        {
            if (DetectableTargetManager.Instance == null)
            {
                Debug.LogError(gameObject.name + " can't find DetectableTargetManager Instance for register itself");
                return;
            }

            DetectableTargetManager.Instance.Register(this);
        }
        
        void DeregisterItself()
        {
            if (DetectableTargetManager.Instance == null)
            {
                Debug.LogError(gameObject.name + " can't find DetectableTargetManager Instance for deregister itself");
                return;
            }

            DetectableTargetManager.Instance.Deregister(this);
        }
    }
}
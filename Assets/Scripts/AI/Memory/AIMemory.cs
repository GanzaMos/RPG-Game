using System;
using System.Collections.Generic;
using AI.Sensors;
using RPG.Attributes;
using RPG.Control;
using RPG.Core;
using RPG.Stats;
using UnityEngine;

namespace AI
{
    public class AIMemory : MonoBehaviour
    {
        //For all seen (with vision sensor) targets.
        //Seen = level 3 awareness for threatsInVisionRadius.
        //Seen = instantly for non-threatsInVisionRadius targets.
        CharacterDataLists.SeenTargets _seenTargets;

        [SerializeField] bool sendDebugEventsToTargetsDetector = true;

        //Cached
        Character _ch;

        //Cashed list
        List<DetectableTarget> _inVisionField;
        List<DetectableTarget> _threatsInVisionRadius;
        Dictionary<DetectableTarget, TrackedTarget> _awarenessTargets;
        
        //Locals
        DetectableTarget _target;
        float _timer1;
        float _timer2;
        float _timerPeriod = 0.1f;

        //Start setup
        void Awake()
        {
            _ch = GetComponentInParent<Character>() ?? InstError<Character>();

            _inVisionField = _ch.DataLists.InVisionField;
            _threatsInVisionRadius = _ch.DataLists.ThreatsInVisionRadius;
            _awarenessTargets = _ch.DataLists.AwarenessTargets;

            _timer1 = _timer2 =_timerPeriod;
            _seenTargets = _ch.DataLists.seenTargets;
        }
        
        T InstError<T>()
        {
            string className = typeof(T).Name;
            if (typeof(T) is Character)
                throw new Exception($"Missing {className} component for {name}, ID {GetInstanceID()}");
            else
                throw new Exception($"Missing {className} component for {name} in {_ch?.gameObject.name}, ID {GetInstanceID()}");
        }
        
        #region Seen Targets list methods
        
        void Update()
        {
            UpdateTimers();
            
            //Updating for non-threats targets instantly
            UpdateNonThreatSeenMemory();
            
            //Updating for threats targets when awareness reaches level >= 2
            UpdateThreatSeenMemory();
        }

        
        void UpdateTimers()
        {
            _timer1 += Time.deltaTime;
            _timer2 += Time.deltaTime;
        }

        
        void UpdateNonThreatSeenMemory()
        {
            //Only timer
            if (!CanUpdateForNonThreat()) return;

            //We check only targets, which we see now 
            for (int i = 0; i < _inVisionField.Count; i++)
            {
                _target = _inVisionField[i];
                
                //skip all threat targets
                if (_threatsInVisionRadius.Contains(_target)) return;

                UpdateSeenMemory(_target);
                SendEventToAIDebugDetector(_target);
            }
        }
        

        bool CanUpdateForNonThreat()
        {
            if (_timer1 >= _timerPeriod)
            {
                _timer1 = 0;
                return true;
            }
            return false;
        }

        
        void UpdateThreatSeenMemory()
        {
            //timer OR isThreatIsNear flag
            if (!CanUpdateForThreat()) return;
            
            //We check only targets, which we see now 
            for (int i = 0; i < _inVisionField.Count; i++)
            {
                _target = _inVisionField[i];
                
                //skip all non-threat targets
                if (!_threatsInVisionRadius.Contains(_target)) continue; 
                
                //skip target if it isn't in the Awareness List
                if (!_awarenessTargets.ContainsKey(_target)) continue;
                
                //skip target if it's Awareness level < 2
                if (_awarenessTargets[_target].Awareness < 2f) continue;
                
                UpdateSeenMemory(_target);
                SendEventToAIDebugDetector(_target);
            }
        }

        bool CanUpdateForThreat()
        {
            if (_timer2 >= _timerPeriod || _ch.data.awareness.ThreatInNear)
            {
                _timer2 = 0;
                return true;   
            }
            return false;
        }
        
        
        void UpdateSeenMemory(DetectableTarget target)
        {
            if (!_seenTargets.ContainsKey(target))
            {
                StartSeenMemorySetup(target);
                SeenMemoryUpdate(target);
            }
            else
            {
                SeenMemoryUpdate(target);
            }
        }

        
        void StartSeenMemorySetup(DetectableTarget target)
        {
            _seenTargets[target] = new MemoryDetails();
            _seenTargets[target].CharacterClass = target.CharacterClass;
            _seenTargets[target].StartPosition = target.StartPosition;
            _seenTargets[target].StartState = target.StartState;
            SetNeedToCheckFromStart(target);
        }
        
        
        void SetNeedToCheckFromStart(DetectableTarget target)
        {
            //For characters who had been already DEAD at the first time when they was spotted
            if (target.IsDead && target.CharacterClass != CharacterClass.Player)
                _seenTargets[target].NeedToCheckFromStart = true;

            //For characters who had been already PROVOKED at the first time when they was spotted
            if (target.IsProvoked && target.CharacterClass != CharacterClass.Player)
                _seenTargets[target].NeedToCheckFromStart = true;
        }
        
        
        void SeenMemoryUpdate(DetectableTarget target)
        {
            _seenTargets[target].IsDead = target.IsDead;
            _seenTargets[target].IsProvoked = target.IsProvoked;
            _seenTargets[target].LastKnownPosition = target.transform.position;
        }
        
        
        void SendEventToAIDebugDetector(DetectableTarget target)
        {
            if (!sendDebugEventsToTargetsDetector) return;
            if (!_seenTargets.ContainsKey(target)) return;
            
            target.ch.Events.UpdateAIMemoryDetectorInfo?.Invoke(_ch.detectableTarget, _seenTargets[target]);
        }
        
        #endregion
    }
}
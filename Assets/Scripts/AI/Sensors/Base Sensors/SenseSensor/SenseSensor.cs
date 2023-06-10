using System;
using System.Collections.Generic;
using RPG.Attributes;
using UnityEngine;

namespace AI.Sensors
{
    public class SenseSensor : BaseSensor
    {
        [SerializeField] LayerMask detectionMask = ~0;

        //Locals
        List<DetectableTarget> _lastTickVisualField = new List<DetectableTarget>();
        DetectableTarget _target;
        
        //Timer variables
        float _timerPeriod = 0.2f;
        float _timer = 0;

        protected override void Awake()
        {
            base.Awake();
        }

        void Update()
        {
            _timer += Time.deltaTime;
            if (!CanPerformMainLogic()) return;
            _timer = 0;
            
            UpdateSenseFieldTargets();
            SenseFieldInOutCheck();
        }

        void UpdateSenseFieldTargets()
        {
            InSenseField.Clear();

            foreach (var target in InSenseRadius)
            {
                RaycastHit hitResult;
                var vectorToTarget = target.transform.position - transform.position;

                if (!base.CanHitCollider(vectorToTarget, target, out hitResult, detectionMask, Ch.data.sense.Radius)) continue;
                if (!base.IsInLineOfSight(hitResult, target)) continue;

                InSenseField.Add(target);

                if (!ThreatsInVisionRadius.Contains(target)) continue;

                Awareness.UpdateAwareness(
                       target: target,
                     position: target.transform.position,
                    direction: vectorToTarget,
                    awareness: target.AwarenessRatePerSecond *
                               target.ch.data.awareness.SenseFactor *
                               Time.deltaTime);
            }
        }


        void SenseFieldInOutCheck()
        {
            for (int i = 0; i < InVisionRadius.Count; i++)
            {
                _target = InVisionRadius[i];
                
                //For targets which disappeared from Sense Field 
                if (!InSenseField.Contains(_target) && _lastTickVisualField.Contains(_target))
                {
                    Ch.Events.UpdateSeenMemory?.Invoke(_target);
                    _target.ch.Events.OutSenseFieldDetection?.Invoke(Ch.detectableTarget);
                }
                //For targets which appeared in Sense Field 
                else if (InSenseField.Contains(_target) && !_lastTickVisualField.Contains(_target))
                {
                    _target.ch.Events.OnSenseFieldDetection?.Invoke(Ch.detectableTarget);
                }
            }

            //clear list with all targets from the last frame
            _lastTickVisualField.Clear();
            
            //refill this list with new targets in Vision Field
            for (int i = 0; i < InSenseField.Count; i++)
                _lastTickVisualField.Add(InSenseField[i]);
        }
        
        bool CanPerformMainLogic()
        {
            return _timer >= _timerPeriod || Ch.data.awareness.ThreatInNear;
        }
    }
}
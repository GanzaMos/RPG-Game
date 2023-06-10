using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using RPG.Attributes;
using RPG.Control;
using RPG.Stats;
using TMPro.EditorUtilities;
using UI;
using UnityEditor;
using UnityEngine;

namespace AI.Sensors
{
    public class VisionSensor: BaseSensor
    {
        //Manageable parameters
        [SerializeField] LayerMask detectionMask = ~0;
        [SerializeField] AnimationCurve visionCurve;

        //Local
        List<DetectableTarget> _lastTickVisualField = new List<DetectableTarget>();
        DetectableTarget _target;
        
        //Timer variables
        float updateTimerPeriod = 0.2f;
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

            UpdateVisionFieldTargets();
        }

        void UpdateVisionFieldTargets()
        {
            InVisionField.Clear();
            for (int i = 0; i < InVisionRadius.Count; i++)
            {
                RaycastHit hitResult;
                var target = InVisionRadius[i];
                var vectorToTarget = target.transform.position - transform.position;
                
                if (!IsInHorizontalAngle(vectorToTarget)) continue;
                if (!IsInVerticalAngle(vectorToTarget, target)) continue;
                if (!base.CanHitCollider(vectorToTarget, target, out hitResult, detectionMask, Ch.data.vision.Radius)) continue;
                if (!base.IsInLineOfSight(hitResult, target)) continue;
                
                InVisionField.Add(target);

                if (!ThreatsInVisionRadius.Contains(target)) continue;

                float awarenessCurveFactor = visionCurve.Evaluate(vectorToTarget.magnitude / Ch.data.vision.Radius);
                Awareness.UpdateAwareness(
                       target: target,
                     position: target.transform.position,
                    direction: vectorToTarget,
                    awareness: target.AwarenessRatePerSecond *
                               awarenessCurveFactor *
                               Time.deltaTime);
            }
        }


        bool CanPerformMainLogic()
        {
            return _timer >= updateTimerPeriod || Ch.data.awareness.ThreatInNear;
        }

        
        bool IsInHorizontalAngle(Vector3 vectorToTarget)
        {
            Vector3 horizontalVector = new Vector3(vectorToTarget.x, 0, vectorToTarget.z).normalized;
            if (Vector3.Dot(horizontalVector, transform.forward) > Ch.data.vision.HorizontalAngleCos)
                return true;
            return false;
        }
        
        
        bool IsInVerticalAngle(Vector3 vectorToTarget, DetectableTarget potentialTarget)
        {
            Vector3 verticalVector = new Vector3(0, vectorToTarget.y, vectorToTarget.z).normalized;
            
            //check vertical angle first
            //check height range between top and bottom character point second
            return (Vector3.Dot(verticalVector, transform.forward) > Ch.data.vision.VerticalAngleCos ||
                    potentialTarget.transform.position.y <= transform.position.y + Ch.data.vision.TopVertex &&
                    potentialTarget.transform.position.y >= transform.position.y + Ch.data.vision.BottomVertex);
        }
        
        void VisionFieldInOutCheck()
        {
            for (int i = 0; i < EdgeList.Count; i++)
            {
                _target = EdgeList[i];
                
                //For targets which disappeared from Vision Field 
                if (!InVisionField.Contains(_target) && _lastTickVisualField.Contains(_target))
                {
                    Ch.Events.UpdateSeenMemory?.Invoke(_target);
                    _target.ch.Events.OutVisualFieldDetection?.Invoke(Ch.detectableTarget);
                }
                //For targets which appeared in Vision Field 
                else if (InVisionField.Contains(_target) && !_lastTickVisualField.Contains(_target))
                {
                    _target.ch.Events.OnVisualFieldDetection?.Invoke(Ch.detectableTarget);
                }
            }

            //clear list with all targets from the last frame
            _lastTickVisualField.Clear();
            
            //refill this list with new targets in Vision Field
            for (int i = 0; i < InVisionField.Count; i++)
                _lastTickVisualField.Add(InVisionField[i]);
        }
    }
}
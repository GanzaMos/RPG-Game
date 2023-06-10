using System;
using System.Collections.Generic;
using RPG.Attributes;
using UnityEngine;

namespace AI.Sensors
{
    public abstract class BaseSensor : MonoBehaviour
    {
        protected Character Ch;
        protected AwarenessSystem Awareness;

        protected List<DetectableTarget> EdgeList { get; set; }
        protected List<DetectableTarget> InHearingRadius { get; set; }
        protected List<DetectableTarget> InVisionRadius { get; set; }
        protected List<DetectableTarget> InVisionField { get; set; }
        protected List<DetectableTarget> InSenseRadius { get; set; }
        protected List<DetectableTarget> InSenseField { get; set; }
        protected List<DetectableTarget> ThreatsInVisionRadius { get; set; }

        protected int ChID;

        protected virtual void Awake()
        {
            Ch = GetComponentInParent<Character>() ?? InstError<Character>();
            Awareness = GetComponentInParent<AwarenessSystem>() ?? InstError<AwarenessSystem>();

            EdgeList = Ch.DataLists.EdgeList;
            InHearingRadius = Ch.DataLists.InHearingRadius;
            InVisionRadius = Ch.DataLists.InVisionRadius;
            InVisionField = Ch.DataLists.InVisionField;
            InSenseRadius = Ch.DataLists.InSenseRadius;
            InSenseField = Ch.DataLists.InSenseField;
            ThreatsInVisionRadius = Ch.DataLists.ThreatsInVisionRadius;

            ChID = Ch.GetInstanceID();
        }
        
        protected T InstError<T>()
        {
            string className = typeof(T).Name;
            if (typeof(T) is Character)
                throw new Exception($"Missing {className} component for {name}, ID {GetInstanceID()}");
            else
                throw new Exception($"Missing {className} component for {name} in {Ch?.gameObject.name}, ID {GetInstanceID()}");
        }
        
        protected bool InRadius(Vector3 vectorToTarget, float sqrDistance)
        {
            return vectorToTarget.sqrMagnitude <= sqrDistance;
        }

        protected bool CanHitCollider(Vector3 vectorToTarget, DetectableTarget potentialTarget, out RaycastHit hit, LayerMask detectionMask, float distance)
        {
            return Physics.Raycast(
                origin: transform.position,
                direction: vectorToTarget,
                hitInfo: out hit,
                maxDistance: distance,
                layerMask: detectionMask,
                queryTriggerInteraction: QueryTriggerInteraction.Collide);
        }
        
        protected bool IsInLineOfSight(RaycastHit hitResult, DetectableTarget potentialTarget)
        {
            return hitResult.collider.GetComponent<DetectableTarget>() == potentialTarget;
        }
    }
}
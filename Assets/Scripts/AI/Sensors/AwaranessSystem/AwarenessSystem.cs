using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using AI.Sensors;
using RPG.Attributes;
using RPG.Control;
using UnityEngine;

public class AwarenessSystem : MonoBehaviour
{
    //Serialized
    [SerializeField] bool sendDebugInfoToTargets = true;
    
    //array for potential targets, use UpdateAwareness() to fill up
    Dictionary<DetectableTarget, TrackedTarget> _awarenessTargets;
    
    //array for cleaning up previous array, if awareness of the target dropped to 0
    List<DetectableTarget> toCleanup = new List<DetectableTarget>();
    
    //Cached
    Character _ch;

    //timer
    float _timer;
    float _timerPeriod = 0.1f;
    
    void Awake()
    {
        _ch = GetComponentInParent<Character>() ?? Utilities.InstError<Character>();
        _awarenessTargets = _ch.DataLists.AwarenessTargets;
        _timer = _timerPeriod;
    }


    void Update()
    {
        _timer += Time.deltaTime;
        
        DecreaseAwareness();
    }

    void DecreaseAwareness()
    {
        foreach (var target in _awarenessTargets.Keys)
        {
            if (_awarenessTargets[target].DecreaseAwareness(
                      decreaseDelayTime: _ch.data.awareness.DecreaseDelay,
                    awarenessToDecrease: _ch.data.awareness.DecreaseRate * Time.deltaTime))
            {
                if (_awarenessTargets[target].Awareness >= 2f)
                {
                    _ch.DataLists.OnDeclineToLevel2Awareness?.Invoke(target);
                }
                else if (_awarenessTargets[target].Awareness >= 1f)
                {
                    _ch.DataLists.OnDeclineToLevel1Awareness?.Invoke(target);
                }
                else if (_awarenessTargets[target].Awareness > 0f)
                {
                    _ch.DataLists.OnDeclineToLevel0Awareness?.Invoke(target);
                }
                else if (_awarenessTargets[target].Awareness <= 0f)
                {
                    _ch.DataLists.OnDeclineToZeroAwareness?.Invoke(target);
                    toCleanup.Add(target);
                }
            }
        }
        
        // cleanup targets that are no longer detected
        foreach (var target in toCleanup)
        {
            _awarenessTargets.Remove(target);
            
            //for AwarenessDebugger, to clean up debug text
            _ch.Events.OnCleanAwarenessList?.Invoke(target);
            
            //for AwarenessDetectorDebugger, to clean up debug text
            target.ch.Events.RemoveFromAwarenessDetectorList?.Invoke(target);
        }
        
        toCleanup.Clear();
    }

    public void UpdateAwareness(DetectableTarget target, Vector3 position, Vector3 direction, float awareness)
    {
        // if not in targets
        if (!_awarenessTargets.ContainsKey(target))
            _awarenessTargets[target] = new TrackedTarget();

        UpdateTargetDebugInfo(target);
        
        // update target awareness
        if (_awarenessTargets[target].UpdateAwareness(target, position, direction, awareness))
        {
            if (_awarenessTargets[target].Awareness >= 3f)
            {
                _ch.DataLists.OnIncreaseToLevel3Awareness?.Invoke(target);
            }
            else if (_awarenessTargets[target].Awareness >= 2f)
            {
                _ch.DataLists.OnIncreaseToLevel2Awareness?.Invoke(target);
            }
            else if (_awarenessTargets[target].Awareness >= 1f)
            {
                _ch.DataLists.OnIncreaseToLevel1Awareness?.Invoke(target);
            }
        }
    }

    void UpdateTargetDebugInfo(DetectableTarget target)
    {
        if (!sendDebugInfoToTargets) return;
        
        if (_timer < _timerPeriod) return;
        _timer = 0;
        
        target.ch.Events.UpdateAwarenessDetectorInfo?.Invoke(target, _awarenessTargets[target]);
    }
}
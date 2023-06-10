using System;
using System.Collections.Generic;
using RPG.Attributes;
using RPG.Stats;
using UI;
using UnityEngine;

namespace AI.Sensors
{
    public class MainSensor: MonoBehaviour
    {
        //Serializable
        [Tooltip("How often will update Edge List, in seconds")]
        [SerializeField] float edgeListTimerPeriod = 2f;
        
        [Tooltip("How often will update Vision and Sense lists without any threat in Vision radius, in seconds")]
        [SerializeField] float visionAndSenseTimerPeriod = 0.2f;
        
        [Tooltip("How often will update Hearing list, in seconds")]
        [SerializeField] float hearingTimerPeriod = 0.2f;
        
        //Serializable Friend or Foe conditions
        [SerializeField] List<ThreatCheckSO> friendOrFoeConditions = new List<ThreatCheckSO>();
        
        //Targets for all Sensors checks.
        //Use AllTargets from DetectableTargetManager as a base list.
        List<DetectableTarget> _edgeList; //updated here
        List<DetectableTarget> _inHearingRadius; //updated here
        List<DetectableTarget> _inVisionRadius; //updated here
        List<DetectableTarget> _inVisionField; //updated in Vision Sensor
        List<DetectableTarget> _inSenseRadius; //updated here
        List<DetectableTarget> _inSenseField; //updated in Sense Sensor
        List<DetectableTarget> _threatsInVisionRadius; //updated here

        
        //List types for debugger
        public enum EMainSensorListType
        {
            EdgeList,
            InHearingRadius,
            InVisionRadius,
            InSenseRadius,
            ThreatsInVisionRadius,
        }

        //Local variables
        float _edgeTimer;
        float _threatTimer;
        float _hearingTimer;

        //Cached
        Character _ch;
        AIMemory _memory;


        //Start setup
        void Awake()
        {
            _ch = GetComponentInParent<Character>() ?? InstError<Character>();
            _memory = GetComponentInParent<AIMemory>() ?? InstError<AIMemory>();
            SetTimers();
            SetLists();
        }


        void SetTimers()
        {
            _edgeTimer = edgeListTimerPeriod;
            _threatTimer = visionAndSenseTimerPeriod;
            _hearingTimer = hearingTimerPeriod;
        }

        
        void SetLists()
        {
            _edgeList = _ch.DataLists.EdgeList;
            _inHearingRadius = _ch.DataLists.InHearingRadius;
            _inVisionRadius = _ch.DataLists.InVisionRadius;
            _inVisionField = _ch.DataLists.InVisionField;
            _inSenseRadius = _ch.DataLists.InSenseRadius;
            _inSenseField = _ch.DataLists.InSenseField;
            _threatsInVisionRadius = _ch.DataLists.ThreatsInVisionRadius;
        }

        
        T InstError<T>()
        {
            string className = typeof(T).Name;
            if (typeof(T) is Character)
                throw new Exception($"Missing {className} component for {name}, ID {GetInstanceID()}");
            else
                throw new Exception($"Missing {className} component for {name} in {_ch?.gameObject.name}, ID {GetInstanceID()}");
        }
        
        
        void Update()
        {
            UpdateTimers();
            
            //Edge List update
            if (_edgeTimer > edgeListTimerPeriod)
            {
                _edgeTimer = 0;
                
                UpdateSensorList(
                       updatedList: _edgeList, 
                        viewedList: DetectableTargetManager.Instance.AllTargets, 
                         sqrRadius: _ch.data.vision.EdgeRadiusSqr, 
                          listType: EMainSensorListType.EdgeList);
            }

            //Hearing List update
            if (_hearingTimer > hearingTimerPeriod)
            {
                _hearingTimer = 0;
                
                //constant update every 0.2 seconds
                UpdateSensorList(
                       updatedList: _inHearingRadius, 
                        viewedList: _edgeList, 
                         sqrRadius: _ch.data.hearing.RadiusSqr, 
                          listType: EMainSensorListType.InHearingRadius); 
            }
            
            //Vision and Sense Lists update
            if (CanUpdateVisionAndSenseLists())
            {
                _threatTimer = 0;
                
                //update in Vision Radius List
                UpdateSensorList(
                       updatedList: _inVisionRadius, 
                        viewedList: _edgeList, 
                         sqrRadius: _ch.data.vision.RadiusSqr, 
                          listType: EMainSensorListType.InVisionRadius); 
                

                //Update in Sense Radius List
                UpdateSensorList(
                       updatedList: _inSenseRadius, 
                        viewedList: _inVisionRadius, 
                         sqrRadius: _ch.data.sense.RadiusSqr, 
                          listType: EMainSensorListType.InSenseRadius);
             
                //Check all FriendOrFoe SO conditions
                //If any is TRUE - set Threat is Near flag to TRUE
                CheckThreats();
            }
        }
        
        void UpdateTimers()
        {
            _edgeTimer += Time.deltaTime;
            _threatTimer += Time.deltaTime;
            _hearingTimer += Time.deltaTime;
        }
        

        bool CanUpdateVisionAndSenseLists()
        {
            return _threatTimer >= visionAndSenseTimerPeriod || _ch.data.awareness.ThreatInNear;
        }
        
        
        //Update the proper List of Detectable Targets, depend on a range to them
        void UpdateSensorList(List<DetectableTarget> updatedList, List<DetectableTarget> viewedList, float sqrRadius, EMainSensorListType listType)
        {
            //Looking for a small amount of targets from a specific List
            //Usually this list have a larger radius for included Detectable Targets
            for (int i = 0; i < viewedList.Count; i++)
            {
                var target = viewedList[i];
                var vectorToTarget = target.transform.position - transform.position;
                
                if (target.gameObject == _ch.gameObject) continue;

                if (IsInRange(vectorToTarget, sqrRadius))
                    AddToList(updatedList, target, listType);
                else
                    RemoveFromList(updatedList, target, listType);
            }
        }


        void CheckThreats()
        {
            //Check all targets in Vision Radius
            for (int i = 0; i < _inVisionRadius.Count; i++)
            {
                var target = _inVisionRadius[i];
                
                //Check through all Friend or FoeSO conditions in friendOrFoeConditions List
                foreach (var condition in friendOrFoeConditions)
                {
                    //Update threatsInVisionRadius List
                    if(condition.IsAThreat(target, _ch.DataLists.seenTargets))
                        AddToList(_threatsInVisionRadius, target, EMainSensorListType.ThreatsInVisionRadius);
                    else
                        RemoveFromList(_threatsInVisionRadius, target, EMainSensorListType.ThreatsInVisionRadius);
                }
            }
            
            //remove from list all out of vision range members
            for (int i = 0; i < _edgeList.Count; i++)
            {
                if (!_inVisionRadius.Contains(_edgeList[i]))
                    RemoveFromList(_threatsInVisionRadius, _edgeList[i], EMainSensorListType.ThreatsInVisionRadius);
            }

            //If threatsInVisionRadius Count is > 0 => activate fast Sensor Update 
            if (_threatsInVisionRadius.Count == 0)
                _ch.data.awareness.ThreatInNear = true;
            else
                _ch.data.awareness.ThreatInNear = false;
        }
        
        
        bool IsInRange(Vector3 vectorToTarget, float sqrRadius)
        {
            return vectorToTarget.sqrMagnitude <= sqrRadius;
        }

        
        void AddToList(List<DetectableTarget> updatedList, DetectableTarget target, EMainSensorListType listType)
        {
            if (!updatedList.Contains(target))
            {
                updatedList.Add(target);
                _ch.Events.AddToMainSensorDebuggerList?.Invoke(target, listType);
                target.ch.Events.AddToMainSensorDebuggerList?.Invoke(_ch.detectableTarget, listType);
            }
        }
        
        
        void RemoveFromList(List<DetectableTarget> updatedList, DetectableTarget target, EMainSensorListType listType)
        {
            if (updatedList.Contains(target))
            {
                updatedList.Remove(target);
                _ch.Events.RemoveFromMainSensorDebuggerList?.Invoke(target, listType);
                target.ch.Events.RemoveFromMainSensorDebuggerList?.Invoke(_ch.detectableTarget, listType);
            }
        }
    }
}
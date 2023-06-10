using System;
using System.Collections.Generic;
using RPG.Attributes;
using UI;
using UnityEngine;

namespace AI.Sensors
{
    public class MainSensorDetectorDebugger : MonoBehaviour
    {
          #region Parameters

        //Serializable
        [Space (5f)]
        [Header ("Show who sense this object")]
        [SerializeField] bool showWhoEdgeListMe;
        bool _oldShowWhoEdgeListMe;
        string _textPrefixEdge = " Edge list";
        List<DetectableTarget> _detectorEdgeList = new List<DetectableTarget>(); 
        
        [SerializeField] bool showWhoInHearingRadiusMe;
        bool _oldShowWhoInHearingRadiusMe;
        string _textPrefixHearing = " Hearing list";
        List<DetectableTarget> _detectorInHearingRadius = new List<DetectableTarget>(); 
        
        [SerializeField] bool showWhoInVisionRadiusMe;
        bool _oldShowWhoInVisionRadiusMe;
        string _textPrefixVision = " Vision list";
        List<DetectableTarget> _detectorInVisionRadius = new List<DetectableTarget>();
        
        [SerializeField] bool showWhoInSenseRadiusMe;
        bool _oldShowWhoInSenseRadiusMe;
        string _textPrefixSense = " Sense list";
        List<DetectableTarget> _detectorInSenseRadius = new List<DetectableTarget>();

        [SerializeField] bool showWhoThreatsInVisionRadiusMe;
        bool _oldShowWhoThreatsInVisionRadiusMe;
        string _textPrefixThreat = " Threat list";
        List<DetectableTarget> _detectorThreatsInVisionRadius = new List<DetectableTarget>(); 
        
        [SerializeField] bool showDistanceToObserver;
        bool _oldShowDistanceToObserver;

        //Locals
        List<DetectableTarget> _tempList = new List<DetectableTarget>();
        float _timerPeriod = 0.1f;
        float _timer;

        //Cashed
        Character _ch;
        
        //Local variables for ComposeText()
        string _textToDisplay;
        string _textPrefixDist = ". Dist: ";

        #endregion //Parameters

        
        #region Initialization

        void Awake()
        {
            _ch = GetComponentInParent<Character>() ?? Utilities.InstError<Character>();
            _timer = _timerPeriod;
            SetBoolVariables();
        }

        void SetBoolVariables()
        {
            _oldShowWhoEdgeListMe = showWhoEdgeListMe;
            _oldShowWhoInHearingRadiusMe = showWhoInHearingRadiusMe;
            _oldShowWhoInVisionRadiusMe = showWhoInVisionRadiusMe;
            _oldShowWhoInSenseRadiusMe = showWhoInSenseRadiusMe;
            _oldShowWhoThreatsInVisionRadiusMe = showWhoThreatsInVisionRadiusMe;
            _oldShowDistanceToObserver = showDistanceToObserver;
        }

        void OnEnable()
        {
            _ch.Events.AddToMainSensorDebuggerList += AddObserverToList;
            _ch.Events.RemoveFromMainSensorDebuggerList += RemoveObserverFromList;
        }

        void OnDisable()
        {
            _ch.Events.AddToMainSensorDebuggerList -= AddObserverToList;
            _ch.Events.RemoveFromMainSensorDebuggerList -= RemoveObserverFromList;
        }

        #endregion //Initialization

        void OnValidate()
        {
            if (!Application.isPlaying) return;

            DebugDistanceFlagCheck(ref showDistanceToObserver, ref _oldShowDistanceToObserver);
            DebugFlagCheck(ref showWhoEdgeListMe, ref _oldShowWhoEdgeListMe, _detectorEdgeList, EDebugType.SensorInEdgeListDetector, _textPrefixEdge);
            DebugFlagCheck(ref showWhoInHearingRadiusMe, ref _oldShowWhoInHearingRadiusMe, _detectorInHearingRadius, EDebugType.SensorInHearingRadiusDetector, _textPrefixHearing);
            DebugFlagCheck(ref showWhoInVisionRadiusMe, ref _oldShowWhoInVisionRadiusMe, _detectorInVisionRadius,EDebugType.SensorInVisionRadiusDetector, _textPrefixVision);
            DebugFlagCheck(ref showWhoInSenseRadiusMe, ref _oldShowWhoInSenseRadiusMe, _detectorInSenseRadius,EDebugType.SensorInSenseRadiusDetector, _textPrefixSense);
            DebugFlagCheck(ref showWhoThreatsInVisionRadiusMe, ref _oldShowWhoThreatsInVisionRadiusMe, _detectorThreatsInVisionRadius,EDebugType.SensorInThreatListDetector, _textPrefixThreat);
        }
        
        void DebugFlagCheck(ref bool currentFlag, ref bool oldFlag, List<DetectableTarget> list, EDebugType debugType, string textPrefix)
        {
            if(currentFlag && !oldFlag)
                UpdateObserversListText(list, debugType, textPrefix);
            else if (!currentFlag && oldFlag)
                ClearObserversListText(list, debugType);
            
            if (currentFlag != oldFlag) 
                oldFlag = currentFlag;
        }

        void DebugDistanceFlagCheck(ref bool currentFlag, ref bool oldFlag)
        {
            if (currentFlag && !oldFlag || !currentFlag && oldFlag)
            {
                _timer = _timerPeriod;
                UpdateAllObserversListText();
            }

            if (currentFlag != oldFlag) 
                oldFlag = currentFlag;
        }


        void Update()
        {
            UpdateAllObserversListText();
        }

        //triggered by Event in AddToList() in MainSensor
        void AddObserverToList(DetectableTarget observer, MainSensor.EMainSensorListType listType)
        {
            switch (listType)
            {
                case MainSensor.EMainSensorListType.EdgeList:
                    AddToList(_detectorEdgeList, observer);
                    if (showWhoEdgeListMe) 
                        AddDebugText(observer, GetDebugType(listType), _textPrefixEdge);
                    break;
                case MainSensor.EMainSensorListType.InHearingRadius:
                    AddToList(_detectorInHearingRadius, observer);
                    if (showWhoInHearingRadiusMe) 
                        AddDebugText(observer, GetDebugType(listType), _textPrefixHearing);
                    break;
                case MainSensor.EMainSensorListType.InSenseRadius:
                    AddToList(_detectorInSenseRadius, observer);
                    if (showWhoInSenseRadiusMe)
                        AddDebugText(observer, GetDebugType(listType), _textPrefixSense);
                    break;
                case MainSensor.EMainSensorListType.InVisionRadius:
                    AddToList(_detectorInVisionRadius, observer);
                    if (showWhoInVisionRadiusMe) 
                        AddDebugText(observer, GetDebugType(listType), _textPrefixVision);
                    break;
                case MainSensor.EMainSensorListType.ThreatsInVisionRadius:
                    AddToList(_detectorThreatsInVisionRadius, observer);
                    if (showWhoThreatsInVisionRadiusMe) 
                        AddDebugText(observer, GetDebugType(listType), _textPrefixThreat);
                    break;
            }
        }
        
        //triggered by Event in RemoveFromList() in MainSensor
        void RemoveObserverFromList(DetectableTarget observer, MainSensor.EMainSensorListType listType)
        {
            switch (listType)
            {
                case MainSensor.EMainSensorListType.EdgeList:
                    RemoveFromList(_detectorEdgeList, observer);
                    ClearDebugText(observer, GetDebugType(listType));
                    break;
                case MainSensor.EMainSensorListType.InHearingRadius:
                    RemoveFromList(_detectorInHearingRadius, observer);
                    ClearDebugText(observer, GetDebugType(listType));
                    break;
                case MainSensor.EMainSensorListType.InSenseRadius:
                    RemoveFromList(_detectorInSenseRadius, observer);
                    ClearDebugText(observer, GetDebugType(listType));
                    break;
                case MainSensor.EMainSensorListType.InVisionRadius:
                    RemoveFromList(_detectorInVisionRadius, observer);
                    ClearDebugText(observer, GetDebugType(listType));
                    break;
                case MainSensor.EMainSensorListType.ThreatsInVisionRadius:
                    RemoveFromList(_detectorThreatsInVisionRadius, observer);
                    ClearDebugText(observer, GetDebugType(listType));
                    break;
            }
        }

        void AddToList(List<DetectableTarget> list, DetectableTarget target)
        {
            if (list.Contains(target)) return;
            list.Add(target);
        }
        
        void RemoveFromList(List<DetectableTarget> list, DetectableTarget target)
        {
            if (!list.Contains(target)) return;
            list.Remove(target);
        }

        void AddDebugText(DetectableTarget target, EDebugType debugType, string textPrefix)
        {
            _ch.Events.DebuggerAddText?.Invoke(
                target.ch.GetInstanceID(), 
                debugType,
                DebugTextCompose(target, textPrefix));
        }
        
        void ClearDebugText(DetectableTarget target, EDebugType debugType)
        {
            _ch.Events.DebuggerClearCertainDebugText?.Invoke(
                target.ch.GetInstanceID(), 
                debugType);
        }
        
        string DebugTextCompose(DetectableTarget target, string textPrefix)
        {
            _textToDisplay = String.Empty;
            _textToDisplay += "In " + target.name + textPrefix;
            
            if (showDistanceToObserver)
                _textToDisplay += _textPrefixDist + Vector3.Distance(target.transform.position, _ch.transform.position).ToString("F2");

            return _textToDisplay;
        }
        
        void UpdateObserversListText(List<DetectableTarget> list, EDebugType debugType, string textPrefix)
        {
            for (int i = 0; i < list.Count; i++)
            {
                AddDebugText(list[i], debugType, textPrefix);
            }
        }
        
        void UpdateAllObserversListText()
        {
            if (!_oldShowDistanceToObserver) return;

            _timer += Time.deltaTime;
            if (_timer < _timerPeriod) return;
            _timer = 0;
            
            if (showWhoEdgeListMe)
                UpdateObserversListText(_detectorEdgeList, EDebugType.SensorInEdgeListDetector, _textPrefixEdge);
            if (showWhoInHearingRadiusMe)
                UpdateObserversListText(_detectorInHearingRadius, EDebugType.SensorInHearingRadiusDetector, _textPrefixHearing);
            if (showWhoInVisionRadiusMe)
                UpdateObserversListText(_detectorInVisionRadius, EDebugType.SensorInVisionRadiusDetector, _textPrefixVision);
            if (showWhoInSenseRadiusMe)
                UpdateObserversListText(_detectorInSenseRadius, EDebugType.SensorInSenseRadiusDetector, _textPrefixSense);
            if (showWhoThreatsInVisionRadiusMe)
                UpdateObserversListText(_detectorThreatsInVisionRadius, EDebugType.SensorInThreatListDetector, _textPrefixThreat);
        }
        
        void ClearObserversListText(List<DetectableTarget> list, EDebugType debugType)
        {
            for (int i = 0; i < list.Count; i++)
            {
                ClearDebugText(list[i], debugType);
            }
        }

        EDebugType GetDebugType(MainSensor.EMainSensorListType listType)
        {
            switch (listType)
            {
                case MainSensor.EMainSensorListType.EdgeList:
                    return EDebugType.SensorInEdgeListDetector; break;
                case MainSensor.EMainSensorListType.InHearingRadius:
                    return EDebugType.SensorInHearingRadiusDetector; break;
                case MainSensor.EMainSensorListType.InVisionRadius:
                    return EDebugType.SensorInVisionRadiusDetector; break;
                case MainSensor.EMainSensorListType.InSenseRadius:
                    return EDebugType.SensorInSenseRadiusDetector; break;
                case MainSensor.EMainSensorListType.ThreatsInVisionRadius:
                    return EDebugType.SensorInThreatListDetector; break;
                default:
                    throw new ArgumentException("Unhandled MainSensorListType: " + listType);
            }
        }
    }
}
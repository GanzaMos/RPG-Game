using System;
using System.Collections.Generic;
using RPG.Attributes;
using UI;
using UnityEngine;

namespace AI.Sensors
{
    [RequireComponent(typeof(MainSensor))]
    public class MainSensorDebugger : MonoBehaviour
    {
        [Header("General Debug Settings")]
        [SerializeField] public EDebugMode gizmoShowMode = EDebugMode.ShowAlways;
        
        
        [Space (5)][Header("Targets in Edge radius list")]
        
        [SerializeField] bool showEdgeRadius = false; //Gizmos
        
        [SerializeField] bool showTargetsInEdgeRadius = false; //Text
        bool _oldShowTargetsInEdgeRadius;
        
        
        [Space (5)][Header("Targets in Vision radius list")]

        [SerializeField] bool showVisionRadius = false; //Gizmos
        
        [SerializeField] bool showTargetsInVisionRadius = false; //Text
        bool _oldShowTargetsInVisionRadius;


        [Space (5)][Header("Targets in Sense Radius list")]
        
        [SerializeField] bool showSenseRadius = false; //Gizmos
        
        [SerializeField] bool showTargetsInSenseRadius = false; //Text
        bool _oldShowTargetsInSenseRadius;


        [Space (5)][Header("Targets in Sound Radius list")]
        
        [SerializeField] bool showMaxSoundRadius = false; //Gizmos
        
        public bool showTargetsInSoundRadius = false; //Text
        bool _oldShowTargetsInSoundRadius;


        [Space(5)] [Header("Targets in Threat list")] 
        
        [SerializeField] bool showThreatTargets = false; //Text
        bool _oldShowThreatTargets;
        
        
        //Local lists
        List<DetectableTarget> _edgeList;
        List<DetectableTarget> _inVisionRadius;
        List<DetectableTarget> _inSenseRadius;
        List<DetectableTarget> _inHearingRadius;
        List<DetectableTarget> _threatsInVisionRadius;
        
        //Cached
        Character _ch;

        //Local variables
        int _chID;
        string _objectName;

        void Awake()
        {
            _ch = GetComponentInParent<Character>() ?? Utilities.InstError<Character>();
            _chID = _ch.gameObject.GetInstanceID();
            _objectName = _ch.gameObject.name;

            SetStartOldBoolAttributes();
            SetLists();
        }

        void SetStartOldBoolAttributes()
        {
            _edgeList = _ch.DataLists.EdgeList;
            _inVisionRadius = _ch.DataLists.InVisionRadius;
            _inSenseRadius = _ch.DataLists.InSenseRadius;
            _inHearingRadius = _ch.DataLists.InHearingRadius;
            _threatsInVisionRadius = _ch.DataLists.ThreatsInVisionRadius;
        }
        
        void SetLists()
        {
            throw new NotImplementedException();
        }

        void OnEnable()
        {
            _ch.Events.AddToMainSensorDebuggerList += AddDebugText;
            _ch.Events.RemoveFromMainSensorDebuggerList += RemoveDebugText;
        }

        void OnDisable()
        {
            _ch.Events.AddToMainSensorDebuggerList -= AddDebugText;
            _ch.Events.RemoveFromMainSensorDebuggerList -= RemoveDebugText;
        }

        #region On Validate

        void OnValidate()
        {
            if (!Application.isPlaying) return;

            CheckBoolChange();
            SetOldBoolAttributes();
        }

        void CheckBoolChange()
        {
            if (showTargetsInEdgeRadius != _oldShowTargetsInEdgeRadius)
                SetupStartDebugMessage(showTargetsInEdgeRadius, MainSensor.EMainSensorListType.EdgeList, _edgeList);

            if (showTargetsInVisionRadius != _oldShowTargetsInVisionRadius)
                SetupStartDebugMessage(showTargetsInVisionRadius, MainSensor.EMainSensorListType.InHearingRadius, _inVisionRadius);

            if (showTargetsInSenseRadius != _oldShowTargetsInSenseRadius)
                SetupStartDebugMessage(showTargetsInSenseRadius, MainSensor.EMainSensorListType.InVisionRadius, _inSenseRadius);

            if (showTargetsInSoundRadius != _oldShowTargetsInSoundRadius)
                SetupStartDebugMessage(showTargetsInSoundRadius, MainSensor.EMainSensorListType.InSenseRadius, _inHearingRadius);

            if (showThreatTargets != _oldShowThreatTargets)
                SetupStartDebugMessage(showThreatTargets, MainSensor.EMainSensorListType.ThreatsInVisionRadius, _threatsInVisionRadius);
        }
        
        void SetupStartDebugMessage(bool showTargets, MainSensor.EMainSensorListType listType, List<DetectableTarget> listToCheck)
        {
            foreach (var target in listToCheck)
            {
                if (showTargets)
                    AddDebugText(target, listType);
                else
                    RemoveDebugText(target, listType);
            }
        }

        void SetOldBoolAttributes()
        {
            if (_oldShowTargetsInEdgeRadius != showTargetsInEdgeRadius)
                _oldShowTargetsInEdgeRadius = showTargetsInEdgeRadius;
            if (_oldShowTargetsInVisionRadius != showTargetsInVisionRadius)
                _oldShowTargetsInVisionRadius = showTargetsInVisionRadius;
            if(_oldShowTargetsInSenseRadius != showTargetsInSenseRadius)
                _oldShowTargetsInSenseRadius = showTargetsInSenseRadius;
            if(_oldShowTargetsInSoundRadius != showTargetsInSoundRadius)
                _oldShowTargetsInSoundRadius = showTargetsInSoundRadius;
            if(_oldShowThreatTargets != showThreatTargets)
                _oldShowThreatTargets = showThreatTargets;
        }

        #endregion //On Validate

        //triggered by Event in AddToList() in MainSensor
        void AddDebugText(DetectableTarget target, MainSensor.EMainSensorListType listType)
        {
            switch (listType)
            {
                case MainSensor.EMainSensorListType.EdgeList:
                    if (showTargetsInEdgeRadius)
                        target.ch.Events.DebuggerAddText?.Invoke(
                            _chID,
                            GetDebugType(listType),
                            $"In {_objectName} Edge List");
                    break;
                case MainSensor.EMainSensorListType.InHearingRadius:
                    if (showTargetsInSoundRadius)
                        target.ch.Events.DebuggerAddText?.Invoke(
                            _chID,
                            GetDebugType(listType),
                            $"In {_objectName} Sound Targets List");
                    break;
                case MainSensor.EMainSensorListType.InVisionRadius:
                    if (showTargetsInVisionRadius)
                        target.ch.Events.DebuggerAddText?.Invoke(
                            _chID,
                            GetDebugType(listType),
                            $"In {_objectName} Vision Radius List");
                    break;
                case MainSensor.EMainSensorListType.InSenseRadius:
                    if (showTargetsInSenseRadius)
                    {
                        target.ch.Events.DebuggerAddText?.Invoke(
                                _chID,
                                GetDebugType(listType),
                                $"In {_objectName} Sense Radius List");
                    }
                    break;
                case MainSensor.EMainSensorListType.ThreatsInVisionRadius:
                    if (showThreatTargets)
                        target.ch.Events.DebuggerAddText?.Invoke(
                            _chID,
                            GetDebugType(listType),
                            $"In {_objectName} Threats in Vision Radius List");
                    break;
            }
        }
        
        //triggered by Event in RemoveFromList() in MainSensor
        void RemoveDebugText(DetectableTarget target, MainSensor.EMainSensorListType listType)
        {
            target.ch.Events.DebuggerClearCertainDebugText?.Invoke(
                _chID,
                GetDebugType(listType));
        }

        EDebugType GetDebugType(MainSensor.EMainSensorListType listType)
        {
            switch (listType)
            {
                case MainSensor.EMainSensorListType.EdgeList:
                    return EDebugType.SensorInEdgeList; break;
                case MainSensor.EMainSensorListType.InHearingRadius:
                    return EDebugType.SensorInHearingList; break;
                case MainSensor.EMainSensorListType.InVisionRadius:
                    return EDebugType.SensorInVisionList; break;
                case MainSensor.EMainSensorListType.InSenseRadius:
                    return EDebugType.SensorInSenseList; break;
                case MainSensor.EMainSensorListType.ThreatsInVisionRadius:
                    return EDebugType.SensorInThreatsList; break;
                default:
                    throw new ArgumentException("Unhandled MainSensorListType: " + listType);
            }
        }
        
        
        #region Edge Targets Gizmos methods

        void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;
            if (gizmoShowMode != EDebugMode.ShowAlways) return;
            DrawEdgeGizmo();
        }

        void OnDrawGizmosSelected()
        {
            if (!Application.isPlaying) return;
            if (gizmoShowMode != EDebugMode.ShowOnClick) return;
            DrawEdgeGizmo();
        }
        

        void DrawEdgeGizmo()
        {
            if (showEdgeRadius)
                Gizmos.DrawWireSphere(transform.position, _ch.data.vision.EdgeRadius);
            if (showVisionRadius)
                Gizmos.DrawWireSphere(transform.position, _ch.data.vision.Radius);
            if (showSenseRadius)
                Gizmos.DrawWireSphere(transform.position, _ch.data.sense.Radius);
            if (showMaxSoundRadius)
                Gizmos.DrawWireSphere(transform.position, _ch.data.hearing.Radius);
        }
        

        #endregion
    }
}
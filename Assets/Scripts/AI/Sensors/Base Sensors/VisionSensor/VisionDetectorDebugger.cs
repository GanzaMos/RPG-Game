using System;
using System.Collections.Generic;
using RPG.Attributes;
using UI;
using UnityEngine;

namespace AI.Sensors
{
    public class VisionDetectorDebugger : BaseSensor
    {
        #region Parameters

        //Serializable
        [Space (5f)]
        [Header ("Show who sees this object")]
        [SerializeField] bool showWhoSeeMe;
        bool _oldShowWhoSeeMe;
    
        [SerializeField] bool showDistanceToObserver;
        bool _oldShowDistanceToObserver;
        
        [SerializeField] bool showMyStatusForObserver;
        bool _oldShowMyStatusForObserver;

        [SerializeField] bool updateVisualDetectorsNearby = true;
        
        
        //Locals
        List<DetectableTarget> _lastTickVisualField = new List<DetectableTarget>();
        DetectableTarget _target;
        float _timerPeriod = 0.1f;
        float _timer1;
        float _timer2;
        
        //Targets add and remove from this list by Events from Visual Sensor
        //They represent all targets in Vision Field at the moment
        List<DetectableTarget> _inVisionFieldDetector = new List<DetectableTarget>();
        
        
        //Local variables for ComposeText()
        string _textToDisplay;
        string _textPrefixName = " see me";
        string _textPrefixDist = ". Dist: ";
        string _textPrefixThreat = ". I'm a Threat!";

        #endregion //Parameters

        
        #region Initialization

        void Awake()
        {
            base.Awake();
            _timer1 = _timer2 = _timerPeriod;
            _oldShowWhoSeeMe = showWhoSeeMe;
            _oldShowDistanceToObserver = showDistanceToObserver;
            _oldShowMyStatusForObserver = showMyStatusForObserver;
        }

        void OnEnable()
        {
            Ch.Events.OnVisualFieldDetection += AddToVisualDetectionList;
            Ch.Events.OutVisualFieldDetection += RemoveFromVisualDetectionList;
        }

        void OnDisable()
        {
            Ch.Events.OnVisualFieldDetection -= AddToVisualDetectionList;
            Ch.Events.OutVisualFieldDetection -= RemoveFromVisualDetectionList;
        }

        #endregion //Initialization

        
        #region On Validate

        void OnValidate()
        {
            if (!Application.isPlaying) return;
            
            IfMajorBoolTurnedOn();
            IfMajorBoolTurnedOff();
            IfMinorBoolTurningOn(); 
            SetOldBoolAttributes();
        }

        //Show text about all observers
        void IfMajorBoolTurnedOn()
        {
            if (showWhoSeeMe && !_oldShowWhoSeeMe)
            {
                UpdateObserversListText();
            }
        }

        //Clear text about all observers
        void IfMajorBoolTurnedOff()
        {
            if (!showWhoSeeMe && _oldShowWhoSeeMe)
            {
                ClearObserversListText();
                _oldShowMyStatusForObserver = showDistanceToObserver = false;
                _oldShowMyStatusForObserver = showMyStatusForObserver = false;
            }
        }

        //turn on debugger if:
        //minor bool is ON
        //major bool is OFF
        void IfMinorBoolTurningOn()
        {
            if (showDistanceToObserver && !_oldShowMyStatusForObserver && !showWhoSeeMe ||
                showMyStatusForObserver && !_oldShowMyStatusForObserver && !showWhoSeeMe)
            {
                showWhoSeeMe = _oldShowWhoSeeMe = true;
            }
            else
            {
                UpdateObserversListText();
            }
        }

        void SetOldBoolAttributes()
        {
            if(_oldShowWhoSeeMe != showWhoSeeMe)
                _oldShowWhoSeeMe = showWhoSeeMe;
            if(_oldShowDistanceToObserver != showDistanceToObserver)
                _oldShowDistanceToObserver = showDistanceToObserver;
            if(_oldShowMyStatusForObserver != showMyStatusForObserver)
                _oldShowMyStatusForObserver = showMyStatusForObserver;
        }

        #endregion //On Validate


        void Update()
        {
            if (!showWhoSeeMe) return;
            if (!(showDistanceToObserver || showMyStatusForObserver)) return;

            UpdateObserversListText();
            UpdateVisionDetectorInTarget();
        }

        
        void UpdateVisionDetectorInTarget()
        {
            _timer2 += Time.deltaTime;
            if (_timer2 < _timerPeriod) return;
            _timer2 = 0;
            
            if(!updateVisualDetectorsNearby) return;
            
            for (int i = 0; i < EdgeList.Count; i++)
            {
                _target = EdgeList[i];
                
                //For targets which disappeared from Vision Field 
                if (!InVisionField.Contains(_target) && _lastTickVisualField.Contains(_target))
                {
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
        
        void AddToVisualDetectionList(DetectableTarget target)
        {
            if (!_inVisionFieldDetector.Contains(target))
            {
                _inVisionFieldDetector.Add(target);
                
                if (!showWhoSeeMe) return; //with it it won't show an empty string
                AddDebugText(target);
            }
        }
        
        void RemoveFromVisualDetectionList(DetectableTarget target)
        {
            if (_inVisionFieldDetector.Contains(target))
            {
                _inVisionFieldDetector.Remove(target);
                ClearDebugText(target);
            }
        }

        void UpdateObserversListText()
        {
            _timer1 += Time.deltaTime;
            if (_timer1 < _timerPeriod) return;
            _timer1 = 0;
            
            if (!showWhoSeeMe) return;
            
            for (int i = 0; i < _inVisionFieldDetector.Count; i++)
            {
                AddDebugText(_inVisionFieldDetector[i]);
            }
        }
        
        void ClearObserversListText()
        {
            for (int i = 0; i < _inVisionFieldDetector.Count; i++)
            {
                ClearDebugText(_inVisionFieldDetector[i]);
            }
        }

        void AddDebugText(DetectableTarget target)
        {
            Ch.Events.DebuggerAddText?.Invoke(
                target.ChID, 
                EDebugType.SensorVisionDetector,
                DebugTextCompose(target));
        }
        
        void ClearDebugText(DetectableTarget target)
        {
            Ch.Events.DebuggerClearCertainDebugText?.Invoke(
                target.ChID, 
                EDebugType.SensorVisionDetector);
        }

        string DebugTextCompose(DetectableTarget target)
        {
            _textToDisplay = String.Empty;
            
            if (showWhoSeeMe)
                _textToDisplay += target.name + _textPrefixName;
            if (showDistanceToObserver)
                _textToDisplay += _textPrefixDist + Vector3.Distance(target.transform.position, Ch.transform.position).ToString("F2");
            if (showMyStatusForObserver && target.ch.DataLists.ThreatsInVisionRadius.Contains(Ch.detectableTarget))
                _textToDisplay += _textPrefixThreat;
            
            return _textToDisplay;
        }
    }
}
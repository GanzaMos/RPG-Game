using System;
using System.Collections.Generic;
using RPG.Attributes;
using UI;
using UnityEngine;

namespace AI.Sensors
{
    public class SenseDetectorDebugger : BaseSensor
    {
         #region Parameters

        //Serializable
        [Space (5f)]
        [Header ("Show who sense this object")]
        [SerializeField] bool showWhoSenseMe;
        bool _oldShowWhoSenseMe;
    
        [SerializeField] bool showDistanceToObserver;
        bool _oldShowDistanceToObserver;
        
        [SerializeField] bool showAmIThreatForObserver;
        bool _oldShowAmIThreatForObserver;

        //Locals
        List<DetectableTarget> _inSenseFieldDetector = new List<DetectableTarget>();
        float _timerPeriod = 0.1f;
        float _timer;

        //Local variables for ComposeText()
        string _textToDisplay;
        string _textPrefixName = " sense me";
        string _textPrefixDist = ". Dist: ";
        string _textPrefixThreat = ". I'm a Threat!";

        #endregion //Parameters

        
        #region Initialization

        void Awake()
        {
            base.Awake();
            _timer = _timerPeriod;
            _oldShowWhoSenseMe = showWhoSenseMe;
            _oldShowDistanceToObserver = showDistanceToObserver;
            _oldShowAmIThreatForObserver = showAmIThreatForObserver;
        }

        void OnEnable()
        {
            Ch.Events.OnSenseFieldDetection += AddToSenseDetectionList;
            Ch.Events.OutSenseFieldDetection += RemoveFromSenseDetectionList;
        }

        void OnDisable()
        {
            Ch.Events.OnSenseFieldDetection -= AddToSenseDetectionList;
            Ch.Events.OutSenseFieldDetection -= RemoveFromSenseDetectionList;
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
            if (showWhoSenseMe && !_oldShowWhoSenseMe)
            {
                UpdateObserversListText();
            }
        }

        //Clear text about all observers
        void IfMajorBoolTurnedOff()
        {
            if (!showWhoSenseMe && _oldShowWhoSenseMe)
            {
                ClearObserversListText();
                _oldShowAmIThreatForObserver = showDistanceToObserver = false;
                _oldShowAmIThreatForObserver = showAmIThreatForObserver = false;
            }
        }

        //turn on debugger if:
        //minor bool is ON
        //major bool is OFF
        void IfMinorBoolTurningOn()
        {
            if (showDistanceToObserver && !_oldShowAmIThreatForObserver && !showWhoSenseMe ||
                showAmIThreatForObserver && !_oldShowAmIThreatForObserver && !showWhoSenseMe)
            {
                showWhoSenseMe = _oldShowWhoSenseMe = true;
            }
            else
            {
                UpdateObserversListText();
            }
        }


        void SetOldBoolAttributes()
        {
            if(_oldShowWhoSenseMe != showWhoSenseMe)
                _oldShowWhoSenseMe = showWhoSenseMe;
            if(_oldShowDistanceToObserver != showDistanceToObserver)
                _oldShowDistanceToObserver = showDistanceToObserver;
            if(_oldShowAmIThreatForObserver != showAmIThreatForObserver)
                _oldShowAmIThreatForObserver = showAmIThreatForObserver;
        }

        #endregion //On Validate


        void Update()
        {
            if (!showWhoSenseMe) return;
            if (!(showDistanceToObserver || showAmIThreatForObserver)) return;

            _timer += Time.deltaTime;
            if (_timer < _timerPeriod) return;
            _timer = 0;
            
            UpdateObserversListText();
        }

        void AddToSenseDetectionList(DetectableTarget target)
        {
            if (!_inSenseFieldDetector.Contains(target))
            {
                _inSenseFieldDetector.Add(target);
                
                if (!showWhoSenseMe) return; //with it it won't show an empty string
                AddDebugText(target);
            }
        }
        
        void RemoveFromSenseDetectionList(DetectableTarget target)
        {
            if (_inSenseFieldDetector.Contains(target))
            {
                _inSenseFieldDetector.Remove(target);
                ClearDebugText(target);
            }
        }

        void UpdateObserversListText()
        {
            if (!showWhoSenseMe) return;
            
            for (int i = 0; i < _inSenseFieldDetector.Count; i++)
            {
                AddDebugText(_inSenseFieldDetector[i]);
            }
        }
        
        void ClearObserversListText()
        {
            for (int i = 0; i < _inSenseFieldDetector.Count; i++)
            {
                ClearDebugText(_inSenseFieldDetector[i]);
            }
        }

        void AddDebugText(DetectableTarget target)
        {
            Ch.Events.DebuggerAddText?.Invoke(
                target.ChID, 
                EDebugType.SensorSenseDetector,
                DebugTextCompose(target));
        }
        
        void ClearDebugText(DetectableTarget target)
        {
            Ch.Events.DebuggerClearCertainDebugText?.Invoke(
                target.ChID, 
                EDebugType.SensorSenseDetector);
        }

        string DebugTextCompose(DetectableTarget target)
        {
            _textToDisplay = String.Empty;
            
            if (showWhoSenseMe)
                _textToDisplay += target.name + _textPrefixName;
            if (showDistanceToObserver)
                _textToDisplay += _textPrefixDist + Vector3.Distance(target.transform.position, Ch.transform.position).ToString("F2");
            if (showAmIThreatForObserver && target.ch.DataLists.ThreatsInVisionRadius.Contains(Ch.detectableTarget))
                _textToDisplay += _textPrefixThreat;
            
            return _textToDisplay;
        }
    }
}
using System;
using System.Collections.Generic;
using RPG.Attributes;
using UI;
using UnityEngine;

namespace AI.Sensors
{
    [RequireComponent(typeof(SenseSensor))]
    public class SenseFieldDebugger : BaseSensor
    {
         #region Parameters
    
    [Header ("Sense Field debugger settings")]
    [Tooltip("Turn on in play mode to see Sense Field")]
    [SerializeField] bool showSenseField;
    [SerializeField] EDebugMode debugMode = EDebugMode.DontShow; 
    [SerializeField] Color senseFieldColor = new Color(226, 135, 135, 127);

    [Space (5f)]
    [Header ("Text list about targets in Sense Field")]
    [SerializeField] bool showTargetsInSenseField;
    bool _oldShowTargetsInSenseField;
    
    [SerializeField] bool showDistanceToTarget;
    bool _oldShowDistanceToTarget;

    [SerializeField] bool showIsTargetAThreat;
    bool _oldShowIsTargetAThreat;
    

    //Local
    List<DetectableTarget> _lastTickVisualField = new List<DetectableTarget>();
    DetectableTarget _target;
    float _timerPeriod = 0.1f;
    float _timer;
    bool _canUpdateDistanceText = false;
    
    //Local variables for ComposeText()
    string _textToDisplay;
    string _textPrefixName = " in Sense Field";
    string _textPrefixDist = ". Dist: ";
    string _textPrefixThreat = ". Threat!";
    
    #endregion //Parameters

    
    #region Initialization

    void Awake()
    {
        base.Awake();
        _timer = _timerPeriod;
        _oldShowTargetsInSenseField = showTargetsInSenseField;
        _oldShowDistanceToTarget = showDistanceToTarget;
        _oldShowIsTargetAThreat = showIsTargetAThreat;
    }
    
    #endregion //Initialization

    
    #region On Validate

    void OnValidate()
    {
        if (!Application.isPlaying) return;
        
        IfMajorBoolTurnedOff();
        IfMinorBoolTurningOn(); 
        SetOldBoolAttributes();
    }

    //clear all debug text when major bool is OFF
    void IfMajorBoolTurnedOff()
    {
        if (!showTargetsInSenseField && _oldShowTargetsInSenseField)
        {
            for (int i = 0; i < EdgeList.Count; i++)
                RemoveText(EdgeList[i]);
        }
    }

    //turn on debugger if:
    //minor bool is ON
    //major bool is OFF
    void IfMinorBoolTurningOn()
    {
        if (!_oldShowDistanceToTarget && showDistanceToTarget && !showTargetsInSenseField ||
            !_oldShowIsTargetAThreat && showIsTargetAThreat && !showTargetsInSenseField)
        {
            showTargetsInSenseField = _oldShowTargetsInSenseField = true;
        }
    }

    void SetOldBoolAttributes()
    {
        if(_oldShowTargetsInSenseField != showTargetsInSenseField)
            _oldShowTargetsInSenseField = showTargetsInSenseField;
        
        if(_oldShowDistanceToTarget != showDistanceToTarget)
            _oldShowDistanceToTarget = showDistanceToTarget;
        
        if(_oldShowIsTargetAThreat != showIsTargetAThreat)
            _oldShowIsTargetAThreat = showIsTargetAThreat;
    }

    #endregion //On Validate


    #region Main methods

    void Update()
    {
        if (!showTargetsInSenseField) return;
        
        TextForTargetsInSenseField();
        UpdateTextDetails();
    }
    

    void TextForTargetsInSenseField()
    {
        //Check last frame and compares what was there and what is now
        for (int i = 0; i < EdgeList.Count; i++)
        {
            _target = EdgeList[i];
            //See a target? Didn't last frame? Show Debug message
            if (InSenseField.Contains(_target) && !_lastTickVisualField.Contains(_target))
            {
                AddDebugText(_target);
                _canUpdateDistanceText = true;
            }

            //Don't see a target? Saw last frame? Clear debug text
            if (InSenseField.Contains(_target) && _lastTickVisualField.Contains(_target))
            {
                RemoveText(_target);
                _canUpdateDistanceText = false;
            }
        }
        
        //clear last frame list
        _lastTickVisualField.Clear();
            
        //refill last frame list with all targets in Sense Field
        for (int i = 0; i < InSenseField.Count; i++)
            _lastTickVisualField.Add(InSenseField[i]);
    }

    
    //Check all targets in InSenseField list
    void UpdateTextDetails()
    {
        _timer += Time.deltaTime;
        if (_timer < _timerPeriod) return;
        _timer = 0;
        
        if (!_canUpdateDistanceText) return;

        for (int i = 0; i < InSenseField.Count; i++)
        {
            AddDebugText(InSenseField[i]);
        }
    }
    
    
    //Send text about target to the Debug Text Controller
    void AddDebugText(DetectableTarget target)
    {
        Ch.Events.DebuggerAddText?.Invoke(
            target.ChID,
            EDebugType.SensorSenseField,
            ComposeText(target));
    }

    
    //Remove text about target from the Debug Text Controller
    void RemoveText(DetectableTarget target)
    {
        Ch.Events.DebuggerClearCertainDebugText?.Invoke(
            target.ChID,
            EDebugType.SensorSenseField);
    }


    string ComposeText(DetectableTarget target)
    {
        _textToDisplay = String.Empty;

        if (showTargetsInSenseField)
            _textToDisplay += target.ch.name + _textPrefixName;
        if (showDistanceToTarget)
            _textToDisplay += _textPrefixDist + Vector3.Distance(Ch.transform.position, target.ch.transform.position).ToString("F2");
        if (showIsTargetAThreat && ThreatsInVisionRadius.Contains(target))
            _textToDisplay += _textPrefixThreat;

        return _textToDisplay;
    }
    
    #endregion //Main methods
    
    
    #region Sense Targets Gizmos methods

    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        if (debugMode != EDebugMode.ShowAlways) return;
        MainGizmoMethod();
    }

    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;
        if (debugMode != EDebugMode.ShowOnClick) return;
        MainGizmoMethod();
    }

    void MainGizmoMethod()
    {
        if (showSenseField)
            DrawSenseGizmo();
    }

    void DrawSenseGizmo()
    {
        Gizmos.color = senseFieldColor;
        Gizmos.DrawWireSphere(transform.position, Ch.data.sense.Radius);
    }
    
    #endregion //Sense Targets Gizmos methods
    
    }
}
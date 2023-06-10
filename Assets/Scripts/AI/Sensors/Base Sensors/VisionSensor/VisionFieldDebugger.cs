using System;
using System.Collections.Generic;
using AI.Sensors;
using RPG.Attributes;
using UI;
using UnityEngine;

public class VisionFieldDebugger : BaseSensor
{
    #region Parameters
    
    [Header ("Vision Field debugger settings")]
    [Tooltip("Turn on in play mode to see Vision Field")]
    [SerializeField] bool showVisionField = false;
    [SerializeField] EDebugMode showMode = EDebugMode.DontShow; 
    [SerializeField] Color visionFieldColor = new Color(226, 135, 135, 127);

    [Space (5f)]
    [Header ("Text list about targets in Visual Field")]
    [SerializeField] bool showTargetsInVisionField = false;
    bool _oldShowTargetsInVisionField;
    
    [SerializeField] bool showDistanceToTarget = false;
    bool _oldShowDistanceToTarget;

    [SerializeField] bool showIsTargetAThreat = false;
    bool _oldShowIsTargetAThreat;

    //Cached
    VisionMeshGenerator _meshGenerator;

    //Local
    List<DetectableTarget> _lastTickVisualField = new List<DetectableTarget>();
    Mesh _visionMesh;
    DetectableTarget _target;
    float _distanceTextUpdatePeriod = 0.1f;
    float _timer;
    bool _canUpdateDistanceText = false;
    
    //Local variables for ComposeText()
    string _textToDisplay;
    string _textPrefixName = " in Vision Field";
    string _textPrefixDist = ". Dist: ";
    string _textPrefixThreat = ". Threat!";
    
    #endregion //Parameters

    
    #region Initialization

    void Awake()
    {
        base.Awake();
        _meshGenerator = GetComponent<VisionMeshGenerator>() ?? InstError<VisionMeshGenerator>();
        _timer = _distanceTextUpdatePeriod;
        _oldShowTargetsInVisionField = showTargetsInVisionField;
        _oldShowDistanceToTarget = showDistanceToTarget;
        _oldShowIsTargetAThreat = showIsTargetAThreat;
    }

    void Start()
    {
        _visionMesh = _meshGenerator.SetMeshData();
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

    //clear all debug text when switching off showTargetsInVisionField
    void IfMajorBoolTurnedOff()
    {
        if (!showTargetsInVisionField && _oldShowTargetsInVisionField)
        {
            for (int i = 0; i < EdgeList.Count; i++)
                RemoveText(EdgeList[i]);
        }
    }

    //turn on debugger if showDistanceToTarget OR showIsTargetAThreat was switching on first
    void IfMinorBoolTurningOn()
    {
        if (!_oldShowDistanceToTarget && showDistanceToTarget && !showTargetsInVisionField ||
            !_oldShowIsTargetAThreat && showIsTargetAThreat && !showTargetsInVisionField)
        {
            showTargetsInVisionField = _oldShowTargetsInVisionField = true;
        }
    }

    void SetOldBoolAttributes()
    {
        if(_oldShowTargetsInVisionField != showTargetsInVisionField)
            _oldShowTargetsInVisionField = showTargetsInVisionField;
        if(_oldShowDistanceToTarget != showDistanceToTarget)
            _oldShowDistanceToTarget = showDistanceToTarget;
        if(_oldShowIsTargetAThreat != showIsTargetAThreat)
            _oldShowIsTargetAThreat = showIsTargetAThreat;
    }

    #endregion //On Validate


    #region Main methods

    void Update()
    {
        if (!showTargetsInVisionField) return;
        
        TextForTargetsInVisionField();
        UpdateDistanceText();
    }
    

    void TextForTargetsInVisionField()
    {
        //Check last frame and compares what was there and what is now
        for (int i = 0; i < EdgeList.Count; i++)
        {
            _target = EdgeList[i];
            //See a target? Didn't last frame? Show Debug message
            if (InVisionField.Contains(_target) && !_lastTickVisualField.Contains(_target))
            {
                AddDebugText(_target, ComposeText(_target));
                _canUpdateDistanceText = true;
            }

            //Don't see a target? Saw last frame? Clear debug text
            if (InVisionField.Contains(_target) && _lastTickVisualField.Contains(_target))
            {
                RemoveText(_target);
                _canUpdateDistanceText = false;
            }
        }
        
        //clear last frame list
        _lastTickVisualField.Clear();
            
        //refill last frame list with all targets in Vision Field
        for (int i = 0; i < InVisionField.Count; i++)
            _lastTickVisualField.Add(InVisionField[i]);
    }

    
    void UpdateDistanceText()
    {
        _timer += Time.deltaTime;
        if (_timer < _distanceTextUpdatePeriod) return;
        _timer = 0;
        
        if (!_canUpdateDistanceText) return;

        for (int i = 0; i < InVisionField.Count; i++)
        {
            //_target = _mainSensor.inVisionField[i];
            AddDebugText(InVisionField[i], ComposeText(InVisionField[i]));
        }
    }
    

    void AddDebugText(DetectableTarget target, string textToDisplay)
    {
        Ch.Events.DebuggerAddText?.Invoke(
            ChID + target.ChID * 1000, //not the best solution, but it will give every object its own text line
            EDebugType.SensorVisionField,
            textToDisplay);
    }

    
    void RemoveText(DetectableTarget target)
    {
        Ch.Events.DebuggerClearCertainDebugText?.Invoke(
            ChID + target.ChID * 1000,
            EDebugType.SensorVisionField);
    }


    string ComposeText(DetectableTarget target)
    {
        _textToDisplay = String.Empty;

        if (showTargetsInVisionField)
            _textToDisplay += target.ch.name + _textPrefixName;
        if (showDistanceToTarget)
            _textToDisplay += _textPrefixDist + Vector3.Distance(Ch.transform.position, target.ch.transform.position).ToString("F2");
        if (showIsTargetAThreat && ThreatsInVisionRadius.Contains(target))
            _textToDisplay += _textPrefixThreat;

        return _textToDisplay;
    }
    
    #endregion //Main methods
    
    
    #region Edge Targets Gizmos methods

    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        if (showMode != EDebugMode.ShowAlways) return;
        MainGizmoMethod();
    }

    void OnDrawGizmosSelected()
    {
        if (!Application.isPlaying) return;
        if (showMode != EDebugMode.ShowOnClick) return;
        MainGizmoMethod();
    }

    void MainGizmoMethod()
    {
        if (showVisionField)
            DrawEdgeGizmo();
    }

    void DrawEdgeGizmo()
    {
        Gizmos.color = visionFieldColor;
        Gizmos.DrawMesh(_visionMesh, transform.position, Quaternion.identity);
    }
    #endregion
}

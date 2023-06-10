using System;
using System.Collections.Generic;
using RPG.Attributes;
using UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace AI.Sensors
{
    public class AwarenessDebugger : MonoBehaviour
    {
        //Serialized
        [SerializeField] bool showAwarenessList;
        bool _oldShowAwarenessList;
        [SerializeField] bool showRawPosition;
        [SerializeField] EDebugMode showMode = EDebugMode.DontShow;

        //Cashed
        Character _ch;
        Dictionary<DetectableTarget, TrackedTarget> _awarenessTargets;
        
        //Locals
        EDebugType _debugType = EDebugType.SensorAwareness;
        Color _gizmosColor = Color.red;
        float _gizmosRadius = 1f;
        int _chID;
        string _debugText;
        
        //Timers
        float _timer;
        float _timerPeriod = 0.1f;

        void Awake()
        {
            //Caching
            _ch = GetComponentInParent<Character>() ?? Utilities.InstError<Character>();
            _awarenessTargets = _ch.DataLists.AwarenessTargets;
            
            //Set variables for OnValidate
            SetOnValidateVariables();
            
            //Set locals
            _timer = _timerPeriod;
            _chID = _ch.GetInstanceID();
        }

        void SetOnValidateVariables()
        {
            _oldShowAwarenessList = showAwarenessList;
        }

        void OnEnable()
        {
            _ch.Events.OnCleanAwarenessList += ClearDebugText;
        }

        void OnDisable()
        {
            _ch.Events.OnCleanAwarenessList -= ClearDebugText;
        }

        void OnValidate()
        {
            if (!Application.isPlaying) return;

            if (!showAwarenessList && _oldShowAwarenessList)
            {
                foreach (var target in _awarenessTargets)
                {
                    ClearDebugText(target.Key);
                }
            }

            if (_oldShowAwarenessList != showAwarenessList)
                _oldShowAwarenessList = showAwarenessList;
        }


        void Update()
        {
            ShowAwarenessText();
        }

        void ShowAwarenessText()
        {
            if (!showAwarenessList) return;

            _timer += Time.deltaTime;
            if (_timer < _timerPeriod) return;
            _timer = 0;

            // foreach (var target in _awarenessTargets)
            // {
            //     UpdateDebugText(target.Key, ComposeText(target.Key, target.Value));
            // }
            foreach (var target in _awarenessTargets)
            {
                UpdateDebugText(target.Key, ComposeText(target.Key, target.Value));
            }
        }

        string ComposeText(DetectableTarget awarenessTarget, TrackedTarget targetDetails)
        {
            return _debugText = awarenessTarget.ch.name + " awareness = " + targetDetails.Awareness.ToString("F2");
        }

        void UpdateDebugText(DetectableTarget awarenessTarget, string debugText)
        {
            _ch.Events.DebuggerAddText?.Invoke(GetID(awarenessTarget), _debugType, debugText);
        }

        void ClearDebugText(DetectableTarget awarenessTarget)
        {
            _ch.Events.DebuggerClearCertainDebugText?.Invoke(GetID(awarenessTarget), _debugType);
        }
        
        int GetID(DetectableTarget awarenessTarget)
        {
            return _chID + awarenessTarget.ch.GetInstanceID() * 10000;
        }


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
            if (showRawPosition)
                DrawEdgeGizmo();
        }

        void DrawEdgeGizmo()
        {
            foreach (var target in _awarenessTargets)
            {
                Gizmos.color = _gizmosColor;
                Gizmos.DrawSphere(target.Value.RawPosition, _gizmosRadius);
            }
        }
        #endregion
    }
}
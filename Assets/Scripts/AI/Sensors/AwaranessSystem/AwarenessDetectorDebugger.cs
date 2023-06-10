using System;
using System.Collections.Generic;
using RPG.Attributes;
using TMPro;
using UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.PlayerLoop;

namespace AI.Sensors
{
    public class AwarenessDetectorDebugger : MonoBehaviour
    {
        //Serialized
        [Header("Show awareness value text")]
        [SerializeField] bool showWhoAwareOfMe;
        bool _oldShowWhoAwareOfMe;
        
        [Space (5f)]
        [Header("Show raw position gizmos")]
        [SerializeField] bool showMyRawAwarePosition;
        [SerializeField] EDebugMode showMode = EDebugMode.DontShow;
        
        //Arrays
        //DetectableTarget - it's who tracks THIS object
        //TrackedTarget - it's DetectableTarget's info about THIS object
        Dictionary<DetectableTarget, TrackedTarget> _whoTrackMe = new Dictionary<DetectableTarget, TrackedTarget>();
        
        //Cashed
        Character _ch;

        //Locals
        int _chID;
        string _debugText;
        string _debugPrefixName = " is aware of me";
        string _debugPrefixValue = ". Value = ";
        Color _gizmosColor = Color.red;
        float _gizmosRadius = 1f;
        EDebugType _debugType = EDebugType.SensorAwarenessDetector; 
        
        //Timers
        float _timer;
        float _timerPeriod = 0.1f;
        
        void Awake()
        {
            //Caching
            _ch = GetComponentInParent<Character>() ?? Utilities.InstError<Character>();

            //Set variables for OnValidate
            SetOnValidateVariables();
            
            //Set locals
            _timer = _timerPeriod;
            _chID = _ch.GetInstanceID();
        }
        
        
        void SetOnValidateVariables()
        {
            _oldShowWhoAwareOfMe = showWhoAwareOfMe;
        }

        void OnEnable()
        {
            _ch.Events.UpdateAwarenessDetectorInfo += AddToAwarenessList;
            _ch.Events.RemoveFromAwarenessDetectorList += RemoveFromAwarenessList;
        }

        void OnDisable()
        {
            _ch.Events.UpdateAwarenessDetectorInfo -= AddToAwarenessList;
            _ch.Events.RemoveFromAwarenessDetectorList -= RemoveFromAwarenessList;
        }

        void OnValidate()
        {
            if (!Application.isPlaying) return;

            //in case if we turning off showWhoAwareOfMe
            //cleaning all the debug text
            if (!showWhoAwareOfMe && _oldShowWhoAwareOfMe)
                ClearAllDebugText();

            if (_oldShowWhoAwareOfMe != showWhoAwareOfMe)
                _oldShowWhoAwareOfMe = showWhoAwareOfMe;

        }

        void AddToAwarenessList(DetectableTarget observer, TrackedTarget infoAboutMe)
        {
            if (!_whoTrackMe.ContainsKey(observer))
                _whoTrackMe.Add(observer, infoAboutMe);
            else
                _whoTrackMe[observer] = infoAboutMe;
        }

        void RemoveFromAwarenessList(DetectableTarget observer)
        {
            if (_whoTrackMe.ContainsKey(observer))
                _whoTrackMe.Remove(observer);
        }

        void Update()
        {
            ShowWhoAwareOfMe();
        }

        void ShowWhoAwareOfMe()
        {
            if (!showWhoAwareOfMe) return;

            _timer += Time.deltaTime;
            if (_timer < _timerPeriod) return;
            _timer = 0;

            ClearAllDebugText();
            foreach (var observer in _whoTrackMe)
            {
                AddDebugTextLine(observer);
            }
        }

        void AddDebugTextLine(KeyValuePair<DetectableTarget, TrackedTarget> observer)
        {
            _ch.Events.DebuggerAddLine(_chID, _debugType, ComposeText(observer));
        }

        void ClearAllDebugText()
        {
            _ch.Events.DebuggerClearCertainDebugText(_chID, _debugType);
        }

        string ComposeText(KeyValuePair<DetectableTarget, TrackedTarget> observer)
        {
            _debugText = String.Empty;
            _debugText += observer.Key.ch.name + _debugPrefixName;
            _debugText += _debugPrefixValue + observer.Value.Awareness.ToString("F2");
            
            return _debugText;
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
            if (showMyRawAwarePosition)
                DrawEdgeGizmo();
        }

        void DrawEdgeGizmo()
        {
            foreach (var target in _whoTrackMe)
            {
                Gizmos.color = _gizmosColor;
                Gizmos.DrawSphere(target.Value.RawPosition, _gizmosRadius);
                Handles.Label(target.Value.RawPosition + Vector3.up * 3f, target.Key.ch.name + " raw position info" );
            }
        }
        #endregion
    }
}
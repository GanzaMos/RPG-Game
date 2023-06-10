using System;
using AI.Sensors;
using RPG.Attributes;
using UI;
using UnityEditor;
using UnityEngine;

namespace AI.Debuggers
{
    public class AIMemoryDetectorDebugger : MonoBehaviour
    {
        [Header("Debug text")]
        [SerializeField] bool showMemoryAboutMe;
        bool _oldShowMemoryAboutMe;
        [SerializeField] bool showStartStateInMemory;
        bool _oldShowStartStateInMemory;
        [SerializeField] bool showIsDeadInMemory;
        bool _oldShowIsDeadInMemory;
        [SerializeField] bool showIsProvokedInMemory;
        bool _oldShowIsProvokedInMemory;
        [SerializeField] bool showCheckFromStartInMemory;
        bool _oldShowCheckFromStartInMemory;
        [SerializeField] bool showLastKnownPositionText;
        bool _oldShowLastKnownPositionText;        
        [SerializeField] bool showLastSeenTimeText;
        bool _oldShowLastSeenTimeText;

        [Space (5f)]
        [Header("Debug gizmos")] 
        [SerializeField] bool showLastKnownPositionGizmo;
        [SerializeField] EDebugMode showMode = EDebugMode.DontShow;
        
        CharacterDataLists.SeenTargets _whoSawMe = new CharacterDataLists.SeenTargets();

        //Cashed
        Character _ch;
        int _chID;
        
        //Local
        Color _gizmosColor = Color.cyan;
        float _gizmosRadius = 1f;
        DetectableTarget _target;
        float _timer;
        float _timerPeriod = 0.1f;
        EDebugType _debugType = EDebugType.AIMemory; 
        
        
        //Strings
        string _debugText;
        string _debugPrefixRememberMe = " remembers me";
        string _debugPrefixCheckFromStart = "Check from start: ";
        string _debugPrefixIsDead = "Is Dead: ";
        string _debugPrefixIsProvoked = "Is Provoked: ";
        string _debugPrefixStartState = "Start state: ";
        string _debugPrefixLastKnownLocation = "Lase known location: ";


        void Awake()
        {
            _ch = GetComponentInParent<Character>() ?? InstError<Character>();
            _chID = _ch.GetInstanceID();
            _timer = _timerPeriod;

            SetOldBool();
        }
        
        T InstError<T>()
        {
            string className = typeof(T).Name;
            if (typeof(T) is Character)
                throw new Exception($"Missing {className} component for {name}, ID {GetInstanceID()}");
            else
                throw new Exception($"Missing {className} component for {name} in {_ch?.gameObject.name}, ID {GetInstanceID()}");
        }
        
        void SetOldBool()
        {
            _oldShowMemoryAboutMe = showMemoryAboutMe;
            _oldShowStartStateInMemory = showStartStateInMemory;
            _oldShowIsDeadInMemory = showIsDeadInMemory;
            _oldShowIsProvokedInMemory = showIsProvokedInMemory;
            _oldShowCheckFromStartInMemory = showCheckFromStartInMemory;
            _oldShowLastKnownPositionText = showLastKnownPositionText;
            _oldShowLastSeenTimeText = showLastSeenTimeText;
        }

        void OnEnable()
        {
            _ch.Events.UpdateAIMemoryDetectorInfo += UpdateAIMemoryDetectorInfo;
        }

        void OnDisable()
        {
            _ch.Events.UpdateAIMemoryDetectorInfo -= UpdateAIMemoryDetectorInfo;
        }

        void OnValidate()
        {
            if (!Application.isPlaying) return;

            EnableMainFlagIfMajorIsEnabled();
            DisableMinorFlagsIfMainIsOff();
            CheckFlags();
        }
        
        //Enable main debugger function if one of minor flags was turned on before it
        void EnableMainFlagIfMajorIsEnabled()
        {
            if (showStartStateInMemory && !_oldShowStartStateInMemory && !showMemoryAboutMe ||
                showIsDeadInMemory && !_oldShowIsDeadInMemory && !showMemoryAboutMe ||
                showIsProvokedInMemory && !_oldShowIsProvokedInMemory && !showMemoryAboutMe ||
                showCheckFromStartInMemory && !_oldShowCheckFromStartInMemory && !showMemoryAboutMe ||
                showLastKnownPositionText && !_oldShowLastKnownPositionText && !showMemoryAboutMe ||
                showLastSeenTimeText && !_oldShowLastSeenTimeText && !showMemoryAboutMe)
            {
                showMemoryAboutMe = true;
            }
        }
        
        void DisableMinorFlagsIfMainIsOff()
        {
            if (!showMemoryAboutMe && _oldShowMemoryAboutMe)
            {
                showStartStateInMemory = false;
                showIsDeadInMemory = false;
                showIsProvokedInMemory = false;
                showCheckFromStartInMemory = false;
                showLastKnownPositionText = false;
                showLastSeenTimeText = false;

                foreach (var observer in _whoSawMe)
                {
                    ClearAllDebugText(observer.Key);
                }
            }
        }

        void CheckFlags()
        {
            CheckFlag(ref showMemoryAboutMe, ref _oldShowMemoryAboutMe);
            CheckFlag(ref showStartStateInMemory, ref _oldShowStartStateInMemory);
            CheckFlag(ref showIsDeadInMemory, ref _oldShowIsDeadInMemory);
            CheckFlag(ref showIsProvokedInMemory, ref _oldShowIsProvokedInMemory);
            CheckFlag(ref showCheckFromStartInMemory, ref _oldShowCheckFromStartInMemory);
            CheckFlag(ref showLastKnownPositionText, ref _oldShowLastKnownPositionText);
            CheckFlag(ref showLastSeenTimeText, ref _oldShowLastSeenTimeText);
        }

        void CheckFlag(ref bool currentFlag, ref bool oldFlag)
        {
            if (oldFlag != currentFlag)
                oldFlag = currentFlag;
        }

        
        void Update()
        {
            ShowWhoAwareOfMe();
        }

        
        void UpdateAIMemoryDetectorInfo(DetectableTarget target, MemoryDetails targetInfo)
        {
            if (!_whoSawMe.ContainsKey(target))
            {
                _whoSawMe.Add(target, targetInfo);
            }
            else
            {
                _whoSawMe[target] = targetInfo;
            }
        }

        
        void ShowWhoAwareOfMe()
        {
            if (!showMemoryAboutMe) return;

            _timer += Time.deltaTime;
            if (_timer < _timerPeriod) return;
            _timer = 0;
            
            foreach (var observer in _whoSawMe)
            {
                ClearAllDebugText(observer.Key);
                AddDebugTextLine(observer.Key, observer.Value);
            }
        }
        
        
        void AddDebugTextLine(DetectableTarget observer, MemoryDetails targetInfo)
        {
            observer.ch.Events.DebuggerAddText(_chID, _debugType, ComposeText(observer, targetInfo));
        }

        
        void ClearAllDebugText(DetectableTarget observer)
        {
            observer.ch.Events.DebuggerClearCertainDebugText(_chID, _debugType);
        }
        
        
        string ComposeText(DetectableTarget observer, MemoryDetails targetInfo)
        {
            _debugText = String.Empty;
            _debugText += observer.ch.name + _debugPrefixRememberMe;
            if (showCheckFromStartInMemory)
                _debugText += "\n" + _debugPrefixCheckFromStart + targetInfo.NeedToCheckFromStart;
            if (showIsDeadInMemory)
                _debugText += "\n" + _debugPrefixIsDead + targetInfo.IsDead;
            if(showIsProvokedInMemory)
                _debugText += "\n" + _debugPrefixIsProvoked + targetInfo.IsProvoked;
            if(showStartStateInMemory)
                _debugText += "\n" + _debugPrefixStartState + targetInfo.StartState;
            if(showLastKnownPositionText)
                _debugText += "\n" + _debugPrefixLastKnownLocation + targetInfo.LastKnownPosition;

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
            if (showLastKnownPositionGizmo)
                DrawEdgeGizmo();
        }

        void DrawEdgeGizmo()
        {
            foreach (var target in _whoSawMe)
            {
                Gizmos.color = _gizmosColor;
                Gizmos.DrawSphere(target.Value.LastKnownPosition, _gizmosRadius);
                Handles.Label(
                    position: target.Value.LastKnownPosition + Vector3.up * 3f,
                    text: target.Key.ch.name + " last know position for " + target.Key.ch.name);
            }
        }
        #endregion
    }
}
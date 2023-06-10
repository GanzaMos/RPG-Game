using System;
using System.Collections.Generic;
using System.Numerics;
using AI;
using AI.Sensors;
using UI;
using Vector3 = UnityEngine.Vector3;

namespace RPG.Attributes
{
    public struct CharacterEvents
    {
        public Action OnAttack;
        public Action OnDeath;

        //Awareness
        public Action<float> OnAwarenessRatePerSecondChange;
        public Action<DetectableTarget> UpdateSeenMemory;
        
        //Health
        public Action<bool> OnIsDeadChange;
        
        //AI
        public Action<bool> OnIsProvokedChange;

        //Data
        public Action<CharacterDataStruct.VisionStruct> OnVisionDataChange;
        
        //Sensors
        public Action<float> OnMakeSound;
        public Action<float, Vector3, DetectableTarget> OnAcceptSound;
        public Action<Dictionary<DetectableTarget, SoundGeneratorSensor.SoundDetails>> OnAcceptSoundDebugger;
        public Action<DetectableTarget> OnVisualFieldDetection;
        public Action<DetectableTarget> OutVisualFieldDetection;
        public Action<DetectableTarget> OnSenseFieldDetection;
        public Action<DetectableTarget> OutSenseFieldDetection;
        
        //Memory
        public Action<DetectableTarget, MemoryDetails> UpdateAIMemoryDetectorInfo;
        public Action<DetectableTarget> RemoveFromAIMemoryDetectorList;
        
        //Awareness
        public Action<DetectableTarget> OnCleanAwarenessList;
        public Action<DetectableTarget, TrackedTarget> UpdateAwarenessDetectorInfo;
        public Action<DetectableTarget> RemoveFromAwarenessDetectorList;
        
        //MainSensor 
        public Action<DetectableTarget, MainSensor.EMainSensorListType> AddToMainSensorDebuggerList;
        public Action<DetectableTarget, MainSensor.EMainSensorListType> RemoveFromMainSensorDebuggerList;

        //Debugger
        public Action DebuggerClearAllText;
        public Action<string> DebuggerForceAddText;
        public Action<int, EDebugType> DebuggerClearCertainDebugText;
        public Action<int, EDebugType, int> DebuggerSetOrder;
        public Action<int, EDebugType, string> DebuggerAddLine;
        public Action<int, EDebugType, string, float> DebuggerAddDestructibleLine;
        public Action<int, EDebugType, string> DebuggerAddText;
        public Action<int, EDebugType, string, float> DebuggerAddDestructibleText;
        
    }
}
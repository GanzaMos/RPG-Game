using System;
using System.Collections.Generic;
using System.Data.Common;
using RPG.Attributes;
using RPG.Control;
using UnityEditor;
using UnityEngine;

namespace AI.Sensors
{
    public class SoundGeneratorSensor : BaseSensor
    {
        [SerializeField] public AnimationCurve noiseDistanceFactor;
        [SerializeField] AnimationCurve soundVolumeFactor;

        [HideInInspector] public bool sendAcceptorsInfoToDebugger = true;
        
        //Cached
        DetectableTarget _thisTarget;

        //Local
        Dictionary<DetectableTarget, SoundDetails> _foeAcceptors = new Dictionary<DetectableTarget, SoundDetails>();
        Vector3 _vectorFromTarget;
        float _soundVolumeForTarget;
        
        protected override void Awake()
        {
            base.Awake();
            _thisTarget = GetComponentInParent<DetectableTarget>() ?? InstError<DetectableTarget>();
        }

        void OnEnable()
        {
            Ch.Events.OnMakeSound += OnMakeSound;
        }

        void OnDisable()
        {
            Ch.Events.OnMakeSound -= OnMakeSound;
        }
        
        
        //Activate by Event 
        void OnMakeSound(float soundVolume)
        {
            //print("Activate OnMakeSound in Hearing sensor");  
            
            float noiseDistance = Ch.data.hearing.Radius * noiseDistanceFactor.Evaluate(soundVolume);
            float noiseDistanceSqr = noiseDistance * noiseDistance;

            //check all candidates
            for (int i = 0; i < InHearingRadius.Count; i++)
            {
                var target = InHearingRadius[i];
                
                if (!ThreatsInVisionRadius.Contains(target)) continue;
                
                _vectorFromTarget = transform.position - target.transform.position; //we will send this info to Acceptor, so it's reversed
                _soundVolumeForTarget = soundVolume * soundVolumeFactor.Evaluate(soundVolume);
                
                //Filling in the Dictionary for debugger
                //Whom we send sound? How far is he? What will be the sound volume for him?
                if (sendAcceptorsInfoToDebugger)
                    _foeAcceptors.Add(target, new SoundDetails(_vectorFromTarget, _soundVolumeForTarget));
                
                //Activate Sound Sensor on target
                target.ch.Events.OnAcceptSound?.Invoke(_soundVolumeForTarget, _vectorFromTarget, _thisTarget);
            }

            //Sending Dictionary to debugger
            if (sendAcceptorsInfoToDebugger)
            {
                Ch.Events.OnAcceptSoundDebugger(_foeAcceptors);
                _foeAcceptors.Clear();
            }
        }

        public struct SoundDetails
        {
            public SoundDetails(Vector3 vectorFromTarget, float soundVolumeForAcceptor)
            {
                Distance = vectorFromTarget.magnitude;
                AcceptedVolume = soundVolumeForAcceptor;
            }
            
            public float Distance;
            public float AcceptedVolume;
        }
    }
}
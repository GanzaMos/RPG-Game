using System;
using RPG.Attributes;
using UI;
using UnityEngine;

namespace AI.Sensors
{
    public class HearingSensorDebugger : BaseSensor
    {
        #region Variables

        //Serialized
        [Tooltip("Will show text for every sound from a threat that this target will hear")]
        [SerializeField] bool showAcceptedSounds = false;
        [SerializeField] bool acceptedSoundsVolume = false;
        [SerializeField] bool acceptedSoundsDistance = false;
        [SerializeField] float timeToDestroyMessages = 1f;

        //Locals
        string _acceptorTextToDisplay;
        string _acceptorTextPrefixName = "Sound src: ";
        string _acceptorTextPrefixDist = ", Dist: ";
        string _acceptorTextPrefixValue = ", Value: ";

        #endregion //Variables


        #region Initialization

        void Awake()
        {
            base.Awake();
        }

        void OnEnable()
        {
            Ch.Events.OnAcceptSound += OnAcceptSound;
        }
        
        void OnDisable()
        {
            Ch.Events.OnAcceptSound -= OnAcceptSound;
        }

        #endregion //Initialization
        

        #region Main methods

        void OnAcceptSound(float soundVolume, Vector3 vectorToSoundSource, DetectableTarget target)
        {
            if(!showAcceptedSounds) return;
            
            _acceptorTextToDisplay = String.Empty;

            if (showAcceptedSounds)
                _acceptorTextToDisplay += _acceptorTextPrefixName + target.ch.name;
            if (acceptedSoundsVolume)
                _acceptorTextToDisplay += _acceptorTextPrefixDist + soundVolume.ToString("F2");
            if (acceptedSoundsDistance)
                _acceptorTextToDisplay += _acceptorTextPrefixValue + vectorToSoundSource.magnitude.ToString("F2");

            DisplaySoundTextLine(_acceptorTextToDisplay);
        }
        
        
        void DisplaySoundTextLine(string textToDisplay)
        {
            Ch.Events.DebuggerAddDestructibleLine(
                ChID, 
                EDebugType.SensorAcceptedSound, 
                textToDisplay, 
                timeToDestroyMessages); 
        }

        #endregion //Main methods
    }
}
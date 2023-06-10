using System;
using System.Collections.Generic;
using RPG.Attributes;
using UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AI.Sensors
{
    public class SoundGeneratorDebugger : BaseSensor
    {
        #region Serializable

        [Header("Sound Debug text")]
        
        [Tooltip("Shows text every time when object makes sound")]
        [SerializeField] bool showMadeSoundsText = false;
        [SerializeField] bool showMaxDistanceInText = false;
        [SerializeField] float timeToDestroyMessages = 1f;

        [Space(5f)]
        [Header("Sound Recipients")]
        
        [Tooltip("Show list of threat targets in sound radius, which accepted made sounds")]
        [SerializeField] public bool showSoundRecipients = false;
        [SerializeField] public bool showDistanceToRecipients = false;
        [SerializeField] public bool showSoundVolumeForRecipients = false;
        
        
        [Space(5f)]
        [Header("Sound Generator")]
        
        [Tooltip("Generate sounds every X seconds with Y volume")]
        [SerializeField] bool activateSoundGenerator = false;
        [SerializeField] float soundGeneratorPeriod = 1f;
        [SerializeField] float soundGeneratorVolume = 1f;
        
        [Space(5f)]
        [Tooltip("Generate sounds will be different")]
        [SerializeField] bool randomizeGeneratedSounds = false;
        [SerializeField] float randomizePercentage = 30f;

        #endregion //Serializable

        #region Locals

        //Cached
        SoundGeneratorSensor _soundGeneratorSensor;

        //Local variables
        string debugTextVolume = "sound, vol=";
        string debugTextDistance = ", maxDist=";
        float _timer;

        //Local variables for ShowRecipientsList()
        string _acceptorTextToDisplay;
        string _acceptorTextPrefixName = "Acceptor: ";
        string _acceptorTextPrefixDist = ", Dist: ";
        string _acceptorTextPrefixValue = ", Value: ";

        #endregion //Locals

        #region Instantiation

        void Awake()
        {
            base.Awake();
            _soundGeneratorSensor = GetComponent<SoundGeneratorSensor>() ?? InstError<SoundGeneratorSensor>();
            _timer = soundGeneratorPeriod;
            _soundGeneratorSensor.sendAcceptorsInfoToDebugger = showSoundRecipients;
        }

        void OnEnable()
        {
            Ch.Events.OnAcceptSoundDebugger += OnAcceptSound;
        }
        
        void OnDisable()
        {
            Ch.Events.OnAcceptSoundDebugger -= OnAcceptSound;
        }

        void OnValidate()
        {
            if (!Application.isPlaying) return;
            
            _soundGeneratorSensor.sendAcceptorsInfoToDebugger = showSoundRecipients;
        }

        #endregion //Instantiation
        
        
        void Update()
        {
            //works only if activateSoundGenerator == TRUE;
            GenerateSound(); 
        }
        
        
        #region Show Text Line On Sound

        //Sends messages to Debug Text Controller through Events
        //Sends every time when a sound is made
        void OnMakeSound(float soundVolume)
        {
            if(!showMadeSoundsText) return;

            DisplaySoundTextLine(VolumeText(soundVolume) + MaxDistanceText(soundVolume));
        }
        
        string VolumeText(float soundVolume)
        {
            return debugTextVolume + soundVolume.ToString("F2");
        }

        string MaxDistanceText(float soundVolume)
        {
            if (showMaxDistanceInText)
            {
                //Uses hearing radius from Data
                //Uses Animation curve from Sound Generator Sensor
                float noiseDistance = Ch.data.hearing.Radius * _soundGeneratorSensor.noiseDistanceFactor.Evaluate(soundVolume);
                return debugTextDistance + noiseDistance.ToString("F2");
            }
            else
            {
                return String.Empty;
            }
        }
        
        void DisplaySoundTextLine(string text)
        {
            Ch.Events.DebuggerAddDestructibleLine(
                ChID, 
                EDebugType.SensorMadeSound, 
                text, 
                timeToDestroyMessages); 
        }

        #endregion //Show Text Line On Sound

        
        #region Show Sound Acceptors

        //Show List of foe sound recipients 
        //Can show their names, distance to them, sound values they accepted
        public void OnAcceptSound(Dictionary<DetectableTarget, SoundGeneratorSensor.SoundDetails> foeAcceptors)
        {
            if (foeAcceptors.Count == 0)
            {
                DisplaySoundTextLine("None threat targets to accept sound");
                return;
            }

            if(!showSoundRecipients) return;
            
            foreach (var pair in foeAcceptors)
            {
                _acceptorTextToDisplay = String.Empty;
                _acceptorTextToDisplay += _acceptorTextPrefixName + pair.Key.ch.name;
                
                if (showDistanceToRecipients)
                    _acceptorTextToDisplay += _acceptorTextPrefixDist + pair.Value.Distance.ToString("F2");
                if (showSoundVolumeForRecipients)
                    _acceptorTextToDisplay += _acceptorTextPrefixValue + pair.Value.AcceptedVolume.ToString("F2");

                DisplaySoundTextLine(_acceptorTextToDisplay);
            }
        }

        #endregion //Show Sound Acceptors

        
        #region Sound generator

        //Generate sounds with different frequency and volume
        void GenerateSound()
        {
            if(!activateSoundGenerator) return;
            
            _timer += Time.deltaTime;
            if (_timer < soundGeneratorPeriod) return;
            _timer = 0;

            if(!randomizeGeneratedSounds)
                Ch.Events.OnMakeSound?.Invoke(soundGeneratorVolume);
            else
                Ch.Events.OnMakeSound?.Invoke(RandomizeSoundVolume(soundGeneratorVolume));
        }

        //randomize sounds Volume for generator, so it will differ from each other in Debug Text Controller
        float RandomizeSoundVolume(float volume)
        {
            float minVolume = volume - (volume * (randomizePercentage / 100f));
            float maxVolume = volume + (volume * (randomizePercentage / 100f));

            return Random.Range(minVolume, maxVolume);
        }

        #endregion //Sound generator
    }
}
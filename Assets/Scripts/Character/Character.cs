using System;
using System.Data.Common;
using AI.Sensors;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;

namespace RPG.Attributes
{
    public class Character : MonoBehaviour
    {
        public CharacterDataStruct data = new CharacterDataStruct();
        public CharacterEvents Events = new CharacterEvents();
        public CharacterDataLists DataLists = new CharacterDataLists();

        
        [SerializeField] public StartStatPresetSO startStatPresetSo;
        [HideInInspector] public AwarenessSystem awarenessSystem;
        [HideInInspector] public DetectableTarget detectableTarget;
        
        
        //setup the CharacterData struct with the start values
        void Awake()
        {
            data.SetCharacterReference(this);
            startStatPresetSo.SetupCharacterStruct(this);
            
            awarenessSystem = GetComponentInChildren<AwarenessSystem>();
            detectableTarget = GetComponent<DetectableTarget>();
        }
    }
}
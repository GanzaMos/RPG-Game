using System;
using System.Collections.Generic;
using RPG.Core;
using RPG.Stats;
using UnityEngine;

namespace AI
{
    //Class contains additional info about Detectable target
    //Used for seenTargets and threatsInVisionRange memory lists;
    [Serializable]
    public class MemoryDetails
    {
        [SerializeField] CharacterClass characterClass;
        public CharacterClass CharacterClass
        {
            get => characterClass;
            set => characterClass = value;
        }
        
        [SerializeField] EStartState startState;
        public EStartState StartState
        {
            get => startState;
            set => startState = value;
        }
        
        [SerializeField] Vector3 startPosition;
        public Vector3 StartPosition
        {
            get => startPosition;
            set => startPosition = value;
        }

        [SerializeField] bool needToCheckFromStart;
        public bool NeedToCheckFromStart
        {
            get => needToCheckFromStart;
            set => needToCheckFromStart = value;
        }

        [SerializeField] bool isDead;
        public bool IsDead
        {
            get => isDead;
            set => isDead = value;
        }

        [SerializeField] bool isProvoked;
        public bool IsProvoked
        {
            get => isProvoked;
            set => isProvoked = value;
        }
        
        [SerializeField] float awarenessFactor;
        public float AwarenessFactor
        {
            get => awarenessFactor;
            set => awarenessFactor = value;
        }

        [SerializeField] Vector3 lastKnownPosition;
        public Vector3 LastKnownPosition
        {
            get => lastKnownPosition;
            set => lastKnownPosition = value;
        }        
        
        [SerializeField] float lastSeenTime;
        public float LastSeenTime
        {
            get => lastSeenTime;
            set => lastSeenTime = value;
        }

        public MemoryDetails()
        {
            AwarenessFactor = 1f;
            CharacterClass = CharacterClass.None;
            StartState = EStartState.None;
        }
    }
}
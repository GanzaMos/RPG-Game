using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0)]

    public class Progression : ScriptableObject
    {
        //Layer 1, how many Character Classes Progressions can we have
        [SerializeField] CharacterClassProgression[] characterClassesProgressions;

        //Layer 2, what is the exact Class and how may stats can we describe in this Character Class Progression?
        [System.Serializable]
        class CharacterClassProgression
        {
            public CharacterClass characterClass;
            public ProgressionStat[] progressionStats;
        }

        //Layer 3, what is it the exact Stat and how many levels it has?
        [System.Serializable]
        class ProgressionStat
        {
            public Stat stat;
            public float[] levels;
        }

        Dictionary<CharacterClass, Dictionary<Stat, float[]>> _lookupTable;   //So, we need a Matroshka dictionary for a quick look up

        
        public float GetStat(CharacterClass characterClass, int currentLevel, Stat stat)
        {
            BuildOurDictionary();
            float[] levels = _lookupTable[characterClass][stat];
            if (levels.Length < currentLevel)
            {
                Debug.Log($"Level array size in Dictionary is {levels.Length}. Level {currentLevel} is acquired");
                return 0f;
            }

            return levels[currentLevel - 1];
        }
        

        void BuildOurDictionary()
        {
            if (_lookupTable != null) return; //we only need to build Dictionary once!

            _lookupTable =
                new Dictionary<CharacterClass,
                    Dictionary<Stat, float[]>>(); //creating an empty Big dictionary of acquired type

            foreach (CharacterClassProgression characterClass in
                     characterClassesProgressions) //looking for all Classes in Classes container
            {
                var statLookupTable =
                    new Dictionary<Stat, float[]>(); //creating an empty Small dictionary of acquired type

                foreach (var progressionStat in
                         characterClass.progressionStats) //looking for all Stats in Stats container
                {
                    statLookupTable[progressionStat.stat] =
                        progressionStat.levels; //setting Small Dictionary, Stat = key, Levels array = value
                }

                _lookupTable[characterClass.characterClass] =
                    statLookupTable; //setting Big Dictionary, Class = key, Small Dictionary = value
            }
        }
    }
}

using System;
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
        
        
        //Start looking for a acquired Character Class in Character Class Progression array from Layer 1
        
        public float GetStat(CharacterClass characterClass, int currentLevel, Stat stat)
        {
            foreach (CharacterClassProgression charClassProgression in characterClassesProgressions)
            {
                if (charClassProgression.characterClass != characterClass) continue;     //go to the next element if current doesn't match
                
                return LookingForAcquiredStat(currentLevel, stat, charClassProgression); //go to the Layer 2
            }
            Debug.Log($"Can't find acquired {characterClass} class in Progression SO");
            return 11.3f;
        }
        
        
        //Start looking for a acquired Stat in Character Class Stat array from Layer 2

        float LookingForAcquiredStat(int currentLevel, Stat stat, CharacterClassProgression charClassProgression)
        {
            foreach (ProgressionStat progressionStat in charClassProgression.progressionStats)
            {
                if (progressionStat.stat != stat) continue;                               //go to the next element if current doesn't match
                if (progressionStat.levels.Length < currentLevel)                         //check if our Stat in Progression SO even has an acquired level 
                {
                    Debug.Log($"{stat} max level in Progression SO is {progressionStat.levels.Length}, but level {currentLevel} is acquired");
                }
                return progressionStat.levels[currentLevel - 1];                          //Layer 3, here is it, now send it back to GetStat!
            }
            Debug.Log($"Can't find acquired {stat} in Progression Stats in Progression SO"); 
            return 11.2f;
        }
    }
}

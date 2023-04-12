using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace RPG.Stats
{
    [CreateAssetMenu(fileName = "Progression", menuName = "Stats/New Progression", order = 0)]

    public class Progression : ScriptableObject
    {
        [SerializeField] CharacterClassProgression[] characterClassesProgressions;

        [System.Serializable]
        class CharacterClassProgression
        {
            public CharacterClass characterClass;
            public ProgressionStat[] progressionStats;
        }
        
        [System.Serializable]
        class ProgressionStat
        {
            public Stat stat;
            public float[] levels;
        }
        
        
        public float GetStat(CharacterClass characterClass, int currentLevel, Stat stat)
        {
            foreach (CharacterClassProgression charClassProgression in characterClassesProgressions)
            {
                if (charClassProgression.characterClass != characterClass) continue;
                foreach (ProgressionStat progressionStat in charClassProgression.progressionStats)
                {
                    if (progressionStat.stat != stat) continue;
                    if (progressionStat.levels.Length < currentLevel)
                    {
                        Debug.Log($"{stat} max level in Progression SO is {progressionStat.levels.Length}, but {currentLevel} level is acquired");
                    }
                    return progressionStat.levels[currentLevel - 1];
                }
                Debug.Log($"Can't find acquired {stat} in Progression Stats in Progression SO");
                return 11.2f;
            }
            Debug.Log($"Can't find acquired {characterClass} class in Progression SO");
            return 11.3f;
        }
    }
}

using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        //Serializable
        [Range(1, 99)] [SerializeField] int startingLevel = 1;
        [SerializeField] CharacterClass characterClass;
        [SerializeField] Progression progression = null;

        
        //Main universal work method
        public float GetStat(Stat stat)
        {
            return progression.GetStat(characterClass, startingLevel, stat);
        }
        
    }
}
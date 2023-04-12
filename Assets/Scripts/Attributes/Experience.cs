using RPG.Saving;
using UnityEngine;

namespace RPG.Attributes
{
    public class Experience : MonoBehaviour, ISaveable
    {
        [SerializeField] float currentExperiencePoints = 0f;
        public float CurrentExperiencePoints => currentExperiencePoints;

        
        //Main Methods
        
        public void GetExperience(float experienceAmount)
        {
            currentExperiencePoints += experienceAmount;
        }

        
        //Saving System
        
        public object CaptureState()
        {
            return currentExperiencePoints;
        }

        public void RestoreState(object state)
        {
            currentExperiencePoints = (float) state;
        }
    }
}
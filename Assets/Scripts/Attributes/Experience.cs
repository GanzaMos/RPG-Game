using RPG.Saving;
using UnityEngine;

namespace RPG.Attributes
{
    public class Experience : MonoBehaviour, ISaveable
    {
        [SerializeField] float currentExperience = 0f;

        
        //Main Methods
        
        public void GetExperience(float experienceAmount)
        {
            currentExperience += experienceAmount;
        }

        
        //Saving System
        
        public object CaptureState()
        {
            return currentExperience;
        }

        public void RestoreState(object state)
        {
            currentExperience = (float) state;
        }
    }
}
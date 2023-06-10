using AI.Sensors;
using RPG.Attributes;
using RPG.Control;
using UnityEngine;

namespace AI.Action_System
{
    public class BaseAction
    {
        protected BaseAction(ActionController actionController)
        {
            ActionController = actionController;
            StateManager = actionController.stateManager;
            ThisCharacter = actionController.ch;
        }

        protected Character ThisCharacter;
        protected ActionController ActionController;
        protected EnemyAIStateManager StateManager;

        public int ActionPriority;
        public DetectableTarget ReasonForAction;
        public float TimeSinceActionInActivePool;
        public float ActionActiveTime;

        protected virtual void GetOldSettings()
        {
        }  

        protected virtual void RestoreOldSettings()
        {
        }
        
        public virtual void EnterAction()
        {
            GetOldSettings();
            ActionActiveTime = 0;
        }

        public virtual void ExitAction()
        {
            RestoreOldSettings();
        }
        
        public virtual void UpdateStartTimer(float timeToAdd)
        {
            TimeSinceActionInActivePool = +timeToAdd;
        }  
        
        public virtual void UpdateActiveTimer(float timeToAdd)
        {
            ActionActiveTime = +timeToAdd;
        }
    }
}
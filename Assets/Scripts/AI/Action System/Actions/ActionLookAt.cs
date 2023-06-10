using System.Collections;
using AI.Sensors;
using UnityEngine;

namespace AI.Action_System
{
    public class ActionLookAt : BaseAction
    {
        public Vector3 PositionToLookAt;
        public float MaxTimeToLookAt;

        float _timer;
        Vector3 _oldLookAtPosition;
        //General Action methods
        
        public ActionLookAt(ActionController actionController) : base(actionController)
        {
        }

        protected override void GetOldSettings()
        {
            _oldLookAtPosition = ThisCharacter.transform.eulerAngles;
        }  

        protected override void RestoreOldSettings()
        {
            ThisCharacter.transform.eulerAngles = _oldLookAtPosition;
        }
        
        public override void EnterAction()
        {
            base.EnterAction();
            _timer = 0;
        }
        
        public override void ExitAction()
        {
            base.ExitAction();
        }

        public override void UpdateStartTimer(float timeToAdd)
        {
            base.UpdateStartTimer(timeToAdd);
        }

        //Specific Action methods
        void SetDataForAction(DetectableTarget reasonForAction, Vector3 positionToLookAt, float timeToLookAtPosition = 3f)
        {
            ReasonForAction = reasonForAction;
            PositionToLookAt = positionToLookAt;
            MaxTimeToLookAt = timeToLookAtPosition;
        }
    }
}
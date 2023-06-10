using RPG.Attributes;
using UnityEngine;

namespace AI.Action_System
{
    public class ActionSearchArea : BaseAction
    {
        public ActionSearchArea(ActionController actionController) : base(actionController) 
        { }
        
        public Character TargetToLookFor;
        public Vector3 CenterOfSearchArea;
        public float SearchRadius;
        
        public int SpotsCountToSearch;
        public int SpotsAlreadySearched;

        public float TimeToSearchOneSpot; 
        
        public float MoveSpeedFractionToSearch;
        
        
        protected override void RestoreOldSettings()
        {
            base.RestoreOldSettings();
        }

        protected override void GetOldSettings()
        {
            base.GetOldSettings();
        }  
        
        public override void EnterAction()
        {
            base.EnterAction();
        }
    }
}
using RPG.Attributes;
using RPG.Combat;

namespace AI.Action_System
{
    public class ActionKillTarget : BaseAction
    {
        public ActionKillTarget(ActionController actionController) : base(actionController)
        {
            ActionPriority = 5;
        }
        
        public Character Ch;
        
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
            StateManager.SwitchState(StateManager.EnemyStateChasing);
        }


    }
}
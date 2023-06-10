using RPG.Attributes;

namespace AI.Action_System
{
    public class ActionGoForHelp : BaseAction
    {
        public ActionGoForHelp(ActionController actionController) : base(actionController) { }
        
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
        }
    }
}
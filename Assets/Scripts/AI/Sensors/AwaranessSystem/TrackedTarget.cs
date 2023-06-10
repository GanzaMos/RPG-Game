using UnityEngine;

namespace AI.Sensors
{
    public class TrackedTarget
    {
        public DetectableTarget Target;
        public Vector3 RawPosition;
        public Vector3 RawDirection;
        public float LastSensedTime = -1f;
        public float Awareness; // 0-1   = not aware (no reaction); 
                                // 1-2   = will look at suspicious place/target
                                // 2-3   = will walk and check what is there
                                // 3     = will run and check what is there
                            

        public bool UpdateAwareness(DetectableTarget target, Vector3 position, Vector3 direction, float awarenessToIncrease)
        {
            var oldAwareness = Awareness;

            if (target == null) 
                Debug.Log($"DetectableTarget for {this} in is Empty");
            
            this.Target      = target;
            RawPosition     = position;
            RawDirection    = direction;
            LastSensedTime  = Time.time;
            Awareness       = Mathf.Clamp(Awareness + awarenessToIncrease, 0f, 3f);
            
            //Debug.Log($"Increasing awareness rate for {target.gameObject.name}");

            if (oldAwareness < 3f && Awareness >= 3f)
                return true;
            if (oldAwareness < 2f && Awareness >= 2f)
                return true;
            if (oldAwareness < 1f && Awareness >= 1f)
                return true;

            return false;
        }

        public bool DecreaseAwareness(float decreaseDelayTime, float awarenessToDecrease)
        {
            // detected too recently - no change
            if ((Time.time - LastSensedTime) < decreaseDelayTime)
                return false;

            var oldAwareness = Awareness;

            Awareness = Mathf.Clamp(Awareness - awarenessToDecrease, 0f, 3f);

            if (oldAwareness >= 3f && Awareness < 3f)
                return true;
            if (oldAwareness >= 2f && Awareness < 2f)
                return true;
            if (oldAwareness >= 1f && Awareness < 1f)
                return true;
            
            return Awareness <= 0f;
        }
    }
}
using UnityEngine;

namespace AI.Sensors
{
    public class HearingSensor : BaseSensor
    {
        void OnEnable()
        {
            Ch.Events.OnAcceptSound += OnAcceptSound;
        }

        void OnDisable()
        {
            Ch.Events.OnAcceptSound -= OnAcceptSound;
        }
        
        void OnAcceptSound(float noiseLevel, Vector3 vectorToTarget, DetectableTarget soundSource)
        {
            Awareness.UpdateAwareness(
                   target: soundSource,
                 position: soundSource.transform.position,
                direction: vectorToTarget,
                awareness: noiseLevel);
        }
    }
}
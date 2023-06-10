using System.Collections.Generic;
using RPG.Attributes;
using RPG.Stats;
using UnityEngine;

namespace AI.Sensors
{
    public class ThreatCheckSO : ScriptableObject
    {
        public virtual bool IsAThreat(DetectableTarget target, CharacterDataLists.SeenTargets seenTargets)
        {
            Debug.Log("Run a base ThreatCheckSO IsAThreatCheck function");
            return false;
        }
    }
}
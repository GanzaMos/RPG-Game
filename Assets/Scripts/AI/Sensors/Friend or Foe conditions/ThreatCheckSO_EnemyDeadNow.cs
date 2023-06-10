using System.Collections.Generic;
using RPG.Attributes;
using RPG.Stats;
using UnityEngine;

namespace AI.Sensors
{
    [CreateAssetMenu(fileName = "ThreatCheckSO_EnemyDeadNow", menuName = "ThreatCheck/Enemy Dead Now", order = 1)]
    public class ThreatCheckSO_EnemyDeadNow : ThreatCheckSO
    {
        public override bool IsAThreat(DetectableTarget target, CharacterDataLists.SeenTargets seenTargets)
        {
            if (!seenTargets.ContainsKey(target)) return false;
            
            //Enemy. Dead now. Wasn't in seen memory.
            if (target.CharacterClass != CharacterClass.Player && target.IsDead && !seenTargets[target].IsDead)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
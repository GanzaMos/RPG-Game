using System.Collections.Generic;
using RPG.Attributes;
using RPG.Stats;
using UnityEngine;

namespace AI.Sensors
{
    [CreateAssetMenu(fileName = "ThreatCheckSO_EnemyProvoked", menuName = "ThreatCheck/Enemy Provoked", order = 2)]
    public class ThreatCheckSO_EnemyProvoked : ThreatCheckSO
    {
        public override bool IsAThreat(DetectableTarget target, CharacterDataLists.SeenTargets seenTargets)
        {
            if (!seenTargets.ContainsKey(target)) return false;
            
            //Enemy. Alive. Provoked. Wasn't provoked in seen memory.
            if (target.CharacterClass != CharacterClass.Player && !target.IsDead && target.IsProvoked && !seenTargets[target].IsProvoked)
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
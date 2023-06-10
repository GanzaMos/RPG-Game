using System.Collections.Generic;
using RPG.Attributes;
using RPG.Stats;
using UnityEngine;

namespace AI.Sensors
{
    [CreateAssetMenu(fileName = "ThreatCheckSO_EnemyDeadFromStart", menuName = "ThreatCheck/Enemy Dead From Start", order = 0)]
    public class ThreatCheckSO_EnemyDeadFromStart : ThreatCheckSO
    {
        public override bool IsAThreat(DetectableTarget target, CharacterDataLists.SeenTargets seenTargets)
        {
            if (!seenTargets.ContainsKey(target)) return false;
            
            //Enemy. Was dead from start;
            if (target.CharacterClass != CharacterClass.Player && seenTargets[target].NeedToCheckFromStart)
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
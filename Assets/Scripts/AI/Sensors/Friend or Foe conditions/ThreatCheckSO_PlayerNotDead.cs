using System.Collections.Generic;
using RPG.Attributes;
using RPG.Stats;
using UnityEngine;

namespace AI.Sensors
{
    [CreateAssetMenu(fileName = "ThreatCheckSO_PlayerNotDead", menuName = "ThreatCheck/Player Not Dead", order = 3)]
    public class ThreatCheckSO_PlayerNotDead : ThreatCheckSO
    {
        public override bool IsAThreat(DetectableTarget target, CharacterDataLists.SeenTargets seenTargets)
        {
            //Player. Isn't dead.
            if (target.CharacterClass == CharacterClass.Player && !target.IsDead)
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
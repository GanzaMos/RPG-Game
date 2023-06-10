using UnityEngine;
using RPG.Core;
using RPG.Attributes;
using RPG.Control;

namespace RPG.Combat
{
    
    [RequireComponent(typeof(Health))]
    
    public class CombatTarget : MonoBehaviour, IRaycastable
    {
        public EInteractType HandleRaycast()
        {
            return EInteractType.Combat;
        }
    }
}
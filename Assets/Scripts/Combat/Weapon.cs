using UnityEngine;
using UnityEngine.Events;

namespace RPG.Combat
{
    public class Weapon : MonoBehaviour
    {
        [SerializeField] UnityEvent onHitEvent;
        
        public void OnHit()
        {
            print("Hit from Weapon!"); //todo need to be optional debug message
            onHitEvent?.Invoke();
        }
    }
}
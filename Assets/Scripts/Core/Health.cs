using RPG.Core.RPG.Core;
using UnityEngine;

namespace RPG.Core
{
    public class Health : MonoBehaviour
    {
        [SerializeField] float healthPoint = 100f;

        public bool isDead;
        
        public void TakeDamage(float damage)
        {
            if (isDead) return;

            healthPoint = Mathf.Max(healthPoint - damage, 0);
            print(transform.name + " health = " + healthPoint);
        
            if (healthPoint <= 0)
            {
                GetComponent<Animator>().SetTrigger("death");
                //GetComponent<BoxCollider>().enabled = false;
                isDead = true;
                GetComponent<ActionScheduler>().CancelCurrentAction();
            }
        }
    }
}
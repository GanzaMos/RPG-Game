using RPG.Core.RPG.Core;
using RPG.Saving;
using UnityEngine;


namespace RPG.Core
{
    [System.Serializable]
    public class Health : MonoBehaviour, ISaveable
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
                Die();
            }
        }

        void Die()
        {
            GetComponent<Animator>().SetTrigger("death");
            isDead = true;
            GetComponent<ActionScheduler>().CancelCurrentAction();
            GetComponent<CapsuleCollider>().enabled = false;
        }

        public object CaptureState()
        {
            return healthPoint;
        }

        
        public void RestoreState(object state)
        {
            float savedHealthPoint = (float) state;
            healthPoint = savedHealthPoint;
            if (healthPoint <= 0)
            {
                GetComponent<Animator>().SetTrigger("death");
                isDead = true;
            }
            else
            {
                isDead = false;
            }
        }
    }
}
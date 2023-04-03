using RPG.Combat;
using RPG.Movement;
using RPG.Core;
using UnityEngine;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        Ray _mousePositionRay;
        RaycastHit _rayHit;
        Mover _mover;
        Fighter _fighter;
        Health _health;

        void Start()
        {
            _mover = GetComponent<Mover>();
            _fighter = GetComponent<Fighter>();
            _health = GetComponent<Health>();
        }

        void Update()
        {
            if (_health.isDead) return;
            
            RayToMousePosition();
            if (InteractWithCombat()) return;
            if (InteractWithMovement()) return;
        }

        void RayToMousePosition()
        {
            _mousePositionRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        bool InteractWithCombat()
        {
            RaycastHit[] hits = Physics.RaycastAll(_mousePositionRay);
            foreach (RaycastHit hit in hits)
            {
                CombatTarget combatTarget = hit.transform.GetComponent<CombatTarget>();
                if (combatTarget == null) continue;
                
                Health health = hit.transform.GetComponent<Health>();
                if (health.isDead) continue;
                
                if (Input.GetMouseButton(0))
                {
                    _fighter.Attack(combatTarget.gameObject);
                }
                return true;
            }
            return false;
        }

        bool InteractWithMovement()
        {
            if (Physics.Raycast(_mousePositionRay, out _rayHit))
            {
                if (Input.GetMouseButton(0))
                {
                    _mover.MoveTo(_rayHit.point);
                }
                return true;
            }
            return false;
        }
    }
}

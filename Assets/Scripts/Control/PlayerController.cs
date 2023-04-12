using System;
using RPG.Combat;
using RPG.Movement;
using RPG.Core;
using RPG.Attributes;
using UnityEngine;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        //Cashed
        Mover _mover;
        Fighter _fighter;
        Health _health;
        
        //Local variables
        Ray _mousePositionRay;
        RaycastHit _rayHit;

        
        //Starting setup
        
        void Start()
        {
            _mover = GetComponent<Mover>()       ?? throw new Exception($"Missing Mover for PlayerController in {gameObject.name}");
            _fighter = GetComponent<Fighter>()   ?? throw new Exception($"Missing Fighter for PlayerController in {gameObject.name}");
            _health = GetComponent<Health>()     ?? throw new Exception($"Missing Health for PlayerController in {gameObject.name}");
        }

        void Update()
        {
            if (_health.IsDead) return;
            
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
                if (!combatTarget) continue;
                
                Health health = hit.transform.GetComponent<Health>();
                if (health.IsDead) continue;
                
                if (Input.GetMouseButton(0))
                {
                    _fighter.SetTarget(combatTarget.gameObject);
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

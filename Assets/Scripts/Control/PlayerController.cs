using System;
using System.Collections.Generic;
using System.Linq;
using RPG.Combat;
using RPG.Movement;
using RPG.Core;
using RPG.Attributes;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

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
        GameObject _nearestRaycastableObject;
        
        [Serializable]
        struct CursorMapping
        {
            public InteractType interactType;
            public Texture2D cursorTexture2D;
            public Vector2 cursorHotspot;
        }

        [SerializeField] float maxDistanceToNavMeshEdge = 1f;
        [SerializeField] float maxNavMeshEdgePathDistance = 40f;
        [SerializeField] CursorMapping[] cursorMappings;

        //Starting setup
        
        void Awake()
        {
            _mover = GetComponent<Mover>()       ?? throw new Exception($"Missing Mover for PlayerController in {gameObject.name}");
            _fighter = GetComponent<Fighter>()   ?? throw new Exception($"Missing Fighter for PlayerController in {gameObject.name}");
            _health = GetComponent<Health>()     ?? throw new Exception($"Missing Health for PlayerController in {gameObject.name}");
        }

        void Update()
        {
            RayToMousePosition();
            
            if (UiCheck()) return;
            if (CheckIfPlayerIsDead()) return;
            if (RaycastableComponentCheck()) return;
            if (IsNavMeshNearCheck()) return;
            if (NavMeshPathCheck()) return;
        }

        
        //Check methods
        
        bool UiCheck()
        {
            if (EventSystem.current.IsPointerOverGameObject())
            {
                SetCursor(InteractType.UI);
                return true;
            }
            else return false;
        }

        bool CheckIfPlayerIsDead()
        {
            if (_health.IsDead)
            {
                SetCursor(InteractType.Movement);
                return true;
            }

            return false;
        }

        bool RaycastableComponentCheck()
        {
            //  Checking for all IRaycastable elements on the rays way, saving their game objects and distances to them 
            RaycastHit[] hits = Physics.RaycastAll(_mousePositionRay);
            Dictionary<float, GameObject> raycastArrayToSort = new Dictionary<float, GameObject>();

            foreach (RaycastHit hit in hits)
            {
                IRaycastable raycastableObject = hit.transform.GetComponent<IRaycastable>();
                if (raycastableObject == null) continue;
                raycastArrayToSort[hit.distance] = hit.transform.gameObject;
            }

            //No IRaycastable on the way? Return o Update
            if (raycastArrayToSort.Count == 0) return false;

            //  Sorting all collected IRaycastable, looking for a closest one
            float maxDistanceSoFar = float.MaxValue;
            for (int i = 0; i < raycastArrayToSort.Count; i++)
            {
                if (raycastArrayToSort.Keys.ElementAt(i) < maxDistanceSoFar)
                {
                    maxDistanceSoFar = raycastArrayToSort.Keys.ElementAt(i);
                    _nearestRaycastableObject = raycastArrayToSort.Values.ElementAt(i);
                }
            }
            
            return InteractWithRaycastableComponent(_nearestRaycastableObject);
        }

        bool IsNavMeshNearCheck()
        {
            NavMeshHit navMeshHit;
            Physics.Raycast(_mousePositionRay, out _rayHit);
            bool isNavMeshNear = NavMesh.SamplePosition(_rayHit.point, out navMeshHit, maxDistanceToNavMeshEdge, NavMesh.AllAreas);

            if (!isNavMeshNear)
            {
                SetCursor(InteractType.None);
                return true;
            }
            else
            {
                return false;
            }
        }

        bool NavMeshPathCheck()
        {
            NavMeshPath path = new NavMeshPath();
            NavMesh.CalculatePath(transform.position, _rayHit.point, NavMesh.AllAreas, path);
            
            float lengthSoFar = 0.0F;
            for (int i = 1; i < path.corners.Length; i++) {
                lengthSoFar += Vector3.Distance(path.corners[i - 1], path.corners[i]);
            }

            //checking if path is completely available (need to exclude NavMesh islands)
            if (path.status != NavMeshPathStatus.PathComplete)
            {
                SetCursor(InteractType.None);
                return false;
            }
            
            //checking if path is small enough so we won't go through all the map
            if (lengthSoFar > maxNavMeshEdgePathDistance)
            {
                SetCursor(InteractType.None);
                return false;
            }
            
            //finally go there!
            else
            {
                SetCursor(InteractType.Movement);
                InteractWithMovement();
                return true;
            }
        }

        
        //Interaction methods
        
        bool InteractWithRaycastableComponent(GameObject nearestRaycastableObject)
        {
            //Setting behavior depend on HandleRaycast() return value in IRaycastable object;
            InteractType nearestObjectInteractType = nearestRaycastableObject.GetComponent<IRaycastable>().HandleRaycast();
            
            if (nearestObjectInteractType == InteractType.Combat)
            {
                Health health = nearestRaycastableObject.GetComponent<Health>();
                if (health.IsDead) return false;
                
                SetCursor(InteractType.Combat);
                InteractWithCombat(nearestRaycastableObject);
                return true;
            }
            
            if (nearestObjectInteractType == InteractType.Pickup)
            {
                SetCursor(InteractType.Pickup);
                InteractWithMovement();
                return true;
            }
            
            return false;
        }
        
        void InteractWithCombat(GameObject combatTarget)
        {
            if (Input.GetMouseButton(0))
            {
                _fighter.SetTarget(combatTarget);
            }
        }
        
        void InteractWithMovement()
        {
            if (Input.GetMouseButton(0))
            {
                _mover.MoveTo(_rayHit.point);
            }
        }
        
        
        //Private methods
        
        void RayToMousePosition()
        {
            _mousePositionRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        void SetCursor(InteractType interactType)
        {
            CursorMapping mapping = GetCursorMapping(interactType);
            Cursor.SetCursor(mapping.cursorTexture2D, mapping.cursorHotspot, CursorMode.Auto);
        }
        
        CursorMapping GetCursorMapping(InteractType interactType)
        {
            foreach (CursorMapping element in cursorMappings)
            {
                if (element.interactType != interactType) continue;
                return element;
            }
            return cursorMappings[0];
        }
    }
}

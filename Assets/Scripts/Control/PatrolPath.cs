using System;
using UnityEngine;

namespace RPG.Control
{
    public class PatrolPath : MonoBehaviour
    {
        [SerializeField] float gizmosRadius = 0.5f;
        void OnDrawGizmos()
        {

            Vector3 currentWaypoint;
            Vector3 nextWaypoint;

            for (int i = 0; i < transform.childCount; i++)
            {
                currentWaypoint = GetWaypointPosition(i);
                nextWaypoint = GetNextWaypointPosition(i);
                Gizmos.DrawSphere(currentWaypoint, gizmosRadius);
                Gizmos.DrawLine(currentWaypoint, nextWaypoint);
            }
    
            Gizmos.color = Color.grey;
        }

        public Vector3 GetWaypointPosition(int i)
        {
            return transform.GetChild(i).position;
        }
        
        public Vector3 GetNextWaypointPosition(int i)
        {
            if (i == transform.childCount-1)
            {
                return transform.GetChild(0).position;
            }
            else
            {
                return transform.GetChild(i + 1).position;
            }
        }
    }
}
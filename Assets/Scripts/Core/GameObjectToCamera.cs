using UnityEngine;

namespace RPG.Core
{
    public class GameObjectToCamera : MonoBehaviour
    {
        void LateUpdate()
        {
            transform.forward = Camera.main.transform.forward;
        }
    }
}
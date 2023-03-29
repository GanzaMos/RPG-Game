using UnityEngine;
using UnityEngine.AI;

public class PlayerMoveHandler : MonoBehaviour
{
    NavMeshAgent _navMeshAgent;
    Animator _animator;

    RaycastHit rayHit;
    Ray lastRay;

    void Start()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _animator = GetComponent<Animator>();
    }


    void Update()
    {
        if (Input.GetMouseButton(0))
        {
            MoveToCursor();
        }

        UpdateAnimator();
    }
    
    void MoveToCursor()
    {
        lastRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(lastRay, out rayHit))
        {
            _navMeshAgent.SetDestination(rayHit.point);
        }

    }
    
    void UpdateAnimator()
    {
        Vector3 velocity = _navMeshAgent.velocity;
        Vector3 localVelocity = transform.InverseTransformDirection(velocity);
        float speed = localVelocity.z;
        _animator.SetFloat("forwardSpeed", speed);
    }
}

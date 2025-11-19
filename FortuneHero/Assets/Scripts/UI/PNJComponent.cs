using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(PatrolComponent))]
public class PNJComponent : MonoBehaviour
{
    PatrolComponent patrol;
    NavMeshAgent agent;
    [SerializeField] float stoppingDistance = 2f;
    [SerializeField] float patrolSpeed = 3.5f;

    void Start()
    {
        patrol = GetComponent<PatrolComponent>();
        agent = GetComponent<NavMeshAgent>();

        // Définit comment on bouge (PatrolComponent appellera cette fonction)
        patrol.move = Move;

        // active le patrol (sinon il reste inactif)
        patrol.isActive = true;

        // règle la vitesse du navmesh agent
        agent.speed = patrolSpeed;
    }

    void Move(Transform newTarget)
    {
        if (Vector3.Distance(newTarget.position, transform.position) <= stoppingDistance)
            newTarget = patrol.NextTarget();

        agent.SetDestination(newTarget.position);
    }
}

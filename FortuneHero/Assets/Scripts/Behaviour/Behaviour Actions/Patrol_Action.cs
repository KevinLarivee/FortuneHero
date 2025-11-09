using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.GraphicsBuffer;

public class Patrol_Action : Behaviour_Node
{
    Transform[] patrolPoints;
    private int currentPatrolIndex = 0;
    NavMeshAgent agent;
    Vector3 target;
    public Patrol_Action(Behaviour_Condition[] conditions,NavMeshAgent agent, params Transform[] patrolPoints)
        : base(conditions)
    {
        this.agent = agent;
        this.patrolPoints = patrolPoints;
    }

    public override void ExecuteAction(Behaviour_Composite parentComposite)
    {
        base.ExecuteAction(parentComposite);
        // Je definit la prochaine destination de la patrouille
        target = patrolPoints[currentPatrolIndex].position;
        agent.SetDestination(target);
    }

    public override void Tick(float deltaTime)
    {
        // Quand l'agent atteint la destination, on passe au prochain point

        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
            FinishAction(true);
        }

    }
    public override void InteruptAction()
    {
        agent.destination = agent.transform.position;
        base.InteruptAction();
    }
}

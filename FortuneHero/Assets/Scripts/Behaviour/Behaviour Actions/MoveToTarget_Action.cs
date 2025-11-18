using UnityEngine;
using UnityEngine.AI;

public class MoveToTarget_Action : Behaviour_Node
{
    float speed;
    NavMeshAgent agent;
    GameObject target;
    // Avec un gros temps, peut devenir "infini"
    float maxTime = 0f;
    float currentTime = 0f;

    float initialSpeed;

    public MoveToTarget_Action(Behaviour_Condition[] behaviour_Conditions, GameObject target, float speed, NavMeshAgent agent, float maxTime) : base(behaviour_Conditions)
    {
        this.target = target;
        this.speed = speed;
        this.agent = agent;
        this.maxTime = maxTime;
    }

    public override void ExecuteAction(Behaviour_Composite parent_composite)
    {
        base.ExecuteAction(parent_composite);
        agent.isStopped = false;
        initialSpeed = agent.speed;
        agent.speed = speed;
        agent.SetDestination(target.transform.position);
        currentTime = maxTime;
    }
    public override void Tick(float deltaTime)
    {
        base.Tick(deltaTime);

        if (currentTime > 0f)
        {
            currentTime -= deltaTime;
            if (currentTime <= 0f)
            {
                FinishAction(false);
            }
            // Si a un timer, empêche de prendre fin avant la fin du timer
            return;
        }

        if (agent.remainingDistance <= agent.stoppingDistance)
            FinishAction(true);
    }
    public override void FinishAction(bool result)
    {
        agent.speed = initialSpeed;
        agent.SetDestination(agent.transform.position);
        base.FinishAction(result);
    }
}

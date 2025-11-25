using UnityEngine;
using UnityEngine.AI;

public class Chase_Action : Behaviour_Node
{
    NavMeshAgent agent;
    Transform target;

    public Chase_Action(Behaviour_Condition[] behaviour_Conditions, NavMeshAgent agent, Transform target) : base(behaviour_Conditions)
    {
        this.agent = agent;
        this.target = target;
    }

    public override void ExecuteAction(Behaviour_Composite parent_composite)
    {
        Debug.Log("Start chasing");
        agent.SetDestination(target.transform.position);
        base.ExecuteAction(parent_composite);
    }
    public override void Tick(float deltaTime)
    {
        if (agent.remainingDistance <= agent.stoppingDistance)
            FinishAction(true);
        else
        {
            if(!interupted)
                agent.SetDestination(target.transform.position);
        }
        base.Tick(deltaTime);
    }
    public override void InteruptAction()
    {
        agent.destination = agent.transform.position;
        base.InteruptAction();
    }
    public override void FinishAction(bool result)
    {
        Debug.Log("End Chasing");
        agent.destination = agent.transform.position;
        base.FinishAction(result);
    }
}

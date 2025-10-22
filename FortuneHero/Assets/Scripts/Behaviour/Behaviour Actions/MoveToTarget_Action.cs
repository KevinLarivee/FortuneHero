using UnityEngine;
using UnityEngine.AI;

public class MoveToTarget_Action : Behaviour_Node
{
    NavMeshAgent agent;
    GameObject target;

    public MoveToTarget_Action(Behaviour_Condition[] behaviour_Conditions, NavMeshAgent agent, GameObject target) : base(behaviour_Conditions)
    {
        this.agent = agent;
        this.target = target;
    }

    public override void ExecuteAction(Behaviour_Composite parent_composite)
    {
        agent.SetDestination(target.transform.position);
        base.ExecuteAction(parent_composite);
    }
    public override void Tick(float deltaTime)
    {
        if (agent.remainingDistance <= agent.stoppingDistance)
            FinishAction(true);
        base.Tick(deltaTime);
    }
}

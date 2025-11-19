using UnityEngine;
using UnityEngine.AI;

public class TeleportToRandom_Action : Behaviour_Node
{
    NavMeshAgent agent;
    Transform[] targets;

    public TeleportToRandom_Action(Behaviour_Condition[] behaviour_Conditions, NavMeshAgent agent, params Transform[] targets) : base(behaviour_Conditions)
    {
        this.agent = agent;
        this.targets = targets;
    }

    public override void ExecuteAction(Behaviour_Composite parent_composite)
    {
        base.ExecuteAction(parent_composite);
        agent.enabled = false;
        agent.transform.position = targets[Random.Range(0, targets.Length)].position;
        FinishAction(true);
    }

    public override void FinishAction(bool result)
    {
        agent.enabled = true;
        agent.SetDestination(agent.transform.position);
        base.FinishAction(result);
    }
}

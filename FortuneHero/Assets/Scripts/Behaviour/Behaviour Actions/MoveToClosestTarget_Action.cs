using UnityEngine;
using UnityEngine.AI;

public class MoveToClosestTarget_Action : Behaviour_Node
{
    NavMeshAgent agent;
    Transform[] targets;

    public MoveToClosestTarget_Action(Behaviour_Condition[] behaviour_Conditions, NavMeshAgent agent, params Transform[] targets) : base(behaviour_Conditions)
    {
        this.agent = agent;
        this.targets = targets;
    }

    public override void ExecuteAction(Behaviour_Composite parent_composite)
    {
        float currDistance = float.MaxValue;
        int currIndex = 0;
        for (int i = 0; i < targets.Length; i++)
        {
            float tempDist;
            if((tempDist = Vector3.Distance(agent.transform.position, targets[i].position)) < currDistance)
            {
                currIndex = i;
                currDistance = tempDist;
            }
        }
        agent.SetDestination(targets[currIndex].position);
        base.ExecuteAction(parent_composite);
    }
    public override void Tick(float deltaTime)
    {
        if (agent.remainingDistance <= agent.stoppingDistance)
            FinishAction(true);
        base.Tick(deltaTime);
    }
    public override void InteruptAction()
    {
        agent.destination = agent.transform.position;
        base.InteruptAction();
    }
}

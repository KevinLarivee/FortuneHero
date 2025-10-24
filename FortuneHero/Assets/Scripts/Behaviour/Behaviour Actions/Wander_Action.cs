using UnityEngine;
using UnityEngine.AI;

public class Wander_Action : Behaviour_Node
{
    NavMeshAgent agent;
    float roamingRange;

    public Wander_Action(Behaviour_Condition[] behaviour_Conditions, NavMeshAgent agent, float roamingRange) : base(behaviour_Conditions)
    {
        this.agent = agent;
        this.roamingRange = roamingRange;
    }

    public override void ExecuteAction(Behaviour_Composite parent_composite)
    {
        base.ExecuteAction(parent_composite);

        Vector3 direction;
        do
            direction = new Vector3(Random.Range(-roamingRange, roamingRange), 0, Random.Range(-roamingRange, roamingRange));
        while (Physics.CapsuleCast(agent.transform.position + new Vector3(0, agent.height / 2, 0), agent.transform.position - new Vector3(0, agent.height / 2, 0), 2, direction));

        agent.destination = agent.transform.position + direction;
    }

    public override void Tick(float deltaTime)
    {
        if(agent.remainingDistance < agent.stoppingDistance + 0.5f)
            FinishAction(true);
    }
    public override void InteruptAction()
    {
        agent.destination = agent.transform.position;
        base.InteruptAction();
    }
}

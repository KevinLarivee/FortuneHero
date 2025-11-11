using UnityEngine;
using UnityEngine.AI;

public class GoToTarget : Node
{
    Transform[] targets;
    NavMeshAgent agent;
    float stoppingDistance;
    int currentTarget = 0;

    public GoToTarget(Condition[] conditions, BehaviorTree BT, Animator animator, Transform [] targets, NavMeshAgent agent, float stoppingDistance) : base(conditions, BT, animator)
    {
        this.targets = targets;
        this.agent = agent;
        this.stoppingDistance = stoppingDistance;
    }

    public override void EvaluateAction()
    {
        agent.SetDestination(targets[currentTarget].position);
        animator.SetBool("isPatrolling", true);
        base.EvaluateAction();
    }

    public override void Tick(float deltaTime)
    {
        if((agent.transform.position - targets[currentTarget].position).magnitude < stoppingDistance)
        {
            currentTarget++;
            FinishAction(true);
        }
        else
        {
            if(!agent.SetDestination(targets[currentTarget].position)) //Set une destination peut echouer (objet disparait, etc.)
                FinishAction(false);
        }
    }
    public override void FinishAction(bool result)
    {
        agent.SetDestination(agent.transform.position);
        animator.SetBool("isPatrolling", false);
        if(currentTarget == targets.Length)
            currentTarget = 0;
        base.FinishAction(result);
    }

    public override void Interrupt()
    {
        agent.SetDestination(agent.transform.position);
        animator.SetBool("isPatrolling", false);
        base.Interrupt();
    }
}

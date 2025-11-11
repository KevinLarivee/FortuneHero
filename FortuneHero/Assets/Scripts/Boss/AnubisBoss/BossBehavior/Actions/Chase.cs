using UnityEngine;
using UnityEngine.AI;

public class Chase : Node
{
    NavMeshAgent agent;
    Transform target;
    float meleeStopDistance;
    float detectionRange;
    public Chase(Condition[] conditions, BehaviorTree BT, Animator animator, Transform target, NavMeshAgent agent, float meleeStopDistance, float detectionRange)
        : base(conditions, BT, animator)
    {
        this.target = target;
        this.agent = agent;
        this.meleeStopDistance = meleeStopDistance;
        this.detectionRange = detectionRange;
     }
    public override void EvaluateAction()
    {
        animator.SetBool("isRunning", true);
        agent.SetDestination(target.position);
        base.EvaluateAction();
    }
    public override void Tick(float deltaTime)
    {
        float distanceFromTarget = Vector3.Distance(agent.transform.position, target.position);
        if (distanceFromTarget < meleeStopDistance) //Si yer assez proche
            FinishAction(true);
        else
        {
            if (distanceFromTarget > detectionRange || !agent.SetDestination(target.position)) //Si trouve pas destination
                FinishAction(false);
        }

        //Continue le tick
    }

    public override void FinishAction(bool result)
    {
        agent.SetDestination(agent.transform.position);
        animator.SetBool("isRunning", false);

        base.FinishAction(result);
    }
    public override void Interrupt()
    {
        base.Interrupt();
    }
}

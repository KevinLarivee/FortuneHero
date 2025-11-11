using UnityEngine;
using UnityEngine.AI;

public class GainDistance : Node
{
    NavMeshAgent agent;
    float distance = 10f;
    Vector3 targetPos;
    Vector3 startPos;
    bool first = false;
    public GainDistance(Condition[] conditions, BehaviorTree BT, Animator animator, NavMeshAgent agent) : base(conditions, BT, animator)
    {
        this.agent = agent;
    }
    public override void EvaluateAction()
    {
        startPos = agent.transform.position;
        targetPos = startPos - agent.transform.forward * distance;
        agent.isStopped = true;
        animator.SetFloat("distanceSpeed", 0.3f);
        animator.SetTrigger("GainDistance");
        base.EvaluateAction();
    }
    public override void Tick(float deltaTime)
    {
        var animState = animator.GetCurrentAnimatorStateInfo(0);
       
        if (!animState.IsName("GainDistance"))
            return;
        var newPos = Vector3.Lerp(startPos, targetPos, animState.normalizedTime - 0.2f);
        newPos.y = Mathf.Sin(Mathf.PI * animState.normalizedTime) + startPos.y;
        agent.transform.position = newPos;


        if (animState.normalizedTime >= 0.95f)
            FinishAction(false);
    }
    public override void FinishAction(bool result)
    {
        agent.SetDestination(agent.transform.position);
        agent.isStopped = false;
        animator.ResetTrigger("GainDistance");
        base.FinishAction(result);
    }
}

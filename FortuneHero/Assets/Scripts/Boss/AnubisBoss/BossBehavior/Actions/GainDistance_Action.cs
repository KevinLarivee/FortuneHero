using UnityEngine;
using UnityEngine.AI;

public class GainDistance_Action : Behaviour_Node
{
    Animator animator;
    NavMeshAgent agent;
    float distance;
    string animName;
    Vector3 targetPos;
    Vector3 startPos;
    //bool first = false;
    public GainDistance_Action(Behaviour_Condition[] conditions, Animator animator, NavMeshAgent agent, float distance, string animName = "GainDistance") : base(conditions)
    {
        this.animator = animator;
        this.agent = agent;
        this.distance = distance;
        this.animName = animName;
    }
    public override void ExecuteAction(Behaviour_Composite parent_composite)
    {
        base.ExecuteAction(parent_composite);
        startPos = agent.transform.position;
        targetPos = startPos - agent.transform.forward * distance;
        //agent.enabled = false; ?
        agent.isStopped = true;
        // Ajuster le distanceSpeed dans le animator directement?
        //animator.SetFloat("distanceSpeed", 0.3f);
        animator.SetTrigger(animName);
    }
    public override void Tick(float deltaTime)
    {
        var animState = animator.GetCurrentAnimatorStateInfo(0);
       
        if (!animState.IsName(animName))
            return;
        var newPos = Vector3.Lerp(startPos, targetPos, animState.normalizedTime - 0.2f);
        newPos.y = Mathf.Sin(Mathf.PI * animState.normalizedTime) + startPos.y;
        agent.transform.position = newPos;


        //Return toujours false, genre pour faire un combo avec une autre attaque? Pourquoi pas une séquence avec un selector?

        if (animState.normalizedTime >= 0.95f)
            FinishAction(false);
    }
    public override void FinishAction(bool result)
    {
        agent.SetDestination(agent.transform.position);
        //agent.enabled = true; ?
        agent.isStopped = false;
        //Pas besoin
        animator.ResetTrigger(animName);
        base.FinishAction(result);
    }
}

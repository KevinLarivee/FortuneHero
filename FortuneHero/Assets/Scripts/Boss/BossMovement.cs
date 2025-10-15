using UnityEngine;
using UnityEngine.AI;

public class BossMove_Action : Behaviour_Node
{
    Animator animator;
    NavMeshAgent agent;
    GameObject target;
    float moveSpeed;
    float stoppingDistance;


    public BossMove_Action(Behaviour_Condition[] behaviour_Conditions, Animator animator, NavMeshAgent agent, GameObject target, float stoppingDistance) : base(behaviour_Conditions)
    {
        this.animator = animator;
        this.agent = agent;
        this.target = target;
        this.stoppingDistance = stoppingDistance;
    }

    public override void ExecuteAction(Behaviour_Composite parent_composite)
    {
        animator.SetBool("isRunning", true);
        agent.isStopped = false;
        agent.destination = target.transform.position;
        base.ExecuteAction(parent_composite);
    }

    public override void Tick(float deltaTime)
    {
        base.Tick(deltaTime);

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Run"))
        {
            animator.SetBool("isRunning", false);
            agent.isStopped = true; //???
            FinishAction();
        }
    }
}
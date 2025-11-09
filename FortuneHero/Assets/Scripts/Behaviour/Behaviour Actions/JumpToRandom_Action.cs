using UnityEngine;
using UnityEngine.AI;

public class JumpToRandom_Action : Behaviour_Node
{
    Animator animator;
    NavMeshAgent agent;
    Transform[] targets;
    float height;
    float donePercentile;
    Vector3 startPos;
    Vector3 targetPos;
    bool gate = false;

    float startOffset = 0f;

    public JumpToRandom_Action(Behaviour_Condition[] behaviour_Conditions, Animator animator, NavMeshAgent agent, float height, params Transform[] targets) : this(behaviour_Conditions, animator, agent, height, 1f, targets)
    { }
    public JumpToRandom_Action(Behaviour_Condition[] behaviour_Conditions,Animator animator, NavMeshAgent agent, float height, float donePercentile, params Transform[] targets) : base(behaviour_Conditions)
    {
        this.animator = animator;
        this.agent = agent;
        this.height = height;
        this.donePercentile = donePercentile;
        this.targets = targets;
    }

    public override void ExecuteAction(Behaviour_Composite parent_composite)
    {
        base.ExecuteAction(parent_composite);
        targetPos = targets[Random.Range(0, targets.Length)].position;
        //animator.SetTrigger("JumpAttack");
        startOffset = 0f;
    }
    public override void Tick(float deltaTime)
    {
        AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0); // 0 = layer index (par défaut)
        if (!gate)
        {
            if (Mathf.Abs(agent.transform.rotation.eulerAngles.y - Quaternion.LookRotation(targetPos - agent.transform.position, Vector3.up).eulerAngles.y) > 5f)
            {
                agent.transform.rotation = Quaternion.RotateTowards(agent.transform.rotation,
                Quaternion.LookRotation(targetPos - agent.transform.position, Vector3.up),
                agent.angularSpeed * deltaTime);
                return;
            }
            else
                animator.SetTrigger("JumpAttack");
            if (stateInfo.IsName("JumpAttack"))
            {
                gate = true;
                agent.transform.LookAt(targetPos);
                agent.enabled = false;
                startPos = agent.transform.position;
                targetPos.y = startPos.y;
                startOffset = stateInfo.normalizedTime;
                animator.ResetTrigger("JumpAttack");
            }
            else
                return;
        }
        if (stateInfo.IsName("JumpAttack"))
        {
            float tlerp = stateInfo.normalizedTime / donePercentile;
            if(tlerp > 1f)
                tlerp = 1f;
            Vector3 newPos = Vector3.Lerp(startPos, targetPos, tlerp);
            // Hauteur du saut
            newPos.y += height * Mathf.Sin(Mathf.PI * tlerp);
            agent.gameObject.transform.position = newPos;
        }

        // Une fois le saut terminé, s'assure que self est au bon endroit
        else
        {
            FinishAction(true);
        }
        base.Tick(deltaTime);
    }

    public override void FinishAction(bool result)
    {
        agent.transform.position = targetPos;
        //Resume normal navmesh behaviour
        //agent.isStopped = false;
        agent.enabled = true;
        gate = false;
        base.FinishAction(result);
    }
}

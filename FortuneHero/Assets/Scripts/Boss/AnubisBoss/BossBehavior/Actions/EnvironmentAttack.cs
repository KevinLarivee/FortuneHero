using UnityEngine;

public class EnvironmentAttack : Node
{
    public bool isActivated = false;
    string animName;
    BossTree newBT;
    public EnvironmentAttack(Condition[] conditions, BehaviorTree BT, Animator animator, string animName) : base(conditions, BT, animator)
    {
        this.animName = animName;
        newBT = BT as BossTree;
    }

    public override void EvaluateAction()
    {
        animator.SetTrigger(animName);
        base.EvaluateAction();
    }
    public override void Tick(float deltaTime)
    {
        var animState = animator.GetCurrentAnimatorStateInfo(0);
        if (!isActivated)
        {
            if (animState.IsName(animName) && animState.normalizedTime >= 0.9f)
            {
                newBT.StartCoroutine(newBT.StartEnvironmentAttack());
                FinishAction(true);
                isActivated = true;
            }
        }
        else
            FinishAction(false);

    }
    public override void FinishAction(bool result)
    {
        animator.ResetTrigger(animName);
        base.FinishAction(result);
    }

}

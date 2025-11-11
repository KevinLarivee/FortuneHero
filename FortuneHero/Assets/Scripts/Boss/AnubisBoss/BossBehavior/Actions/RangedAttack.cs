using UnityEngine;

public class RangedAttack : Node
{
    Transform self;
    Transform target;

    public RangedAttack(Condition[] conditions, BehaviorTree BT, Animator animator, Transform self, Transform target)
        : base(conditions, BT, animator)
    {
        this.self = self;
        this.target = target;
    }

    public override void EvaluateAction()
    {
        //animator.SetTrigger("ChargeThrust");
        animator.Play("ChargeThrust", 0, 0f);
        base.EvaluateAction();
    }
    public override void Tick(float deltaTime)
    {
        var rotation = Quaternion.LookRotation(target.position - self.position);
        rotation.x = 0f;
        self.rotation = rotation;

        var animState = animator.GetCurrentAnimatorStateInfo(0);

        if (animState.IsName("RangedThrust") && animState.normalizedTime >= 0.8f)
        {
            FinishAction(true);
        }
    }
    public override void FinishAction(bool result)
    {
        //animator.ResetTrigger("ChargeThrust");
        animator.Play("Idle", 0, 0f);
        base.FinishAction(result);
    }
}

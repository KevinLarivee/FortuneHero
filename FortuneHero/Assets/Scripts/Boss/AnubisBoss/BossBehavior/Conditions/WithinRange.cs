using UnityEngine;

public class WithinRange : Condition
{
    Transform target;
    Transform self;
    float stopDistance;
    public WithinRange(bool reverseCondition, Transform self, Transform target, float stopDistance)
    {
        this.reverseCondition = reverseCondition;
        this.self = self;
        this.target = target;
        this.stopDistance = stopDistance;
    }
    public override bool Evaluate()
    {
        if ((self.position - target.position).magnitude <= stopDistance)
            return CheckForReverseCondition(true);

        return CheckForReverseCondition(false);
    }
}

using UnityEngine;

public class HasVision_Condition : Behaviour_Condition
{
    //Comparer le HasVision de kevin

    GameObject self;
    GameObject target;
    float maxAngle;
    float maxDistance;

    public HasVision_Condition(bool reverseCondition, GameObject self, GameObject target, float maxAngle, float maxDistance)
    {
        this.reverseCondition = reverseCondition;
        this.self = self;
        this.target = target;
        this.maxAngle = maxAngle;
        this.maxDistance = maxDistance;
    }

    public override bool Evaluate()
    {
        Vector3 directionToTarget = (target.transform.position - self.transform.position).normalized;

        float angleToTarget = Vector3.Angle(self.transform.forward, directionToTarget);

        if (angleToTarget > maxAngle / 2)
            return CheckForReverseCondition(false);

        if (Physics.Raycast(self.transform.position, directionToTarget, out RaycastHit hit, maxDistance))
        {
            if (hit.collider.gameObject != target)
                return CheckForReverseCondition(false);
        }
        else
            return CheckForReverseCondition(false);

        return CheckForReverseCondition(true);
    }
}

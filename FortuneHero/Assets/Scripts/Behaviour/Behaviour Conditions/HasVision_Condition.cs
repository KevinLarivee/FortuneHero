using UnityEngine;

public class HasVision_Condition : Behaviour_Condition
{
    GameObject self;
    GameObject target;
    float maxAngle;
    float maxDistance;

    public HasVision_Condition(GameObject self, GameObject target, float maxAngle, float maxDistance)
    {
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
            return false;

        if (Physics.Raycast(self.transform.position, directionToTarget, out RaycastHit hit, maxDistance))
        {
            if (hit.collider.gameObject != target)
                return false;
        }
        else
            return false;

        return true;
    }
}

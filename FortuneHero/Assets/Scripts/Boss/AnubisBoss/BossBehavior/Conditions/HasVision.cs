using UnityEngine;

public class HasVision : Condition
{
    LayerMask layer;
    Transform self;
    GameObject target;
    float viewAngle;
    float maxRange;

    public HasVision(bool reverseCondition, Transform self, GameObject target, LayerMask layer, float viewAngle, float maxRange)
    {
        this.reverseCondition = reverseCondition;
        this.self = self;
        this.target = target;
        this.layer = layer;
        this.viewAngle = viewAngle;
        this.maxRange = maxRange;
    }
    public override bool Evaluate()
    {
        Vector3 directionToTarget = target.transform.position - self.position;
        var angleToTarget = Vector3.Angle(self.forward, directionToTarget);
        var distanceToTarget = Vector3.Distance(target.transform.position, self.position);
        
        if (angleToTarget > viewAngle || distanceToTarget > maxRange)
            return CheckForReverseCondition(false);

        if(Physics.Raycast(self.position, directionToTarget, out RaycastHit hit, maxRange, layer))
        {
            if(hit.collider.gameObject != target)
            {
                Debug.Log("Hit : "+ hit.collider.gameObject);
                return CheckForReverseCondition(false);
            }
        }
        
        return CheckForReverseCondition(true);
    }
}

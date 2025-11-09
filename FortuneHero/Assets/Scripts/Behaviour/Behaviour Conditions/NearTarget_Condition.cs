using UnityEngine;

public class NearTarget_Condition : Behaviour_Condition
{
    Transform self;
    float maxDistance;
    Transform[] targets;

    public NearTarget_Condition(Transform self, float maxDistance, params Transform[] targets)
    {
        this.self = self;
        this.maxDistance = maxDistance;
        this.targets = targets;
    }

    public override bool Evaluate()
    {
        foreach (Transform target in targets)
        {
            if (Vector3.Distance(self.position, target.position) <= maxDistance)
            {
                return true;
            }
        }
        return false;
    }
}

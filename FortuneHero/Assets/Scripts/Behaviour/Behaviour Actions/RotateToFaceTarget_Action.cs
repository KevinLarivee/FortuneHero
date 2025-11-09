using UnityEngine;
using UnityEngine.AI;

public class RotateToFaceTarget_Action : Behaviour_Node
{
    float rotationSpeed;
    Transform self;
    GameObject target;
    // Avec un gros temps, peut devenir "infini"
    float maxTime = 0f;
    float currentTime = 0f;
    public RotateToFaceTarget_Action(Behaviour_Condition[] behaviour_Conditions, GameObject target, float rotationSpeed, Transform self, float maxTime) : base(behaviour_Conditions)
    {
        this.target = target;
        this.rotationSpeed = rotationSpeed;
        this.self = self;
        this.maxTime = maxTime;
    }
    public override void ExecuteAction(Behaviour_Composite parent_composite)
    {
        base.ExecuteAction(parent_composite);
        currentTime = maxTime;
    }
    public override void Tick(float deltaTime)
    {
        base.Tick(deltaTime);
        self.rotation = Quaternion.RotateTowards(self.rotation,
            Quaternion.LookRotation(target.transform.position - self.position, Vector3.up),
            rotationSpeed * deltaTime);

        if (currentTime > 0f)
        {
            currentTime -= deltaTime;
            if (currentTime <= 0f)
            {
                FinishAction(false);
            }
            // Si a un timer, empêche de prendre fin avant la fin du timer
            return;
        }

        else if (Mathf.Abs(self.rotation.eulerAngles.y - Quaternion.LookRotation(target.transform.position - self.position, Vector3.up).eulerAngles.y) <= 5f)
            FinishAction(true);
    }
}

using UnityEngine;
using UnityEngine.AI;

public class RotateToFaceTarget_Action : Behaviour_Node
{
    float rotationSpeed;
    NavMeshAgent agent;
    GameObject target;
    public RotateToFaceTarget_Action(Behaviour_Condition[] behaviour_Conditions, GameObject target, float rotationSpeed, NavMeshAgent agent) : base(behaviour_Conditions)
    {
        this.target = target;
        this.rotationSpeed = rotationSpeed;
        this.agent = agent;
    }

    public override void Tick(float deltaTime)
    {
        base.Tick(deltaTime);

        agent.transform.rotation = Quaternion.RotateTowards(agent.transform.rotation,
            Quaternion.LookRotation(target.transform.position - agent.transform.position, Vector3.up),
            rotationSpeed * deltaTime);

        if(Mathf.Abs(agent.transform.rotation.eulerAngles.y - Quaternion.LookRotation(target.transform.position - agent.transform.position, Vector3.up).eulerAngles.y) <= 5f)
            FinishAction(true);
    }
}

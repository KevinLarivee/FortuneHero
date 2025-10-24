using UnityEngine;

public class AttackDemo_Action : Behaviour_Node
{
    GameObject AtkPrefab;
    GameObject owner;
    float distance;

    public AttackDemo_Action(Behaviour_Condition[] behaviour_Conditions, GameObject AtkPrefab, GameObject owner, float distance) : base(behaviour_Conditions)
    {
        this.owner = owner;
        this.AtkPrefab = AtkPrefab;
        this.distance = distance;
    }

    public override void ExecuteAction(Behaviour_Composite parent_composite)
    {
        base.ExecuteAction(parent_composite);
        if (AtkPrefab != null)
        {
            GameObject.Instantiate(AtkPrefab, owner.transform.position + owner.transform.forward * distance/* + new Vector3(0, 1, 0)*/, owner.transform.rotation);
            FinishAction(true);
        }
        else
            FinishAction(false);

    }
}

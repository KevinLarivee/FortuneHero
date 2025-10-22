using UnityEngine;

public class Behaviour_Node
{
    protected Behaviour_Condition[] behaviour_Conditions;
    protected Behaviour_Composite parent_Composite;
    //Interupt

    public Behaviour_Node(Behaviour_Condition[] behaviour_Conditions)
    {
        this.behaviour_Conditions = behaviour_Conditions;
    }

    public bool EvaluateConditions()
    {
        if (behaviour_Conditions == null)
            return true;

        foreach (Behaviour_Condition condition in behaviour_Conditions)
            if(!condition.Evaluate())
                return false;
        return true;
    }

}

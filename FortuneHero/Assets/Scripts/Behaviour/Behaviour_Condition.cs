using Unity.VisualScripting;
using UnityEngine;

public abstract class Behaviour_Condition
{
    protected bool reverseCondition;
    public abstract bool Evaluate();

    public bool CheckForReverseCondition(bool result)
    {
        if (reverseCondition)
            result = !result;
        return result;
    }
}
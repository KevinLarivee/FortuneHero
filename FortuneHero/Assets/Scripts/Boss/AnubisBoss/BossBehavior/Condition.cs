using UnityEngine;

public abstract class Condition
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

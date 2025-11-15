using UnityEngine;

public class Random_Condition : Behaviour_Condition
{
    public float probability;

    public Random_Condition(bool reverseCondition, float probability)
    {
        this.reverseCondition = reverseCondition;
        this.probability = probability;
    }

    public override bool Evaluate()
    {
        return CheckForReverseCondition(Random.value <= probability);
    }
}

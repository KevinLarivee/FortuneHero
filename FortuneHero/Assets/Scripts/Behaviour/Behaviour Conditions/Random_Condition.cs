using UnityEngine;

public class Random_Condition : Behaviour_Condition
{
    public float probability;

    public Random_Condition(float probability)
    {
        this.probability = probability;
    }

    public override bool Evaluate()
    {
        return Random.value <= probability;
    }
}

using UnityEngine;

public class RandomOdds : Condition
{
    float oddsInDecimal;
    int max = 100;
    public RandomOdds(bool reverseCondition, float oddsInDecimal)
    {
        this.reverseCondition = reverseCondition;
        this.oddsInDecimal = oddsInDecimal;
    }
    public override bool Evaluate()
    {
        int rand = Random.Range(0, max + 1);
        if(rand <= max * oddsInDecimal) //ex.: rand = 30, oddsInPercentage = 0.7 (70%) --> ?30 <= 100/0.7 (70) --> true
        {
            return CheckForReverseCondition(true);
        }
        return CheckForReverseCondition(false);

    }
}

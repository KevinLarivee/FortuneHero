using System.Threading.Tasks;
using UnityEngine;

public class Behaviour_Interupt
{
    BehaviourTree bt;
    Behaviour_Condition[] conditions;
    bool[] conditionStates;

    public Behaviour_Interupt(BehaviourTree bt, Behaviour_Condition[] conditions)
    {
        this.bt = bt;
        this.conditions = conditions;
        conditionStates = new bool[conditions.Length];

        UpdateState();
        CheckForInterupt();
    }

    void UpdateState()
    {
        for(int i = 0; i < conditions.Length; i++)
        {
            conditionStates[i] = conditions[i].Evaluate();
        }
    }

    async void CheckForInterupt()
    {
        for (int i = 0; i < conditions.Length; i++)
        {
            if(conditionStates[i] != conditions[i].Evaluate())
            {
                bt.Interupt();
                UpdateState();
            }
        }

        await Task.Delay(100);

        CheckForInterupt();
    }
}

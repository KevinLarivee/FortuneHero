using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public class Interrupt
{
    Condition[] conditions;
    BehaviorTree BT;
    bool[] conditionsState;

    CancellationTokenSource cts;
    public Interrupt(Condition[] conditions, BehaviorTree BT)
    {
        this.conditions = conditions;
        this.BT = BT;
        conditionsState = new bool[conditions.Length];
        
        Start();
    }

    private async void CheckConditions(CancellationToken token)
    {
        while (!token.IsCancellationRequested) //While a la place de rappeler la fct après le await (sinon potentiel stackOverflow)
        {
            for (int i = 0; i < conditions.Length; ++i)
            {
                if (conditions[i].Evaluate() != conditionsState[i])
                {
                    BT.Interrupt();
                    UpdateState();
                    break;
                }
            }
            await Task.Delay(100);
        }
    }
    private void UpdateState()
    {
        for(int i = 0; i < conditions.Length; ++i)
        {
            conditionsState[i] = conditions[i].Evaluate();
        }
    }

    public void Start()
    {
        cts = new CancellationTokenSource();
        UpdateState();
        CheckConditions(cts.Token);
    }
    public void Stop()
    {
        cts.Cancel();
    }
}

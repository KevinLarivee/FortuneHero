using Unity.VisualScripting;
using UnityEngine;

public class Wait : Node
{
    float secondsToWait;
    float timer;

    public Wait(Condition[] conditions, BehaviorTree BT, Animator animator, float secondsToWait) : base(conditions, BT, animator)
    {
        this.secondsToWait = secondsToWait;
    }

    public override void EvaluateAction()
    {
        timer = 0;
        base.EvaluateAction();
    }
    public override void Tick(float deltaTime)
    {
        timer += deltaTime;
        if(timer > secondsToWait)
        {
            FinishAction(true);
        }
    }
}

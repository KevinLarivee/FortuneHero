using UnityEngine;

public class Wait_Action : Behaviour_Node
{
    float secondsToWait;
    float timer;
    public Wait_Action(Behaviour_Condition[] behaviour_Conditions, float secondsToWait) : base(behaviour_Conditions)
    {
        this.secondsToWait = secondsToWait;
    }

    public override void ExecuteAction(Behaviour_Composite parent_composite)
    {
        timer = 0;
        base.ExecuteAction(parent_composite);
    }

    public override void Tick(float deltaTime)
    {
        base.Tick(deltaTime);

        timer += deltaTime;
        if (timer >= secondsToWait)
        {
            FinishAction(true);
        }
    }
}

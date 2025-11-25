using UnityEngine;

public class CoolDown_Condition : Behaviour_Condition
{
    float delay;
    float lastTime;

    public CoolDown_Condition(bool reverseCondition, float delay)
    {
        this.reverseCondition = reverseCondition;
        this.delay = delay;
        lastTime = Time.time;
    }

    public override bool Evaluate()
    {
        if(Time.time - lastTime >= delay)
        {
            lastTime = Time.time;
            return CheckForReverseCondition(true);
        }
        return CheckForReverseCondition(false);
    }
}

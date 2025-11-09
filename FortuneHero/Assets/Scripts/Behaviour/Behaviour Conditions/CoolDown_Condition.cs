using UnityEngine;

public class CoolDown_Condition : Behaviour_Condition
{
    float delay;
    float lastTime;

    public CoolDown_Condition(float delay)
    {
        this.delay = delay;
    }

    public override bool Evaluate()
    {
        if(Time.time - lastTime >= delay)
        {
            lastTime = Time.time;
            return true;
        }
        return false;
    }
}

using UnityEngine;

public class Cooldown : Condition
{
    float cooldown;
    float time;
    public Cooldown(bool reverseCondition, float cooldown)
    {
        this.reverseCondition = reverseCondition;
        this.cooldown = cooldown;
        time = Time.time;
    }
    public override bool Evaluate()
    {
        if(Time.time - time > cooldown)
        {
            time = Time.time;
            return CheckForReverseCondition(true);
        }

        return CheckForReverseCondition(false);
    }
}

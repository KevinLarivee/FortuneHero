using UnityEngine;

public class AboveSelf_Condition : Behaviour_Condition
{
    Transform self;
    Transform other;

    public AboveSelf_Condition(bool reverseCondition, Transform self, Transform other)
    {
        this.reverseCondition = reverseCondition;
        this.self = self;
        this.other = other;
    }

    public override bool Evaluate() =>
        CheckForReverseCondition(other.position.y > self.position.y + 2f);
}

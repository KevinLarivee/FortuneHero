using UnityEngine;

public class AboveSelf_Condition : Behaviour_Condition
{
    Transform self;
    Transform other;

    public AboveSelf_Condition(Transform self, Transform other)
    {
        this.self = self;
        this.other = other;
    }

    public override bool Evaluate() =>
        other.position.y > self.position.y;
}

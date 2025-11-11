using Unity.VisualScripting;
using UnityEngine;

public class Sequence : Node
{
    Node[] children;
    int index;
    int nodesToSucceed;
    public Sequence(Condition[] conditions, BehaviorTree BT, Animator animator, Node[] children, int nodesToSucceed = -1) : base(conditions, BT, animator)
    {
        this.children = children;
        foreach (Node child in children)
            child.SetParent(this);
        if (nodesToSucceed == -1)
            this.nodesToSucceed = children.Length;
        else
            this.nodesToSucceed = nodesToSucceed;
    }
    public override void EvaluateAction()
    {
        base.EvaluateAction();
        index = 0;
        children[index].EvaluateAction();
    }
    public override void FinishAction(bool result)
    {
        if (!result)
        {
            result = index >= nodesToSucceed;
            base.FinishAction(result);
        }
        else if (index == children.Length - 1)
            base.FinishAction(true);
        else
        {
            ++index;
            children[index].EvaluateAction();
        }
    }

    public override void Tick(float deltaTime)
    {
        base.Tick(deltaTime);
    }
}

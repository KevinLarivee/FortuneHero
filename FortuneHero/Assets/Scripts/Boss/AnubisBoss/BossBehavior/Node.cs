using UnityEngine;

public abstract class Node
{
    protected Node parent;
    protected BehaviorTree BT;
    protected Condition[] conditions;
    protected Animator animator;
    public Node(Condition[] conditions, BehaviorTree BT, Animator animator)
    {
        this.conditions = conditions;
        this.BT = BT;
        this.animator = animator;
    }

    public void SetParent(Node parent)
    {
        this.parent = parent;
    }
    public bool EvaluateConditions()
    {
        if (conditions == null)
            return true;

        foreach (Condition c in conditions)
        {
            if (!c.Evaluate())
                return false;
        }
        return true;
    }
    virtual public void EvaluateAction()
    {
        if (!EvaluateConditions())
            FinishAction(false);
        else
            BT.activeNode = this;
    }
    virtual public void Tick(float deltaTime) { }
    virtual public void FinishAction(bool result)
    {
        if (parent != null) //Si parent est pas null, donc pas root
            parent.FinishAction(result);
        else //Si root
        {
            BT.EvaluateTree();
        }
    }
    virtual public void Interrupt()
    {
        if (parent != null)
            parent.Interrupt();
    }
}

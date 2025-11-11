using System;
using UnityEngine;
using static Unity.VisualScripting.Metadata;

public class Selector : Node
{
    Node[] children;
    int index;
    public Selector(Condition[] conditions, BehaviorTree BT, Animator animator, Node[] children) : base(conditions, BT, animator)
    {
        this.children = children;
        foreach (Node child in children)
            child.SetParent(this);
    }

    public override void EvaluateAction()
    {
        base.EvaluateAction();
        index = 0;
        children[index].EvaluateAction();
    }

    public override void FinishAction(bool result)
    {
        if (result) //si enfant de selector reussi, il est lui aussi une réussite, donc monte (base.finishAction)
            base.FinishAction(false);
        else if (index == children.Length - 1) //si c'est le derniner enfant (et qu'il est un echec)
            base.FinishAction(false);
        else //sinon evalue le prochain
        {
            ++index;
            children[index].EvaluateAction();
        }
    }
}

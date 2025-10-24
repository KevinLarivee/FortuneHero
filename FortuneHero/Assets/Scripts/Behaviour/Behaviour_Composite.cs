using System;
using UnityEngine;

public class Behaviour_Composite : Behaviour_Node
{
    public enum CompositeType { Selector, Sequence }

    Behaviour_Node[] nodes;
    CompositeType compositeType;
    public BehaviourTree behaviourTree;

    int sequenceIndex = 0;
    int selectorIndex = 0;

    public Behaviour_Composite(Behaviour_Condition[] behaviour_Conditions, CompositeType compositeType, BehaviourTree behaviourTree, Behaviour_Node[] nodes) : base(behaviour_Conditions)
    {
        this.compositeType = compositeType;
        this.behaviourTree = behaviourTree;
        this.nodes = nodes;
    }

    public override void ExecuteAction(Behaviour_Composite parent_composite)
    {
        base.ExecuteAction(parent_composite);

        switch(compositeType)
        {
            case CompositeType.Selector:
                SelectorStart(0);
                break;
            case CompositeType.Sequence:
                SequenceContinue(0);
                break;
        }
    }
    public override void FinishAction(bool result)
    {
        if(compositeType == CompositeType.Sequence && sequenceIndex < nodes.Length - 1)
        {
            if (!result)
            {
                EndComposite(false);
                return;
            }
            SequenceContinue(sequenceIndex + 1);
            return;
        }
        else if (compositeType == CompositeType.Selector && !result)
        {
            if(selectorIndex < nodes.Length - 1)
            {
                SelectorStart(selectorIndex + 1);
                return;
            }
            EndComposite(false);
            return;
        }



        EndComposite(true);
    }

    void EndComposite(bool result)
    {
        if (parent_Composite != null)
            parent_Composite.FinishAction(result);

        else
            behaviourTree.RunTree();
    }

    void SequenceContinue(int index)
    {
        sequenceIndex = index;
        if (!nodes[sequenceIndex].EvaluateConditions())
            FinishAction(false);
        else
            nodes[sequenceIndex].ExecuteAction(this);

    }

    void SelectorStart(int index)
    {
        for(int i = selectorIndex; i < nodes.Length; i++)
        {
            if (!nodes[i].EvaluateConditions())
                continue;
            else
            {
                selectorIndex = index;
                nodes[i].ExecuteAction(this);
                return;
            }
        }
        FinishAction(false);
    }
}

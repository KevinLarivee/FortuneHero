using System;
using UnityEngine;

public class Behaviour_Composite : Behaviour_Node
{
    public enum CompositeType { Selector, Sequence }

    Behaviour_Node[] nodes;
    CompositeType compositeType;
    public BehaviourTree behaviourTree;

    int sequenceIndex = 0;

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
                SelectorStart();
                break;
            case CompositeType.Sequence:
                SequenceContinue(0);
                break;
        }
    }
    public override void FinishAction()
    {
        if(compositeType == CompositeType.Sequence && sequenceIndex < nodes.Length - 1)
        {
            SequenceContinue(sequenceIndex + 1);
            return;
        }
        
        if(parent_Composite != null)
            parent_Composite.FinishAction();

        else
            behaviourTree.RunTree();
    }
    void SequenceContinue(int index)
    {
        sequenceIndex = index;
        if (!nodes[sequenceIndex].EvaluateConditions())
            FinishAction();
        else
            nodes[sequenceIndex].ExecuteAction(this);

    }

    void SelectorStart()
    {
        foreach(Behaviour_Node node in nodes)
        {
            if (!node.EvaluateConditions())
                continue;
            else
            {
                node.ExecuteAction(this);
            }
        }
    }
}

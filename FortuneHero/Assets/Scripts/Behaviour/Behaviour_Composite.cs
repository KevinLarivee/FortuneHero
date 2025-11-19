using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Behaviour_Composite : Behaviour_Node
{
    //static int compositeID = 0;
    public string compositeInstanceID;
    public enum CompositeType { Selector, Sequence, Parallel }

    Behaviour_Node[] nodes;
    public CompositeType compositeType;
    public BehaviourTree behaviourTree;

    bool earlyStop;
    bool parallelFinished = false;
    int minSuccess = 0;

    int sequenceIndex = 0;
    int selectorIndex = 0;
    List<bool> results = new List<bool>();

    public Behaviour_Composite(Behaviour_Condition[] behaviour_Conditions, CompositeType compositeType, BehaviourTree behaviourTree, Behaviour_Node[] nodes, string compositeId = "", bool earlyStop = true, int minSuccess = 0) : base(behaviour_Conditions)
    {
        this.compositeType = compositeType;
        this.behaviourTree = behaviourTree;
        this.nodes = nodes;
        this.earlyStop = earlyStop;
        this.minSuccess = minSuccess;
        this.compositeInstanceID = compositeId;
    }

    public override void ExecuteAction(Behaviour_Composite parent_composite)
    {
        if (compositeType == CompositeType.Parallel)
            SetNodeAsActive();

        base.ExecuteAction(parent_composite);

        switch(compositeType)
        {
            case CompositeType.Selector:
                SelectorStart(0);
                break;
            case CompositeType.Sequence:
                results.Clear();
                SequenceContinue(0);
                break;
            case CompositeType.Parallel:
                ParallelStart();
                break;
        }
        SetNodeAsActive();
    }
    public override void FinishAction(bool result)
    {
        // Note :
        // - Si la dernière action d'une séquence échoue, elle retourne false?
        // - Si la dernière action d'un sélecteur, retourne toujours le result?

        if(compositeType == CompositeType.Sequence && sequenceIndex < nodes.Length - 1 && (result || !earlyStop))
        {
            results.Add(result);
            result = results.Contains(true);
            //if (!result)
            //{
            //    EndComposite(false);
            //}
            //else
            SequenceContinue(sequenceIndex + 1);
            // Après le execute de la prochaine action, pour que le plus jeune soit le activeNode
            SetNodeAsActive();
            return;
        }
        else if (compositeType == CompositeType.Selector && !result && selectorIndex < nodes.Length - 1)
        {
            //if(selectorIndex < nodes.Length - 1)
            //{
                SelectorStart(selectorIndex + 1);
            //}
            //else
            //    EndComposite(false);
            // Après le execute de la prochaine action, pour que le plus jeune soit le activeNode
            SetNodeAsActive();
            return;
        }
        else if(compositeType == CompositeType.Parallel && !parallelFinished && !earlyStop)
        {
            results.Add(result);
            //Return pour pas faire le EndComposite
            if (results.Count < nodes.Length)
                return;
            //Donc a fini même si pas early stop
            int success = 0;
            foreach(bool r in results)
            {
                if(r)
                    success++;
            }
            result = success >= minSuccess;

            //Parallel devrait toujours être le activeNode, à moins qu'un de ses ancêtres soit un parallel.
            //Dans ce cas, il ne devrait jamais devenir le activeNode.
        }
        //SetNodeAsActive();
        if (!parallelFinished)
            EndComposite(result);
    }

    void EndComposite(bool result)
    {
        if (compositeType == CompositeType.Parallel)
        {
            parallelFinished = true;
            foreach (Behaviour_Node node in nodes)
            {
                //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!! Était true...
                node.FinishAction(false);
            }
        }
        if(behaviourTree.activeNode == this)
            behaviourTree.activeNode = null;

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
        for(int i = index; i < nodes.Length; i++)
        {
            if (!nodes[i].EvaluateConditions())
                continue;
            else
            {
                selectorIndex = i;
                nodes[i].ExecuteAction(this);
                return;
            }
        }
        FinishAction(false);
    }
    void ParallelStart()
    {
        results.Clear();
        parallelFinished = false;
        for (int i = 0; i < nodes.Length; i++)
        {
            if (nodes[i].EvaluateConditions())
            {
                nodes[i].ExecuteAction(this);
            }
            else
            {
                FinishAction(false);
            }
        }
    }
    //Appelé seulement pour un parallel normalement
    override public void Tick(float deltaTime)
    {
        switch (compositeType)
        {
            case CompositeType.Selector:
                nodes[selectorIndex].Tick(deltaTime);
                break;
            case CompositeType.Sequence:
                nodes[sequenceIndex].Tick(deltaTime);
                break;
            case CompositeType.Parallel:
                //Exécute tous les nodes, même ceux qui ont finis...
                foreach (Behaviour_Node node in nodes)
                {
                    node.Tick(deltaTime);
                }
                break;
        }
    }
    void SetNodeAsActive()
    {
        //if (parent_Composite == null || parent_Composite.compositeType == Behaviour_Composite.CompositeType.Parallel || (this is Behaviour_Composite && (this as Behaviour_Composite).compositeType != Behaviour_Composite.CompositeType.Parallel))
        //    return;
        if (behaviourTree.activeNode == null)
        {
            behaviourTree.activeNode = this;
        }    
    }
}

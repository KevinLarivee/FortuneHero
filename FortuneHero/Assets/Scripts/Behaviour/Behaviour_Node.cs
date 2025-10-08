using UnityEngine;

public class Behaviour_Node
{
    protected Behaviour_Condition[] behaviour_Conditions;
    protected Behaviour_Composite parent_Composite;
    //Interupt

    public Behaviour_Node(Behaviour_Condition[] behaviour_Conditions)
    {
        this.behaviour_Conditions = behaviour_Conditions;
    }

    public bool EvaluateConditions()
    {
        if (behaviour_Conditions == null)
            return true;

        foreach (Behaviour_Condition condition in behaviour_Conditions)
            if(!condition.Evaluate())
                return false;
        return true;
    }

    virtual public void ExecuteAction(Behaviour_Composite parent_composite)
    {
        this.parent_Composite = parent_composite;
        SetNodeAsActive();


        //Faire une action
    }


    virtual public void FinishAction()
    {
        parent_Composite.FinishAction();
    }


    virtual public void Tick(float deltaTime)
    {

    }


    //Simplifier, peut nécessiter changement
    void SetNodeAsActive()
    {
        if (parent_Composite == null || this is Behaviour_Composite)
            return;

        parent_Composite.behaviourTree.activeNode = this;
    }
}

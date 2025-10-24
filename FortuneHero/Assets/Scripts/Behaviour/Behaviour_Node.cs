using UnityEngine;

public class Behaviour_Node
{
    protected Behaviour_Condition[] behaviour_Conditions;
    protected Behaviour_Composite parent_Composite;

    protected bool interupted = false;

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
        interupted = true;
        SetNodeAsActive();


        //Faire une action
    }


    virtual public void FinishAction(bool result)
    {
        if(!interupted)
            parent_Composite.FinishAction(result);
    }


    virtual public void Tick(float deltaTime)
    {

    }

    virtual public void InteruptAction()
    {
        interupted = true;
        FinishAction(false);
    }


    //Simplifier, peut nécessiter changement
    void SetNodeAsActive()
    {
        if (parent_Composite == null || this is Behaviour_Composite)
            return;

        parent_Composite.behaviourTree.activeNode = this;
    }
}

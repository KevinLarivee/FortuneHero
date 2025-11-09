using Unity.VisualScripting;
using UnityEngine;

abstract public class Behaviour_Condition
{
    abstract public bool Evaluate();

    // Permet d'utiliser une condition inverser
    public Behaviour_Condition Invert() => 
        new Behaviour_InvertedCondition(this);
}
// Représente une condition inversée
public class Behaviour_InvertedCondition : Behaviour_Condition
{
    Behaviour_Condition condition;

    public Behaviour_InvertedCondition(Behaviour_Condition condition)
    {
        this.condition = condition;
    }

    public override bool Evaluate() => 
        !condition.Evaluate();
}
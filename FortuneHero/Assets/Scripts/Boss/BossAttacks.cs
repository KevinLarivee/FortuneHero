using UnityEngine;

public class Attack_Action : Behaviour_Node
{
    Animator animator;
    string attackName;
    GameObject target;
    //GameObject attackPrefab;

    public Attack_Action(Behaviour_Condition[] behaviour_Conditions, Animator animator, string attackName/*, GameObject attackPrefab = null*/) : base(behaviour_Conditions)
    {
        this.animator = animator;
        this.attackName = attackName;
        //this.attackPrefab = attackPrefab;
    }

    public override void ExecuteAction(Behaviour_Composite parent_composite)
    {
        animator.SetTrigger(attackName);
        base.ExecuteAction(parent_composite);
    }

    public override void Tick(float deltaTime)
    {
        base.Tick(deltaTime);

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName(attackName))
        {
            FinishAction(true);
        }
    }
}
public class JumpAttack_Action : Behaviour_Node
{
    Animator animator;
    public JumpAttack_Action(Behaviour_Condition[] behaviour_Conditions, Animator animator) : base(behaviour_Conditions)
    {
        this.animator = animator;
    }

    public override void ExecuteAction(Behaviour_Composite parent_composite)
    {
        animator.SetTrigger("JumpAttack");
        base.ExecuteAction(parent_composite);
    }

    public override void Tick(float deltaTime)
    {
        base.Tick(deltaTime);

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("JumpAttack"))
        {
            //Faire spawn whatever particle effect (pour wave quand il atterit)
            FinishAction(true);
        }
    }
}

public class FireBreath_Action : Behaviour_Node
{
    Animator animator;
    public FireBreath_Action(Behaviour_Condition[] behaviour_Conditions, Animator animator) : base(behaviour_Conditions)
    {
        this.animator = animator;
    }

    public override void ExecuteAction(Behaviour_Composite parent_composite)
    {
        animator.SetTrigger("FireBreath");
        //Faire spawn whatever particle effect (pour le feu)
        base.ExecuteAction(parent_composite);
    }

    public override void Tick(float deltaTime)
    {
        base.Tick(deltaTime);

        if (!animator.GetCurrentAnimatorStateInfo(0).IsName("FireBreath"))
        {
            FinishAction(true);
        }
    }
}

using UnityEngine;

public class Attack_Action : Behaviour_Node
{
    Animator animator;
    string attackName;
    public Attack_Action(Behaviour_Condition[] behaviour_Conditions, Animator animator, string attackName) : base(behaviour_Conditions)
    {
        this.animator = animator;
        this.attackName = attackName;
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
            FinishAction();
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
            FinishAction();
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
            FinishAction();
        }
    }
}

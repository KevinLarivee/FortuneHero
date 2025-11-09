using UnityEngine;

public class Animation_Action : Behaviour_Node
{
    // À améliorer pour supporter tous les types de paramètres

    Animator animator;
    string parameter;
    string animationName;
    float donePercentile;
    AnimatorControllerParameterType type;

    bool gate = false;


    public Animation_Action(Behaviour_Condition[] behaviour_Conditions, Animator animator, string parameter, AnimatorControllerParameterType type = AnimatorControllerParameterType.Trigger) : this(behaviour_Conditions, animator, parameter, parameter, 1f, type)
    { }
    public Animation_Action(Behaviour_Condition[] behaviour_Conditions, Animator animator, string parameter, string animationName, float donePercentile = 1f, AnimatorControllerParameterType type = AnimatorControllerParameterType.Trigger) : base(behaviour_Conditions)
    {
        this.animator = animator;
        this.parameter = parameter;
        this.animationName = animationName;
        this.donePercentile = donePercentile;
        this.type = type;
    }
    public override void ExecuteAction(Behaviour_Composite parent_composite)
    {
        base.ExecuteAction(parent_composite);
        // Pour savoir si l'animation existe
        try
        {
            switch(type)
            {
                case AnimatorControllerParameterType.Trigger:
                    animator.SetTrigger(parameter);
                    break;
                case AnimatorControllerParameterType.Bool:
                    animator.SetBool(parameter, true);
                    break;

                // À améliorer
                case AnimatorControllerParameterType.Float:
                    animator.SetFloat(parameter, 1f);
                    break;
                case AnimatorControllerParameterType.Int:
                    animator.SetInteger(parameter, 1);
                    break;
            }
        }
        catch(System.Exception ex)
        {
            FinishAction(false);
        }

    }
    public override void Tick(float deltaTime)
    {
        AnimatorStateInfo temp = animator.GetCurrentAnimatorStateInfo(0);
        if (!gate)
        {
            if (temp.IsName(animationName))
            {
                gate = true;
            }
        }
        else if (!interupted && (!temp.IsName(animationName) || temp.normalizedTime >= donePercentile))
        {
            FinishAction(true);
        }
        base.Tick(deltaTime);
    }

    public override void FinishAction(bool result)
    {
        //switch (type)
        //{
        //    case AnimatorControllerParameterType.Trigger:
        //        animator.ResetTrigger(parameter);
        //        break;
        //    case AnimatorControllerParameterType.Bool:
        //        animator.SetBool(parameter, false);
        //        break;

        //    // À améliorer
        //    case AnimatorControllerParameterType.Float:
        //        animator.SetFloat(parameter, 0f);
        //        break;
        //    case AnimatorControllerParameterType.Int:
        //        animator.SetInteger(parameter, 0);
        //        break;
        //}
        if (type == AnimatorControllerParameterType.Bool)
        {
            animator.SetBool(parameter, false);
        }
        gate = false;
        base.FinishAction(result);
    }
}



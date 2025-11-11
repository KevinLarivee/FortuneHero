using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class MeleeAttack : Node
{
    Transform self;
    Transform target;
    List<string> animNames; 
    int atkCount; //Les anims prennent du temps a changer de state,
                  //so le tick avait le temps de rentrer dans la boucle 25 fois avant que l'anim change de "Name"
    public MeleeAttack(Condition[] conditions, BehaviorTree BT, Animator animator, Transform self, Transform target, List<string> animNames) : base(conditions, BT, animator)
    {
        this.self = self;
        this.target = target;
        this.animNames = animNames;
    }
    public override void EvaluateAction()
    {
        atkCount = 0;
        animator.SetTrigger(animNames[atkCount]);
        base.EvaluateAction();
    }
    public override void Tick(float deltaTime) //Pour Projet, ajouter option de melee atks (soit faire differentes nodes, ou changer code)
    {
        var rotation = Quaternion.LookRotation(target.position - self.position);
        rotation.x = 0f; //Si le joueur saute, boff
        self.rotation = rotation;

        var animState = animator.GetCurrentAnimatorStateInfo(0);
        if (!animState.IsName(animNames[atkCount]))
            return;
        if (atkCount == 0 && animState.normalizedTime >= 0.5f) //normalizedTime --> 0-1... ex.: 0.5 == moitier de l'anim
        {
            if (animNames.Count > 1)
            {
                ++atkCount;
                int rand = Random.Range(0, 2);
                if (rand == 0)
                {
                    animator.SetTrigger(animNames[atkCount]);
                }
                else
                {
                    animator.SetTrigger(animNames[++atkCount]);
                }
            }
        }
        if (animState.IsName(animNames[atkCount]) && animState.normalizedTime >= 0.70f)
        {
            FinishAction(true);
        }
    }
    public override void FinishAction(bool result)
    {
        if(atkCount == 0)
            animator.SetTrigger(animNames[0]);
        base.FinishAction(result);
    }
}

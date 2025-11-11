using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class TpMeleeAttack : Node
{
    Transform self;
    Transform target;
    NavMeshAgent agent;

    List<int> availableTpPos;
    Vector3[] possibleTpPos = new Vector3[4];
    Vector3 playerPos;
    int atkCount;
    float offset;
    bool hasAttacked = false;
    bool firstAttack = false;

    public TpMeleeAttack(Condition[] conditions, BehaviorTree BT, Animator animator, Transform self, Transform target, NavMeshAgent agent, float offset) : base(conditions, BT, animator)
    {
        this.self = self;
        this.target = target;
        this.agent = agent;
        this.offset = offset;
    }

    public override void EvaluateAction()
    {
        availableTpPos = new() { 0, 1, 2, 3 };
        agent.isStopped = true;
        firstAttack = true;
        atkCount = 0;
        base.EvaluateAction();
    }
    public override void Tick(float deltaTime)
    {
        SetTpPositions();

        var animState = animator.GetCurrentAnimatorStateInfo(0);
        var animInfo = animator.GetNextAnimatorClipInfo(0);
        if (firstAttack)
        {
            firstAttack = false;
            TpAttack(); //Premiere attaque dans le tick, sinon, mm si condition est fausse, fait une attaque
        }
        if (atkCount < 4 && animState.IsName("Thrust"))
        {
            if (!hasAttacked && animState.normalizedTime >= 0.9f)
                TpAttack();
            else if(animState.normalizedTime < 0.9f) //des que ya attaquer, remet a false. Pourra pas rerentrer dans le if en haut pcq animation va pas etre terminer
                hasAttacked = false;
        }
        else if (atkCount >= 4)
        {
            FinishAction(true); //Peux importe si tu hit le joueur, quand le boss fini cette node, success
        }
    }
    public override void FinishAction(bool result)
    {
        //Activer tp effect
        agent.SetDestination(agent.transform.position);
        agent.isStopped = false;
        base.FinishAction(result);
    }

    private void SetTpPositions()
    {
        playerPos = target.position;
        possibleTpPos[0] = new Vector3(playerPos.x - offset, playerPos.y, playerPos.z);
        possibleTpPos[1] = new Vector3(playerPos.x + offset, playerPos.y, playerPos.z);
        possibleTpPos[2] = new Vector3(playerPos.x, playerPos.y, playerPos.z - offset);
        possibleTpPos[3] = new Vector3(playerPos.x, playerPos.y, playerPos.z + offset);
    }
    private void TpAttack()
    {
        int randIndex = Random.Range(0, availableTpPos.Count); //Ex.: availableTpPos { 0, 1, 2, 3 }, use index 2 --> availableTpPos { 0, 1, 3 }, donc ce qui est donner a possibleTpPos est tt le temps unique
        int randPos = availableTpPos[randIndex];

        availableTpPos.RemoveAt(randIndex);

        animator.SetTrigger("Thrust");
        hasAttacked = true;
        //Activate le Shader de tp ?        

        self.position = possibleTpPos[randPos];
        self.rotation = Quaternion.LookRotation(target.position - self.position);

        atkCount++;
    }
}


using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class TpMeleeAttack_Action : Behaviour_Node
{
    //En cours...
    //Si j'utilisse la même instance 4 fois, je peux carry de l'info
    Animator animator;
    NavMeshAgent agent;
    Transform target;
    float offset;
    string animName;

    List<Vector3> possibleTpPos;
    Vector3 playerPos;
    int atkCount;
    bool hasAttacked = false;
    bool firstAttack = false;

    public TpMeleeAttack_Action(Behaviour_Condition[] conditions, Animator animator, NavMeshAgent agent, Transform target, float offset, string animName = "Thrust") : base(conditions)
    {
        this.animator = animator;
        this.agent = agent;
        this.target = target;
        this.offset = offset;
        this.animName = animName;
    }

    public override void ExecuteAction(Behaviour_Composite parent_composite)
    {
        base.ExecuteAction(parent_composite);

        if (possibleTpPos == null || possibleTpPos.Count == 0)
        {
            SetTpPositions();
            firstAttack = true;
            atkCount = 0;
        }
        

        //agent.enabled = false; ?
        agent.isStopped = true;
    }
    public override void Tick(float deltaTime)
    {
        playerPos = target.position;

        var animState = animator.GetCurrentAnimatorStateInfo(0);
        if (firstAttack)
        {
            firstAttack = false;
            TpAttack(); //Premiere attaque dans le tick, sinon, mm si condition est fausse, fait une attaque
        }
        if (atkCount < 4 && animState.IsName(animName))
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
        //agent.enabled = true; ?
        agent.isStopped = false;
        base.FinishAction(result);
    }

    private void SetTpPositions()
    {
        possibleTpPos.Clear();
        possibleTpPos.Add(new Vector3(-offset, 0f, 0f));// [0]
        possibleTpPos.Add(new Vector3(offset, 0f, 0f));// [1]
        possibleTpPos.Add(new Vector3(0f, 0f, -offset));// [2]
        possibleTpPos.Add(new Vector3(0f, 0f, offset));// [3]
    }
    private void TpAttack()
    {
        int randIndex = Random.Range(0, possibleTpPos.Count);
        Vector3 posOffset = possibleTpPos[randIndex];

        possibleTpPos.RemoveAt(randIndex);

        animator.SetTrigger(animName);
        hasAttacked = true;
        //Activate le Shader de tp ?        

        agent.transform.position = playerPos + posOffset;
        agent.transform.rotation = Quaternion.LookRotation(target.position - agent.transform.position);

        atkCount++;
    }
}


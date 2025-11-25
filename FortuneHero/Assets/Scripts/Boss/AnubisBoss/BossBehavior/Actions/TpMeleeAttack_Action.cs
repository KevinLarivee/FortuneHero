using GLTFast.Schema;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class TpMeleeAttack_Action : Behaviour_Node
{
    //En cours...
    //Si j'utilisse la même instance 4 fois, je peux carry de l'info ===> ******* Pour l'instant, j'ai enlever les autres tp dans la sequence, tu les remettra si tu fais sa
    Animator animator;
    NavMeshAgent agent;
    Transform target;
    float offset;
    string animName;
    bool gate;

    static public List<Vector3> possibleTpPos;
    Vector3 playerPos;
    public TpMeleeAttack_Action(Behaviour_Condition[] conditions, Animator animator, NavMeshAgent agent, Transform target, float offset, string animName = "TpThrust") : base(conditions)
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
        }
        agent.isStopped = true;
        gate = false;
        TpAttack(); //Vu que votre version check la condition avant, on peut juste mettre le premier TpAttack dans le ExecuteAction
    }
    public override void Tick(float deltaTime)
    {

        var animState = animator.GetCurrentAnimatorStateInfo(0);
        if (!gate)
        {
            if (animState.IsName(animName) && animState.normalizedTime < 0.9f)
            {
                gate = true;
            }
        }
        else if(!interupted && (!animState.IsName(animName) || animState.normalizedTime >= 0.9f))
        {
            FinishAction(true);
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
        possibleTpPos = new List<Vector3>();
        possibleTpPos.Add(new Vector3(-offset, 0f, 0f)); // [0]
        possibleTpPos.Add(new Vector3(offset, 0f, 0f));  // [1]
        possibleTpPos.Add(new Vector3(0f, 0f, -offset)); // [2]
        possibleTpPos.Add(new Vector3(0f, 0f, offset));  // [3]
    }
    private void TpAttack()
    {
        playerPos = target.position;
        NavMeshQueryFilter filter = new NavMeshQueryFilter();
        filter.areaMask = 1 << NavMesh.GetAreaFromName("Walkable");
        filter.agentTypeID = agent.agentTypeID;
        int randIndex = -1;
        Vector3 posOffset;
        bool valid = false;
        do
        {
            randIndex = Random.Range(0, possibleTpPos.Count);
            posOffset = possibleTpPos[randIndex];
            possibleTpPos.RemoveAt(randIndex);
            valid = NavMesh.SamplePosition(playerPos + posOffset, out NavMeshHit hit, 2f, NavMesh.AllAreas);

        } while (possibleTpPos.Count > 0 && !valid);
        
        if(!valid)
        {
            SetTpPositions();
            FinishAction(false);
            return;
        }


        animator.SetTrigger(animName);
        //Activate le Shader de tp ?        

        agent.transform.position = playerPos + posOffset;
        agent.transform.rotation = Quaternion.LookRotation(target.position - agent.transform.position);
    }
}


using UnityEngine;
using UnityEngine.AI;

public class Testing_BT : BehaviourTree
{
    NavMeshAgent agent;
    GameObject target;
    [SerializeField] float waitTime = 2f;

    Wait_Action wait_Action;
    MoveToTarget_Action move_action;

    public override void InitializeTree()
    {
        agent = GetComponent<NavMeshAgent>();
        target = FindFirstObjectByType<PlayerComponent>().gameObject;

        // ---------------------------

        wait_Action = new Wait_Action(null, waitTime);
        move_action = new MoveToTarget_Action(null, agent, target);

        root = new Behaviour_Composite(null, Behaviour_Composite.CompositeType.Sequence, this, new Behaviour_Node[] { wait_Action, move_action});
    }
}

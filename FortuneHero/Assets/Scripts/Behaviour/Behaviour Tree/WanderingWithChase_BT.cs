using UnityEngine;
using UnityEngine.AI;

public class WanderingWithChase_BT : BehaviourTree
{
    [SerializeField] int waitTime = 3;
    [SerializeField] float wanderingRange = 10f;
    [SerializeField] float rotationSpeed = 100f;
    [SerializeField] float visionRange = 10f;
    [SerializeField] float visionAngle = 180f;
    [SerializeField] GameObject atkPrefab;

    NavMeshAgent agent;
    PlayerComponent player;


    // Tree Actions
    Chase_Action chase_Action;
    RotateToFaceTarget_Action rotateToFaceTarget_Action;
    AttackDemo_Action attackDemo_Action;
    Wait_Action attack_Wait_Action;

    Wander_Action wander_Action;
    Wait_Action wait_Action;

    // Composites
    Behaviour_Composite sequencer_1;
    Behaviour_Composite sequencer_2;
    Behaviour_Composite selector_1;

    // Conditions
    HasVision_Condition hasVision;

    public override void InitializeTree()
    {
        agent = GetComponent<NavMeshAgent>();
        player = PlayerComponent.Instance;

        // Conditions
        hasVision = new HasVision_Condition(gameObject, player.gameObject, visionAngle, visionRange);

        // Interupt
        new Behaviour_Interupt(this, new Behaviour_Condition[] { hasVision });

        // Action Set #1
        chase_Action = new Chase_Action(null, agent, player.gameObject);
        rotateToFaceTarget_Action = new RotateToFaceTarget_Action(null, player.gameObject, rotationSpeed, agent.transform, 0f);
        attackDemo_Action = new AttackDemo_Action(null, atkPrefab, agent.gameObject, 0.5f);
        attack_Wait_Action = new Wait_Action(null, 0.5f);

        // Composite (Action Set #1)
        sequencer_1 = new Behaviour_Composite(null, Behaviour_Composite.CompositeType.Sequence, this, new Behaviour_Node[] {chase_Action, rotateToFaceTarget_Action, attack_Wait_Action, attackDemo_Action, attack_Wait_Action});

        // Action Set #2

        // Composite (Action Set #2)
        selector_1 = new Behaviour_Composite(new Behaviour_Condition[] { hasVision }, Behaviour_Composite.CompositeType.Selector, this, new Behaviour_Node[] { sequencer_1, attack_Wait_Action });

        // Action Set #3
        wander_Action = new Wander_Action(null, agent, wanderingRange);
        wait_Action = new Wait_Action(null, waitTime);

        // Composite (Action Set #3)
        sequencer_2 = new Behaviour_Composite(null, Behaviour_Composite.CompositeType.Sequence, this, new Behaviour_Node[] { wander_Action, wait_Action });

        // Root Composite
        root = new Behaviour_Composite(null, Behaviour_Composite.CompositeType.Selector, this, new Behaviour_Node[] { selector_1, sequencer_2 });
    }
}

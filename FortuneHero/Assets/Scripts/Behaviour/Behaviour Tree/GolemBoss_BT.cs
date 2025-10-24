using UnityEngine;
using UnityEngine.AI;
/* Tree: (Inlcure mort, changement de phase?)
RootSequencer: WalkTowardsPlayerAction -> AttackSelector

            IsNearLava && NotPreviousLava  Random() > currentOdds  IsNearPlayer && Random() > currentOdds  
AttackSelector: LavaAttackSequencer          -> JumpAttackAction -> MeleeAttackSequencer                -> RangeAttackSequencer

LavaAttackSequencer: GoToLavaAction -> EatLavaAction -> *LookAtTargetAction -> FireBreathAction
MeleeAttackSequencer: PunchAction -> *LookAtTargetAction -> PunchAction -> *LookAtTargetAction -> PunchAction
RangeAttackSequencer: SwipeAction -> *LookAtTargetAction -> SwipeAction -> *LookAtTargetAction -> SwipeAction
*/





public class GolemBoss_BT : BehaviourTree
{
    [SerializeField] int waitTime = 3;
    [SerializeField] float rotationSpeed = 100f;
    [SerializeField] GameObject JumpPrefab;
    [SerializeField] GameObject RangePrefab;
    [SerializeField] GameObject FireBreathPrefab;

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
    Behaviour_Composite root_sequencer;
    Behaviour_Composite lavaAttack_sequencer;
    Behaviour_Composite meleeAttack_sequencer;
    Behaviour_Composite rangeAttack_sequencer;
    Behaviour_Composite attacks_selector;

    public override void InitializeTree()
    {
        agent = GetComponent<NavMeshAgent>();
        player = PlayerComponent.Instance;
    }
}

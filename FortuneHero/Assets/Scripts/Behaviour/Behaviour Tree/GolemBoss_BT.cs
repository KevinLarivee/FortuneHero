using NUnit.Framework;
using System.Collections.Generic;
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




[RequireComponent(typeof(LavaBossComponent))]
public class GolemBoss_BT : BehaviourTree
{
    [SerializeField] float chaseTime = 3f;
    [SerializeField] float rotationSpeed = 100f;
    [SerializeField] float CdLava = 20f;
    [SerializeField] float jumpHeight = 3f;
    //[SerializeField] GameObject JumpPrefab;
    //[SerializeField] GameObject RangePrefab;
    LavaBossComponent lavaBoss;
    //NavMeshAgent agent;
    PlayerComponent player;


    // Tree Actions
    Chase_Action chase_Action;
    Wait_Action wait_Action;

    MoveToClosestTarget_Action moveToClosestLava_Action;
    Animation_Action animation_EatLava_Action;
    //Animation_Action animation_Firebreath_Action;

    JumpToRandom_Action jumpAttack_Player_Action;
    JumpToRandom_Action jumpAttack_Lava_Action;

    RotateToFaceTarget_Action rotateToFaceTarget_Endlessly_Action;

    Animation_Action animation_Melee1_Action;
    Animation_Action animation_Melee2_Action;
    Animation_Action animation_Melee3_Action;

    Animation_Action animation_Range1_Action;
    //Animation_Action animation_Range2_Action;
    //Animation_Action animation_Range3_Action;


    // Composites
    Behaviour_Composite chase_parallel;
    Behaviour_Composite attacks_selector;
    Behaviour_Composite lava_sequencer;
    Behaviour_Composite lava_parallel;
    Behaviour_Composite melee_parallel;
    Behaviour_Composite meleeCombo_sequencer;
    Behaviour_Composite range_parallel;
    Behaviour_Composite rangeCombo_sequencer;


    // Conditions
    NearTarget_Condition isNear_Lava;
    NearTarget_Condition isFar_Lava;
    CoolDown_Condition coolDown_Lava;
    Random_Condition random_Movement;
    NearTarget_Condition isNear_Player;
    AboveSelf_Condition notAbove_Player;
    Random_Condition random_Melee;


    public override void InitializeTree()
    {
        //agent = GetComponent<NavMeshAgent>();
        player = PlayerComponent.Instance;
        lavaBoss = GetComponent<LavaBossComponent>();

        //lavaBoss.animator.SetBool("isRunning", true);

        // Conditions
        List<Transform> lavaTransforms = new List<Transform>();
        foreach(Transform t in lavaBoss.lavaFalls.transform)
        {
            lavaTransforms.Add(t);
        }
        isNear_Lava = new NearTarget_Condition(false, transform, lavaBoss.trackPlayer.farThreshold, lavaTransforms.ToArray());
        isFar_Lava = new NearTarget_Condition(true, transform, lavaBoss.trackPlayer.farThreshold, lavaTransforms.ToArray());
        coolDown_Lava = new CoolDown_Condition(false, CdLava);
        random_Movement = new Random_Condition(false, lavaBoss.movementProbability);
        isNear_Player = new NearTarget_Condition(false, transform, lavaBoss.attackStopDistance, player.transform);
        notAbove_Player = new AboveSelf_Condition(true, transform, player.transform);
        random_Melee = new Random_Condition(false,lavaBoss.meleeProbability);

        // Action Set #1
        chase_Action = new Chase_Action(null, lavaBoss.agent, player.transform);
        wait_Action = new Wait_Action(null, chaseTime);

        // Composite (Action Set #1)
        chase_parallel = new Behaviour_Composite(null, Behaviour_Composite.CompositeType.Parallel, this, new Behaviour_Node[] { chase_Action, wait_Action}, "chase_parallel");

        // Action Set #2
        jumpAttack_Lava_Action = new JumpToRandom_Action(new Behaviour_Condition[] { isFar_Lava }, lavaBoss.animator, lavaBoss.agent, jumpHeight, 0.42f, lavaTransforms.ToArray());

        moveToClosestLava_Action = new MoveToClosestTarget_Action(new Behaviour_Condition[] { isNear_Lava }, lavaBoss.agent, lavaTransforms.ToArray());

        rotateToFaceTarget_Endlessly_Action = new RotateToFaceTarget_Action(null, player.gameObject, rotationSpeed, transform, float.MaxValue);
        animation_EatLava_Action = new Animation_Action(null, lavaBoss.animator, "EatLava", "FireBreath");

        lava_parallel = new Behaviour_Composite(null, Behaviour_Composite.CompositeType.Parallel, this, new Behaviour_Node[] { rotateToFaceTarget_Endlessly_Action, animation_EatLava_Action }, "lava_parallel");
        //Fais rien, mais devrait marcher
        //animation_Firebreath_Action = new Animation_Action(null, lavaBoss.animator, "FireBreath");

        // Composite (Action Set #2)
        lava_sequencer = new Behaviour_Composite(new Behaviour_Condition[] { coolDown_Lava }, Behaviour_Composite.CompositeType.Sequence, this, new Behaviour_Node[] { jumpAttack_Lava_Action, moveToClosestLava_Action, lava_parallel/*, animation_Firebreath_Action */}, "lava_sequencer", false);

        // Action Set #3
        animation_Melee1_Action = new Animation_Action(null, lavaBoss.animator, "Punch", "Attack_Punch", 0.5f);
        animation_Melee2_Action = new Animation_Action(new Behaviour_Condition[] { isNear_Player }, lavaBoss.animator, "Swipe", "Attack_Swipe", 0.5f);
        animation_Melee3_Action = new Animation_Action(new Behaviour_Condition[] { isNear_Player }, lavaBoss.animator, "Punch", "Attack_Punch");

        // Composite (Action Set #3)
        meleeCombo_sequencer = new Behaviour_Composite(null, Behaviour_Composite.CompositeType.Sequence, this, new Behaviour_Node[] { animation_Melee1_Action, animation_Melee2_Action, animation_Melee3_Action }, "meleeCombo_sequencer");

        // Action Set #4
        

        // Composite (Action Set #4)
        melee_parallel = new Behaviour_Composite(new Behaviour_Condition[] { isNear_Player, random_Melee }, Behaviour_Composite.CompositeType.Parallel, this, new Behaviour_Node[] { rotateToFaceTarget_Endlessly_Action, meleeCombo_sequencer }, "melee_parallel");

        // Action Set #5
        animation_Range1_Action = new Animation_Action(null, lavaBoss.animator, "startCombo", "Combo_DoubleSwing", 0.8f, AnimatorControllerParameterType.Bool);
        //Font rien mais devraient marcher
        //animation_Range2_Action = new Animation_Action(null, lavaBoss.animator, "startCombo", "Combo_Punch", 0.9f, AnimatorControllerParameterType.Bool);
        //animation_Range3_Action = new Animation_Action(null, lavaBoss.animator, "startCombo", "Combo_DoubleSwing", 1f, AnimatorControllerParameterType.Bool);

        // Composite (Action Set #5)
        rangeCombo_sequencer = new Behaviour_Composite(null, Behaviour_Composite.CompositeType.Sequence, this, new Behaviour_Node[] { animation_Range1_Action/*, animation_Range2_Action, animation_Range3_Action */}, "rangeCombo_sequencer");

        // Action Set #6

        // Composite (Action Set #6)
        range_parallel = new Behaviour_Composite(null, Behaviour_Composite.CompositeType.Parallel, this, new Behaviour_Node[] { rotateToFaceTarget_Endlessly_Action, rangeCombo_sequencer }, "range_parallel");

        // Action Set #7
        //lavaTransforms.Add(player.transform);
        jumpAttack_Player_Action = new JumpToRandom_Action(new Behaviour_Condition[] { notAbove_Player, random_Movement }, lavaBoss.animator, lavaBoss.agent, jumpHeight, 0.42f, player.transform);

        // Composite (Action Set #7)
        attacks_selector = new Behaviour_Composite(null, Behaviour_Composite.CompositeType.Selector, this, new Behaviour_Node[] { lava_sequencer, jumpAttack_Player_Action, melee_parallel, range_parallel}, "attacks_selector");

        // Root Composite
        root = new Behaviour_Composite(null, Behaviour_Composite.CompositeType.Sequence, this, new Behaviour_Node[] { chase_parallel, attacks_selector }, "root");
    }
    public void ChangeMovementProbability(float probability)
    {
        random_Movement.probability = probability;
    }
}

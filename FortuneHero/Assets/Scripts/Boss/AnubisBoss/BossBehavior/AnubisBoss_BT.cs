using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class AnubisBoss_BT : BehaviourTree
{
    Transform target;
    //[SerializeField] float meleeStopDistance = 3f;
    [SerializeField] float tpStopDistance = 1f;
    [SerializeField] float gainDistance = 10f;
    [SerializeField] float rotationSpeed = 100f;
    [SerializeField] float rangedSpeed = 1f;


    [SerializeField] string envAnimName;
    [SerializeField] string[] meleesAnimName;
    [SerializeField] string rangedParameter;
    [SerializeField] string rangedAnimName;
    [SerializeField] string tpAnimName;
    [SerializeField] string dashAnimName;
    //[SerializeField] float envDuration = 10f;
    [SerializeField] float envCd = 60f;
    [SerializeField] float tpCd = 20f;
    [SerializeField] float dashCd = 10f;
    [SerializeField] GameObject tpPoints;

    [Header("Conditions")]
    [SerializeField] float nearPlayerDistance = 30f;

    //Odds pour faire chaque nodes (en decimal)
    [Header("Node odds (Decimal")]
    [SerializeField] float gainDistanceOdds = 0.3f;
    [SerializeField] float tpRandomOdds = 0.7f;
    [SerializeField] float tpAtkOdds = 0.2f;
    [SerializeField] float rangeOdds = 0.5f;


    //EnvironmentAttack envAtk;
    TpMeleeAttack_Action tpMeleeAtk_Action;
    Chase_Action chase_Action;
    GainDistance_Action gainDistance_Action;
    RotateToFaceTarget_Action rotateToFaceTarget_Endlessly_Action;
    MoveToTarget_Action moveToTarget_SlowlyEndlessly_Action;
    TeleportToRandom_Action tpToCenter_Action;
    TeleportToRandom_Action tpToRandom_Action;

    Animation_Action animation_Tp_Action;
    Animation_Action[] animation_Melees_Action;
    Animation_Action animation_Dash_Action;
    Animation_Action animation_EnvAtk_Action;
    Animation_Action animation_Ranged_Action;

    AnubisBossComponent anubis;
    PlayerComponent player;
    //Interrupt interrupt;

    //Composites
    Behaviour_Composite env_sequencer;
    Behaviour_Composite randomTp_sequencer;
    Behaviour_Composite dash_sequencer;
    Behaviour_Composite attack_sequencer;
    Behaviour_Composite melee_sequencer;
    Behaviour_Composite tpAttack_sequencer;
    Behaviour_Composite ranged_sequencer;
    Behaviour_Composite ranged_parallel;

    // Conditions
    NearTarget_Condition isNear_Player;
    NearTarget_Condition isChaseRange_Player;
    NearTarget_Condition isNotChaseRange_Player;
    Random_Condition random_GainDistance;
    Random_Condition random_TpRandom;
    Random_Condition random_TpAttack;
    Random_Condition random_Range;
    CoolDown_Condition coolDown_Environnement;
    CoolDown_Condition coolDown_TpRandom;
    CoolDown_Condition coolDown_Dash;
    AboveSelf_Condition notAbove_Player;

    public override void InitializeTree()
    {
        //Components****
        target = PlayerComponent.Instance.transform;
        player = PlayerComponent.Instance;
        anubis = GetComponent<AnubisBossComponent>();

        
        List<Transform> temp = new List<Transform>();
        foreach (Transform t in tpPoints.transform)
        {
            temp.Add(t);
        }
        Transform[] points = temp.ToArray();

        //Conditions****
        isNear_Player = new NearTarget_Condition(false, transform, nearPlayerDistance, target);
        isChaseRange_Player = new NearTarget_Condition(false, transform, 2.5f, target);
        isNotChaseRange_Player = new NearTarget_Condition(true, transform, 2.5f, target);
        random_GainDistance = new Random_Condition(false, gainDistanceOdds);
        random_TpRandom = new Random_Condition(false, tpRandomOdds);
        random_TpAttack = new Random_Condition(false, tpAtkOdds);
        random_Range = new Random_Condition(false, rangeOdds); //50% de chance d'utiliser l'attaque?
        coolDown_Environnement = new CoolDown_Condition(false, envCd);
        coolDown_TpRandom = new CoolDown_Condition(false, tpCd);
        coolDown_Dash = new CoolDown_Condition(false, dashCd);
        notAbove_Player = new AboveSelf_Condition(true, transform, player.transform);


        //Interrupt****
        //interrupt = new Interrupt(null, this);

        // Action Set #1
        animation_Tp_Action = new Animation_Action(null, anubis.animator, tpAnimName, tpAnimName, 0.8f);
        tpToCenter_Action = new TeleportToRandom_Action(null, anubis.agent, points[0]);
        animation_EnvAtk_Action = new Animation_Action(null, anubis.animator, envAnimName);

        // Composite (Action Set #1)
        env_sequencer = new Behaviour_Composite(new Behaviour_Condition[] { coolDown_Environnement }, Behaviour_Composite.CompositeType.Sequence, this, new Behaviour_Node[] { animation_Tp_Action, tpToCenter_Action, animation_EnvAtk_Action }, "env_sequencer");

        // Action Set #2
        tpToRandom_Action = new TeleportToRandom_Action(null, anubis.agent, points);

        // Composite (Action Set #2)
        randomTp_sequencer = new Behaviour_Composite(new Behaviour_Condition[] { coolDown_TpRandom, random_TpRandom }, Behaviour_Composite.CompositeType.Sequence, this, new Behaviour_Node[] { animation_Tp_Action, tpToRandom_Action }, "randomTp_sequencer");

        // Action Set #3
        animation_Dash_Action = new Animation_Action(new Behaviour_Condition[] { isNear_Player, isNotChaseRange_Player }, anubis.animator, dashAnimName);

        // Composite (Action Set #3)
        dash_sequencer = new Behaviour_Composite(new Behaviour_Condition[] { coolDown_Dash }, Behaviour_Composite.CompositeType.Sequence, this, new Behaviour_Node[] { animation_Dash_Action, animation_Dash_Action, animation_Dash_Action }, "dash_sequencer");

        // Action Set #4
        //Doit se déplacer plus vite que le joueur, sinon timer?
        chase_Action = new Chase_Action(null, anubis.agent, player.transform);
        //Normalement, pas besoin de condition
        animation_Melees_Action = new Animation_Action[meleesAnimName.Length];
        for(int i = 0; i < meleesAnimName.Length; i++)
        {
            animation_Melees_Action[i] = new Animation_Action(null, anubis.animator, meleesAnimName[i]);
        }
        gainDistance_Action = new GainDistance_Action(new Behaviour_Condition[] { random_GainDistance }, anubis.animator, anubis.agent, gainDistance);

        List<Behaviour_Node> attackNodes = new List<Behaviour_Node>();
        attackNodes.Add(chase_Action);
        attackNodes.AddRange(animation_Melees_Action);
        attackNodes.Add(gainDistance_Action);

        // Composite (Action Set #4)
        attack_sequencer = new Behaviour_Composite(new Behaviour_Condition[] { isChaseRange_Player }, Behaviour_Composite.CompositeType.Sequence, this, attackNodes.ToArray(), "attack_sequencer");

        // Action Set #5
        
        // Composite (Action Set #5)
        melee_sequencer = new Behaviour_Composite(new Behaviour_Condition[] { notAbove_Player, isNear_Player }, Behaviour_Composite.CompositeType.Sequence, this, new Behaviour_Node[] { dash_sequencer, attack_sequencer }, "melee_sequencer");

        // Action Set #6
        tpMeleeAtk_Action = new TpMeleeAttack_Action(null, anubis.animator, anubis.agent, target, tpStopDistance);

        // Composite (Action Set #6)
        tpAttack_sequencer = new Behaviour_Composite(new Behaviour_Condition[] { notAbove_Player, random_TpAttack }, Behaviour_Composite.CompositeType.Sequence, this, new Behaviour_Node[] { animation_Tp_Action, tpMeleeAtk_Action, tpMeleeAtk_Action, tpMeleeAtk_Action, tpMeleeAtk_Action }, "tpAttack_sequencer");

        // Action Set #7
        animation_Ranged_Action = new Animation_Action(new Behaviour_Condition[] { isNotChaseRange_Player }, anubis.animator, rangedParameter, rangedAnimName);

        // Composite (Action Set #7)
        ranged_sequencer = new Behaviour_Composite(null, Behaviour_Composite.CompositeType.Sequence, this, new Behaviour_Node[] { animation_Ranged_Action, animation_Ranged_Action, animation_Ranged_Action, animation_Ranged_Action }, "ranged_sequencer");

        // Action Set #8
        rotateToFaceTarget_Endlessly_Action = new RotateToFaceTarget_Action(null, player.gameObject, rotationSpeed, transform, float.MaxValue);
        moveToTarget_SlowlyEndlessly_Action = new MoveToTarget_Action(null, player.gameObject, rangedSpeed, anubis.agent, float.MaxValue);

        // Composite (Action Set #8)
        ranged_parallel = new Behaviour_Composite(null, Behaviour_Composite.CompositeType.Parallel, this, new Behaviour_Node[] { rotateToFaceTarget_Endlessly_Action, moveToTarget_SlowlyEndlessly_Action, ranged_sequencer }, "ranged_parallel");

        // Root Composite
        root = new Behaviour_Composite(null, Behaviour_Composite.CompositeType.Selector, this, new Behaviour_Node[] { env_sequencer, randomTp_sequencer, melee_sequencer, tpAttack_sequencer, ranged_parallel }, "root");
    }

    //private void OnDisable()
    //{
    //    interrupt.Stop();
    //}
    //private void OnEnable()
    //{
    //    if (interrupt != null) //Initialize appeler après Enable
    //        interrupt.Start();
    //}
}

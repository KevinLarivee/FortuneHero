using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class BossTree : BehaviorTree
{
    Transform target;
    [SerializeField] float meleeStopDistance = 3f;
    [SerializeField] float tpStopDistance = 1f;

    
    [SerializeField] string envAnimName;
    [SerializeField] float envDuration = 10f;
    [SerializeField] float envCd = 20f;
    [SerializeField] ParticleSystem[] envPlatforms;
    [SerializeField] List<string> bossAnimNames;

    //Odds pour faire chaque nodes (en decimal)
    [Header("Node odds (Decimal")]
    [SerializeField] float gainDistanceOdds = 0.3f;
    [SerializeField] float rangedAtkOdds = 0.7f;
    [SerializeField] float tpMeleeOdds = 0.2f;


    EnvironmentAttack envAtk;
    //Interrupt interrupt;

    protected override void InitializeTree()
    {
        //Components****
        target = PlayerComponent.Instance.transform;
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        Animator animator = GetComponent<Animator>();

        //Interrupt****
        //interrupt = new Interrupt(null, this);

        //Nodes****
        Chase chase = new Chase(new Condition[] { new WithinRange(true, transform, target, meleeStopDistance) }, this, animator, target, agent, meleeStopDistance, 30);
        GainDistance gainDistance = new GainDistance(new Condition[] { new RandomOdds(false, gainDistanceOdds) }, this, animator, agent);

        MeleeAttack meleeAtk = new MeleeAttack(null, this, animator, transform, target, bossAnimNames);
        TpMeleeAttack tpMeleeAtk = new TpMeleeAttack(new Condition[] { new RandomOdds(false, tpMeleeOdds) }, this, animator, transform, target, agent, tpStopDistance);
        RangedAttack rangedAtk = new RangedAttack(new Condition[] { new RandomOdds(false, rangedAtkOdds), new WithinRange(true, transform, target, meleeStopDistance) }, this, animator, transform, target);
        envAtk = new EnvironmentAttack(new Condition[] { new Cooldown(false, envCd) }, this, animator, envAnimName);

        //Composites****

        //Attacks
        Sequence meleeAtkSequence = new Sequence(null, this, animator, new Node[] { meleeAtk, gainDistance });
        Sequence rangedSequence = new Sequence(null, this, animator, new Node[] { rangedAtk, rangedAtk, rangedAtk }, 1);
        Selector atkSelector = new Selector(null, this, animator, new Node[] { envAtk, meleeAtkSequence, tpMeleeAtk, rangedSequence });

        //Root****
        root = new Selector(null, this, animator, new Node[] { chase, atkSelector });

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

    public IEnumerator StartEnvironmentAttack()
    {
        Debug.Log("In Coroutine");
        PlayParticles();
        yield return new WaitForSeconds(envDuration);
        StopParticles();
        envAtk.isActivated = false;
    }

    private void PlayParticles()
    {
        foreach (var p in envPlatforms)
        {
            p.gameObject.SetActive(true);
            p.Play();
        }
    }
    private void StopParticles()
    {
        foreach (var p in envPlatforms)
        {
            p.gameObject.SetActive(false);
            p.Stop();
        }
    }
}

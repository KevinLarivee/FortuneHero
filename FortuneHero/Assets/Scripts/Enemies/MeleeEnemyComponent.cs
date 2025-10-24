using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

[RequireComponent(typeof(PatrolComponent))]
public class MeleeEnemyComponent : EnemyComponent
{
    [SerializeField] Collider rightHandCollider;
    [SerializeField] Collider leftHandCollider;
    NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        //Si besoin de remplacer ceux du parent (probablement)
        //detector.targetDetected = PlayerDetected;
        agent.speed = moveSpeed;
        patrol.move = Move;
    }
    
    protected override void PlayerDetected(Vector3 targetPosition)
    {
        base.PlayerDetected(targetPosition);
        agent.isStopped = enemyState == EnemyState.Attacking;
    }
    protected override void Move(Transform newTarget)
    {
        agent.destination = newTarget.position;
        if(agent.speed != moveSpeed / 2)
            agent.speed = moveSpeed / 2;
        
        base.Move(newTarget);
    }
    protected override void ChasingMove()
    {
       agent.destination = target;
        if (agent.speed != moveSpeed)
            agent.speed = moveSpeed;
        base.ChasingMove();
    }
    protected override void SlowEnemy(float divider)
    {
        base.SlowEnemy(divider);
        agent.speed = moveSpeed;
    }
    protected override void SpeedUpEnemy(float multiplier)
    {
        base.SpeedUpEnemy(multiplier);
        agent.speed = moveSpeed;
    }
    public override void ToggleParalyze(float aoeDuration)
    {
        base.ToggleParalyze(aoeDuration);
        agent.destination = transform.position;
    }
    public void EnableRightHandCollider() => rightHandCollider.enabled = true;

    public void DisableRightHandCollider() => rightHandCollider.enabled = false;

    public void EnableLeftHandCollider() => leftHandCollider.enabled = true;

    public void DisableLeftHandCollider() => leftHandCollider.enabled = false;
}

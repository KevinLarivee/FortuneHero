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
    int atkCount = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
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

    public void EnableRightHandCollider()
    {
        if (rightHandCollider == null) { Debug.LogError("EnableRightHandCollider: rightHandCollider is NULL"); return; }
        rightHandCollider.enabled = true;
        Debug.Log("EnableRightHandCollider called — enabled right collider");
    }

    public void DisableRightHandCollider()
    {
        if (rightHandCollider == null) { Debug.LogError("DisableRightHandCollider: rightHandCollider is NULL"); return; }
        rightHandCollider.enabled = false;
        Debug.Log("DisableRightHandCollider called — disabled right collider");
    }

    public void EnableLeftHandCollider()
    {
        if (leftHandCollider == null) { Debug.LogError("EnableLeftHandCollider: leftHandCollider is NULL"); return; }
        leftHandCollider.enabled = true;
        Debug.Log("EnableLeftHandCollider called — enabled left collider");
    }

    public void DisableLeftHandCollider()
    {
        if (leftHandCollider == null) { Debug.LogError("DisableLeftHandCollider: leftHandCollider is NULL"); return; }
        leftHandCollider.enabled = false;
        Debug.Log("DisableLeftHandCollider called — disabled left collider");
    }
}

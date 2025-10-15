using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(PatrolComponent))]
public class MeleeEnemyComponent : EnemyComponent
{
    NavMeshAgent agent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        //Si besoin de remplacer ceux du parent (probablement)
        //detector.targetDetected = PlayerDetected;
        patrol.move = Move;
    }

    void Update()
    {
        base.Update();
    }
    protected override void PlayerDetected(Vector3 targetPosition)
    {
        //timeUntilPatrolTimer = 0;
        //patrol.isActive = false;
        //enemyState = EnemyState.Chasing;
        //target = targetPosition;
        //agent.destination = target;
        //Vector3 posToTarget = target - transform.position;

        //if (posToTarget.sqrMagnitude <= attackStopDistance * attackStopDistance)
        //    enemyState = EnemyState.Attacking;
        base.PlayerDetected(targetPosition);
        agent.isStopped = enemyState == EnemyState.Attacking;

    }
    protected override void Move(Transform newTarget)
    {
        //if (agent.remainingDistance <= agent.stoppingDistance)
        //{
        //    newTarget = patrol.NextTarget();
        //}
        //target = newTarget.position;
        agent.destination = newTarget.position;
        //Vector3 posToTarget = target - transform.position;
        //transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
        //Quaternion targetRotation = Quaternion.LookRotation(posToTarget);
        //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        //animator.SetBool("isPatrolling", true);
        //animator.SetBool("isChasing", false);
        base.Move(newTarget);
    }
    protected override void ChasingMove()
    {
       agent.destination = target;
       base.ChasingMove();
    }
    //protected override IEnumerator Attack()
    //{
        

    //}

    //protected override void Hit()
    //{
    //    animator.SetTrigger("hit");
    //    base.Hit();
    //}
    //protected override void Death()
    //{
    //    //animator.SetTrigger("death");
    //    //agent.isStopped = true;
    //    //À la fin de l'anim de mort
    //    Destroy(gameObject);
    //    enemyDrops.SpawnDrops();
    //    base.Death();
    //}
}

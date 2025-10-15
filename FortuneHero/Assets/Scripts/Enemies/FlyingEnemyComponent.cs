using System.Collections;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(PatrolComponent))]
public class FlyingEnemyComponent : EnemyComponent
{
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
        patrol.move = Move;
        //detector.targetDetected = PlayerDetected;
        //healthComponent.OnHit = Hit;
        //healthComponent.OnDeath = Death;
    }

    //void Update()
    //{
    //    if (enemyState == EnemyState.Chasing)
    //        ChasingMove();
    //    else if (enemyState == EnemyState.Attacking)
    //        StartCoroutine(Attack());

    //    timeUntilPatrolTimer += Time.deltaTime; //start le timer 

    //    if (timeUntilPatrolTimer >= timeUntilPatrol) //si le timer atteint le max:
    //    {
    //        //Enable le patrol
    //        patrol.isActive = true;
    //        enemyState = EnemyState.Patrol;
    //    }
    //}

    //protected override void PlayerDetected(Vector3 targetPosition)
    //{
    //    timeUntilPatrolTimer = 0;
    //    patrol.isActive = false;
    //    enemyState = EnemyState.Chasing;
    //    target = targetPosition;
    //    Vector3 posToTarget = target - transform.parent.position;

    //    if (posToTarget.sqrMagnitude <= attackStopDistance * attackStopDistance)
    //        enemyState = EnemyState.Attacking;
    //    base.PlayerDetected(targetPosition);
    //}


    //à cause du parent... à retravailler anyways
    protected override void Move(Transform newTarget)
    {
        //Déjà dans base
        target = newTarget.position;
        Vector3 posToTarget = target - transform.position;

        //À override
        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
        Quaternion targetRotation = Quaternion.LookRotation(posToTarget);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        //Déjà dans base
        if (posToTarget.sqrMagnitude <= stoppingDistance * stoppingDistance)
            newTarget = patrol.NextTarget();
        animator.SetBool("isPatrolling", true);
        animator.SetBool("isChasing", false);
    }

    protected override void ChasingMove()
    {
        Vector3 posToTarget = target - transform.position;
        transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
        //Envoie un message dans la console si posToTarget est 0...
        Quaternion targetRotation = Quaternion.LookRotation(posToTarget);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);


        base.ChasingMove();
        //animator.SetBool("isChasing", true);
        //animator.SetBool("isPatrolling", false);
    }
    //protected override IEnumerator Attack()
    //{
    //    animator.SetBool("isChasing", false);
    //    animator.SetTrigger("Attack");

    //    yield return new WaitForSeconds(animationTime);
    //    enemyState = EnemyState.Chasing;

    //    yield return new WaitForSeconds(attackCd);

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

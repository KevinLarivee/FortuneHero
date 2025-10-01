using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeleeEnemyComponent : MonoBehaviour
{
    Animator animator;
    PatrolComponent patrol;
    DetectorComponent detector;
    NavMeshAgent agent;

    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float attackStopDistance = 3f;
    [SerializeField] float attackCd = 1f;
    [SerializeField] float animationTime = 0.7f;

    float timeUntilPatrol = 10f;
    float timeUntilPatrolTimer = 0f;
    float rotationSpeed = 5f;

    Vector3 target;
    enum EnemyState { Patrol, Attacking, Chasing }
    EnemyState enemyState;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        animator = GetComponentInParent<Animator>();
        patrol = GetComponent<PatrolComponent>();
        agent = GetComponent<NavMeshAgent>();
        detector = GetComponentInChildren<DetectorComponent>();
        detector.targetDetected = PlayerDetected;
        patrol.move = Move;
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyState == EnemyState.Chasing)
            ChasingMove();
        else if (enemyState == EnemyState.Attacking)
            StartCoroutine(Attack());

        timeUntilPatrolTimer += Time.deltaTime; //start le timer 

        if (timeUntilPatrolTimer >= timeUntilPatrol) //si le timer atteint le max:
        {
            //Enable le patrol
            patrol.isActive = true;
            enemyState = EnemyState.Patrol;
        }
    }
    public void PlayerDetected(Vector3 targetPosition)
    {
        timeUntilPatrolTimer = 0;
        patrol.isActive = false;
        enemyState = EnemyState.Chasing;
        target = targetPosition;
        agent.destination = target;
        Vector3 posToTarget = target - transform.position;

        if (posToTarget.sqrMagnitude <= attackStopDistance * attackStopDistance)
            enemyState = EnemyState.Attacking;
    }
    void Move(Transform newTarget)
    {
        if (agent.remainingDistance <= agent.stoppingDistance)
        {
            newTarget = patrol.NextTarget();
        }
        target = newTarget.position;
        agent.destination = newTarget.position;
        //Vector3 posToTarget = target - transform.position;
        //transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
        //Quaternion targetRotation = Quaternion.LookRotation(posToTarget);
        //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        animator.SetBool("isPatrolling", true);
        animator.SetBool("isChasing", false);
    }
    public void ChasingMove()
    {
        //Vector3 posToTarget = target - transform.position;
        //transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
        //Quaternion targetRotation = Quaternion.LookRotation(posToTarget);
        //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        animator.SetBool("isChasing", true);
        animator.SetBool("isPatrolling", false);
    }
    public IEnumerator Attack()
    {
        animator.SetBool("isChasing", false);
        animator.SetTrigger("Attack");

        yield return new WaitForSeconds(animationTime);
        enemyState = EnemyState.Chasing;

        yield return new WaitForSeconds(attackCd);

    }
}

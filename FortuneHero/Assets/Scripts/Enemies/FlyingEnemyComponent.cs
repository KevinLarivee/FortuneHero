using System.Collections;
using System.Net;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(PatrolComponent))]
public class FlyingEnemyComponent : MonoBehaviour
{
    Animator animator;
    PatrolComponent patrol;
    DetectorComponent detector;

    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float attackStopDistance = 3f;
    [SerializeField] float patrolStopDistance = 0.5f;
    [SerializeField] float attackCd = 1f;
    [SerializeField] float animationTime = 0.7f;

    float timeUntilPatrol = 10f;
    float timeUntilPatrolTimer = 0f;
    float rotationSpeed = 5f;

    [SerializeField] bool isDetecting = false; //Placeholder

    Vector3 target;
    enum EnemyState { Patrol, Attacking, Chasing }
    EnemyState enemyState;

    void Start()
    {
        animator = GetComponentInParent<Animator>();
        patrol = GetComponent<PatrolComponent>();
        detector = GetComponentInChildren<DetectorComponent>();
        detector.targetDetected = PlayerDetected;
        patrol.move = Move;
    }

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
        Vector3 posToTarget = target - transform.parent.position;

        if (posToTarget.sqrMagnitude <= attackStopDistance * attackStopDistance)
            enemyState = EnemyState.Attacking;
    }

    void Move(Transform newTarget)
    {
        target = newTarget.position;
        Vector3 posToTarget = target - transform.parent.position;
        transform.parent.position = Vector3.MoveTowards(transform.parent.position, target, moveSpeed * Time.deltaTime);
        Quaternion targetRotation = Quaternion.LookRotation(posToTarget);
        transform.parent.rotation = Quaternion.Slerp(transform.parent.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        if (posToTarget.sqrMagnitude <= patrolStopDistance * patrolStopDistance)
        {
            newTarget = patrol.NextTarget();
        }

        animator.SetBool("isPatrolling", true);
        animator.SetBool("isChasing", false);
    }

    public void ChasingMove()
    {
        Vector3 posToTarget = target - transform.parent.position;
        transform.parent.position = Vector3.MoveTowards(transform.parent.position, target, moveSpeed * Time.deltaTime);
        //Envoie un message dans la console si posToTarget est 0...
        Quaternion targetRotation = Quaternion.LookRotation(posToTarget);
        transform.parent.rotation = Quaternion.Slerp(transform.parent.rotation, targetRotation, rotationSpeed * Time.deltaTime);

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

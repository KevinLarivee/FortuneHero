using System.Net;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(PatrolComponent), typeof(DetectorComponent))]
public class FlyingEnemyComponent : MonoBehaviour
{
    Animator animator;
    PatrolComponent patrol;
    DetectorComponent detector;

    [SerializeField] GameObject player;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float attackStopDistance = 3f;
    [SerializeField] float patrolStopDistance = 0.5f;


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
        patrol.move = Move;
        detector = GetComponent<DetectorComponent>();
        detector.targetDetected = PlayerDetected;
    }

    void Update()
    {
        if (enemyState == EnemyState.Chasing /*&& voit pas l'ennemi*/)
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


        //    case EnemyState.Chasing:
        //    transform.parent.position = Vector3.MoveTowards(transform.parent.position, target, moveSpeed * Time.deltaTime);
        //    Quaternion targetRotation = Quaternion.LookRotation(posToTarget);
        //    transform.parent.rotation = Quaternion.Slerp(transform.parent.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        //    if (posToTarget.sqrMagnitude <= attackStopDistance * attackStopDistance)
        //        enemyState = EnemyState.Attacking;

        //    animator.SetBool("isPatrolling", false);
        //    animator.SetBool("isChasing", true);
        //    break;

        //case EnemyState.Attacking:
        //    Attack();
        //    animator.SetBool("isChasing", false);
        //    break;
        //}

    }

    void Move(Transform newTarget)
    {
        target = newTarget.position;
        Vector3 posToTarget = target - transform.parent.position;

        if (posToTarget.sqrMagnitude <= patrolStopDistance * patrolStopDistance)
        {
            newTarget = patrol.NextTarget();
        }

        animator.SetBool("isPatrolling", true);
        animator.SetBool("isChasing", false);
    }

    public void Attack()
    {
        animator.SetTrigger("Attack");
    }
}

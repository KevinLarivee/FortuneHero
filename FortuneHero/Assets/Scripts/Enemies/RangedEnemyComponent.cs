using System.Net;
using UnityEngine;
using UnityEngine.AI;

public class RangedEnemyComponent : MonoBehaviour
{
    Animator animator;
    PatrolComponent patrol;
    DetectorComponent detector;
    NavMeshAgent agent;

    [SerializeField] GameObject player;
    [SerializeField] float moveSpeed = 5f;
    [SerializeField] float attackStopDistance = 3f;
    [SerializeField] float attackCd = 1f;
    [SerializeField] float animationTime = 0.7f;
    [SerializeField] GameObject Projectile;
    [SerializeField] GameObject exitPoint;

    float timeUntilPatrol = 10f;
    float timeUntilPatrolTimer = 0f;
    float rotationSpeed = 5f;
    float timeUntilNextAttack = 1f;
    float nextAttackTimer = 0f;

    Vector3 target;
    enum EnemyState { Patrol, Attacking, Chasing }
    EnemyState enemyState;

    bool canAttack = false;
    [SerializeField] bool isDetecting = false; //Placeholder
    void Start()
    {
        animator = GetComponent<Animator>();
        patrol = GetComponent<PatrolComponent>();
        agent = GetComponent<NavMeshAgent>();
        detector = GetComponentInChildren<DetectorComponent>();
        detector.targetDetected = PlayerDetected;
        patrol.move = Move;

    }

    void Update()
    {
        if (isDetecting)
        {
            if (timeUntilPatrolTimer > 0) //si il detecte de nouveau apres avoir parti le timer = reset
                timeUntilPatrolTimer = 0;
        }
        else //Detecte pas le joueur
        {
            timeUntilPatrolTimer += Time.deltaTime; //start le timer 
            if (timeUntilPatrolTimer >= timeUntilPatrol) //si le timer atteint le max:
            {
                //Enable le patrol
                //animator.SetBool("isPatrolling", true);
            }
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

    public void Attack()
    {
        //animator.SetTrigger("Attack");
        Instantiate(Projectile, exitPoint.transform.position, gameObject.transform.rotation);

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
}

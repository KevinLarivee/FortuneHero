using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyDrops), typeof(HealthComponent))]
public class EnemyComponent : MonoBehaviour, IPoolable
{
    //!!! Quoi mettre dans EnnemyComponent vs SpecificEnnemyComponent
    protected enum EnemyState { Patrol, Attacking, Chasing }

    protected EnemyState enemyState;
    public ObjectPoolComponent Pool { get; set; }

    public int dmg = 1;
    public int collisionDmg = 10;

    [SerializeField] protected float moveSpeed = 5f;
    [SerializeField] protected float stoppingDistance = 0.5f;
    [SerializeField] protected float attackStopDistance = 3f;
    [SerializeField] protected float attackCd = 1f;
    [SerializeField] protected float animationTime = 0.7f;


    protected Animator animator;
    protected AnimationClip animClip;
    protected EnemyDrops enemyDrops;
    protected HealthComponent healthComponent;
    protected PatrolComponent patrol;
    protected DetectorComponent detector;

    protected float timeUntilPatrol = 10f;
    protected float timeUntilPatrolTimer = 0f;
    protected float rotationSpeed = 5f;

    protected Vector3 target;
    protected bool isAttacking = false;

    //NavMeshAgent agent;
    //bool _isDead = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        animator = GetComponent<Animator>();
        patrol = GetComponent<PatrolComponent>();
        detector = GetComponentInChildren<DetectorComponent>();
        enemyDrops = GetComponent<EnemyDrops>();
        healthComponent = GetComponent<HealthComponent>();

        healthComponent.onHit += Hit;
        healthComponent.onDeath += Death;

        detector.targetDetected = PlayerDetected;
        patrol.move = Move;
        //agent = GetComponent<NavMeshAgent>();
        //agent.updatePosition = false;
        //agent.updateRotation = true;
    }

    void Update()
    {
        if (enemyState == EnemyState.Chasing)
            ChasingMove();
        else if (enemyState == EnemyState.Attacking && !isAttacking){
            Debug.Log("should attack");
            StartCoroutine(Attack());
        }

        timeUntilPatrolTimer += Time.deltaTime; //start le timer 

        if (timeUntilPatrolTimer >= timeUntilPatrol) //si le timer atteint le max:
        {
            //Enable le patrol
            patrol.isActive = true;
            enemyState = EnemyState.Patrol;
        }
    }

    protected virtual void PlayerDetected(Vector3 targetPosition)
    {
        timeUntilPatrolTimer = 0;
        patrol.isActive = false;
        enemyState = EnemyState.Chasing;
        target = targetPosition;
        //agent.destination = target;
        Vector3 posToTarget = target - transform.position;

        if (posToTarget.sqrMagnitude <= attackStopDistance * attackStopDistance)
        {
            enemyState = EnemyState.Attacking;
        }
    }

    protected virtual void Move(Transform newTarget)
    {
        if (Vector3.Distance(target, transform.position) <= stoppingDistance)
            newTarget = patrol.NextTarget();

        target = newTarget.position;
        //agent.destination = newTarget.position;
        //Vector3 posToTarget = target - transform.position;
        //transform.position = Vector3.MoveTowards(transform.position, target, moveSpeed * Time.deltaTime);
        //Quaternion targetRotation = Quaternion.LookRotation(posToTarget);
        //transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        animator.SetBool("isPatrolling", true);
        animator.SetBool("isChasing", false);
    }

    protected virtual void ChasingMove()
    {
        animator.SetBool("isChasing", true);
        animator.SetBool("isPatrolling", false);
        if(Vector3.Distance(target, transform.position) <= stoppingDistance)
        {
            animator.SetBool("isChasing", false);
        }
    }

    protected virtual IEnumerator Attack()
    {
        isAttacking = true;
        animator.SetBool("isChasing", false);
        //animator.SetTrigger("Attack");
        animator.SetBool("isAttacking", isAttacking);

        //yield return new WaitForSeconds(0.1f);
        yield return new WaitForNextFrameUnit();
        yield return new WaitUntil(() => !animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"));
        enemyState = EnemyState.Chasing;
        isAttacking = false;
        animator.SetBool("isAttacking", isAttacking);

        yield return new WaitForSeconds(attackCd);
    }

    //private void OnEnable()
    //{
    //    agent.destination = patrol.NextTarget().position;
    //}

    protected virtual void Hit()
    {
        animator.SetTrigger("hit");
    }
    protected virtual void Death()
    {
        //animator.SetTrigger("death");
        //agent.isStopped = true;
        //À la fin de l'anim de mort
        enemyDrops.SpawnDrops();
        animator.SetTrigger("isDead");
        Destroy(gameObject);
    }
    //À revoir...
    //void Move(Transform destination)
    //{
    //    agent.destination = destination.position;
    //    Vector3 worldDeltaPosition = destination.position - transform.position;
    //    float deltaMagnitude = worldDeltaPosition.magnitude;
    //    float smooth = Mathf.Min(1, Time.deltaTime / 0.1f);
    //    if (deltaMagnitude > agent.radius / 2f)
    //    {
    //        transform.position = Vector3.Lerp(
    //            animator.rootPosition,
    //            agent.nextPosition,
    //            smooth
    //        );
    //    }
    //    if (Vector3.Distance(destination.position, transform.position) <= agent.stoppingDistance)
    //    {
    //        patrol.NextTarget();
    //    }
    //}
    //RootMotion?
    //void OnAnimatorMove()
    //{
    //    var rootPosition = animator.rootPosition;
    //    transform.position = rootPosition;
    //    agent.nextPosition = rootPosition;
    //}
    //Vector3 RandomDestination()
    //{
    //    NavMeshHit navHit;
    //    Vector3 spawnPosition;
    //    do
    //    {
    //        var goal = Random.insideUnitCircle * destinationRange;
    //        spawnPosition = new Vector3(goal.x, 0f, goal.y);
    //    } while (!NavMesh.SamplePosition(spawnPosition, out navHit, 10f, NavMesh.AllAreas));
    //    return navHit.position;
    //}
    //public void Hit()
    //{
    //    //if (!_isDead)
    //    //    StartCoroutine(DeathDelay());
    //    //_isDead = true;
    //}



    protected void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<HealthComponent>().Hit(collisionDmg); //Lier le dmg au dmg de l'enemy
        }
    }

}

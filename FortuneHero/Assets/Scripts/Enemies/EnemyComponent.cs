using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(EnemyDrops), typeof(HealthComponent), typeof(DamageCollision))]
public class EnemyComponent : MonoBehaviour, IPoolable
{
    protected enum EnemyState { Patrol, Attacking, Chasing }
    protected EnemyState enemyState;
    public ObjectPoolComponent Pool { get; set; }

    public int dmg = 10;
    //public int collisionDmg = 5;

    [SerializeField] GameObject paralyzePrefab;
    public bool isParalyzed = false;
    float paraTimer;

    [SerializeField] protected float moveSpeed = 5f;
    [SerializeField] protected float stoppingDistance = 0.5f;
    [SerializeField] protected float attackStopDistance = 3f;
    [SerializeField] protected float attackCd = 1f;
    [SerializeField] protected float animationTime = 0.7f;
    [SerializeField] protected bool isStatic = false;


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
    }

    protected void Update()
    {
        if (!isParalyzed)
        {
            if (enemyState == EnemyState.Chasing)
                ChasingMove();
            else if (enemyState == EnemyState.Attacking && !isAttacking)
            {
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
        else
        {
            paraTimer -= Time.deltaTime;
            paralyzePrefab.SetActive(true);
            //animation on
            if (paraTimer <= 0)
            {
                paralyzePrefab.SetActive(false);
                isParalyzed = false;
                //animation off
            }
        }
    }

    protected virtual void PlayerDetected(Vector3 targetPosition)
    {
        timeUntilPatrolTimer = 0;
        patrol.isActive = false;
        enemyState = EnemyState.Chasing;
        target = targetPosition;

        Vector3 posToTarget = target - transform.position;
        Quaternion targetRotation = Quaternion.LookRotation(posToTarget);
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        if (IsInAttackDistance())
        {
            enemyState = EnemyState.Attacking;
        }
    }

    protected virtual void Move(Transform newTarget)
    {
        if (!isParalyzed && !isStatic)
        {
            if (Vector3.Distance(target, transform.position) <= stoppingDistance)
                newTarget = patrol.NextTarget();

            target = newTarget.position;

            animator.SetBool("isPatrolling", true);
            animator.SetBool("isChasing", false);
        }
       
    }

    protected virtual void ChasingMove()
    {
        if (!isParalyzed && !isStatic)
        {
            animator.SetBool("isChasing", true);
            animator.SetBool("isPatrolling", false);
            if (Vector3.Distance(target, transform.position) <= stoppingDistance)
            {
                animator.SetBool("isChasing", false);
            }
        }
       
    }
    protected virtual IEnumerator Attack()
    {
        if (!isParalyzed)
        {
            isAttacking = true;
            animator.SetBool("isChasing", false);
            animator.SetBool("isAttacking", isAttacking);

            yield return new WaitForNextFrameUnit();
            yield return new WaitUntil(() => !animator.GetCurrentAnimatorStateInfo(0).IsName("Attack"));

            if (!IsInAttackDistance())
                enemyState = EnemyState.Chasing;
            Debug.Log(enemyState);
            isAttacking = false;
            animator.SetBool("isAttacking", isAttacking);

            yield return new WaitForSeconds(attackCd);
        }
        
    }
    public IEnumerator HitByIceBall(float speedChange, float slowDuration, GameObject explosionObj)
    {
        SlowEnemy(speedChange);
        Debug.Log(explosionObj);
        yield return new WaitForSeconds(slowDuration);
        SpeedUpEnemy(speedChange);
        Destroy(explosionObj);
    }
    public virtual void ToggleParalyze(float paraDuration)
    {
        isParalyzed = true;
        paraTimer = paraDuration;
        animator.SetBool("isPatrolling", false);
        animator.SetBool("isChasing", false);
    }

    protected virtual void SlowEnemy(float divider)
    {
        moveSpeed /= divider;
    }
    protected virtual void SpeedUpEnemy(float multiplier)
    {
        moveSpeed *= multiplier;
    }

    protected virtual void Hit()
    {
        animator.SetTrigger("isHit");
    }
    protected virtual void Death()
    {

        animator.SetTrigger("isDead"); //faudra wait ou qqchose 
        //À la fin de l'anim de mort
        enemyDrops.SpawnDrops();
        Destroy(gameObject);
    }
    protected bool IsInAttackDistance()
    {
        Vector3 posToTarget = target - transform.position;
        if (posToTarget.sqrMagnitude <= attackStopDistance * attackStopDistance)
        {
            return true;
        }
        return false;
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



    //protected void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        collision.gameObject.GetComponent<HealthComponent>().Hit(collisionDmg);
    //    }
    //}

    protected void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<HealthComponent>().Hit(dmg); 
        }
    }
}

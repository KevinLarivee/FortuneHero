using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(HealthComponent), typeof(BehaviourTree), typeof(TrackPlayerComponent))]
public class BossComponent : MonoBehaviour
{
    public float meleeDefense = 1f;
    public float rangeDefense = 1f;
    public float movementProbability = 0.3f;
    public float meleeProbability = 0.8f;
    public bool meleeStatus = false;
    public bool rangeStatus = false;

    public int meleeDmg = 1;
    public int rangeDmg = 5;
    public int collisionDmg = 10;

    [SerializeField] protected float moveSpeed = 5f;
    public float attackStopDistance = 3f;
    [SerializeField, Range(0, 1)] protected float[] phases;
    [SerializeField] int debuffsPerPhase = 2;
    protected int currentPhase = 0;
    public GameObject rangePrefab;

    [HideInInspector] public Animator animator;
    [HideInInspector] public NavMeshAgent agent;
    [HideInInspector] public TrackPlayerComponent trackPlayer;
    //protected EnemyDrops enemyDrops;
    protected HealthComponent healthComponent;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        animator = GetComponent<Animator>();
        agent = GetComponent<NavMeshAgent>();
        healthComponent = GetComponent<HealthComponent>();
        trackPlayer = GetComponent<TrackPlayerComponent>();

        healthComponent.onHit += Hit;
        healthComponent.onDeath += Death;
    }
    void Update()
    {
        //if (enemyState == EnemyState.Chasing)
        //    ChasingMove();
        //else if (enemyState == EnemyState.Attacking)
        //    StartCoroutine(Attack());

        //timeUntilPatrolTimer += Time.deltaTime; //start le timer 
        //    //Enable le patrol
        //    patrol.isActive = true;
        //    enemyState = EnemyState.Patrol;
        //}
        animator.SetFloat("Speed", agent.velocity.magnitude);
    }

    //protected virtual IEnumerator Attack()
    //{
    //    animator.SetBool("isChasing", false);
    //    animator.SetTrigger("Attack");

    //    yield return new WaitForSeconds(animationTime);
    //    enemyState = EnemyState.Chasing;

    //    yield return new WaitForSeconds(attackCd);
    //}

    protected virtual void Hit()
    {
        //animator.SetTrigger("hit");
        if (currentPhase < phases.Length && healthComponent.hp / healthComponent.maxHp <= phases[currentPhase])
        {
            currentPhase++;
            trackPlayer.GetTopStats(debuffsPerPhase);
            //animations?
        }
    }
    protected virtual void Death()
    {
        //animator.SetTrigger("death");
        //agent.isStopped = true;
        //À la fin de l'anim de mort
        //enemyDrops.SpawnDrops();

        //Déclenché fin de niveau!!!!
        GameManager.Instance.OnPlayerWin();
        Debug.Log("death");
        Destroy(gameObject);
    }

    //protected void OnCollisionEnter(Collision collision)
    //{
    //    if (collision.gameObject.CompareTag("Player"))
    //    {
    //        collision.gameObject.GetComponent<HealthComponent>().Hit(collisionDmg); //Lier le dmg au dmg de l'enemy
    //    }
    //}

}


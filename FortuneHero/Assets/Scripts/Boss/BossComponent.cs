using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(HealthComponent), typeof(BehaviourTree), typeof(TrackPlayerComponent))]
public class BossComponent : MonoBehaviour
{
    [SerializeField] string bossName;
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
    [SerializeField] GameObject paralyzePrefab;


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
    public void StartParalyzeBoss(float duration)
    {
        StartCoroutine(ParalyzeBoss(duration));
    }
    public void StartHitByIceBall(float speedChange, float slowDuration, GameObject explosionObj)
    {
        StartCoroutine(HitByIceBall(speedChange, slowDuration, explosionObj));
    }
    private IEnumerator ParalyzeBoss(float duration) //OPTI ??
    {
        agent.isStopped = true;
        animator.enabled = false;
        paralyzePrefab.SetActive(true);
        GolemBoss_BT golemComponent = null;
        AnubisBoss_BT anubisComponent = null;
        switch (bossName)
        {
            case "Golem":
                golemComponent = GetComponent<GolemBoss_BT>();
                golemComponent.enabled = false;
                break;
            case "Anubis":
                anubisComponent = GetComponent<AnubisBoss_BT>();
                anubisComponent.enabled = false;
                break;
        }

        yield return new WaitForSeconds(duration);
        switch (bossName)
        {
            case "Golem":
                golemComponent.enabled = true;
                break;
            case "Anubis":
                anubisComponent.enabled = true;
                break;
        }
        agent.isStopped = false;
        animator.enabled = true;

        paralyzePrefab.SetActive(false);
    }
    public IEnumerator HitByIceBall(float speedChange, float slowDuration, GameObject explosionObj)
    {
        agent.speed -= speedChange;
        yield return new WaitForSeconds(slowDuration);
        agent.speed += speedChange;
        Destroy(explosionObj);
    }

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


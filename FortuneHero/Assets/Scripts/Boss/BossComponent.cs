using System.Collections;
using UnityEngine;


[RequireComponent (typeof(HealthComponent), typeof(Behaviour), typeof(TrackPlayerComponent))]
public class BossComponent : MonoBehaviour
{
    public float meleeDefense = 1f;
    public float rangeDefense = 1f;
    public float movementProbability = 0.3f;

    public int meleeDmg = 1;
    public int rangeDmg = 5;
    public int collisionDmg = 10;

    [SerializeField] protected float moveSpeed = 5f;
    [SerializeField] protected float attackStopDistance = 3f;
    [SerializeField, Range(0, 1)] protected float[] phases;
    [SerializeField] int debuffsPerPhase = 2;
    protected int currentPhase = 0;
    protected float maxHp;

    protected Animator animator;
    //protected EnemyDrops enemyDrops;
    protected HealthComponent healthComponent;

    protected TrackPlayerComponent trackPlayer;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        animator = GetComponent<Animator>();
        healthComponent = GetComponent<HealthComponent>();
        trackPlayer = GetComponent<TrackPlayerComponent>();

        healthComponent.onHit += Hit;
        healthComponent.onDeath += Death;
    }

    void Start()
    {
        maxHp = healthComponent.hp;
        //Rendu là, mettre par défaut Presets à true?
        trackPlayer.AllPresets();
    }
    void Update()
    {
        //if (enemyState == EnemyState.Chasing)
        //    ChasingMove();
        //else if (enemyState == EnemyState.Attacking)
        //    StartCoroutine(Attack());

        //timeUntilPatrolTimer += Time.deltaTime; //start le timer 

        //if (timeUntilPatrolTimer >= timeUntilPatrol) //si le timer atteint le max:
        //{
        //    //Enable le patrol
        //    patrol.isActive = true;
        //    enemyState = EnemyState.Patrol;
        //}
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
        if(currentPhase < phases.Length && healthComponent.hp / maxHp <= phases[currentPhase])
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
        Destroy(gameObject);
    }

    protected void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<HealthComponent>().Hit(collisionDmg); //Lier le dmg au dmg de l'enemy
        }
    }

}


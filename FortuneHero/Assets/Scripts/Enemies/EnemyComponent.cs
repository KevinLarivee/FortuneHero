using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyDrops), typeof(HealthComponent))]
public class EnemyComponent : MonoBehaviour, IPoolable
{
    //!!! Quoi mettre dans EnnemyComponent vs SpecificEnnemyComponent

    public ObjectPoolComponent Pool { get; set; }

    Animator animator;
    EnemyDrops enemyDrops;
    HealthComponent healthComponent;
    //NavMeshAgent agent;
    //bool _isDead = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        animator = GetComponent<Animator>();
        enemyDrops = GetComponent<EnemyDrops>();
        healthComponent = GetComponent<HealthComponent>();
        //agent = GetComponent<NavMeshAgent>();
        //agent.updatePosition = false;
        //agent.updateRotation = true;
    }
    //private void OnEnable()
    //{
    //    agent.destination = patrol.NextTarget().position;
    //}
    // Update is called once per frame
    void Update()
    {

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
    public void Hit()
    {
        //animator.SetTrigger("hit");
        //healthBar.fillAmount = 1f;
        //if(no hp) Kill();
    }
    //public void Hit()
    //{
    //    //if (!_isDead)
    //    //    StartCoroutine(DeathDelay());
    //    //_isDead = true;
    //}
    void Kill()
    {
        //enemyDrops.Die();
    }
}

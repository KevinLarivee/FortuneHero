using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(PatrolComponent))]
public class RangedEnemyComponent : EnemyComponent
{
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private Transform firePoint;
    //[SerializeField] private float fireDelayInAnim = 0.35f; // moment du tir dans l’anim

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = moveSpeed;
        patrol.move = Move;
    }
    protected override void PlayerDetected(Vector3 targetPosition)
    {
        base.PlayerDetected(targetPosition);
        agent.isStopped = enemyState == EnemyState.Attacking;
    }
    protected override void Move(Transform newTarget)
    {
        agent.destination = newTarget.position;
        if (agent.speed != moveSpeed / 2)
            agent.speed = moveSpeed / 2;

        base.Move(newTarget);
    }
    protected override void ChasingMove()
    {
        agent.destination = target;
        if (agent.speed != moveSpeed)
            agent.speed = moveSpeed;
        base.ChasingMove();
    }
    protected override void SlowEnemy(float divider)
    {
        base.SlowEnemy(divider);
        agent.speed = moveSpeed;
    }
    protected override void SpeedUpEnemy(float multiplier)
    {
        base.SpeedUpEnemy(multiplier);
        agent.speed = moveSpeed;
    }
    public void FireProjectile()
    {
        Vector3 dir = target - firePoint.position;
        if(dir.y < 0)
            dir.y = 0;
        dir.Normalize();

        Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, rot);

        // Optionnel: si tu veux forcer la vitesse ici (en plus / à la place de ProjectileMovement.Start)
        //var rb = proj.GetComponent<Rigidbody>();
        //if (rb != null)
        //{
        //    rb.linearVelocity = dir * rb.linearVelocity.magnitude; // conserve la magnitude si définie sur le prefab
        //}
    }

    //protected override IEnumerator Attack()
    //{
    //    animator.SetBool("isChasing", false);
    //    animator.SetTrigger("Attack");

    //    float fireAt = Mathf.Clamp(fireDelayInAnim, 0f, animationTime);
    //    if (fireAt > 0f) yield return new WaitForSeconds(fireAt);

    //    FireProjectile();

    //    float rest = Mathf.Max(0f, animationTime - fireAt);
    //    if (rest > 0f) yield return new WaitForSeconds(rest);

    //    enemyState = EnemyState.Chasing;

    //    if (attackCd > 0f) yield return new WaitForSeconds(attackCd);
    //}

}

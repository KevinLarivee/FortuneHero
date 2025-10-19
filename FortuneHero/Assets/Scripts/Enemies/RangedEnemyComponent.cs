using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(PatrolComponent))]
public class RangedEnemyComponent : EnemyComponent
{
    [Header("Ranged")]
    [SerializeField] private GameObject projectilePrefab;   // ← ton prefab avec ProjectileMovement + Rigidbody
    [SerializeField] private Transform firePoint;           // ← un enfant "sortie de tir"
    [SerializeField] private float fireDelayInAnim = 0.35f; // moment du tir dans l’anim

    private NavMeshAgent agent;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        patrol.move = Move;
    }

    protected override void Move(Transform newTarget)
    {
        if (agent != null) 
            agent.destination = newTarget.position;
        base.Move(newTarget);
    }

    public void FireProjectile()
    {

        // Direction vers la dernière position connue du joueur (stockée dans 'target' par EnemyComponent)
        Vector3 dir = (target - firePoint.position);
        if (dir.sqrMagnitude < 1e-6f) dir = transform.forward;
        dir.Normalize();

        // Oriente le projectile et instancie
        Quaternion rot = Quaternion.LookRotation(dir, Vector3.up);
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, rot);

        // Optionnel: si tu veux forcer la vitesse ici (en plus / à la place de ProjectileMovement.Start)
        var rb = proj.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = dir * rb.linearVelocity.magnitude; // conserve la magnitude si définie sur le prefab
        }
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
}

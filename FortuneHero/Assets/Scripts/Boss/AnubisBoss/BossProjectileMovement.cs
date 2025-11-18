using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent (typeof(Rigidbody))]
public class BossProjectileMovement : MonoBehaviour
{
    [SerializeField] float distanceUntilRecycled = 80f;
    [SerializeField] float projectileVelocity = 20f;
    [SerializeField] float dmg = 20f;
    public bool launchProjectile = false;
    Rigidbody rb;
    Transform target;
    Vector3 initialPos;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        initialPos = transform.position;
        target = PlayerComponent.Instance.transform;
    }
    void Update()
    {
        if (launchProjectile)
        {
            rb.linearVelocity = transform.forward * projectileVelocity;
            transform.rotation = Quaternion.LookRotation(target.position - transform.position) * Quaternion.Euler(90, 95, 90); 
            // <--- Trial and error pour trouver la bonne rotation apres que le projectile est lancer, lookRotation pour que la rotation soit tt le temps baser sur ou le joueur est * le offset
            launchProjectile = false;
        }
        if ((transform.position - initialPos).sqrMagnitude > distanceUntilRecycled * distanceUntilRecycled)
        {
            Destroy(gameObject);
            //ObjectPool
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<HealthComponent>().Hit(dmg);
            Destroy(gameObject);
        }
    }
}


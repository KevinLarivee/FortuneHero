using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;

[RequireComponent (typeof(Rigidbody))]
public class BossProjectileMovement : MonoBehaviour
{
    public UnityEvent<CSquareEvent> onTrigger;
    [SerializeField] float distanceUntilRecycled = 80f;
    [SerializeField] float projectileVelocity = 20f;
    [SerializeField] float dmg = 20f;
    public bool launchProjectile = false;
    Rigidbody rb;
    Transform target;
    Vector3 initialPos;
    Collider selfCollider;
    bool isDestroyed = false;
    void Start()
    {
        selfCollider = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
        initialPos = transform.position;
        target = PlayerComponent.Instance.transform;
    }
    void Update()
    {
        if (launchProjectile)
        {
            transform.rotation = Quaternion.LookRotation(target.position + Vector3.up * 1f - transform.position) * Quaternion.Euler(90, 90, 90);
            rb.linearVelocity = transform.up * projectileVelocity;
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
        if (!isDestroyed)
            onTrigger?.Invoke(new(selfCollider, other));
        isDestroyed = true;
        Destroy(gameObject);
    }
}


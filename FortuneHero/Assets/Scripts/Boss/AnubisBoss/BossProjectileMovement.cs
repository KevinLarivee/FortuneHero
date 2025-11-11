using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
public class BossProjectileMovement : MonoBehaviour
{
    [SerializeField] float distanceUntilRecycled = 80f;
    [SerializeField] float projectileVelocity = 20f;
    [SerializeField] float dmg = 20f;
    Rigidbody rb;
    Vector3 initialPos;
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * projectileVelocity;
        initialPos = transform.position;
    }
    void Update()
    {
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


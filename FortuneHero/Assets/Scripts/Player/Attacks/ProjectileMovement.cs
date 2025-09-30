using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
public class ProjectileMovement : MonoBehaviour
{
    Rigidbody rb;
    PlayerMovement player;

    [SerializeField] float distanceUntilRecycled = 80f;
    [SerializeField] float projectileVelocity = 20f;
    Vector3 initialPos;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = PlayerMovement.Instance;
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
}


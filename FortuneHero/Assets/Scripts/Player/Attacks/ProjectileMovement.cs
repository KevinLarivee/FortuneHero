using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
public class ProjectileMovement : MonoBehaviour
{
    [SerializeField] float distanceUntilRecycled = 80f;
    [SerializeField] float projectileVelocity = 20f;
    Rigidbody rb;
    Vector3 initialPos;
    PlayerActions instance;
    void Start()
    {
        instance = PlayerActions.Instance;
        rb = GetComponent<Rigidbody>();
        rb.linearVelocity = transform.forward * projectileVelocity;
        initialPos = transform.position;
    }
    void Update()
    {
        if ((transform.position - initialPos).sqrMagnitude > distanceUntilRecycled * distanceUntilRecycled)
        {
            if (instance.currentType == ProjectileType.IceBall)
                instance.SetToIceBall(false);
            Destroy(gameObject);
            //ObjectPool
        }
    }
}


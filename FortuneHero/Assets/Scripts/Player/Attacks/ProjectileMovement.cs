using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
public class ProjectileMovement : MonoBehaviour
{
    Rigidbody rb;
    PlayerMovement player;

    [SerializeField] float distanceUntilRecycled = 80f;
    [SerializeField] float projectileVelocity = 20f;
    [SerializeField] Vector3 initialPosX;
    [SerializeField] Vector3 currentPosX;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = FindAnyObjectByType<PlayerMovement>();
        rb.linearVelocity = player.transform.forward * projectileVelocity;
        initialPosX = player.transform.position;
    }
    void Update()
    {
        currentPosX = gameObject.transform.position;
        if ((currentPosX - initialPosX).sqrMagnitude > distanceUntilRecycled * distanceUntilRecycled)
        {
            Destroy(gameObject);
            //ObjectPool
        }
    }
}


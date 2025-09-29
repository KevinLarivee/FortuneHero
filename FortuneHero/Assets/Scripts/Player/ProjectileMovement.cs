using UnityEngine;

[RequireComponent (typeof(Rigidbody))]
public class ProjectileMovement : MonoBehaviour
{
    Rigidbody rb;
    PlayerMovement player;

    [SerializeField] int distanceUntilRecycled = 100;
    [SerializeField] float projectileVelocity = 20f;
    float initialPosX;
    float currentPosX;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        player = FindAnyObjectByType<PlayerMovement>();
        rb.linearVelocity = player.transform.forward * projectileVelocity;
        initialPosX = player.transform.position.x;
    }
    void Update()
    {
        currentPosX = gameObject.transform.position.x;
        if (currentPosX - initialPosX > distanceUntilRecycled)
        {
            Destroy(gameObject);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
        //ObjectPool
    }

}


using UnityEngine;

public class MovingSpikeWall : MonoBehaviour
{
    [SerializeField] private Vector3 direction = Vector3.left; 
    [SerializeField] private float speed = 2f; 
    [SerializeField] private int damage = 20; 
    [SerializeField] private Transform pointA; // position initiale
    [SerializeField] private Transform pointB; // limite max

    private Vector3 startPosition;
    private Vector3 endPosition;

    private void Start()
    {
        startPosition = pointA.position;
        endPosition = pointB.position;
    }

    private void Update()
    {
        transform.position += direction * speed * Time.fixedDeltaTime;

        // Vérifie si on a atteint pointA ou pointB
        if (Vector3.Distance(transform.position, endPosition) < 0.05f && direction == (endPosition - startPosition).normalized)
        {
            direction = -direction;
        }
        else if (Vector3.Distance(transform.position, startPosition) < 0.05f && direction == (startPosition - endPosition).normalized)
        {
            direction = -direction;
        }
    }

    void OnCollisionEnter(Collision other)
    {
        //if (other.gameObject.CompareTag("Player"))
        //{
        //    PlayerHealth ph = other.gameObject.GetComponent<PlayerHealth>();
        //    if (ph != null)
        //        ph.TakeDamage(damage);
        //}
        //else if (other.gameObject.CompareTag("Spike"))
        //{
        //    direction = -direction;
        //}
    }


}

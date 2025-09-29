using UnityEngine;

public class ProjectileCollision : MonoBehaviour
{
    void Start()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Destroy(gameObject);
        //ObjectPool
    }
}

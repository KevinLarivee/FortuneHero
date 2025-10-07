using UnityEngine;

public class ProjectileCollision : MonoBehaviour
{
    public int dmg = 2;
    public string target = "Enemy";

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(target))
        {
            var enemyHealthComponent = collision.gameObject.GetComponent<HealthComponent>();
            enemyHealthComponent.Hit(dmg);
        }

        Destroy(gameObject);
        //ObjectPool
    }
}

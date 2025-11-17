using UnityEngine;

public class EnemyProjectileCollision : MonoBehaviour
{
    [SerializeField] float projectileDamage = 10f;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<HealthComponent>().Hit(projectileDamage);
        }
        Destroy(gameObject);
    }
}

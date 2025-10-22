using UnityEngine;
using UnityEngine.AI;

public class ProjectileCollision : MonoBehaviour
{
    [SerializeField] GameObject iceBallExplosionPrefab;
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            var enemyHealthComponent = collision.gameObject.GetComponent<HealthComponent>();
            if(PlayerActions.Instance.currentType == ProjectileType.IceBall)
            {
                var iceExplosionObj = Instantiate(iceBallExplosionPrefab, collision.gameObject.transform.position, Quaternion.identity);
                PlayerActions.Instance.SetToIceBall(false);

                var enemyComponent = collision.gameObject.GetComponent<EnemyComponent>();
                Debug.Log(enemyComponent);
                enemyComponent.StartCoroutine(enemyComponent.HitByIceBall(PlayerActions.Instance.speedDrop, PlayerActions.Instance.slowDuration, iceExplosionObj));
            }
            enemyHealthComponent.Hit(PlayerComponent.Instance.rangedAtkDmg);
        }

        Destroy(gameObject);
        //ObjectPool
    }
}

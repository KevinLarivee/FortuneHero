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

                var enemyComponent = collision.gameObject.GetComponent<EnemyComponent>();
                enemyComponent.StartCoroutine(enemyComponent.HitByIceBall(PlayerActions.Instance.speedDrop, PlayerActions.Instance.slowDuration, iceExplosionObj));
            }
            enemyHealthComponent.Hit(PlayerComponent.Instance.rangedAtkDmg);
            TrackPlayerComponent tracker;
            if ((tracker = collision.gameObject.GetComponent<TrackPlayerComponent>()) != null)
                tracker.IncreaseStat("playerRangeDmg", PlayerComponent.Instance.meleeAtkDmg);
        }
        PlayerActions.Instance.SetToIceBall(false);
        Destroy(gameObject);
        //ObjectPool
    }
}

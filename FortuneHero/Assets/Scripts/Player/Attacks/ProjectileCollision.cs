using UnityEngine;
using UnityEngine.AI;

public class ProjectileCollision : MonoBehaviour
{
    [SerializeField] GameObject iceBallExplosionPrefab;
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            var enemyHealthComponent = other.GetComponentInParent<HealthComponent>();

            if (PlayerActions.Instance.currentType == ProjectileType.IceBall)
            {
                var iceExplosionObj = Instantiate(iceBallExplosionPrefab, other.gameObject.transform.position, Quaternion.identity);
                iceExplosionObj.transform.parent = other.transform;
                var enemyComponent = other.gameObject.GetComponentInParent<EnemyComponent>();
                enemyComponent.StartCoroutine(enemyComponent.HitByIceBall(PlayerActions.Instance.speedDrop, PlayerActions.Instance.slowDuration, iceExplosionObj));
            }
            enemyHealthComponent.Hit(PlayerComponent.Instance.rangedAtkDmg);
            TrackPlayerComponent tracker;
            if ((tracker = other.gameObject.GetComponent<TrackPlayerComponent>()) != null)
                tracker.IncreaseStat("playerRangeDmg", PlayerComponent.Instance.meleeAtkDmg);
        }
        PlayerActions.Instance.SetToIceBall(false);
        Destroy(gameObject);
        //ObjectPool
    }
}

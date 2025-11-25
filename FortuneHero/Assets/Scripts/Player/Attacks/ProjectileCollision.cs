using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class ProjectileCollision : MonoBehaviour
{
    [SerializeField] GameObject iceBallExplosionPrefab;
    [SerializeField] float heightOffset = 2;
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy") || other.CompareTag("Boss"))
        {
            var enemyHealthComponent = other.GetComponentInParent<HealthComponent>();
            var bossComponent = other.GetComponent<BossComponent>();

            if (PlayerActions.Instance.currentType == ProjectileType.IceBall) //si iceball
            {
                var iceExplosionObj = Instantiate(iceBallExplosionPrefab, other.gameObject.transform.position, Quaternion.identity);
                iceExplosionObj.transform.parent = other.transform;
                iceExplosionObj.transform.position = new Vector3(iceExplosionObj.transform.position.x, iceExplosionObj.transform.position.y + heightOffset, iceExplosionObj.transform.position.z);
                if (bossComponent != null)
                {
                    bossComponent.StartHitByIceBall(PlayerActions.Instance.speedDrop, PlayerActions.Instance.slowDuration, iceExplosionObj);
                }
                else
                {
                    var enemyComponent = other.gameObject.GetComponentInParent<EnemyComponent>();
                    enemyComponent.StartCoroutine(enemyComponent.HitByIceBall(PlayerActions.Instance.speedDrop, PlayerActions.Instance.slowDuration, iceExplosionObj));
                }
            }

            enemyHealthComponent.Hit(PlayerComponent.Instance.rangedAtkDmg / (bossComponent != null ? bossComponent.rangeDefense : 1));
            TrackPlayerComponent tracker;
            if ((tracker = other.gameObject.GetComponent<TrackPlayerComponent>()) != null)
                tracker.IncreaseStat("playerRangeDmg", PlayerComponent.Instance.meleeAtkDmg / (bossComponent != null ? bossComponent.rangeDefense : 1));
        }
        PlayerActions.Instance.SetToIceBall(false);
        Destroy(gameObject);
        //ObjectPool
    }
}

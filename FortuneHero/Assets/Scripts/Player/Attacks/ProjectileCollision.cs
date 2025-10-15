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
                PlayerActions.Instance.isIceBall = false; //Mettre a false pour retourner a false. Noah doit mettre sa a true quand on appui sur la touche
                                                          //Quand on appui sur une autre touche en etant sur iceBall = true, mais qu'on a pas tirer, mettre a false
                                                          //Ne pas oublier d'enlever le power up apres utilisation

                var enemyComponent = collision.gameObject.GetComponent<EnemyComponent>();
                Debug.Log(enemyComponent);
                enemyComponent.TryGetComponent<NavMeshAgent>(out NavMeshAgent agent); 
                enemyComponent.StartCoroutine(enemyComponent.HitByIceBall(PlayerActions.Instance.speedDrop, PlayerActions.Instance.slowDuration, agent, iceExplosionObj));
            }
            enemyHealthComponent.Hit(PlayerComponent.Instance.rangedAtkDmg);
        }

        Destroy(gameObject);
        //ObjectPool
    }
}

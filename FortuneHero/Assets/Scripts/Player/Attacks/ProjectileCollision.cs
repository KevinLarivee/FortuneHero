using UnityEngine;

public class ProjectileCollision : MonoBehaviour
{
    [SerializeField] GameObject player;
    void Start()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            var enemyHealthComponent = collision.gameObject.GetComponent<HealthComponent>();
            enemyHealthComponent.Hit(player.GetComponent<PlayerActions>().rangedAtkDmg);
            //if (enemyHealthComponent.hp <= 0)
            //    enemyHealthComponent.Die();   OU Appeler Die dans Hit ? + Appeler SpawnDrops dans Die
            enemyHealthComponent.gameObject.GetComponentInParent<Animator>().SetTrigger("isHit");

            //Trop de getComponent ?
            //Pas getComponent dans start pour que si ya un upgrade au dmg, le nb de dmg s'update
        }

        Destroy(gameObject);
        //ObjectPool
    }
}

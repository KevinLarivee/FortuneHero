using UnityEngine;

public class ProjectileCollision : MonoBehaviour
{
    PlayerActions player;
    void Start()
    {
        player = PlayerMovement.Instance.GetComponent<PlayerActions>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Enemy"))
        {
            var enemyHealthComponent = collision.gameObject.GetComponent<HealthComponent>();
            enemyHealthComponent.Hit(player.rangedAtkDmg);
            //if (enemyHealthComponent.hp <= 0)
            //    enemyHealthComponent.Die();   OU Appeler Die dans Hit ? + Appeler SpawnDrops dans Die
            var animator = enemyHealthComponent.gameObject.GetComponentInParent<Animator>();
            if (animator == null)
            {
                enemyHealthComponent.gameObject.GetComponent<Animator>().SetTrigger("isHit");
            }
            else
            {
                animator.SetTrigger("isHit");
            }

            //Trop de getComponent ?
            //Pas getComponent dans start pour que si ya un upgrade au dmg, le nb de dmg s'update
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            var playerHealthComponent = PlayerMovement.Instance.GetComponent<HealthComponent>();
            playerHealthComponent.Hit(2); 
        }

        Destroy(gameObject);
        //ObjectPool
    }
}

using UnityEngine;

public class MeleeAttackCollision : MonoBehaviour
{
    [SerializeField] GameObject player;
    float meleeDmg;
    
    private void Start()
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("You have attacked the enemy !!!");
            var enemyHealthComponent = other.gameObject.GetComponent<HealthComponent>();
            enemyHealthComponent.Hit(player.GetComponent<PlayerActions>().meleeAtkDmg);
            //if (enemyHealthComponent.hp <= 0)
            //    enemyHealthComponent.Die();   OU Appeler Die dans Hit ? + Appeler SpawnDrops dans Die
            var caca = enemyHealthComponent.gameObject.GetComponentInParent<Animator>();
            if (caca == null)
            {
                enemyHealthComponent.gameObject.GetComponent<Animator>().SetTrigger("hit");
            }
            else
            {
                caca.SetTrigger("hit");
            }
            

            //Trop de getComponent ?
            //Pas getComponent dans start pour que si ya un upgrade au dmg, le nb de dmg s'update
        }
    }
}

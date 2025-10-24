using UnityEngine;

public class MeleeAttackCollision : MonoBehaviour
{
    PlayerActions player;
    
    void Start()
    {
        player = PlayerActions.Instance;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("You have attacked the enemy !!!");
            var enemyHealthComponent = other.gameObject.GetComponent<HealthComponent>();
            enemyHealthComponent.Hit(PlayerComponent.Instance.meleeAtkDmg);
        }
    }
}

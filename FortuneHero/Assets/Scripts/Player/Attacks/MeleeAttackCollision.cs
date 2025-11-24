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
        if (other.CompareTag("Enemy") || other.CompareTag("Boss"))
        {
            var enemyHealthComponent = other.GetComponentInParent<HealthComponent>();
            enemyHealthComponent.Hit(PlayerComponent.Instance.meleeAtkDmg);
            TrackPlayerComponent tracker;
            if ((tracker = other.gameObject.GetComponent<TrackPlayerComponent>()) != null)
                tracker.IncreaseStat("playerMeleeDmg", PlayerComponent.Instance.meleeAtkDmg);
        }
    }
}

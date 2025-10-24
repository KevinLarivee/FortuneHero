using UnityEngine;

public class EnnemyMeleeAttack : MonoBehaviour
{
    PlayerComponent player;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        RaycastHit hit;
        if(Physics.SphereCast(transform.position, 0.5f, transform.forward, out hit))
        {
            if((player = hit.collider.gameObject.GetComponent<PlayerComponent>()) != null)
            {
                player.GetComponent<HealthComponent>().Hit(10);
            }
        }

        Destroy(gameObject);
    }
}

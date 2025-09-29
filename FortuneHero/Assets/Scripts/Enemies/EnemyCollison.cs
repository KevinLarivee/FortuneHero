using UnityEngine;

public class EnemyCollison : MonoBehaviour
{
    Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.CompareTag("RangedAtk"))
        {
            gameObject.GetComponent<EnemyDrops>().SpawnDrops();
            //gameObject.SetActive(false);
            animator.SetTrigger("isHit");
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<HealthComponent>().Hit(10); //Lier le dmg au dmg de l'enemy
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("MeleeAtk"))
        {
            gameObject.GetComponent<EnemyDrops>().SpawnDrops();
            Debug.Log("isHit");
            //gameObject.SetActive(false);
            animator.SetTrigger("isHit");
        }
    }
}

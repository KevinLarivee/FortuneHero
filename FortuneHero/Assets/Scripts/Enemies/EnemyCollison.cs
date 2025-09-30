using UnityEngine;

public class EnemyCollison : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<HealthComponent>().Hit(10); //Lier le dmg au dmg de l'enemy
        }
    }
}

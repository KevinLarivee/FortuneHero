using UnityEngine;

public class DamageCollision : MonoBehaviour
{
    [SerializeField] int damage = 1;
    [SerializeField] string targetTag = "Player";
    [SerializeField] string statusEffect = "None"; // sera une enum plus tard
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(targetTag))
        {
            HealthComponent targetHealth = collision.gameObject.GetComponent<HealthComponent>();
            if (targetHealth != null)
            {
                //targetHealth.TakeDamage(damage);
                // Appliquer l'effet de statut si nécessaire
                if (statusEffect != "None")
                {
                    // Appliquer l'effet de statut ici
                    Debug.Log($"Applying status effect: {statusEffect}");
                }
            }
        }
    }

}

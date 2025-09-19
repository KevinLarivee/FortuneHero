using UnityEngine;

public class DamageCollision : MonoBehaviour
{
    [Header("Paramètres d'attaque")]
    [SerializeField] private int damage = 1;
    [SerializeField] private string targetTag = "Player";
    [SerializeField] private StatusEffect statusEffect = StatusEffect.Knockback;

    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 10f;
    [SerializeField] private float knockbackDuration = 0.25f;
    [Range(0f, 1f)][SerializeField] private float verticalFactor = 1f;

    private void HandleHit(GameObject other, string via)
    {
        if (!other.CompareTag(targetTag)) return;

        //Debug.Log($"[DamageCollision] {name} touche {other.name} via {via} | dmg={damage} | effect={statusEffect}");

        var targetHealth = other.GetComponent<HealthComponent>();
        if (targetHealth != null)
        {
            // targetHealth.Hit(damage, statusEffect);
        }

        var kb = other.GetComponent<KnockbackReceiver>();
        if (kb != null)
        {
            Vector3 dir = (other.transform.position - transform.position);
            dir.y = Mathf.Abs(dir.y) * verticalFactor;
            dir = dir.normalized;

            kb.ApplyKnockback(dir, knockbackForce, knockbackDuration);
            Debug.Log($"[DamageCollision] Knockback -> dir={dir}, force={knockbackForce}, dur={knockbackDuration}");
        }
        else
        {
            Debug.Log($"[DamageCollision] {other.name} n'a pas de KnockbackReceiver.");
        }
    }

    private void OnCollisionEnter(Collision collision) => HandleHit(collision.gameObject, "OnCollisionEnter");
    private void OnTriggerEnter(Collider other) => HandleHit(other.gameObject, "OnTriggerEnter");
}

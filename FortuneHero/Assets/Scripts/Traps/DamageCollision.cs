using UnityEngine;

public class DamageCollision : MonoBehaviour
{
    [Header("Paramètres d'attaque")]
    [SerializeField] int damage = 1;
    [SerializeField] string targetTag = "Player";
    [SerializeField] StatusEffect statusEffect = StatusEffect.Knockback;

    [Header("Knockback")]
    [SerializeField] float knockbackForce = 10f;
    [SerializeField] float knockbackDuration = 0.25f;
    [Range(0f, 3f)][SerializeField] float verticalFactor = 1f;

    /// <summary>
    /// Call this from PlayerCollision (OnControllerColliderHit) or other code paths.
    /// sourcePos = where the hit originates (usually spike center or hit collider center)
    /// via = for debug ("OnControllerColliderHit", "Manual", etc.)
    /// </summary>
    public void ApplyTo(GameObject other, Vector3 sourcePos, string via = "Manual")
    {
        if (other == null) return;
        if (!other.CompareTag(targetTag)) return;

        Debug.Log($"[DamageCollision] {name} touche {other.name} via {via} | dmg={damage} | effect={statusEffect}");

        // Optionnel: dégâts
        var targetHealth = other.GetComponent<HealthComponent>();
        // if (targetHealth != null) targetHealth.Hit(damage, statusEffect);

        // Knockback (no Rigidbody required on player)
        var kb = other.GetComponent<KnockbackReceiver>();
        if (kb != null)
        {
            Vector3 dir = (other.transform.position - sourcePos);
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

    // Still works if you later decide to use a trigger path (requires a Rigidbody somewhere).
    private void OnTriggerEnter(Collider other)
    {
        ApplyTo(other.gameObject, transform.position, "OnTriggerEnter");
    }
}

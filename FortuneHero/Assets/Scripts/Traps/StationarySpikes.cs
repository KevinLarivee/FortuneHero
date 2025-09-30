using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class StationarySpike : MonoBehaviour
{
    [Header("Cible & Dégâts")]
    [SerializeField] private string targetTag = "Player";
    [SerializeField] private int damage = 10;                  // dégâts infligés

    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 6f;        // intensité du push (AddForce VelocityChange)
    [Range(0f, 3f)][SerializeField] private float verticalFactor = 0.2f; // petit lift

    [Header("Anti-spam")]
    [SerializeField] private float reHitCooldown = 0.15f;      // évite le spam si on frotte la surface
    private float _nextAllowedTime = 0f;

    private void Reset()
    {
        var col = GetComponent<Collider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (Time.time < _nextAllowedTime) return;
        GameObject other = collision.gameObject;
        if (!other.CompareTag(targetTag)) return;

        var hc = other.GetComponent<HealthComponent>();
        //  hc.Hit(damage, StatusEffect.Knockback);
        Debug.Log($"[StationarySpike] {other.name} touché. Dégâts: {damage}");
        Rigidbody rb = collision.rigidbody; 
        Vector3 dir = collision.GetContact(0).normal;
        dir.y = Mathf.Abs(dir.y) * verticalFactor;
        dir = dir.normalized;
        rb.AddForce(dir * knockbackForce, ForceMode.VelocityChange);
        _nextAllowedTime = Time.time + reHitCooldown;
    }

   
}

using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class LightningTrap : MonoBehaviour
{
    [Header("Timing")]
    [Tooltip("Seconds ON, then OFF, then ON... (symmetrical duty cycle).")]
    [SerializeField] private float intervalSeconds = 5f;

    [Tooltip("Start disabled (OFF) or enabled (ON).")]
    [SerializeField] private bool disabled = false;

    [Header("Target & Damage (commented call below)")]
    [SerializeField] private string targetTag = "Player";
    [SerializeField] private int damage = 10;

    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 8f;
    [SerializeField] private float knockbackDuration = 0.20f;
    [Range(0f, 3f)][SerializeField] private float verticalFactor = 0.15f;

    [Header("Anti-spam")]
    [SerializeField] private float reHitCooldown = 0.15f;
    private float _nextAllowedHitTime = 0f;

    private Collider _col;

    private void Reset()
    {
        // Use a trigger volume so we don't physically shove the player; we just detect and knockback.
        _col = GetComponent<Collider>();
        if (_col != null) _col.isTrigger = true;
    }

    private void Awake()
    {
        _col = GetComponent<Collider>();
        _col.isTrigger = true;
    }

    private void OnEnable()
    {
        StartCoroutine(ToggleLoop());
    }

    private IEnumerator ToggleLoop()
    {
        // Simple symmetrical ON/OFF loop using intervalSeconds
        while (true)
        {
            // Phase A
            SetEnabled(true);
            yield return new WaitForSeconds(Mathf.Max(0.01f, intervalSeconds));

            // Phase B
            SetEnabled(false);
            yield return new WaitForSeconds(Mathf.Max(0.01f, intervalSeconds));
        }
    }

    private void SetEnabled(bool on)
    {
        disabled = !on;
        gameObject.SetActive(disabled);
        // Optional: change visuals here (material emission, particle enable, SFX, etc.)
        // Example:
        // if (renderer != null) renderer.material.SetColor("_EmissionColor", on ? Color.cyan : Color.black);

        // Also optional: enable/disable a child VFX object
        // if (vfxRoot != null) vfxRoot.SetActive(on);
    }

    private void OnTriggerEnter(Collider other)
    {
        TryHit(other, "OnTriggerEnter");
    }

    private void OnTriggerStay(Collider other)
    {
        // In case the player sits in the trap right when it turns ON
        TryHit(other, "OnTriggerStay");
    }

    private void TryHit(Collider other, string via)
    {
        if (disabled) return;
        if (Time.time < _nextAllowedHitTime) return;
        if (!other.CompareTag(targetTag)) return;

        // Direction: push the player away from the trap’s center, with a small vertical lift
        Vector3 source = transform.position;
        Vector3 dir = (other.transform.position - source);
        dir.y = Mathf.Abs(dir.y) * verticalFactor;
        dir = dir.sqrMagnitude > 1e-6f ? dir.normalized : transform.forward;

        // --- Knockback (via your PlayerMovement) ---
        // var pm = other.GetComponent<PlayerMovement>();
        // if (pm != null) pm.Knockback(dir, knockbackForce, knockbackDuration);

        // --- Damage (via your HealthComponent) ---
        // var hp = other.GetComponent<HealthComponent>();
        // if (hp != null) hp.Hit(damage, StatusEffect.Knockback);

        Debug.Log($"[LightningTrap] Hit {other.name} ({via}) | dir={dir} | F={knockbackForce} | T={knockbackDuration}");

        _nextAllowedHitTime = Time.time + reHitCooldown;
    }
}

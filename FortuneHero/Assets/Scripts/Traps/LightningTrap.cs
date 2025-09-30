//code a revoir
//ca marche mais ca dlair complique pour rien
using System.Collections;
using UnityEngine;
using DigitalRuby.LightningBolt;

[DisallowMultipleComponent]
[RequireComponent(typeof(Collider))]
public class LightningTrap : MonoBehaviour
{
    [Header("Timing")]
    [SerializeField] private float intervalSeconds = 3f;
    [SerializeField] private bool startDisabled = false;

    [Header("Cible & Dégâts (appels commentés plus bas)")]
    [SerializeField] private string targetTag = "Player";
    [SerializeField] private int damage = 10;

    [Header("Knockback")]
    [SerializeField] private float knockbackForce = 8f;
    [SerializeField] private float knockbackDuration = 0.20f;
    [Range(0f, 3f)][SerializeField] private float verticalFactor = 0.15f;

    [Header("Anti-spam")]
    [SerializeField] private float reHitCooldown = 0.15f;
    private float _nextAllowedHitTime = 0f;
    
    LightningBoltScript lightningBolt;

    private Collider _col;
    private LineRenderer[] _lineRenderers; // on ne touche qu’aux lignes (l’éclair), pas aux meshes de LightningStart/End
    private bool _isEnabledNow;

    private void Awake()
    {
        _col = GetComponent<Collider>();
        _col.isTrigger = true; // détection uniquement
        _lineRenderers = GetComponentsInChildren<LineRenderer>(true);
        lightningBolt = GetComponentInChildren<LightningBoltScript>(true);
    }

    private void OnEnable()
    {
        SetEnabled(!startDisabled);
        StartCoroutine(ToggleLoop());
    }

    private IEnumerator ToggleLoop()
    {
        float t = Mathf.Max(0.01f, intervalSeconds);
        while (true)
        {
            SetEnabled(true);
            yield return new WaitForSeconds(t);

            SetEnabled(false);
            yield return new WaitForSeconds(t);
        }
    }

    private void SetEnabled(bool on)
    {
        _isEnabledNow = on;

        // 1) Activer/désactiver la zone de hit
        if (_col != null) _col.enabled = on;

        // 2) Afficher/masquer l’éclair en activant/désactivant les LineRenderer
        if (_lineRenderers != null)
        {
            for (int i = 0; i < _lineRenderers.Length; i++)
            {
                if (_lineRenderers[i] != null)
                    _lineRenderers[i].enabled = on;
            }
        }

        // 3) Si l’éclair est en ManualMode, on trigge le bolt quand on passe ON (pas besoin de modifier le script d’origine)
        if (on && lightningBolt != null && lightningBolt.ManualMode)
        {
            lightningBolt.Trigger();
        }
    }

    private void OnTriggerEnter(Collider other) => TryHit(other, "OnTriggerEnter");
    private void OnTriggerStay(Collider other) => TryHit(other, "OnTriggerStay");

    private void TryHit(Collider other, string via)
    {
        if (!_isEnabledNow) return;
        if (Time.time < _nextAllowedHitTime) return;
        if (!other.CompareTag(targetTag)) return;

        // Direction depuis la trap vers le joueur + petit lift vertical
        Vector3 dir = other.transform.position - transform.position;
        dir.y = Mathf.Abs(dir.y) * verticalFactor;
        dir = dir.sqrMagnitude > 1e-6f ? dir.normalized : transform.forward;

        // --- Knockback (via ton PlayerMovement) ---
        // var pm = other.GetComponent<PlayerMovement>();
        // if (pm != null) pm.Knockback(dir, knockbackForce, knockbackDuration);

        // --- Dégâts (via ton HealthComponent) ---
        // var hp = other.GetComponent<HealthComponent>();
        // if (hp != null) hp.Hit(damage, StatusEffect.Knockback);

        Debug.Log($"[LightningTrap] Hit {other.name} ({via}) | ON={_isEnabledNow} | dir={dir} | F={knockbackForce}");

        _nextAllowedHitTime = Time.time + reHitCooldown;
    }
}

// Code a revoir (chat)
using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class KnockbackReceiver : MonoBehaviour
{
    [Header("Réglages généraux (sans Rigidbody)")]
    [Tooltip("Si un CharacterController est présent, on l'utilise; sinon on translate directement le Transform.")]
    [SerializeField] private bool allowWhileAirborne = true; // gardé pour futur filtrage (raycast sol, etc.)

    [Header("Sol (pour pouvoir réactiver le contrôle, optionnel)")]
    [SerializeField] private LayerMask groundLayers = ~0;        // assigne ta layer Ground
    [SerializeField] private float groundCheckRadius = 0.25f;    // rayon du check au sol
    [SerializeField] private Vector3 groundCheckOffset = new Vector3(0f, 0.1f, 0f);

    private CharacterController _cc;

    private Vector3 _impactVelocity;
    private float _impactTimeLeft;
    private bool _isApplyingImpact;

    void Awake()
    {
        _cc = GetComponent<CharacterController>();
    }

    /// <summary>
    /// Applique un knockback sans Rigidbody.
    /// - Si CharacterController: utilise controller.Move(delta).
    /// - Sinon: translate directement le Transform.
    /// </summary>
    public void ApplyKnockback(Vector3 direction, float force, float duration)
    {
        if (direction.sqrMagnitude < 0.0001f || force <= 0f) return;

        // Normalise et stocke une "vitesse d'impact" sur une durée
        _impactVelocity = direction.normalized * force;
        _impactTimeLeft = Mathf.Max(0.01f, duration);

        if (!_isApplyingImpact) StartCoroutine(ApplyImpactOverTime());
    }

    private IEnumerator ApplyImpactOverTime()
    {
        _isApplyingImpact = true;

        while (_impactTimeLeft > 0f)
        {
            float dt = Time.deltaTime;

            // Déplacement dû au knockback (décroissance linéaire simple)
            Vector3 delta = _impactVelocity * dt;

            if (_cc != null)
            {
                _cc.Move(delta);
            }
            else
            {
                // Fallback si pas de CharacterController (moins précis pour collisions)
                transform.position += delta;
            }

            _impactTimeLeft -= dt;
            yield return null;
        }

        _impactVelocity = Vector3.zero;
        _isApplyingImpact = false;
    }

    /// <summary>
    /// Renvoie vrai si on considère le joueur "au sol".
    /// - Avec CharacterController: utilise isGrounded.
    /// - Sinon: petit CheckSphere au niveau des pieds.
    /// </summary>
    public bool IsGrounded()
    {
        if (_cc != null) return _cc.isGrounded;

        Vector3 origin = transform.position + groundCheckOffset;
        return Physics.CheckSphere(origin, groundCheckRadius, groundLayers, QueryTriggerInteraction.Ignore);
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 origin = transform.position + groundCheckOffset;
        Gizmos.DrawWireSphere(origin, groundCheckRadius);
    }
#endif
}

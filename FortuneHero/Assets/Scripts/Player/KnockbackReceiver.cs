//Code a revoir (chat)
using System.Collections;
using UnityEngine;

[DisallowMultipleComponent]
public class KnockbackReceiver : MonoBehaviour
{
    [Header("R�glages g�n�raux")]
    [SerializeField] private bool allowWhileAirborne = true; // garde-le si tu veux filtrer par sol + raycast
    [SerializeField] private float dragWhileKnockback = 0f;

    private Rigidbody _rb;
    private CharacterController _cc;

    private Vector3 _impactVelocity;
    private float _impactTimeLeft;
    private bool _isApplyingImpact;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _cc = GetComponent<CharacterController>();
    }

    public void ApplyKnockback(Vector3 direction, float force, float duration)
    {
        if (direction.sqrMagnitude < 0.0001f || force <= 0f) return;

        if (_rb != null && !_rb.isKinematic)
        {
            if (dragWhileKnockback > 0f) _rb.linearDamping = dragWhileKnockback;
            _rb.AddForce(direction.normalized * force, ForceMode.VelocityChange);
            if (dragWhileKnockback > 0f) StartCoroutine(ResetDragAfter(duration));
            return;
        }

        _impactVelocity = direction.normalized * force;
        _impactTimeLeft = Mathf.Max(0.01f, duration);
        if (!_isApplyingImpact) StartCoroutine(ApplyImpactOverTime());
    }

    private IEnumerator ResetDragAfter(float t)
    {
        yield return new WaitForSeconds(t);
        if (_rb != null) _rb.linearDamping = 0f;
    }

    private IEnumerator ApplyImpactOverTime()
    {
        _isApplyingImpact = true;
        while (_impactTimeLeft > 0f)
        {
            float dt = Time.deltaTime;
            Vector3 delta = _impactVelocity * dt;

            if (_cc != null) _cc.Move(delta);
            else transform.position += delta;

            _impactTimeLeft -= dt;
            yield return null;
        }
        _impactVelocity = Vector3.zero;
        _isApplyingImpact = false;
    }
}

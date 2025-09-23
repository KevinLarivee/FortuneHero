//code a revoir (chat)
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerCollision : MonoBehaviour
{

    void Awake()
    {
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (!hit.gameObject.CompareTag("Trap")) return;

        var dc = hit.collider.GetComponent<DamageCollision>();
        if (dc == null) dc = hit.collider.GetComponentInParent<DamageCollision>();
        if (dc == null) return;

        Vector3 sourcePos = hit.collider.bounds.center;

        dc.ApplyTo(gameObject, sourcePos, "OnControllerColliderHit");
    }
}

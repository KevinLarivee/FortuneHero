using UnityEngine;

public class DamageCollision : MonoBehaviour
{
    [Header("Param�tres d'attaque")]
    [SerializeField] int damage = 1;
    [SerializeField] string targetTag = "Player";
    [SerializeField] StatusEffect statusEffect = StatusEffect.Knockback;

    [Header("Knockback")]
    [SerializeField] float knockbackForce = 10f;
    [SerializeField] float knockbackDuration = 0.25f;
    [Range(0f, 3f)][SerializeField] float verticalFactor = 1f;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            //faire degats
            collision.gameObject.GetComponent<HealthComponent>().Hit(damage);//statusEffect
            Vector3 sourcePos = collision.collider.bounds.center;
            //appel knockback
            //PlayerMovement.Instance.KnockBack(sourcePos, knockbackForce, knockbackDuration, verticalFactor);
            Debug.Log($"[StationarySpike] {collision.gameObject.name} touch�. D�g�ts: {damage}");
        }
    }
}

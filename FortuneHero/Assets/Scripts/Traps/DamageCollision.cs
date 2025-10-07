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

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Damage(collision);
        }
    }

    public void Damage(Collision collision)
    {
        //faire degats
        collision.gameObject.GetComponent<HealthComponent>().Hit(damage);//statusEffect
        Vector3 sourcePos = collision.collider.bounds.center;
        //appel knockback
        //PlayerMovement.Instance.KnockBack(sourcePos, knockbackForce, knockbackDuration, verticalFactor);
        Debug.Log($"{collision.gameObject.name} touché. Dégats: {damage}");

        if (Physics.Raycast(collision.contacts[0].point, -collision.contacts[0].normal, 1f, 69))
            RespawnManager.Instance.Respawn();
    }
}
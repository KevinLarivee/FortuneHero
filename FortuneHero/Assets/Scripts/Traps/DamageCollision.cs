using NaughtyAttributes;
using UnityEngine;

public class DamageCollision : MonoBehaviour
{
    [Header("Paramètres d'attaque")]
    [SerializeField] int damage = 1;
    [SerializeField] string targetTag = "Player";
    [EnumFlags] public StatusEffect statusEffects = StatusEffect.None;

    [Header("Knockback")]
    [SerializeField, ShowIf(nameof(HasKnockback))] float knockbackForce = 10f;
    [SerializeField, ShowIf(nameof(HasKnockback))] float verticalMultiplier = 0.4f;

    [Header("Burn")]
    [SerializeField, ShowIf(nameof(HasBurn))] int burnDamage = 5;
    [SerializeField, ShowIf(nameof(HasBurn))] float afterBurnTime = 3f;

    [Header("Paralyze")]
    [SerializeField, ShowIf(nameof(HasParalyze))] int paralyzeDamage = 1;
    [SerializeField, ShowIf(nameof(HasParalyze))] float paralyzeDuration = 0.1f;

    [Header("Freeze")]
    [SerializeField, ShowIf(nameof(HasFreeze))] int freezeDamage = 1;
    [SerializeField, ShowIf(nameof(HasFreeze))] float freezeMultiplier = 2f;
    [SerializeField, ShowIf(nameof(HasFreeze))] float freezeDuration = 2;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(targetTag))
        {
            Damage(collision);
        }
    }

    public void Damage(Collision collision)
    {
        //faire degats
        collision.gameObject.GetComponent<HealthComponent>().Hit(damage);

        Vector3 knockDir = collision.contacts[0].normal;
        knockDir.y = 0f;
        knockDir.Normalize();

        PlayerMovement.Instance.KnockBack(transform.position + knockDir, knockbackForce, verticalMultiplier);
        Debug.Log($"{collision.gameObject.name} touché. Dégats: {damage}");

        if (Physics.Raycast(collision.contacts[0].point, -collision.contacts[0].normal, 1f, 69))
            RespawnManager.Instance.Respawn();
    }
    //public bool HasEffect(StatusEffect effect)
    //{
    //    return (statusEffects & effect) != 0;
    //}

    public void AddEffect(StatusEffect effect)
    {
        statusEffects |= effect;
    }

    public void RemoveEffect(StatusEffect effect)
    {
        statusEffects &= ~effect;
    }

    public void ClearEffects()
    {
        statusEffects = StatusEffect.None;
    }

    // Exemple de raccourcis pratiques
    public bool HasBurn => statusEffects.HasFlag(StatusEffect.Burn);//HasEffect(StatusEffect.Burn);
    public bool HasFreeze => statusEffects.HasFlag(StatusEffect.Freeze); //HasEffect(StatusEffect.Freeze);
    public bool HasParalyze => statusEffects.HasFlag(StatusEffect.Paralyze); //HasEffect(StatusEffect.Paralyze);
    public bool HasKnockback => statusEffects.HasFlag(StatusEffect.Knockback); //HasEffect(StatusEffect.Knockback);
}
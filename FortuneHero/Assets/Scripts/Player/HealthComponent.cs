using UnityEngine;
using UnityEngine.Events;

public enum StatusEffect { None, Burn, Freeze, Paralyze, Knockback }

//Code a revoir (chat)
public class HealthComponent : MonoBehaviour
{
    // Événement optionnel pour écouter les coups ailleurs (UI, sons, etc.)
    public UnityAction<int, StatusEffect> OnHit;

    // Exemple de stockage de PV si tu veux l’activer plus tard:
    // [SerializeField] private int maxHp = 10;
    // private int currentHp;

    // void Start() { currentHp = maxHp; }

    public void Hit(int dmg, StatusEffect status = StatusEffect.None)
    {
        // Pour l’instant: LOG uniquement.
        Debug.Log($"[HealthComponent] {name} a reçu {dmg} dégâts. Effet: {status}");

        // Si tu veux émettre un event:
        OnHit?.Invoke(dmg, status);

        // Si tu veux gérer des PV plus tard:
        // currentHp = Mathf.Max(0, currentHp - dmg);
        // if (currentHp == 0) { Debug.Log($"{name} est KO."); }
    }
}

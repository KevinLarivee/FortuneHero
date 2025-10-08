//Code de chat (a voir)
using System.Collections.Generic;
using UnityEngine;

public class SkillComponent : MonoBehaviour
{
    // Types de skills supportés
    public enum SkillType
    {
        MeleeAtkPlus,     // atk + (mêlée)
        RangeAtkPlus,     // atk + (distance)
        ShieldBlockTime,  // block time +
        DashCooldownMinus,// cd -
        MaxHealthPlus,    // point de vie +
        SpeedPlus         // vitesse +
    }


    // === Paramètres simples pour acheter/appliquer les skills ===
    [Header("Monnaie des skills")]
    public int skillPoints = 0;

    [Header("Coûts de base par skill")]
    [SerializeField] private int meleeCost = 1;
    [SerializeField] private int rangeCost = 1;
    [SerializeField] private int shieldCost = 1;
    [SerializeField] private int dashCost = 1;
    [SerializeField] private int healthCost = 1;
    [SerializeField] private int speedCost = 1;

    [Header("Valeurs appliquées par achat (cumulables)")]
    [SerializeField] private float meleeAtkPerBuy = 1f;
    [SerializeField] private float rangeAtkPerBuy = 1f;
    [SerializeField] private float shieldBlockTimePerBuy = 0.25f;
    [SerializeField] private float dashCooldownMinusPerBuy = 0.2f; // réduit le CD
    [SerializeField] private int maxHealthPerBuy = 10;
    [SerializeField] private float speedPerBuy = 0.2f;

    // === Modificateurs actifs (pour le niveau en cours) ===
    public float meleeAtkBonus { get; private set; }
    public float rangeAtkBonus { get; private set; }
    public float shieldBlockTimeBonus { get; private set; }
    public float dashCooldownReduction { get; private set; }
    public int maxHealthBonus { get; private set; }
    public float speedBonus { get; private set; }

    // Suivi du nombre d’achats par skill pour ce niveau (utile si tu veux afficher/limiter)
    private readonly Dictionary<SkillType, int> _buys = new Dictionary<SkillType, int>()
    {
        { SkillType.MeleeAtkPlus, 0 },
        { SkillType.RangeAtkPlus, 0 },
        { SkillType.ShieldBlockTime, 0 },
        { SkillType.DashCooldownMinus, 0 },
        { SkillType.MaxHealthPlus, 0 },
        { SkillType.SpeedPlus, 0 }
    };

    // Table des coûts (remplie au Start pour rester simple/éditable depuis l’inspecteur)
    private Dictionary<SkillType, int> _costs;

    // Start est appelé avant la 1re frame
    void Start()
    {
        //skillPoints = PlayerPrefs.GetInt("skill", skillPoints);
        _costs = new Dictionary<SkillType, int>
        {
            { SkillType.MeleeAtkPlus, meleeCost },
            { SkillType.RangeAtkPlus, rangeCost },
            { SkillType.ShieldBlockTime, shieldCost },
            { SkillType.DashCooldownMinus, dashCost },
            { SkillType.MaxHealthPlus, healthCost },
            { SkillType.SpeedPlus, speedCost }
        };
    }

    void OnDisable()
    {
        PlayerPrefs.SetInt("skill", skillPoints);
        PlayerPrefs.Save();
    }

    // Exemple d’API très simple : choisir/acheter un skill par type
    public bool TryBuySkill(SkillType type)
    {
        if (_costs == null) Start(); // sécurité si appelé très tôt

        int cost = _costs[type];
        if (skillPoints < cost)
        {
            Debug.LogWarning($"[SkillComponent] Pas assez de skill points ({skillPoints}) pour acheter {type} (coût {cost}).");
            return false;
        }

        // Débiter
        skillPoints -= cost;

        // Appliquer l’effet (cumulable)
        ApplySkillEffect(type);

        // Compter l’achat (utile pour UI / limites éventuelles)
        _buys[type]++;

        Debug.Log($"[SkillComponent] Achat de {type} réussi. Restant: {skillPoints} SP.");
        return true;
    }

    // Retirer un skill (optionnel, si tu veux permettre de "déséquipe" un skill)
    public bool RemoveSkill(SkillType type)
    {
        if (_buys[type] <= 0)
        {
            Debug.LogWarning($"[SkillComponent] Impossible de retirer {type} car aucun achat n’a été fait.");
            return false;
        }
        int cost = _costs[type];
        // Rembourser
        skillPoints += cost;
        // Retirer l’effet (inverse de ApplySkillEffect)
        switch (type)
        {
            case SkillType.MeleeAtkPlus:
                meleeAtkBonus -= meleeAtkPerBuy;
                break;
            case SkillType.RangeAtkPlus:
                rangeAtkBonus -= rangeAtkPerBuy;
                break;
            case SkillType.ShieldBlockTime:
                shieldBlockTimeBonus -= shieldBlockTimePerBuy;
                break;
            case SkillType.DashCooldownMinus:
                dashCooldownReduction -= dashCooldownMinusPerBuy;
                break;
            case SkillType.MaxHealthPlus:
                maxHealthBonus -= maxHealthPerBuy;
                break;
            case SkillType.SpeedPlus:
                speedBonus -= speedPerBuy;
                break;
        }
        // Compter le retrait
        _buys[type]--;
        Debug.Log($"[SkillComponent] Retrait de {type} réussi. Restant: {skillPoints} SP.");
        return true;
    }

    // Variante: une méthode "ChooseSkill" qui pourrait être appelée depuis des boutons UI
    // (ex: binder un bouton par skill et lui passer le type voulu)
    public void ChooseSkill(SkillType type, bool remove)
    {
        if (remove)
        {
            RemoveSkill(type);
        }
        else
        {
            TryBuySkill(type);
        }
    }

    // Applique le bonus correspondant
    private void ApplySkillEffect(SkillType type)
    {
        switch (type)
        {
            case SkillType.MeleeAtkPlus:
                meleeAtkBonus += meleeAtkPerBuy;
                break;
            case SkillType.RangeAtkPlus:
                rangeAtkBonus += rangeAtkPerBuy;
                break;
            case SkillType.ShieldBlockTime:
                shieldBlockTimeBonus += shieldBlockTimePerBuy;
                break;
            case SkillType.DashCooldownMinus:
                dashCooldownReduction += dashCooldownMinusPerBuy;
                break;
            case SkillType.MaxHealthPlus:
                maxHealthBonus += maxHealthPerBuy;
                break;
            case SkillType.SpeedPlus:
                speedBonus += speedPerBuy;
                break;
        }
    }

    // À appeler en fin de niveau pour effacer tous les buffs temporaires
    public void ResetAllSkills()
    {
        meleeAtkBonus = 0f;
        rangeAtkBonus = 0f;
        shieldBlockTimeBonus = 0f;
        dashCooldownReduction = 0f;
        maxHealthBonus = 0;
        speedBonus = 0f;

        // Réinitialiser le compteur d’achats
        _buys[SkillType.MeleeAtkPlus] = 0;
        _buys[SkillType.RangeAtkPlus] = 0;
        _buys[SkillType.ShieldBlockTime] = 0;
        _buys[SkillType.DashCooldownMinus] = 0;
        _buys[SkillType.MaxHealthPlus] = 0;
        _buys[SkillType.SpeedPlus] = 0;

        Debug.Log("[SkillComponent] Tous les skills temporaires ont été réinitialisés pour le prochain niveau.");
    }

    // Optionnel: ajouter des points (drop, récompense, etc.)
    public void AddSkillPoints(int amount)
    {
        if (amount <= 0) return;
        skillPoints += amount;
    }
    public int GetBuys(SkillType type)
    {
        return _buys[type];
    }
    // Update est appelé une fois par frame
    void Update()
    {
        // Rien d’obligatoire ici; on laisse vide.
    }
}

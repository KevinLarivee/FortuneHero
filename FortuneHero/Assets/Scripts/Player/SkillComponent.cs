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

    // === Références joueur / vie ===
    private PlayerComponent player;
    private HealthComponent health;
    private PlayerActions playerAction;

    // Valeurs de base lues au début (avant bonus)
    private int baseMeleeAtk;
    private int baseRangeAtk;
    private float baseDashCooldown;
    private float baseSpeedMultiplier;
    private float baseMaxHp;

    // === Paramètres simples pour acheter/appliquer les skills ===
    [Header("Monnaie des skills")]
    public int skillPoints;

    [Header("Coûts de base par skill")]
    [SerializeField] private int meleeCost = 1;
    [SerializeField] private int rangeCost = 1;
    [SerializeField] private int shieldCost = 1;
    [SerializeField] private int dashCost = 1;
    [SerializeField] private int healthCost = 1;
    [SerializeField] private int speedCost = 1;

    [Header("Valeurs appliquées par achat (cumulables)")]
    [SerializeField] private int meleeAtkPerBuy = 1;
    [SerializeField] private int rangeAtkPerBuy = 1;
    [SerializeField] private float shieldBlockTimePerBuy = 0.25f;
    [SerializeField] private float dashCooldownMinusPerBuy = 0.2f;  // réduit le CD
    [SerializeField] private float maxHealthPerBuy = 10f;
    [SerializeField] private float speedPerBuy = 0.2f;

    // === Modificateurs actifs (pour le niveau en cours) ===
    public float meleeAtkBonus { get; private set; }
    public float rangeAtkBonus { get; private set; }
    public float shieldBlockTimeBonus { get; private set; }
    public float dashCooldownReduction { get; private set; }
    public float maxHealthBonus { get; private set; }
    public float speedBonus { get; private set; }

    // Suivi du nombre d’achats par skill pour ce niveau
    private readonly Dictionary<SkillType, int> _buys = new Dictionary<SkillType, int>()
    {
        { SkillType.MeleeAtkPlus, 0 },
        { SkillType.RangeAtkPlus, 0 },
        { SkillType.ShieldBlockTime, 0 },
        { SkillType.DashCooldownMinus, 0 },
        { SkillType.MaxHealthPlus, 0 },
        { SkillType.SpeedPlus, 0 }
    };

    // Table des coûts
    private Dictionary<SkillType, int> _costs;

    void Start()
    {
        player = PlayerComponent.Instance;
        health = player.healthComponent;

        if (player != null)
        {
            baseMeleeAtk = player.meleeAtkDmg;
            baseRangeAtk = player.rangedAtkDmg;
            baseDashCooldown = player.dashCooldown;
            baseSpeedMultiplier = player.speedMultiplier;
        }

        if (health != null)
        {
            baseMaxHp = health.maxHp;
        }
        skillPoints = PlayerPrefs.GetInt("Skill", skillPoints);
        _buys[SkillType.MeleeAtkPlus] = PlayerPrefs.GetInt("MeleeAtkPlus", 0);
        _buys[SkillType.RangeAtkPlus] = PlayerPrefs.GetInt("RangeAtkPlus", 0);
        _buys[SkillType.ShieldBlockTime] = PlayerPrefs.GetInt("ShieldBlockTime", 0);
        _buys[SkillType.DashCooldownMinus] = PlayerPrefs.GetInt("DashCooldownMinus", 0);
        _buys[SkillType.MaxHealthPlus] = PlayerPrefs.GetInt("MaxHealthPlus", 0);
        _buys[SkillType.SpeedPlus] = PlayerPrefs.GetInt("SpeedPlus", 0);

        foreach (KeyValuePair<SkillType, int> buy in _buys)
        {
            for (int i = 0; i < buy.Value; i++)
            {
                ApplySkillEffect(buy.Key);
            }
        }

        _costs = new Dictionary<SkillType, int>
        {
            { SkillType.MeleeAtkPlus, meleeCost },
            { SkillType.RangeAtkPlus, rangeCost },
            { SkillType.ShieldBlockTime, shieldCost },
            { SkillType.DashCooldownMinus, dashCost },
            { SkillType.MaxHealthPlus, healthCost },
            { SkillType.SpeedPlus, speedCost }
        };

        // Si jamais tu reload une scène avec des bonus déjà présents
        ApplyAllBonusesToPlayer();
    }

    void OnDisable()
    {
        PlayerPrefs.SetInt("Skill", skillPoints);
        PlayerPrefs.Save();
    }

    // Acheter un skill
    public bool TryBuySkill(SkillType type)
    {
        skillPoints = PlayerPrefs.GetInt("Skill", skillPoints);

        if (_costs == null) Start(); // sécurité au cas où

        int cost = _costs[type];
        if (skillPoints < cost)
        {
            Debug.LogWarning($"[SkillComponent] Pas assez de skill points ({skillPoints}) pour acheter {type} (coût {cost}).");
            return false;
        }
        // Compter l’achat
        _buys[type]++;

        // Débiter
        PlayerPrefs.SetInt("Skill", skillPoints - 1);
        PlayerPrefs.SetInt(nameof(type), _buys[type]);
        PlayerPrefs.Save();

        // Appliquer l’effet (cumulable)
        ApplySkillEffect(type);

        Debug.Log($"[SkillComponent] Achat de {type} réussi. Restant: {skillPoints} SP.");
        return true;
    }

    // Retirer un skill (si tu veux permettre de déséquiper)
    public bool RemoveSkill(SkillType type)
    {
        if (_buys[type] <= 0)
        {
            Debug.LogWarning($"[SkillComponent] Impossible de retirer {type} car aucun achat n’a été fait.");
            return false;
        }

        int cost = _costs[type];
        _buys[type]--;

        // Débiter


        // Rembourser
        PlayerPrefs.SetInt("Skill", skillPoints + 1);
        PlayerPrefs.SetInt(nameof(type), _buys[type]);
        PlayerPrefs.Save();

        // Retirer l’effet (inverse de ApplySkillEffect)
        switch (type)
        {
            case SkillType.MeleeAtkPlus:
                player.meleeAtkDmg -= meleeAtkPerBuy;
                break;
            case SkillType.RangeAtkPlus:
                player.rangedAtkDmg -= rangeAtkPerBuy;
                break;
            case SkillType.ShieldBlockTime:
                playerAction.defenceMaxCharge -= shieldBlockTimePerBuy;
                break;
            case SkillType.DashCooldownMinus:
                player.dashCooldown -= dashCooldownMinusPerBuy;
                break;
            case SkillType.MaxHealthPlus:
                health.maxHp -= maxHealthPerBuy;
                health.hp -= maxHealthPerBuy;
                break;
            case SkillType.SpeedPlus:
                player.speedMultiplier -= speedPerBuy;
                break;
        }

        // Recalculer les stats
        ApplyAllBonusesToPlayer();

        

        Debug.Log($"[SkillComponent] Retrait de {type} réussi. Restant: {skillPoints} SP.");
        return true;
    }

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

    // Applique le bonus au modèle interne, puis recalcule les stats
    private void ApplySkillEffect(SkillType type)
    {
        switch (type)
        {
            case SkillType.MeleeAtkPlus:
                player.meleeAtkDmg += meleeAtkPerBuy;
                break;
            case SkillType.RangeAtkPlus:
                player.rangedAtkDmg += rangeAtkPerBuy;
                break;
            case SkillType.ShieldBlockTime:
                playerAction.defenceMaxCharge += shieldBlockTimePerBuy;
                break;
            case SkillType.DashCooldownMinus:
                player.dashCooldown += dashCooldownMinusPerBuy;
                break;
            case SkillType.MaxHealthPlus:
                health.maxHp += maxHealthPerBuy;
                health.hp += maxHealthPerBuy;
                break;
            case SkillType.SpeedPlus:
                player.speedMultiplier += speedPerBuy;
                break;
        }

        ApplyAllBonusesToPlayer();
    }

    // Recalcule toutes les stats dans PlayerComponent / HealthComponent
    private void ApplyAllBonusesToPlayer()
    {
        if (player != null)
        {
            // Attaques
            player.meleeAtkDmg = baseMeleeAtk + Mathf.RoundToInt(meleeAtkBonus);
            player.rangedAtkDmg = baseRangeAtk + Mathf.RoundToInt(rangeAtkBonus);

            // Dash cooldown (clamp pour éviter 0 ou négatif)
            player.dashCooldown = Mathf.Max(0.1f, baseDashCooldown - dashCooldownReduction);

            // Vitesse
            player.speedMultiplier = baseSpeedMultiplier + speedBonus;
        }

        if (health != null)
        {
            // Points de vie max / actuels
            health.maxHp = baseMaxHp + maxHealthBonus;

            if (health.hp > health.maxHp)
            {
                health.hp = health.maxHp;
            }

            health.SetBar(health.hp / health.maxHp);
        }
    }

    // Reset des buffs (ex: fin de niveau)
    public void ResetAllSkills()
    {
        //meleeAtkBonus = 0f;
        //rangeAtkBonus = 0f;
        //shieldBlockTimeBonus = 0f;
        //dashCooldownReduction = 0f;
        //maxHealthBonus = 0f;
        //speedBonus = 0f;

        //_buys[SkillType.MeleeAtkPlus] = 0;
        //_buys[SkillType.RangeAtkPlus] = 0;
        //_buys[SkillType.ShieldBlockTime] = 0;
        //_buys[SkillType.DashCooldownMinus] = 0;
        //_buys[SkillType.MaxHealthPlus] = 0;
        //_buys[SkillType.SpeedPlus] = 0;

        //ApplyAllBonusesToPlayer();

        //Debug.Log("[SkillComponent] Tous les skills temporaires ont été réinitialisés pour le prochain niveau.");

        PlayerPrefs.SetInt("Skill", 0);
        PlayerPrefs.SetInt("MeleeAtkPlus", 0);
        PlayerPrefs.SetInt("RangeAtkPlus", 0);
        PlayerPrefs.SetInt("ShieldBlockTime", 0);
        PlayerPrefs.SetInt("DashCooldownMinus", 0);
        PlayerPrefs.SetInt("MaxHealthPlus", 0);
        PlayerPrefs.SetInt("SpeedPlus", 0);
        PlayerPrefs.Save();
    }

    public void AddSkillPoints(int amount)
    {
        if (amount <= 0) return;
        skillPoints += amount;
        PlayerPrefs.SetInt("Skill", skillPoints);
        PlayerPrefs.Save();
    }

    public int GetBuys(SkillType type)
    {
        return _buys[type];
    }

    void Update()
    {
        // vide pour l’instant
    }
}

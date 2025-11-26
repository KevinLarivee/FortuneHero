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
        if (player == null)
        {
            Debug.LogError("[SkillComponent] Aucun PlayerComponent.Instance trouvé.");
            return;
        }

        health = player.healthComponent;
        playerAction = player.GetComponent<PlayerActions>();

        // valeurs de base
        baseMeleeAtk = player.meleeAtkDmg;
        baseRangeAtk = player.rangedAtkDmg;
        baseDashCooldown = player.dashCooldown;
        baseSpeedMultiplier = player.speedMultiplier;

        if (health != null)
        {
            baseMaxHp = health.maxHp;
        }

        // Charger les skill points
        skillPoints = PlayerPrefs.GetInt("Skill", skillPoints);

        // Charger le nombre d’achats par skill
        _buys[SkillType.MeleeAtkPlus] = PlayerPrefs.GetInt(SkillType.MeleeAtkPlus.ToString(), 0);
        _buys[SkillType.RangeAtkPlus] = PlayerPrefs.GetInt(SkillType.RangeAtkPlus.ToString(), 0);
        _buys[SkillType.ShieldBlockTime] = PlayerPrefs.GetInt(SkillType.ShieldBlockTime.ToString(), 0);
        _buys[SkillType.DashCooldownMinus] = PlayerPrefs.GetInt(SkillType.DashCooldownMinus.ToString(), 0);
        _buys[SkillType.MaxHealthPlus] = PlayerPrefs.GetInt(SkillType.MaxHealthPlus.ToString(), 0);
        _buys[SkillType.SpeedPlus] = PlayerPrefs.GetInt(SkillType.SpeedPlus.ToString(), 0);

        // Recalculer les bonus à partir des achats déjà faits
        RebuildBonusesFromBuys();

        // Initialise la table de coûts
        _costs = new Dictionary<SkillType, int>
        {
            { SkillType.MeleeAtkPlus, meleeCost },
            { SkillType.RangeAtkPlus, rangeCost },
            { SkillType.ShieldBlockTime, shieldCost },
            { SkillType.DashCooldownMinus, dashCost },
            { SkillType.MaxHealthPlus, healthCost },
            { SkillType.SpeedPlus, speedCost }
        };

        // Appliquer les bonus sur le joueur (atk, hp, speed, etc.)
        ApplyAllBonusesToPlayer();
    }

    void OnDisable()
    {
        PlayerPrefs.SetInt("Skill", skillPoints);
        PlayerPrefs.Save();
    }

    // Reconstruit les bonus (meleeAtkBonus, etc.) à partir de _buys
    private void RebuildBonusesFromBuys()
    {
        meleeAtkBonus = _buys[SkillType.MeleeAtkPlus] * meleeAtkPerBuy;
        rangeAtkBonus = _buys[SkillType.RangeAtkPlus] * rangeAtkPerBuy;
        shieldBlockTimeBonus = _buys[SkillType.ShieldBlockTime] * shieldBlockTimePerBuy;
        dashCooldownReduction = _buys[SkillType.DashCooldownMinus] * dashCooldownMinusPerBuy;
        maxHealthBonus = _buys[SkillType.MaxHealthPlus] * maxHealthPerBuy;
        speedBonus = _buys[SkillType.SpeedPlus] * speedPerBuy;
    }

    // Acheter un skill
    public bool TryBuySkill(SkillType type)
    {
        if (_costs == null)
        {
            Debug.LogError("[SkillComponent] _costs n’est pas initialisé.");
            return false;
        }

        // Toujours sync depuis PlayerPrefs avant d’acheter
        skillPoints = PlayerPrefs.GetInt("Skill", skillPoints);

        int cost = _costs[type];
        if (skillPoints < cost)
        {
            Debug.LogWarning($"[SkillComponent] Pas assez de skill points ({skillPoints}) pour acheter {type} (coût {cost}).");
            return false;
        }

        // Débiter
        skillPoints -= cost;
        PlayerPrefs.SetInt("Skill", skillPoints);

        // Compter l’achat
        _buys[type]++;
        PlayerPrefs.SetInt(type.ToString(), _buys[type]);

        // Appliquer l’effet (cumulable sur les bonus)
        ApplySkillEffect(type);

        PlayerPrefs.Save();

        Debug.Log($"[SkillComponent] Achat de {type} réussi. Restant: {skillPoints} SP.");
        return true;
    }

    // Retirer un skill (si tu veux permettre de déséquiper)
    public bool RemoveSkill(SkillType type)
    {
        if (_costs == null)
        {
            Debug.LogError("[SkillComponent] _costs n’est pas initialisé.");
            return false;
        }

        if (_buys[type] <= 0)
        {
            Debug.LogWarning($"[SkillComponent] Impossible de retirer {type} car aucun achat n’a été fait.");
            return false;
        }

        int cost = _costs[type];

        // Décrémenter le nombre d’achats
        _buys[type]--;
        PlayerPrefs.SetInt(type.ToString(), _buys[type]);

        // Rembourser
        skillPoints += cost;
        PlayerPrefs.SetInt("Skill", skillPoints);

        // Retirer l’effet sur les bonus
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

        // Recalculer les stats à partir de base + bonus
        ApplyAllBonusesToPlayer();

        PlayerPrefs.Save();

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

    // Applique le bonus au modèle interne (bonus), puis recalcule les stats
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
                dashCooldownReduction += dashCooldownMinusPerBuy; // +réduction => -cooldown
                break;
            case SkillType.MaxHealthPlus:
                maxHealthBonus += maxHealthPerBuy;
                break;
            case SkillType.SpeedPlus:
                speedBonus += speedPerBuy;
                break;
        }

        ApplyAllBonusesToPlayer();
    }

    // Recalcule toutes les stats dans PlayerComponent / HealthComponent à partir des valeurs de base + bonus
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
            health.hp += maxHealthPerBuy;

            if (health.hp > health.maxHp)
            {
                health.hp = health.maxHp;
            }

            health.SetBar(health.hp / health.maxHp);
        }

        // Si tu veux que shieldBlockTimeBonus agisse sur PlayerActions:
        if (playerAction != null)
        {
            playerAction.defenceMaxCharge += 0; // TODO: si tu as une valeur de base, tu peux faire base + shieldBlockTimeBonus
        }
    }

    // Reset des buffs (ex: fin de niveau)
    public void ResetAllSkills()
    {
        PlayerPrefs.SetInt("Skill", 0);
        PlayerPrefs.SetInt(SkillType.MeleeAtkPlus.ToString(), 0);
        PlayerPrefs.SetInt(SkillType.RangeAtkPlus.ToString(), 0);
        PlayerPrefs.SetInt(SkillType.ShieldBlockTime.ToString(), 0);
        PlayerPrefs.SetInt(SkillType.DashCooldownMinus.ToString(), 0);
        PlayerPrefs.SetInt(SkillType.MaxHealthPlus.ToString(), 0);
        PlayerPrefs.SetInt(SkillType.SpeedPlus.ToString(), 0);
        PlayerPrefs.Save();

        // Remettre tout à zéro côté bonus + stats
        foreach (var key in new List<SkillType>(_buys.Keys))
            _buys[key] = 0;

        meleeAtkBonus = 0f;
        rangeAtkBonus = 0f;
        shieldBlockTimeBonus = 0f;
        dashCooldownReduction = 0f;
        maxHealthBonus = 0f;
        speedBonus = 0f;

        ApplyAllBonusesToPlayer();

        Debug.Log("[SkillComponent] Tous les skills ont été réinitialisés.");
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

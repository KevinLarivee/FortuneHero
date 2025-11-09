using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class Stat
{
    public float value = 0f;
    public float multiplier;
    public Action drawBack;

    public Stat(float multiplier, Action drawBack)
    {
        this.multiplier = multiplier;
        this.drawBack = drawBack;
    }
}

public class TrackPlayerComponent : MonoBehaviour
{
    PlayerComponent player;
    HealthComponent playerHealth;

    BossComponent boss;
    HealthComponent bossHealth;

    public float yThreshold = 15f;
    public float farThreshold = 15f;

    public float phaseTime = 100f;
    public StatusEffect statusDrawBack = StatusEffect.None;
    [HideIf(nameof(statusDrawBack), StatusEffect.None)] public GameObject statusEffectPrefab;

    float previousPlayerHealth;

    [SerializeField] GameObject nearZoneDebuff;
    [SerializeField] TriggerProjectile[] rangedProjectile;
    [SerializeField] DamageCollision[] meleeCollision;
    [SerializeField] string meleeSpeedAnimatorParameter = "meleeSpeed";
    [SerializeField] float rangeSizeIncrease = 0.5f;

    void Start()
    {
        player = PlayerComponent.Instance;
        playerHealth = player.GetComponent<HealthComponent>();
        previousPlayerHealth = playerHealth.hp;
        playerHealth.onHit += OnPlayerHit;

        boss = GetComponent<BossComponent>();
        bossHealth = boss.GetComponent<HealthComponent>();
        bossHealth.onHit += OnBossHit;

        boss.rangePrefab.transform.localScale = Vector3.one;
    }
    void Update()
    {
        if (presets["phaseElapsedTime"])
            IncreaseStat("phaseElapsedTime", -Time.deltaTime);

        //Doit gérer les scénarios inversé (2e boss)
        if (presets["playerY"] && player.transform.position.y >= yThreshold)
            IncreaseStat("playerY", Time.deltaTime);

        float sqrDistance = (player.transform.position - boss.transform.position).sqrMagnitude;
        if (presets["playerFar"] && sqrDistance > farThreshold * farThreshold)
            IncreaseStat("playerFar", Time.deltaTime);
        if (presets["playerNear"] && sqrDistance <= farThreshold * farThreshold)
            IncreaseStat("playerNear", Time.deltaTime);
    }

    #region Stats
    public Dictionary<string, Stat> stats = new();
    public void GetTopStats(int top)
    {
        if (top <= 0 || stats.Count <= top) return;

        if (presets["playerHealth"])
        {
            //Éviter les divisions par 0
            SetStat("playerHealth", 100f / (previousPlayerHealth + 1 - playerHealth.hp));
            previousPlayerHealth = playerHealth.hp;
        }

        stats = stats.OrderByDescending(s => s.Value.value * s.Value.multiplier).ToDictionary(s => s.Key, s => s.Value);
        foreach ((string key, Stat stat) in stats)
        {
            Debug.Log(key + " of value " + stat.value * stat.multiplier);
        }
        for (int i = 0; i < top; ++i)
        {
            stats.First().Value.drawBack();
            Debug.Log(stats.First().Key + " picked");
            RemoveStat(stats.First().Key);
        }
        //À réfléchir...
        ResetStats();
        SetStat("phaseElapsedTime", phaseTime);
    }

    public void IncreaseStat(string key, float value)
    {
        if (stats.ContainsKey(key))
            stats[key].value += value;
    }
    public void SetStat(string key, float value)
    {
        if (stats.ContainsKey(key))
            stats[key].value = value;
    }
    public void ResetStats()
    {
        foreach (var stat in stats)
            stat.Value.value = 0f;
    }

    public void AddStat(string key, float multiplier, Action drawBack)
    {
        if (presets.ContainsKey(key))
            presets[key] = true;
        if (!stats.TryAdd(key, new(multiplier, drawBack)))
        {
            RemoveStat(key);
            AddStat(key, multiplier, drawBack);
        }
    }
    public void RemoveStat(string key)
    {
        if (presets.ContainsKey(key))
            presets[key] = false;
        stats.Remove(key);
    }
    #endregion


    #region Presets
    void PreSetStat(string key, float multiplier, Action drawBack, bool active)
    {
        presets[key] = active;
        if (active)
            AddStat(key, multiplier, drawBack);
        else
            RemoveStat(key);
    }

    public Dictionary<string, bool> presets = new Dictionary<string, bool>
    {
        //Boss
        ["phaseElapsedTime"] = false, //*Augmenter def ou HP, bref ralonger le combat

        ["bossMeleeMiss"] = false, //Obliger update via script Boss?  Accélérer l'attaque ou augmenter la hitbox
        ["bossMeleeBlocked"] = false, //*Obliger update via script Boss? (ou player)   Empêcher le block 
        ["bossMeleeHit"] = false, //Obliger update via script ? Augmenter le dmg melee du boss

        ["bossRangeMiss"] = false, //Obliger update via script Boss?  Accélérer l'attaque ou augmenter la hitbox
        ["bossRangeBlocked"] = false, //*Obliger update via script Boss? (ou player)    Empêcher le block
        ["bossRangeHit"] = false, //Obliger update via script Boss? Augmenter le dmg range du boss

        //Player
        ["playerY"] = false, //Déclencher l'event pour les platformes/sable mouvant
        ["playerFar"] = false, //Augmenter la vitesse du boss, ou les attaques le rapprochant du joueur, ou ralentir le joueur?
        ["playerNear"] = false, //*Déclencher plus d'attaques de déplacement

        //Idée : Ultime debuff, car devrait seulement se déclencher si le joueur joue très bien.
        ["playerHealth"] = false, // Si trop court, se déclenche avec phaseElapsedTime. Si trop long, + d'attaques boss donc ne sera pas déclenché

        //["playerBlocking"] = false,
        //["playerDashing"] = false,

        ["playerMeleeDmg"] = false, //*Augmenter la defense melee du Boss
        ["playerRangeDmg"] = false  //*Augmenter la defense range du Boss
    };
    public void AllPresets()
    {
        PhaseElapsedTime(null);
        BossMeleeMiss(null);
        BossMeleeBlocked(null);
        BossMeleeHit(null);
        BossRangeMiss(null);
        BossRangeBlocked(null);
        BossRangeHit(null);
        PlayerY(null);
        PlayerFar(null);
        PlayerNear(null);
        PlayerHealth(null);
        PlayerMeleeDmg(null);
        PlayerRangeDmg(null);
    }
    public void PhaseElapsedTime(Action drawBack, bool active = true)
    {
        PreSetStat("phaseElapsedTime", 1f, drawBack ?? PhaseElapsedTimeDrawBack, active);
        SetStat("phaseElapsedTime", phaseTime);
    }

    public void BossMeleeMiss(Action drawBack, bool active = true) =>
        PreSetStat("bossMeleeMiss", 1f, drawBack ?? BossMeleeMissDrawBack, active);
    public void BossMeleeBlocked(Action drawBack, bool active = true) =>
        PreSetStat("bossMeleeBlocked", 1f, drawBack ?? BossMeleeBlockedDrawBack, active);
    public void BossMeleeHit(Action drawBack, bool active = true) =>
        PreSetStat("bossMeleeHit", 1f, drawBack ?? BossMeleeHitDrawBack, active);

    public void BossRangeMiss(Action drawBack, bool active = true) =>
        PreSetStat("bossRangeMiss", 1f, drawBack ?? BossRangeMissDrawBack, active);
    public void BossRangeBlocked(Action drawBack, bool active = true) =>
        PreSetStat("bossRangeBlocked", 1f, drawBack ?? BossRangeBlockedDrawBack, active);
    public void BossRangeHit(Action drawBack, bool active = true) =>
        PreSetStat("bossRangeHit", 1f, drawBack ?? BossRangeHitDrawBack, active);


    public void PlayerY(Action drawBack, bool active = true) =>
        PreSetStat("playerY", 0.05f, drawBack ?? PlayerYDrawBack, active);
    public void PlayerFar(Action drawBack, bool active = true) =>
        PreSetStat("playerFar", 0.05f, drawBack ?? PlayerFarDrawBack, active);
    public void PlayerNear(Action drawBack, bool active = true) =>
        PreSetStat("playerNear", 0.05f, drawBack ?? PlayerNearDrawBack, active);

    public void PlayerHealth(Action drawBack, bool active = true) =>
        PreSetStat("playerHealth", 1f, drawBack ?? PlayerHealthDrawBack, active);

    //public void PlayerBlooking(Action drawBack, bool active = true) =>
    //    PreSetStat("playerBlocking", 0.1f, drawBa ?? ck, active);
    //public void PlayerDashing(Action drawBack, bool active = true) =>
    //    PreSetStat("playerDashing", 0.5f, drawBa ?? ck, active);

    public void PlayerMeleeDmg(Action drawBack, bool active = true) =>
        PreSetStat("playerMeleeDmg", 0.2f, drawBack ?? PlayerMeleeDmgDrawBack, active); //dépendrais de maxHp du boss?
    public void PlayerRangeDmg(Action drawBack, bool active = true) =>
        PreSetStat("playerRangeDmg", 0.2f, drawBack ?? PlayerRangeDmgDrawBack, active); //dépendrais de maxHp du boss?
    #endregion

    #region Functions Utiles
    void OnPlayerHit()
    {
        //Différencier quelles attaques a frappées le joueur?
    }
    void OnBossHit()
    {
        //Différencier quelles attaques a frappées le boss?
    }
    #endregion

    #region Default DrawBacks
    //Si je fais juste appeler une fonction différente pour chaque, ne sert à rien d'avoir des defaults values
    void PhaseElapsedTimeDrawBack()
    {
        //Augmenter def ou HP, bref ralonger le combat
        boss.meleeDefense *= 1.5f;
        boss.rangeDefense *= 1.5f;
        //Retirer des stats si déclenché?
    }
    void BossMeleeMissDrawBack()
    {
        //Accélérer l'attaque ou augmenter la hitbox
        float speed = boss.animator.GetFloat(meleeSpeedAnimatorParameter);
        boss.animator.SetFloat(meleeSpeedAnimatorParameter, speed * 1.5f);
    }
    void BossMeleeBlockedDrawBack()
    {
        //Empêcher le block (Dans boss component)
        foreach (var melee in meleeCollision)
        {
            //melee.statusEffect = statusDrawBack;

        }
    }
    void BossMeleeHitDrawBack()
    {
        //Augmenter le dmg melee du boss
        boss.meleeDmg *= 2; //trop?
    }

    void BossRangeMissDrawBack()
    {
        //Accélérer l'attaque ou augmenter la hitbox
        boss.rangePrefab.transform.localScale *= (1f + rangeSizeIncrease);
    }
    void BossRangeBlockedDrawBack()
    {
        //Empêcher le block (Dans boss component)
    }
    void BossRangeHitDrawBack()
    {
        //Augmenter le dmg range du boss
        boss.rangeDmg *= 2; //trop?
    }


    void PlayerYDrawBack()
    {
        //Déclencher l'event pour les platformes/sable mouvant (Dans boss component)
    }
    void PlayerFarDrawBack()
    {
        //Augmenter la vitesse du boss, ou les attaques le rapprochant du joueur, ou ralentir le joueur?
        if (nearZoneDebuff != null)
            nearZoneDebuff.SetActive(true);
        RemoveStat("playerNear");
    }
    void PlayerNearDrawBack()
    {
        //Déclencher un knockback périodique? Ou éloigner le boss du joueur.
        boss.movementProbability *= 1.5f;
        RemoveStat("playerFar");
    }

    void PlayerHealthDrawBack()
    {
        //Idée : Ultime debuff, car devrait seulement se déclencher si le joueur joue très bien.
        // Si trop court, se déclenche avec phaseElapsedTime. Si trop long, + d'attaques boss donc ne sera pas déclenché
        //changer time.scale?
        Time.timeScale *= 1.2f;
    }

    //void PlayerBlookingDrawBack()
    //void PlayerDashingDrawBack()
    void PlayerMeleeDmgDrawBack()
    {
        //Augmenter la defense melee du Boss
        boss.meleeDefense *= 2f;
        RemoveStat("playerRangeDmg");
    }
    void PlayerRangeDmgDrawBack()
    {
        //Augmenter la defense range du Boss
        boss.rangeDefense *= 2f;
        RemoveStat("playerMeleeDmg");
    }
    #endregion
}

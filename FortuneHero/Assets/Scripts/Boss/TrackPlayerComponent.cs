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
    Dictionary<string, Stat> stats = new();
    public void GetTopStats(int top)
    {
        var temp = stats.OrderByDescending(s => s.Value.value * s.Value.multiplier).ToList();
        for (int i = 0; i < top; ++i)
        {
            temp[0].Value.drawBack();
            stats.Remove(temp[0].Key);
        }
    }

    public void IncreaseStat(string key, float value)
    {
        stats[key].value += value;
    }

    public void AddStat(string key, float multiplier, Action drawBack)
    {
        stats.TryAdd(key, new(multiplier, drawBack));
    }
    public void RemoveStat(string key)
    {
        stats.Remove(key);
    }

    void Update()
    {

    }

    //Preset de stats
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
        ["phaseElapsedTime"] = false,

        ["bossMeleeMiss"] = false,
        ["bossMeleeBlocked"] = false,

        ["bossRangeMiss"] = false,
        ["bossRangeBlocked"] = false,

        //Player
        ["playerY"] = false,
        ["playerFar"] = false,
        ["playerNear"] = false,

        ["playerHealth"] = false,

        ["playerMoving"] = false,//hmmmm
        ["playerBlocking"] = false,
        ["playerDashing"] = false,

        ["playerMeleeDmg"] = false,
        ["playerRangeDmg"] = false
    };
    public void PhaseElapsedTime(Action drawBack, bool active = true) =>
        PreSetStat("phaseElapsedTime", 0.001f, drawBack, active);

    public void BossMeleeMiss(Action drawBack, bool active = true) =>
        PreSetStat("bossMeleeMiss", 1f, drawBack, active);
    public void BossMeleeBlocked(Action drawBack, bool active = true) =>
        PreSetStat("bossMeleeBlocked", 1f, drawBack, active);

    public void BossRangeMiss(Action drawBack, bool active = true) =>
        PreSetStat("bossRangeMiss", 1f, drawBack, active);
    public void BossRangeBlocked(Action drawBack, bool active = true) =>
        PreSetStat("bossRangeBlocked", 1f, drawBack, active);


    public void PlayerY(Action drawBack, bool active = true) =>
        PreSetStat("PlayerY", 0.05f, drawBack, active);
    public void PlayerFar(Action drawBack, bool active = true) =>
        PreSetStat("playerFar", 0.05f, drawBack, active);
    public void PlayerNear(Action drawBack, bool active = true) =>
        PreSetStat("playerNear", 0.05f, drawBack, active);

    public void PlayerHealth(Action drawBack, bool active = true) =>
        PreSetStat("playerHealth", 0.1f, drawBack, active);

    public void PlayerMoving(Action drawBack, bool active = true) =>
        PreSetStat("playerMoving", 0.05f, drawBack, active); //hmmmmmm
    public void PlayerBlooking(Action drawBack, bool active = true) =>
        PreSetStat("playerBlocking", 0.1f, drawBack, active);
    public void PlayerDashing(Action drawBack, bool active = true) =>
        PreSetStat("playerDashing", 0.5f, drawBack, active);

    public void PlayerMeleeDmg(Action drawBack, bool active = true) =>
        PreSetStat("playerMeleeDmg", 0.2f, drawBack, active); //dépendrais de maxHp du boss?
    public void PlayerRangeDmg(Action drawBack, bool active = true) =>
        PreSetStat("playerRangeDmg", 0.2f, drawBack, active); //dépendrais de maxHp du boss?
}

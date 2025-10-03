using System;
using System.Collections.Generic;
using System.Linq;
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
        for(int i = 0; i < top; ++i)
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
        stats.Add(key, new(multiplier, drawBack));
    }

    void Update()
    {
        
    }

    //Preset de stats
    bool playerHeight = false;
    bool playerDefenseUp = false;
    public bool TrackPlayerHeight(Action drawBack)
    {
        playerHeight = stats.TryAdd("playerHeight", new(0.05f, drawBack));
        if (!playerHeight)
            stats.Remove("playerHeight");
        return playerHeight;
    }
    public bool TrackPlayerDefenseUp(Action drawBack)
    {
        playerDefenseUp = stats.TryAdd("playerHeight", new(0.1f, drawBack));
        if (!playerDefenseUp)
            stats.Remove("playerHeight");
        return playerDefenseUp;
    }

}

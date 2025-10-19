using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[Serializable]
public class PowerUp
{
    public PowerUpTypes Type;
    public UnityEvent Action;
    public RawImage Image;
    public GameObject Prefab;
    public GameObject ShopPrefab;
    public int Price;

}

public enum PowerUpTypes { Melee, Distance, Defence }

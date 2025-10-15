using UnityEngine;

public class ShieldCollision : MonoBehaviour, IBlockable
{
    public int DefenceDamageDivider { get; } = 2;
}

interface IBlockable
{
    public int DefenceDamageDivider { get; }
}

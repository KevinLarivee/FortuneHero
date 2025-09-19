using UnityEngine;
using UnityEngine.Events;
public enum StatusEffect { None, Burn, Freeze, Paralyze, Knockback }

public class HealthComponent : MonoBehaviour
{
    public UnityAction hit;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Hit(int dmg, StatusEffect status = StatusEffect.None)
    {
        //hit(dmg, status);
    }
}

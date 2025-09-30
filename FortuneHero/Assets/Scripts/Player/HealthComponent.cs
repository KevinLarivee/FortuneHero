using System;
using UnityEngine;
using UnityEngine.Events;

public enum StatusEffect { None, Burn, Freeze, Paralyze, Knockback }

//Code a revoir (chat)
public class HealthComponent : MonoBehaviour
{
    [SerializeField] UnityEngine.UI.Image bar;

    [SerializeField] float maxHp = 100;
    public float hp = 10;
    public bool alive = true;
    public Action OnDeath;
    public Action OnHit;
    // Événement optionnel pour écouter les coups ailleurs (UI, sons, etc.)
    //public UnityAction<int, StatusEffect> OnHit;

    // Exemple de stockage de PV si tu veux l’activer plus tard:
    // [SerializeField] private int maxHp = 10;
    // private int currentHp;

    // void Start() { currentHp = maxHp; }

    //public void Hit(int dmg, StatusEffect status = StatusEffect.None)
    //{
    //    // Pour l’instant: LOG uniquement.
    //    Debug.Log($"[HealthComponent] {name} a reçu {dmg} dégâts. Effet: {status}");
    //    hp -= dmg;
    //    // Si tu veux émettre un event:
    //    //OnHit?.Invoke(dmg, status);

    //    // Si tu veux gérer des PV plus tard:
    //    // currentHp = Mathf.Max(0, currentHp - dmg);
    //    // if (currentHp == 0) { Debug.Log($"{name} est KO."); }
    //}


    // Start is called once before the first execution of Update after the MonoBehaviour is created

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        hp = maxHp;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void SetBar(float fraction)
    {
        bar.fillAmount = fraction;
    }
    public void Hit(float dmg)
    {
        hp -= dmg;
        //OnHit();
        //SetBar(hp / maxHp);
        alive = hp > 0;
        if (!alive)
            OnDeath();
    }
    public void ResetHealth()
    {
        hp = maxHp;
    }
}


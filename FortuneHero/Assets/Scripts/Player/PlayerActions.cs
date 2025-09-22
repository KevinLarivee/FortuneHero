using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActions : MonoBehaviour
{
    Animator animator;

    [SerializeField] int meleeAtkDmg = 10; //Dmg = damage
    [SerializeField] int rangedAtkDmg = 20;
    [SerializeField] Collider weaponCollider;
    [SerializeField] GameObject exitPoint;
    [SerializeField] GameObject projectilePrefab;


    float defenceChargeTime = 10f;
    float defenceChargeIncrement = 1f;
    float defenceConsumption = 1f; //Vitesse a laquelle le joeur perds de l'energie en bloquant

    [SerializeField] float rangedAtkCd = 1.5f;
    [SerializeField] float rangedAtkTimer = 0f;
    bool canRangedAtk = false;
    bool canMeleeAtk = false;

    [SerializeField] float meleeAtkCd = 0.5f;
    [SerializeField] float meleeAtkTimer = 0f;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        if(!canMeleeAtk)
        {
            meleeAtkTimer -= Time.deltaTime;
            if(meleeAtkTimer <= 0f)
                canMeleeAtk = true;
        }
       if(!canRangedAtk)
       {
            rangedAtkTimer -= Time.deltaTime;
            if (rangedAtkTimer <= 0f)
                canRangedAtk = true;
       }
            
    }

    public void MeleeAttack(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && canMeleeAtk)
        {
            animator.SetTrigger("MeleeAttack");
            meleeAtkTimer = meleeAtkCd;
            canMeleeAtk = false;
        }
    }
    public void RangedAttack(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && canRangedAtk)
        {
            animator.SetTrigger("RangedAttack");
            rangedAtkTimer = rangedAtkCd;
            canRangedAtk = false;
        }
    }
    public void ShootProjectile()
    {
        Instantiate(projectilePrefab, exitPoint.transform.position, gameObject.transform.rotation);
    }
    public void EnableWeaponCollider()
    {
        weaponCollider.enabled = true;
    }
    public void DisableWeaponCollider()
    {
        weaponCollider.enabled = false;
    }
}


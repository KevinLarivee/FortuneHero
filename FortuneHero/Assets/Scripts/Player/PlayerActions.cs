using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActions : MonoBehaviour
{
    Animator animator;

    [SerializeField] int meleeAtkDmg = 10;
    [SerializeField] int rangedAtkDmg = 20;
    [SerializeField] Collider weaponCollider;
    [SerializeField] GameObject exitPoint;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] GameObject shield;
    PlayerMovement player;

    [SerializeField] float defenceCurrentCharge = 0f;
    bool showShield = false;
    float defenceMaxCharge = 10f;
    bool canDefend = false;

    [SerializeField] float rangedAtkCd = 1.5f;
    [SerializeField] float rangedAtkTimer = 0f;
    bool canRangedAtk = false;
    bool canMeleeAtk = false;

    [SerializeField] float meleeAtkCd = 0.5f;
    [SerializeField] float meleeAtkTimer = 0f;

    void Start()
    {
        animator = GetComponent<Animator>();
        defenceCurrentCharge = defenceMaxCharge;
    }

    void Update()
    {
        if (!canMeleeAtk)
        {
            meleeAtkTimer -= Time.deltaTime;
            if (meleeAtkTimer <= 0f)
                canMeleeAtk = true;
        }
        if (!canRangedAtk)
        {
            rangedAtkTimer -= Time.deltaTime;
            if (rangedAtkTimer <= 0f)
                canRangedAtk = true;
        }

        if (defenceCurrentCharge <= 0)
            canDefend = false;
        else
            canDefend = true;

        if (showShield)
        {
            Mathf.Max(defenceCurrentCharge -= Time.deltaTime, 0);
        }
        else
        {
            if (defenceCurrentCharge < defenceMaxCharge)
            {
                Mathf.Min(defenceCurrentCharge += Time.deltaTime, defenceMaxCharge);
            }
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
    public void Defend(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && canDefend)
        {
            animator.SetBool("isDefending", true);
            ShowShield(true);
            PlayerMovement.Instance.SlowPlayer(2);
        }
        else if (ctx.canceled && canDefend)
        {
            animator.SetBool("isDefending", false);
            ShowShield(false);
            PlayerMovement.Instance.SpeedUpPlayer(2);
        }
    }
    public void ShootProjectile()
    {
        Instantiate(projectilePrefab, exitPoint.transform.position, gameObject.transform.rotation);
    }
    public void ShowShield(bool show)
    {
        showShield = show;
        shield.SetActive(show);
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


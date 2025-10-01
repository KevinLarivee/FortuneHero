using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerActions : MonoBehaviour
{
    Animator animator;

    [SerializeField] Collider weaponCollider;
    [SerializeField] GameObject exitPoint;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] GameObject shield;
    [SerializeField] Camera aimCamera;

    [SerializeField] float defenceCurrentCharge = 0f;
    bool showShield = false;
    bool canDefend = false;
    float defenceMaxCharge = 10f;
    float defenceSpeedMultiplier = 2f;

    public int rangedAtkDmg = 20;
    [SerializeField] float rangedAtkCd = 1.5f;
    [SerializeField] float rangedAtkTimer = 0f;
    bool canRangedAtk = false;
    
    public int meleeAtkDmg = 10;
    [SerializeField] float meleeAtkCd = 0.5f;
    [SerializeField] float meleeAtkTimer = 0f;
    bool canMeleeAtk = false;

    PlayerOverlayComponent overlay;

    public bool isPaused = false;

    void Start()
    {
        animator = GetComponent<Animator>();
        defenceCurrentCharge = defenceMaxCharge;
        overlay = GetComponent<PlayerOverlayComponent>();
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
        {
            canDefend = false;
            ShowShield(false);
        }
        else
            canDefend = true;

        if (showShield)
        {
            defenceCurrentCharge = Mathf.Max(defenceCurrentCharge - Time.deltaTime, 0);
            overlay.UseShield(defenceCurrentCharge);
        }
        else if (defenceCurrentCharge < defenceMaxCharge)
        {
            defenceCurrentCharge = Mathf.Min(defenceCurrentCharge + Time.deltaTime, defenceMaxCharge);
            overlay.UseShield(defenceCurrentCharge);
        }
    }

    public void MeleeAttack(InputAction.CallbackContext ctx)
    {
        if(isPaused) return;

        if (ctx.performed && canMeleeAtk)
        {
            animator.SetTrigger("MeleeAttack");
            meleeAtkTimer = meleeAtkCd;
            canMeleeAtk = false;
        }
    }
    public void RangedAttack(InputAction.CallbackContext ctx)
    {
        if (isPaused) return;

        if (ctx.performed && canRangedAtk)
        {
            animator.SetTrigger("RangedAttack");
            rangedAtkTimer = rangedAtkCd;
            canRangedAtk = false;
        }
    }
    public void Defend(InputAction.CallbackContext ctx)
    {
        //if(isPaused) return;

        if (isPaused || (ctx.canceled && showShield))
        {
            ShowShield(false);
        }

        else if (ctx.performed && canDefend)
        {
            ShowShield(true);
        }
    }
    public void ShootProjectile()
    {
        Vector3 screenCenter =  new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
        Instantiate(projectilePrefab, exitPoint.transform.position, PlayerMovement.Instance.isAiming ? Quaternion.LookRotation(aimCamera.ScreenPointToRay(screenCenter).direction) : transform.rotation);
    }
    public void ShowShield(bool show)
    {
        showShield = show;
        if (showShield)
        {
            animator.SetBool("isDefending", true);
            PlayerMovement.Instance.SlowPlayer(defenceSpeedMultiplier);
            Debug.Log("Slow");
        }
        else
        {
            Debug.Log("SpeedUp");
            animator.SetBool("isDefending", false);
            PlayerMovement.Instance.SpeedUpPlayer(defenceSpeedMultiplier);
        }

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


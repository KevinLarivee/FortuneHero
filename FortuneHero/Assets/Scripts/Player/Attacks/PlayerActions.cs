using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

public enum ProjectileType //Potentiel d'en ajouter
{
    Default,
    IceBall
}

public class PlayerActions : MonoBehaviour
{

    static PlayerActions instance;
    public static PlayerActions Instance { get { return instance; } }

    public List<PowerUp> powerUps;
    


    [SerializeField] Collider weaponCollider;
    [SerializeField] GameObject exitPoint;
    [SerializeField] GameObject shield;
    [SerializeField] Camera aimCamera;

    [SerializeField] GameObject defaultProjectilePrefab;
    [SerializeField] GameObject iceBallPrefab;
    [SerializeField] GameObject aoeParaPrefab;
    [SerializeField] GameObject invShieldPrefab; //inv --> invincibility


    [Header("Shield")]
    [SerializeField] float defenceCurrentCharge = 0f;
    bool showShield = false;
    bool canDefend = false;
    float defenceMaxCharge = 10f;
    float speedWhileShielding = 2f;

    [Header("RangedAtk")]
    [SerializeField] float rangedAtkCd = 1.5f;
    [SerializeField] float rangedAtkTimer = 0f;
    bool canRangedAtk = false;

    [Header("MeleeAtk")]
    [SerializeField] float meleeAtkCd = 0.5f;
    [SerializeField] float meleeAtkTimer = 0f;
    bool canMeleeAtk = false;

    [Header("PowerUps")]
    public float slowDuration = 5f;
    public float speedDrop = 2f;
    [SerializeField] LayerMask layersAffectedByAoe;
    [SerializeField] float aoeDuration = 4f;
    [SerializeField] float paralyzeDuration = 2f;
    [SerializeField] float invShieldDuration = 4f;
    bool isIceBall = false;
    float aoeTimer;



    PlayerOverlayComponent overlay;
    Animator animator;
    HealthComponent health;

    public ProjectileType currentType;
    public bool isPaused = false;

    void Awake()
    {
        instance = this;
        defenceCurrentCharge = defenceMaxCharge;

        animator = GetComponent<Animator>();
        overlay = GetComponent<PlayerOverlayComponent>();
        health = GetComponent<HealthComponent>();
    }

    void Update()
    {
        if (aoeTimer > 0)
            aoeTimer -= Time.deltaTime;

        if (isIceBall) //Issue de quand tu active, tant que la ice ball a pas toucher l'ennemi, isIceBall est pas mis a false, donc si elle vole longtemps, tu pourrais retirer une iceBall
            currentType = ProjectileType.IceBall;
        else
            currentType = ProjectileType.Default;

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
        if (isPaused) return;

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
        if (PlayerComponent.Instance.bossDisableShield) return;

        if (isPaused || (ctx.canceled && showShield))
        {
            ShowShield(false);
        }

        else if (ctx.performed && canDefend)
        {
            ShowShield(true);
        }
    }
    public void StartAoeParalyze()
    {
        StartCoroutine(AoeParalyze());
    }
    private IEnumerator AoeParalyze()
    {
        Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z);
        var obj = Instantiate(aoeParaPrefab, spawnPos, Quaternion.identity);
        aoeTimer = aoeDuration;

        while (aoeTimer > 0)
        {
            CheckForEnemies(spawnPos);
            yield return new WaitForSeconds(0.3f);
        }

        Destroy(obj);
    }

    private void CheckForEnemies(Vector3 spawnPos)
    {
        Collider[] colliders = Physics.OverlapSphere(new Vector3(spawnPos.x, spawnPos.y + 1.6f, spawnPos.z), 4, layersAffectedByAoe);
        foreach (Collider collider in colliders)
            collider.transform.root.GetComponent<EnemyComponent>().ToggleParalyze(paralyzeDuration);
    }

    public void SetToIceBall(bool trueOrFalse)
    {
        isIceBall = trueOrFalse;
    }
    public void ShootProjectile()
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
        GameObject prefab;
        switch (currentType)
        {
            case ProjectileType.IceBall:
                prefab = iceBallPrefab;
                break;
            default:
                prefab = defaultProjectilePrefab;
                break;
        }

        RaycastHit hit;
        Quaternion rotation;
        if (Physics.Raycast(aimCamera.transform.position + aimCamera.transform.forward * .5f, aimCamera.transform.forward, out hit, 500))
            rotation = Quaternion.LookRotation(hit.point - exitPoint.transform.position, Vector3.up);
        else
            rotation = Quaternion.LookRotation((aimCamera.transform.position + aimCamera.transform.forward * 500) - exitPoint.transform.position, Vector3.up);

        Instantiate(prefab, exitPoint.transform.position, PlayerMovement.Instance.isAiming ? rotation : transform.rotation);
    }
    public void UseInvShield()
    {
        StartCoroutine(InvShield());
    }
    private IEnumerator InvShield()
    {
        health.isInvincible = true;
        var obj = Instantiate(invShieldPrefab, transform.position, Quaternion.identity);
        obj.transform.parent = transform;

        yield return new WaitForSeconds(invShieldDuration);
        health.isInvincible = false;
        Destroy(obj);
    }

    public void ShowShield(bool show)
    {
        showShield = show;
        if (showShield)
        {
            animator.SetBool("isDefending", true);
            PlayerMovement.Instance.SlowPlayer(speedWhileShielding);
        }
        else
        {
            animator.SetBool("isDefending", false);
            PlayerMovement.Instance.SpeedUpPlayer(speedWhileShielding);
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


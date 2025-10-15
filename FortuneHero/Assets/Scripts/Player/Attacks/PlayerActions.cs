using System.Collections;
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

    [SerializeField] Collider weaponCollider;
    [SerializeField] GameObject exitPoint;
    [SerializeField] GameObject shield;
    [SerializeField] Camera aimCamera;

    [SerializeField] GameObject defaultProjectilePrefab;
    [SerializeField] GameObject iceBallPrefab;
    [SerializeField] GameObject aoeParaPrefab;


    [Header("Shield")]
    [SerializeField] float defenceCurrentCharge = 0f;
    bool showShield = false;
    bool canDefend = false;
    float defenceMaxCharge = 10f;
    float speedWhileShielding = 2f;

    [Header("RangedAtk")]
    //public int rangedAtkDmg = 20;
    [SerializeField] float rangedAtkCd = 1.5f;
    [SerializeField] float rangedAtkTimer = 0f;
    bool canRangedAtk = false;

    [Header("MeleeAtk")]
    //public int meleeAtkDmg = 10;
    [SerializeField] float meleeAtkCd = 0.5f;
    [SerializeField] float meleeAtkTimer = 0f;
    bool canMeleeAtk = false;

    [Header("PowerUps")]
    public float slowDuration = 5f;
    public float speedDrop = 2f;
    public bool isIceBall = false; 
    public bool AoeParaActivated = false;
    [SerializeField] float aoeRadius = 4f;
    [SerializeField] float aoeOffset = 2f;
    [SerializeField] float aoeDuration = 4f;
    [SerializeField] LayerMask layersAffectedByAoe;



    PlayerOverlayComponent overlay;
    Animator animator;

    public ProjectileType currentType;
    public bool isPaused = false;

    void Awake()
    {
        instance = this;
        animator = GetComponent<Animator>();
        defenceCurrentCharge = defenceMaxCharge;
        overlay = GetComponent<PlayerOverlayComponent>();
    }

    void Update()
    {
        if (AoeParaActivated)
            StartCoroutine(AoeParalyse());

        if(isIceBall)
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
    public IEnumerator AoeParalyse()
    {
        AoeParaActivated = false;
        Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z);
        var obj = Instantiate(aoeParaPrefab, spawnPos, Quaternion.identity);

        Collider[] colliders = Physics.OverlapSphere(new Vector3(spawnPos.x, spawnPos.y + 1.6f, spawnPos.z), 4, layersAffectedByAoe);
        foreach (Collider collider in colliders)
        {
            if (collider.CompareTag("Enemy"))//Temp pcq melee a des colliders sur les mains et sa fuck tt et j'ai besoin quil soit sur enemy layer pour les dmgs
            {
                Debug.Log($"Hit: {collider.name} | Root: {collider.transform.root.name}");
                collider.gameObject.GetComponent<EnemyComponent>().ToggleParalyze(aoeDuration);
            }
        }

        yield return new WaitForSeconds(aoeDuration);
        Destroy(obj);
    }
    private void OnDrawGizmosSelected()
    {
        Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y + 1.7f, transform.position.z);

        // translucent fill
        Gizmos.color = new Color(0.2f, 0.6f, 1f, 0.15f);
        Gizmos.DrawSphere(spawnPos, aoeRadius);

        // outline
        Gizmos.color = new Color(0.2f, 0.6f, 1f, 0.9f);
        Gizmos.DrawWireSphere(spawnPos, aoeRadius);
    }
    public void ShootProjectile()
    {
        Vector3 screenCenter =  new Vector3(Screen.width / 2f, Screen.height / 2f, 0);
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
    public void ShowShield(bool show)
    {
        showShield = show;
        if (showShield)
        {
            animator.SetBool("isDefending", true);
            PlayerMovement.Instance.SlowPlayer(speedWhileShielding);
            Debug.Log("Slow");
        }
        else
        {
            Debug.Log("SpeedUp");
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


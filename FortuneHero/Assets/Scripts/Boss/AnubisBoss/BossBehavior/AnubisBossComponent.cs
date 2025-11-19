using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class AnubisBossComponent : BossComponent
{
    [Header("Anubis Boss Specific")]
    [SerializeField] Collider weaponCollider;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] GameObject dashEffectPrefab;
    [SerializeField] float envAtkSpeed = 15f;
    [SerializeField] Transform pyramidTop;
    [SerializeField] Transform exitPoint;
    //[SerializeField] GameObject platforms;
    [SerializeField] List<ParticleSystem> platforms;
    [SerializeField] float yThreshold = 10f;
    [SerializeField] float yDefaultValue = 30f;
    [SerializeField] GameObject ground;
    [SerializeField] Material quickSand;
    [SerializeField] string tpMelee;
    [SerializeField] float dashDistance = 5f;
    [SerializeField] float rotationSpeed = 150f;

    ParticleSystem[] envPlatforms;
    Transform target;
    DissolveComponent dissolve;
    AnubisBoss_BT bt;
    GameObject projectile;

    public int dmg = 10;
    int tpCount = 0;
    float bufferCd = 0.2f;
    float bufferTimer = 0;
    bool isStartingDash = false;

    void Start()
    {
        trackPlayer.AllPresets();

        trackPlayer.yThreshold = yThreshold;

        //Pour reverse
        trackPlayer.PlayerY(QuickSand, -0.05f);
        trackPlayer.SetStat("playerY", yDefaultValue);

        healthComponent = GetComponent<HealthComponent>();
        dissolve = GetComponent<DissolveComponent>();
        healthComponent.onDeath += Death;
        bufferTimer = bufferCd;
        target = PlayerComponent.Instance.transform;

        //List<ParticleSystem> temp = new List<ParticleSystem>();
        //foreach (Transform t in platforms.transform)
        //{
        //    temp.Add(t.GetComponent<ParticleSystem>());
        //}
        //envPlatforms = temp.ToArray();

        bt = GetComponent<AnubisBoss_BT>();
    }
    void Update()
    {
        if (bufferTimer < bufferCd)
        {
            bufferTimer += Time.deltaTime;
        }
    }
    protected override void Hit()
    {
        bool willChangePhase = currentPhase < phases.Length && healthComponent.hp / healthComponent.maxHp <= phases[currentPhase];
        base.Hit();
        if (willChangePhase)
        {
            trackPlayer.SetStat("playerY", yDefaultValue);
        }
    }
    public void StartEnvironnementAttack()
    {
        StartCoroutine(EnvironmentAttack());
    }
    public IEnumerator EnvironmentAttack()
    {
        Debug.Log("In Coroutine");
        GameObject envAtk = Instantiate(rangePrefab, exitPoint.position, Quaternion.identity);
        //PlayParticles();
        //Changer pour le "mini Boss"
        //yield return new WaitUntil(() => );
        //Temps de chargement de la boule
        yield return new WaitForSeconds(2f);
        while (Vector3.Distance(envAtk.transform.position, pyramidTop.position) > 1f)
        {
            //Pas sur
            //if (envAtk == null)
            //    yield break;
            envAtk.transform.position = Vector3.MoveTowards(envAtk.transform.position, pyramidTop.position, envAtkSpeed * Time.deltaTime);
            yield return null;
        }
        if (envAtk != null)
        {
            PlayParticles();
            envAtk.AddComponent<HealthComponent>().onDeath += () => StopParticles(envAtk);
        }
        //StopParticles();
        //envAtk.isActivated = false;
    }

    private void PlayParticles()
    {
        foreach (ParticleSystem p in /*envPlatforms*/ platforms)
        {
            p.gameObject.SetActive(true);
            p.Play();
        }
    }
    private void StopParticles(GameObject envAtk)
    {
        foreach (ParticleSystem p in /*envPlatforms*/ platforms)
        {
            p.gameObject.SetActive(false);
            p.Stop();
        }
        Destroy(envAtk);
    }

    public void InstantiateProjectile()
    {
        projectile = Instantiate(projectilePrefab, exitPoint.position, Quaternion.Euler(90, transform.rotation.y, transform.rotation.z));
        projectile.GetComponent<BossProjectileMovement>().onTrigger.AddListener(RangedExplosion);
        projectile.transform.parent = weaponCollider.gameObject.transform;
    }
    public void RangedAttack()
    {
        projectile.transform.parent = null;
        //projectile.transform.rotation = Quaternion.LookRotation(target.position - exitPoint.position);
        projectile.GetComponent<BossProjectileMovement>().launchProjectile = true;
    }
    public void RangedExplosion(CSquareEvent colliders)
    {
        if (colliders.other.CompareTag("Shield"))
        {
            trackPlayer.IncreaseStat("bossRangeMiss", -1);
            trackPlayer.IncreaseStat("bossRangeBlocked", 1);
            return;
        }
        else if (colliders.other.CompareTag("Player"))
        {
            trackPlayer.IncreaseStat("bossRangeMiss", -1);
            trackPlayer.IncreaseStat("bossRangeHit", 1);
            colliders.other.gameObject.GetComponent<HealthComponent>().Hit(rangeDmg);
        }
        //Surement plus de code ici
        Instantiate(explosionPrefab, colliders.self.transform.position, Quaternion.identity);
        //Rajouter des layers?
        if (Physics.OverlapSphere(colliders.self.transform.position, 3f, LayerMask.GetMask("Player")).Any(c => c.CompareTag("Player")))
        {
            PlayerComponent.Instance.GetComponent<HealthComponent>().Hit(rangeDmg);
        }
    }
    public void EnableWeaponCollider()
    {
        weaponCollider.enabled = true;
    }
    public void DisableWeaponCollider()
    {
        weaponCollider.enabled = false;
    }
    public void StartDissolve()
    {
        if (bt.activeNode is Behaviour_Composite && (bt.activeNode as Behaviour_Composite).compositeInstanceID == tpMelee)
            tpCount = (tpCount + 1) % 4;
        if (tpCount != 3)
            StartCoroutine(dissolve.Dissolve());
        else //Ne va rien faire au pire
            ReverseDissolve();
    }
    public void ReverseDissolve()
    {
        StartCoroutine(dissolve.Dissolve(true));
    }

    public void StartDash()
    {
        StartCoroutine(Dash());
    }
    public void StartRotation()
    {
        StartCoroutine(RotateBoss());
        //RotateBoss2();
    }
    public IEnumerator RotateBoss()
    {
        agent.isStopped = true;
        while (!isStartingDash)
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.LookRotation(target.transform.position - transform.position, Vector3.up), rotationSpeed * Time.deltaTime);
            dashEffectPrefab.SetActive(true);
            yield return null;
        }
    }
    public void RotateBoss2()
    {
        transform.rotation = Quaternion.LookRotation(target.transform.position - transform.position, Vector3.up);
        dashEffectPrefab.SetActive(true);
    }
    public IEnumerator Dash()
    {
        isStartingDash = true;
        Vector3 startPos = transform.position;
        Vector3 targetPos = startPos + transform.forward * dashDistance;

        float animTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
        while (animTime < 0.95f)
        {
            animTime = animator.GetCurrentAnimatorStateInfo(0).normalizedTime;
            transform.position = Vector3.Lerp(startPos, targetPos, animTime);
            yield return null;
        }
        agent.isStopped = false;
        dashEffectPrefab.SetActive(false);
        isStartingDash = false;

    }

    public void QuickSand()
    {
        ground.GetComponent<MeshRenderer>().material = quickSand;
        ground.AddComponent<QuickSandComponent>();
    }

    void OnTriggerEnter(Collider other)
    {
        if (bufferTimer >= bufferCd && other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<HealthComponent>().Hit(dmg);
            bufferTimer = 0;
        }
    }
}

using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class AnubisBossComponent : BossComponent
{
    [Header("Anubis Boss Specific")]
    [SerializeField] AudioClip rangeChargeSFX;
    [SerializeField] AudioClip startTpSFX;
    [SerializeField] AudioClip endTpSFX;
    [SerializeField] AudioClip dashSFX;
    [SerializeField] AudioClip swingSFX;



    [SerializeField] Collider weaponCollider;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] GameObject dashEffectPrefab;
    [SerializeField] float envAtkSpeed = 15f;
    [SerializeField] Transform pyramidTop;
    [SerializeField] Transform exitPoint;
    [SerializeField] GameObject platforms;
    [SerializeField] float yDefaultValue = 30f;
    [SerializeField] MeshRenderer[] ground;
    [SerializeField] float dashDistance = 5f;
    [SerializeField] float rotationSpeed = 150f;
    [SerializeField] float paralyseTime = 0.5f;

    ParticleSystem[] envPlatforms;
    Transform target;
    DissolveComponent dissolve;
    GameObject projectile;
    GameObject envAtk;

    PlayerMovement playerM;

    public int dmg = 10;
    float bufferTimer = 0;
    bool isStartingDash = false;

    bool rangeMiss = false;
    bool meleeAlreadyHit = false;

    void Start()
    {
        trackPlayer.AllPresets();
        trackPlayer.PlayerNear(null, 0.25f);
        trackPlayer.PlayerFar(null, 0.5f);
        trackPlayer.BossMeleeMiss(null, 0.125f);
        trackPlayer.BossMeleeBlocked(null, 1f);
        trackPlayer.BossMeleeHit(null, 0.75f);
        trackPlayer.BossRangeMiss(null, 0.5f);
        trackPlayer.BossRangeBlocked(null, 1f);
        trackPlayer.BossRangeHit(null, 0.75f);
        trackPlayer.PlayerY(() => StartCoroutine(QuickSand()), -0.05f);//Pour reverse
        trackPlayer.SetStat("playerY", -yDefaultValue);
        trackPlayer.BossRangeMiss(() => rangeMiss = true);

        healthComponent = GetComponent<HealthComponent>();
        dissolve = GetComponent<DissolveComponent>();
        healthComponent.onDeath += Death;
        bufferTimer = 0;
        target = PlayerComponent.Instance.transform;

        List<ParticleSystem> temp = new List<ParticleSystem>();
        foreach (Transform t in platforms.transform)
        {
            t.GetChild(0).gameObject.SetActive(true);
            temp.Add(t.GetComponentInChildren<ParticleSystem>());
            t.GetChild(0).gameObject.SetActive(false);
        }
        envPlatforms = temp.ToArray();

        playerM = PlayerMovement.Instance;
    }
    protected override void Hit()
    {
        bool willChangePhase = currentPhase < phases.Length && healthComponent.hp / healthComponent.maxHp <= phases[currentPhase];
        base.Hit();
        if (willChangePhase)
        {
            trackPlayer.SetStat("playerY", -yDefaultValue);
        }
    }
    public void StartEnvironnementAttack()
    {
        StartCoroutine(EnvironmentAttack());
    }
    public IEnumerator EnvironmentAttack()
    {
        if(envAtk != null)
            Destroy(envAtk);

        envAtk = Instantiate(rangePrefab, exitPoint.position, Quaternion.identity);
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
            HealthComponent temp = envAtk.AddComponent<HealthComponent>();
            temp.onDeath += StopParticles;
            temp.hp = 30;
            Destroy(envAtk.GetComponent<BossProjectileMovement>());
        }
        //StopParticles();
        //envAtk.isActivated = false;
    }

    private void PlayParticles()
    {
        foreach (ParticleSystem p in envPlatforms)
        {
            p.gameObject.SetActive(true);
            p.Play();
        }
    }
    public void StopParticles()
    {
        Debug.Log("Stoped");
        foreach (ParticleSystem p in envPlatforms)
        {
            p.Stop();
            p.gameObject.SetActive(false);
        }
        Destroy(envAtk);
        envAtk = null;
    }

    public void InstantiateProjectile()
    {
        SFXManager.Instance.PlaySFX(rangeChargeSFX, transform, PlayerComponent.Instance.SFXGroup);
        projectile = Instantiate(projectilePrefab, exitPoint.position, Quaternion.Euler(90, transform.rotation.y, transform.rotation.z));
        projectile.GetComponent<BossProjectileMovement>().onTrigger.AddListener(RangedExplosion);
        projectile.transform.parent = weaponCollider.gameObject.transform;
    }
    public void RangedAttack()
    {
        //Crash parfois...
        projectile.transform.parent = null;
        //projectile.transform.rotation = Quaternion.LookRotation(target.position - exitPoint.position);
        projectile.GetComponent<BossProjectileMovement>().launchProjectile = true;
        trackPlayer.IncreaseStat("bossRangeMiss", 1);

    }
    public void RangedExplosion(CSquareEvent colliders)
    {
        bool isHit = false;
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
            isHit = true;
        }
        if (rangeMiss)
        {
            Instantiate(explosionPrefab, colliders.self.transform.position, Quaternion.identity);
            //Rajouter des layers?
            if (Physics.OverlapSphere(colliders.self.transform.position, 3f, LayerMask.GetMask("Player")).Any(c => c.CompareTag("Player")))
            {
                isHit = true;
                colliders.other.gameObject.GetComponent<HealthComponent>().Hit(1);
            }
        }
        if(isHit && rangeStatus)
            playerM.ToggleParalyse(paralyseTime);
    }
    public void EnableWeaponCollider()
    {
        SFXManager.Instance.PlaySFX(swingSFX, transform, PlayerComponent.Instance.SFXGroup);
        weaponCollider.enabled = true;
        trackPlayer.IncreaseStat("bossMeleeMiss", 1);
        meleeAlreadyHit = false;
    }
    public void DisableWeaponCollider()
    {
        weaponCollider.enabled = false;
    }
    public void StartDissolve()
    {
        if (TpMeleeAttack_Action.possibleTpPos == null || TpMeleeAttack_Action.possibleTpPos.Count != 0)
        {
            SFXManager.Instance.PlaySFX(startTpSFX, transform, PlayerComponent.Instance.SFXGroup);
            StartCoroutine(dissolve.Dissolve());
        }
        else //Ne va rien faire au pire
            ReverseDissolve();
    }
    public void ReverseDissolve()
    {
        SFXManager.Instance.PlaySFX(endTpSFX, transform, PlayerComponent.Instance.SFXGroup);
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
        SFXManager.Instance.PlaySFX(dashSFX, transform, PlayerComponent.Instance.SFXGroup_Louder);
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

    public override void ChangeMovementProbability(float probability, string buff)
    {
        if (buff == "near")
            GetComponent<AnubisBoss_BT>().random_TpRandom.probability *= probability;
        else
            GetComponent<AnubisBoss_BT>().random_TpAttack.probability *= probability;
    }

    public IEnumerator QuickSand()
    {
        while (isActiveAndEnabled)
        {
            foreach (MeshRenderer m in ground)
            {
                m.material.SetVector("_Direction", new Vector4(5, 0));
                m.GetComponent<QuickSandComponent>().enabled = true;
            }
            yield return new WaitForSeconds(Random.Range(1f, 5f));
            foreach (MeshRenderer m in ground)
            {
                m.material.SetVector("_Direction", Vector4.zero);
                m.GetComponent<QuickSandComponent>().enabled = false;
            }
            yield return new WaitForSeconds(Random.Range(5f, 10f));
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.excludeLayers == LayerMask.GetMask("IgnoreTrigger") || meleeAlreadyHit)
            return;
        if (other.CompareTag("Shield"))
        {
            trackPlayer.IncreaseStat("bossMeleeBlocked", 1);
            trackPlayer.IncreaseStat("bossMeleeMiss", -1);
            meleeAlreadyHit = true;
        }
        else if (other.CompareTag("Player"))
        {
            other.gameObject.GetComponent<HealthComponent>().Hit(meleeDmg);
            trackPlayer.IncreaseStat("bossMeleeHit", 1);
            trackPlayer.IncreaseStat("bossMeleeMiss", -1);
            meleeAlreadyHit = true;
        }
        if (meleeStatus)
        {
            playerM.ToggleParalyse(paralyseTime);
        }
    }
}

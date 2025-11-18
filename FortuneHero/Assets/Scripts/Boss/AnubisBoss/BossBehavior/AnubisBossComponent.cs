using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnubisBossComponent : BossComponent
{
    [Header("Anubis Boss Specific")]
    [SerializeField] Collider weaponCollider;
    [SerializeField] GameObject projectilePrefab;
    [SerializeField] GameObject explosionPrefab;
    [SerializeField] float envAtkSpeed = 15f;
    [SerializeField] GameObject envAtkPrefab;
    [SerializeField] Transform pyramidTop;
    [SerializeField] Transform exitPoint;
    [SerializeField] GameObject platforms;
    [SerializeField] float yThreshold = 10f;
    [SerializeField] float yDefaultValue = 30f;
    [SerializeField] GameObject quicksand;
    [SerializeField] string tpMelee;
    ParticleSystem[] envPlatforms;
    Transform target;
    DissolveComponent dissolve;
    AnubisBoss_BT bt;
    public int dmg = 10;
    float bufferCd = 0.2f;
    float bufferTimer = 0;
    int tpCount = 0;

    void Start()
    {
        trackPlayer.AllPresets();

        trackPlayer.yThreshold = yThreshold;

        //Pour reverse
        trackPlayer.PlayerY(() => quicksand.SetActive(true), -0.05f);
        trackPlayer.SetStat("playerY", yDefaultValue);

        healthComponent = GetComponent<HealthComponent>();
        dissolve = GetComponent<DissolveComponent>();
        healthComponent.onDeath += Death;
        bufferTimer = bufferCd;
        target = PlayerComponent.Instance.transform;

        List<ParticleSystem> temp = new List<ParticleSystem>();
        foreach (Transform t in platforms.transform)
        {
            temp.Add(t.GetComponent<ParticleSystem>());
        }
        envPlatforms = temp.ToArray();

        bt = GetComponent<AnubisBoss_BT>();
    }
    void Update()
    {
        if(bufferTimer < bufferCd)
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
    public IEnumerator StartEnvironmentAttack()
    {
        Debug.Log("In Coroutine");
        GameObject envAtk = Instantiate(envAtkPrefab, exitPoint.position, Quaternion.identity);
        //PlayParticles();
        //Changer pour le "mini Boss"
        //yield return new WaitUntil(() => );
        //Temps de chargement de la boule
        yield return new WaitForSeconds(2f);
        while(Vector3.Distance(envAtk.transform.position, pyramidTop.position) > 1f)
        {
            //Pas sur
            if (envAtk == null)
                yield break;
            envAtk.transform.position = Vector3.MoveTowards(envAtk.transform.position, pyramidTop.position, envAtkSpeed * Time.deltaTime);
            yield return null;
        }
        if(envAtk != null)
        {
            PlayParticles();
            envAtk.GetComponent<HealthComponent>().onDeath += () => StopParticles(envAtk);
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
    private void StopParticles(GameObject envAtk)
    {
        foreach (ParticleSystem p in envPlatforms)
        {
            p.gameObject.SetActive(false);
            p.Stop();
        }
        Destroy(envAtk);
    }

    public void RangedAttack()
    {
        Quaternion rotation = Quaternion.LookRotation(target.position - exitPoint.position);
        GameObject ranged = Instantiate(projectilePrefab, exitPoint.position, rotation);
        ranged.GetComponent<TriggerProjectile>().onTrigger.AddListener(RangedExplosion);
    }
    public void RangedExplosion(CSquareEvent colliders)
    {
        if (colliders.other.CompareTag("Shield"))
        {
            trackPlayer.IncreaseStat("bossRangeMiss", -1);
            trackPlayer.IncreaseStat("bossRangeBlocked", 1);
        }
        else if (colliders.other.CompareTag("Player"))
        {
            trackPlayer.IncreaseStat("bossRangeMiss", -1);
            trackPlayer.IncreaseStat("bossRangeHit", 1);
            colliders.other.gameObject.GetComponent<HealthComponent>().Hit(rangeDmg);
        }
        //Surement plus de code ici
        Instantiate(explosionPrefab, colliders.self.transform.position, Quaternion.identity);
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
        if(bt.activeNode is Behaviour_Composite && (bt.activeNode as Behaviour_Composite).compositeInstanceID == tpMelee)
            tpCount = (tpCount + 1) % 4;
        if(tpCount != 3)
            StartCoroutine(dissolve.Dissolve());
        else //Ne va rien faire au pire
            ReverseDissolve();
    }
    public void ReverseDissolve()
    {
        StartCoroutine(dissolve.Dissolve(true));
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

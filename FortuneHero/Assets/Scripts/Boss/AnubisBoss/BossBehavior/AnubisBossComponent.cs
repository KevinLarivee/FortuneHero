using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnubisBossComponent : BossComponent
{
    [Header("Anubis Boss Specific")]
    [SerializeField] Collider weaponCollider;
    [SerializeField] GameObject projectilePrefab;
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
        PlayParticles();
        //Changer pour le "mini Boss"
        //yield return new WaitUntil(() => );
        yield return new WaitForSeconds(20f);
        StopParticles();
        //envAtk.isActivated = false;
    }

    private void PlayParticles()
    {
        foreach (var p in envPlatforms)
        {
            p.gameObject.SetActive(true);
            p.Play();
        }
    }
    private void StopParticles()
    {
        foreach (var p in envPlatforms)
        {
            p.gameObject.SetActive(false);
            p.Stop();
        }
    }

    public void RangedAttack()
    {
        var rotation = Quaternion.LookRotation(target.position - exitPoint.position);
        Instantiate(projectilePrefab, exitPoint.position, rotation);
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

using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class LavaBossComponent : BossComponent
{
    [Header("Audio")]
    [SerializeField] AudioClip fireBreathSFX;
    [SerializeField] AudioClip slashSFX;
    [SerializeField] AudioClip groundSmashSFX;


    [SerializeField] GameObject platforms;
    public GameObject lavaFalls;
    [SerializeField] Collider[] hands;
    [SerializeField] GameObject groundSmash;
    [SerializeField] int jumpDmg = 10;
    [SerializeField] float burnTime = 2f;

    FireTrapComponent fireBreath;
    PlayerMovement playerM;
    //GolemBoss_BT bt;

    int currentSlash = 0;
    bool meleeAlreadyHit = false;
    void Start()
    {
        trackPlayer.AllPresets();

        fireBreath = GetComponentInChildren<FireTrapComponent>();
        fireBreath.GetComponent<AimToTargetComponent>().target = PlayerComponent.Instance.transform;
        //bt = GetComponent<GolemBoss_BT>();
        float lowestY = float.MaxValue;
        foreach (Transform platform in platforms.transform)
        {
            if (platform.position.y < lowestY)
            {
                lowestY = platform.position.y;
            }
        }
        trackPlayer.yThreshold = lowestY;

        trackPlayer.PlayerY(RemovePlatforms);

        fireBreath.gameObject.SetActive(false);

        playerM = PlayerMovement.Instance;
    }

    void RemovePlatforms()
    {
        platforms.SetActive(false);
    }

    public void FireBreathStart()
    {
        SFXManager.Instance.PlaySFX(fireBreathSFX, transform, PlayerComponent.Instance.SFXGroup);
        fireBreath.gameObject.SetActive(true);
    }
    public void FireBreathStop()
    {
        fireBreath.gameObject.SetActive(false);
    }
    public void EmitSlash()
    {
        if (currentSlash < hands.Length)
        {
            CreateSlash(hands[currentSlash]);
            currentSlash++;
        }
        else
        {
            foreach (Collider hand in hands)
            {
                CreateSlash(hand);
            }
            currentSlash = 0;
        }
    }
    void CreateSlash(Collider hand)
    {
        //SFXManager.Instance.PlaySFX(slashSFX, transform, PlayerComponent.Instance.SFXGroup);
        GameObject temp = Instantiate(rangePrefab, hand.transform.position, hand.transform.rotation);
        temp.transform.LookAt(PlayerComponent.Instance.transform);
        if (rangeStatus)
            Instantiate(trackPlayer.statusEffect, temp.transform);
        trackPlayer.IncreaseStat("bossRangeMiss", 1);
        temp.GetComponent<TriggerProjectile>().onTrigger.AddListener((CSquareEvent c) =>
        {
            if (c.other.CompareTag("Shield"))
            {
                trackPlayer.IncreaseStat("bossRangeMiss", -1);
                trackPlayer.IncreaseStat("bossRangeBlocked", 1);
            }
            else if (c.other.CompareTag("Player"))
            {
                trackPlayer.IncreaseStat("bossRangeMiss", -1);
                trackPlayer.IncreaseStat("bossRangeHit", 1);
                c.other.gameObject.GetComponent<HealthComponent>().Hit(rangeDmg);
            }
            if (rangeStatus)
            {
                playerM.ToggleBurn(true);
                playerM.AfterBurn(burnTime);
            }
        });
    }
    public void GroundSmash()
    {
        Vector3 origin = transform.position + Vector3.up / 10f;
        Vector3 pos = origin;
        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 100f, LayerMask.GetMask("Default", "Water")))
        {
            pos = hit.point;
        }
        GameObject temp = Instantiate(groundSmash, pos, Quaternion.identity);
        SFXManager.Instance.PlaySFX(groundSmashSFX, transform, PlayerComponent.Instance.SFXGroup_Louder);
        foreach (TriggerExpand triggerExpand in temp.GetComponentsInChildren<TriggerExpand>())
        {
            triggerExpand.onTrigger.AddListener((Collider c) =>
            {
                if (c.CompareTag("Player"))
                {
                    c.gameObject.GetComponent<HealthComponent>().Hit(jumpDmg);
                }
            });
        }
    }
    public void EnableRightHandCollider()
    {
        hands[1].enabled = true;
        trackPlayer.IncreaseStat("bossMeleeMiss", 1);
        meleeAlreadyHit = false;
    }

    public void DisableRightHandCollider()
    {
        hands[1].enabled = false;
    }

    public void EnableLeftHandCollider()
    {
        hands[0].enabled = true;
        trackPlayer.IncreaseStat("bossMeleeMiss", 1);
        meleeAlreadyHit = false;
    }

    public void DisableLeftHandCollider()
    {
        hands[0].enabled = false;
    }

    public override void ChangeMovementProbability(float probability, string buff)
    {
        if(buff == "near")
            GetComponent<GolemBoss_BT>().random_Movement.probability = probability;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.excludeLayers == LayerMask.GetMask("IgnoreTrigger") || meleeAlreadyHit)
            return;
        //Melee Attack
        //Peut-être?
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
            playerM.ToggleBurn(true);
            playerM.AfterBurn(burnTime);
        }
    }
}

using NaughtyAttributes;
using System.Collections;
using UnityEngine;

public class FireTrapComponent : MonoBehaviour
{
    [SerializeField] float inactiveTime = 1f;
    [SerializeField] float activeTime = 1f;
    [SerializeField] float activeDistance = 20f;
    PlayerMovement player;
    //FireComponent flame;
    Coroutine fireCycle;
    float distance = 0f;
    bool isActive = false;

    [Header("FireComponent")]
    [SerializeField] string target = "Player";
    [SerializeField] GameObject firePrefab;
    [SerializeField] float fireRate = 0.1f;
    [SerializeField] float afterBurnTime = 3f;
    [SerializeField] bool slowness = true;
    [SerializeField, ShowIf(nameof(slowness))] float slownessValue = 2f;
    [SerializeField] bool preventDash = true;
    [SerializeField] GameObject lavaPrefab;
    [SerializeField, ShowIf(nameof(ShowDelay))] float lavaDelay = 10f;

    Coroutine afterBurn;
    ParticleSystem[] effects;
    //List<ParticleCollisionEvent> collisionEvents;
    public static bool playerIsEnter = false;
    float elapsedTime = 0f;
    float elapsedTimeLava = 0f;
    LayerMask ignoreTrigger;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //flame = GetComponentInChildren<FireComponent>();
        effects = GetComponents<ParticleSystem>();
        ignoreTrigger = LayerMask.GetMask("IgnoreTrigger");
        StopFire();
        player = PlayerMovement.Instance;
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(player.transform.position, transform.position);
        if (distance <= activeDistance && fireCycle == null)
        {
            fireCycle = StartCoroutine(FireCycle());
        }
        if (elapsedTime > 0f)
            elapsedTime -= Time.deltaTime;
        else if(isActive)
        {
            ShootFireCollision();
            elapsedTime = fireRate;
        }
        if (elapsedTimeLava > 0f)
            elapsedTimeLava -= Time.deltaTime;
    }
    public void PlayFire()
    {
        foreach (ParticleSystem p in effects)
        {
            p.Play();
        }
        isActive = true;
    }

    public void StopFire()
    {
        foreach (ParticleSystem p in effects)
        {
            p.Stop();
        }
        if (playerIsEnter)
        {
            ExitFire();
        }
        isActive = false;
    }
    IEnumerator FireCycle()
    {
        while (distance <= activeDistance)
        {
            if(inactiveTime > 0f)
                yield return new WaitForSeconds(inactiveTime);
            PlayFire();
            if(activeTime  > 0f)
                yield return new WaitForSeconds(activeTime);
            StopFire();
        }
        fireCycle = null;
    }
    public void EnterFire(CSquareEvent c2)
    {
        if (c2.other.CompareTag(target) && c2.other.excludeLayers != ignoreTrigger)
        {
            if (afterBurn != null)
                StopCoroutine(afterBurn);
            afterBurn = StartCoroutine(AfterBurn());
            if (!playerIsEnter)
            {
                playerIsEnter = true;
                Debug.Log("Start Burn");
                //Appliquer Effet de feu à la cible
                PlayerMovement.Instance.ToggleBurn(true);
                if (slowness)
                {
                    Debug.Log("Start Slowness");
                    //Appliquer l'effet de slowness à la cible
                    PlayerMovement.Instance.SlowPlayer(slownessValue);
                }
                if (preventDash)
                {
                    Debug.Log("Start Prevent Dash");
                    //Appliquer l'effet de slowness à la cible
                    PlayerMovement.Instance.ToggleDash(false);
                }
            }
        }
        if (lavaPrefab != null && elapsedTimeLava <= 0)
        {

            Vector3 pos = c2.other.ClosestPoint(c2.self.transform.position) + Vector3.up / 10f;

            RaycastHit hit;
            if (Physics.Raycast(pos, Vector3.down, out hit, 100f, LayerMask.GetMask("Default")))
            {
                pos = hit.point;
            }
            //À tester
            GameObject lava = Instantiate(lavaPrefab, pos, Quaternion.Euler(hit.normal));

            elapsedTimeLava = lavaDelay;
        }
    }
    void ExitFire()
    {
        if (playerIsEnter)
        {
            if (slowness)
            {
                Debug.Log("Stop slowness");
                //Retirer slowness
                PlayerMovement.Instance.SpeedUpPlayer(slownessValue);
            }
            if (preventDash)
            {
                Debug.Log("Stop prevent dash");
                //Retirer slowness
                PlayerMovement.Instance.ToggleDash(true);
            }
            playerIsEnter = false;
        }
    }

    IEnumerator AfterBurn()
    {
        //Pour essayer avec des particles systems
        yield return new WaitForSeconds(0.1f);
        ExitFire();
        yield return new WaitForSeconds(afterBurnTime);
        Debug.Log("Stop Burn");
        //Retirer burning de la target
        PlayerMovement.Instance.ToggleBurn(false);
        afterBurn = null;
    }
    public void ShootFireCollision()
    {
        GameObject fireCollision = Instantiate(firePrefab, transform.position, transform.rotation);
        fireCollision.GetComponent<TriggerProjectile>().onTrigger.AddListener(EnterFire);
    }
    bool ShowDelay => lavaPrefab != null;
}

using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireComponent : MonoBehaviour
{
    //!!!!! Problèmes : entre plusieurs fois dans le trigger et 1 StopBurn si on sort et rerentre
    //Ne fonctionne que avec Player pour l'instant

    //[SerializeField] float dmg = 1f;
    [SerializeField] string target = "Player";
    [SerializeField] GameObject firePrefab;
    [SerializeField] float fireRate = 0.1f;
    [SerializeField] float afterBurnTime = 3f;
    [SerializeField] bool slowness = true;
    [SerializeField, ShowIf(nameof(slowness))] float slownessValue = 2f;
    [SerializeField] bool preventDash = true;
    [SerializeField] GameObject lavaPrefab;
    [SerializeField, ShowIf(nameof(ShowDelay))] float lavaDelay = 0.5f;

    Coroutine afterBurn;
    ParticleSystem[] effects;
    //List<ParticleCollisionEvent> collisionEvents;
    public static bool playerIsEnter = false;
    float elapsedTime = 0f;

    void Awake()
    {
        effects = GetComponents<ParticleSystem>();
        //collisionEvents = new List<ParticleCollisionEvent>();
    }
    void OnEnable()
    {
        PlayFire();
    }

    void Update()
    {
        //if(elapsedTime > 0f)
        //    elapsedTime -= Time.deltaTime;

    }

    public void PlayFire()
    {
        foreach (ParticleSystem p in effects)
        {
            p.Play();
        }
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
    }

    //private void OnParticleCollision(GameObject other)
    //{
    //    if (other.CompareTag(target))
    //    {
    //        if (afterBurn != null)
    //            StopCoroutine(afterBurn);
    //        afterBurn = StartCoroutine(AfterBurn());
    //        if (!playerIsEnter)
    //        {
    //            playerIsEnter = true;
    //            Debug.Log("Start Burn");
    //            //Appliquer Effet de feu à la cible
    //            PlayerMovement.Instance.ToggleBurn(true);
    //            if (slowness)
    //            {
    //                Debug.Log("Start Slowness");
    //                //Appliquer l'effet de slowness à la cible
    //                PlayerMovement.Instance.SlowPlayer(slownessValue);
    //            }
    //            if (preventDash)
    //            {
    //                Debug.Log("Start Prevent Dash");
    //                //Appliquer l'effet de slowness à la cible
    //                PlayerMovement.Instance.ToggleDash(false);
    //            }
    //        }
    //    }
    //    if (lavaPrefab != null && elapsedTime <= 0)
    //    {
    //        elapsedTime = lavaDelay;

    //        //Instancier de la lave au point d'impact
    //        int numCollisionEvents = effects[0].GetCollisionEvents(other, collisionEvents);
            
    //        Vector3 pos = collisionEvents[0].intersection + Vector3.up / 10f;

    //        if (Physics.Raycast(pos, Vector3.down, out RaycastHit hit, 100f, LayerMask.GetMask("Default")))
    //        {
    //            pos = hit.point;
    //        }
    //        //À tester
    //        GameObject lava = Instantiate(lavaPrefab, pos, Quaternion.Euler(collisionEvents[0].normal));
    //    }
    //}

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

    bool ShowDelay => lavaPrefab != null;
}

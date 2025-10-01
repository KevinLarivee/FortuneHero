using System.Collections;
using UnityEditor.Build;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class FireComponent : MonoBehaviour
{
    /*!!!!! Problèmes : entre plusieurs fois dans le trigger et 1 StopBurn si on sort et rerentre*/

    [SerializeField] float dmg = 1f;
    [SerializeField] float afterBurnTime = 3f;
    [SerializeField] float slownessValue = 2f;
    [SerializeField] bool slowness = true;
    [SerializeField] bool preventDash = true;
    [SerializeField] string target = "Player";

    Coroutine afterBurn;
    ParticleSystem[] effects;
    Collider collider;
    bool playerIsEnter = false;

    LayerMask ignoreTrigger;

    void Awake()
    {
        effects = GetComponents<ParticleSystem>();
        collider = GetComponent<Collider>();
        ignoreTrigger = LayerMask.GetMask("IgnoreTrigger");
    }

    public void PlayFire()
    {
        foreach(ParticleSystem p in effects)
        {
            p.Play();
        }
        collider.enabled = true;
    }

    public void StopFire()
    {
        foreach (ParticleSystem p in effects)
        {
            p.Stop();
        }
        if (playerIsEnter)
        {
            ExitFire(PlayerMovement.Instance.gameObject.GetComponent<Collider>());
        }
        collider.enabled = false;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(target) && other.excludeLayers != ignoreTrigger)
        {
            playerIsEnter = true;
            if (afterBurn != null)
                StopCoroutine(afterBurn);

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
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(target) && other.excludeLayers != ignoreTrigger)
        {
            ExitFire(other);
        }
    }

    void ExitFire(Collider other)
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
        if (afterBurn != null)
            StopCoroutine(afterBurn);
        afterBurn = StartCoroutine(AfterBurn(other));
        playerIsEnter = false;
    }

    IEnumerator AfterBurn(Collider other)
    {
        yield return new WaitForSeconds(afterBurnTime);
        Debug.Log("Stop Burn");
        //Retirer burning de la target
        PlayerMovement.Instance.ToggleBurn(false);
        afterBurn = null;
    }
}

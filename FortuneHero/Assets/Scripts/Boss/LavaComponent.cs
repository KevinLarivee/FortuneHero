using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaComponent : MonoBehaviour
{
    //Trop similaire à d'autres scripts...
    [SerializeField] string target = "Player";
    [SerializeField] float afterBurnTime = 3f;
    [SerializeField] bool slowness = true;
    [SerializeField, ShowIf(nameof(slowness))] float slownessValue = 2f;
    [SerializeField] bool preventDash = true;
    [SerializeField] bool preventJump = true;

    Coroutine afterBurn = null;

    LayerMask ignoreTrigger;

    private void Awake()
    {
        ignoreTrigger = LayerMask.GetMask("IgnoreTrigger");
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Enter Trigger " + other.GetType());
        if (other.CompareTag(target) && other.excludeLayers != ignoreTrigger)
        {
            if (afterBurn != null)
                StopCoroutine(afterBurn);
            if (!FireComponent.playerIsEnter)
            {
                FireComponent.playerIsEnter = true;
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
                    //Appliquer l'effet de preventDash à la cible
                    PlayerMovement.Instance.ToggleDash(false);
                }
                if (preventJump)
                {
                    Debug.Log("Start Prevent Jump");
                    //Appliquer l'effet de preventJump à la cible
                    PlayerMovement.Instance.ToggleJump(false);
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        Debug.Log("Exit Trigger " + other.GetType());
        if (other.CompareTag(target) && other.excludeLayers != ignoreTrigger)
        {
            ExitFire();
        }
    }
    private void OnDestroy()
    {
        ExitFire();
    }
    void ExitFire()
    {
        if (FireComponent.playerIsEnter)
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
            if (preventJump)
            {
                Debug.Log("Start Prevent Jump");
                //Appliquer l'effet de preventJump à la cible
                PlayerMovement.Instance.ToggleJump(true);
            }
            if (afterBurn != null)
                StopCoroutine(afterBurn);
            afterBurn = StartCoroutine(AfterBurn());
            FireComponent.playerIsEnter = false;
        }
    }

    IEnumerator AfterBurn()
    {
        yield return new WaitForSeconds(afterBurnTime);
        Debug.Log("Stop Burn");
        //Retirer burning de la target
        PlayerMovement.Instance.ToggleBurn(false);
        afterBurn = null;
    }
}

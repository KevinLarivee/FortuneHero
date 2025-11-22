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

    LayerMask ignoreTrigger;

    public static bool playerIsEnter = false;

    PlayerMovement playerM;

    private void Awake()
    {
        ignoreTrigger = LayerMask.GetMask("IgnoreTrigger");
    }
    private void Start()
    {
        playerM = PlayerMovement.Instance;
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Enter Trigger " + other.GetType());
        if (other.CompareTag(target) && other.excludeLayers != ignoreTrigger)
        {
            if (!playerIsEnter)
            {
                playerIsEnter = true;
                Debug.Log("Start Burn");
                //Appliquer Effet de feu à la cible
                playerM.ToggleBurn(true);
                if (slowness)
                {
                    Debug.Log("Start Slowness");
                    //Appliquer l'effet de slowness à la cible
                    playerM.SlowPlayer(slownessValue);
                }
                if (preventDash)
                {
                    Debug.Log("Start Prevent Dash");
                    //Appliquer l'effet de preventDash à la cible
                    playerM.ToggleDash(false);
                }
                if (preventJump)
                {
                    Debug.Log("Start Prevent Jump");
                    //Appliquer l'effet de preventJump à la cible
                    playerM.ToggleJump(false);
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
        if (playerIsEnter)
        {
            if (slowness)
            {
                Debug.Log("Stop slowness");
                //Retirer slowness
                playerM.SpeedUpPlayer(slownessValue);
            }
            if (preventDash)
            {
                Debug.Log("Stop prevent dash");
                //Retirer slowness
                playerM.ToggleDash(true);
            }
            if (preventJump)
            {
                Debug.Log("Start Prevent Jump");
                //Appliquer l'effet de preventJump à la cible
                playerM.ToggleJump(true);
            }
            playerM.AfterBurn(afterBurnTime);
            playerIsEnter = false;
        }
    }
}

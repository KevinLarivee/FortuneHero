using NaughtyAttributes;
using UnityEngine;

public class QuickSandComponent : MonoBehaviour
{
    [Header("Paramètres d'attaque")]
    [SerializeField] string targetTag = "Player";
    [SerializeField] float slownessValue = 2f;
    [SerializeField] bool preventDash = true;
    [SerializeField] bool preventJump = true;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag(targetTag))
        {
            Debug.Log("Start Slowness");
            //Appliquer l'effet de slowness à la cible
            PlayerMovement.Instance.SlowPlayer(slownessValue);
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
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.CompareTag(targetTag))
        {
            ExitQuickSand();
        }
    }
    private void OnDestroy()
    {
        ExitQuickSand();
    }
    void ExitQuickSand()
    {
        Debug.Log("Stop slowness");
        //Retirer slowness
        PlayerMovement.Instance.SpeedUpPlayer(slownessValue);
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
    }
}

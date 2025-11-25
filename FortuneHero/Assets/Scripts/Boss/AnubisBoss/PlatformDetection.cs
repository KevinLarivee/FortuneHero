using UnityEngine;

public class PlatformDetection : MonoBehaviour
{
    [SerializeField] float paralyzeDuration = 2f;
    [SerializeField] float dmg = 5f;

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerMovement.Instance.ToggleParalyse(paralyzeDuration, dmg);
        }
    }

}

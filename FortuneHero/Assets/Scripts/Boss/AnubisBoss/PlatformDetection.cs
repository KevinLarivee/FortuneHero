using UnityEngine;

public class PlatformDetection : MonoBehaviour
{
    [SerializeField] float paralyzeDuration = 2f;
    [SerializeField] float dmg = 5f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<HealthComponent>().Hit(dmg);
            PlayerMovement.Instance.ToggleParalyse(paralyzeDuration);
        }
    }

}

using UnityEngine;
[RequireComponent(typeof(Collider))]
public class KillPlayer : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.GetComponent<HealthComponent>().Hit(10);
            RespawnManager.Instance.Respawn();
        }
    }
}

using UnityEngine;
[RequireComponent(typeof(Collider))]
public class KillPlayer : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            RespawnManager.Instance.Respawn();
    }
}

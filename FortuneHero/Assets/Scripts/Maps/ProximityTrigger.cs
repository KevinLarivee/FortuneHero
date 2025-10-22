
using Unity.Cinemachine;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ProximityCameraTrigger : MonoBehaviour
{
    [SerializeField] CinemachineCamera tableVCam;
    [SerializeField] int nearPriority = 20;
    [SerializeField] int farPriority = 0;

    void Reset() { var c = GetComponent<Collider>(); c.isTrigger = true; }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && tableVCam) tableVCam.Priority = nearPriority;
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && tableVCam) tableVCam.Priority = farPriority;
    }
}

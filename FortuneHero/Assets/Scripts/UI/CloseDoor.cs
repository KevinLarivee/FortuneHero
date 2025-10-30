using UnityEngine;

public class CloseDoor : MonoBehaviour
{
    public GameObject door; 
    public GameObject boss;

    private bool activated = false; 

    private void OnTriggerEnter(Collider other)
    {
        if (!activated && other.CompareTag("Player"))
        {
            door.SetActive(true);
            boss.SetActive(true);
            activated = true;      // Empêche toute nouvelle activation
        }
    }
}

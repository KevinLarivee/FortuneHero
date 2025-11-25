using UnityEngine;

public class CloseDoor : MonoBehaviour
{
    [SerializeField] GameObject door;
    [SerializeField] GameObject boss;
    [SerializeField] string target = "Player";


    private bool activated = false; 

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(target))
        {
            door.SetActive(true);
            boss.SetActive(true);
            Destroy(gameObject);
        }
    }
}

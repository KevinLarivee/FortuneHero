using UnityEngine;

public class MeleeAttackCollision : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("You have attacked the enemy !!!");
        }
    }
}

using UnityEngine;

public class JumpPad : MonoBehaviour
{
    public float jumpForce = 20f;
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.excludeLayers != LayerMask.GetMask("IgnoreTrigger")) 
        {
            Debug.Log("jump");
            PlayerMovement player = other.GetComponent<PlayerMovement>(); // ou le nom de ton script
            if (player != null)
            {
                //player.SetJumpPadForce(transform.up * jumpForce);
               
            }
        }
    }
}

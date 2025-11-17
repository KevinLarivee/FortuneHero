using UnityEngine;

public class JumpPad : MonoBehaviour
{
    public float jumpForce = 20f;
    public GameObject jumpPadEffect;



    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && other.excludeLayers != LayerMask.GetMask("IgnoreTrigger")) 
        {
            PlayerMovement player = other.GetComponent<PlayerMovement>(); 
            if (player != null)
            {
                player.SetJumpPadForce(transform.up * jumpForce);

                if (jumpPadEffect != null)
                    jumpPadEffect.SetActive(true);

            }
        }
    }
}

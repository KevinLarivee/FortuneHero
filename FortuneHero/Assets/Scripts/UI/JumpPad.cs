using UnityEngine;

public class JumpPad : MonoBehaviour
{
    Rigidbody rb;
    public float jumpForce = 20f;
    private void Start()
    {
         rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (rb != null)
            {
                //rb.velocity = new Vector3(rb.velocity.x, 0, rb.velocity.z);

                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }
    }
}

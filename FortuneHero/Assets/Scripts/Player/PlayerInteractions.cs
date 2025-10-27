using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.UI.Image;

public class PlayerInteractions : MonoBehaviour
{
    [SerializeField] LayerMask interactable;
    [SerializeField] float interactionRadius = 4f;
    [SerializeField] float enterRadius = 20f;
    [SerializeField] float exitRadius = 25f;

    Collider[] cols;

    public bool isPaused = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if ((cols = Physics.OverlapSphere(transform.position, exitRadius, interactable)).Length > 0)
        {
            cols = cols.OrderBy(c => Vector3.Distance(transform.position, c.transform.position)).ToArray();
            if (Vector3.Distance(transform.position, cols[0].transform.position) <= enterRadius)
                cols[0].GetComponent<IInteractable>().Enter();
            else
                cols[0].GetComponent<IInteractable>().Exit();
            for (int i = 1; i < cols.Length; i++)
            {
                cols[i].GetComponent<IInteractable>().Exit();
            }
        }
    }

    public void Interact(InputAction.CallbackContext ctx)
    {
        if (isPaused) return;

        if (ctx.started)
        {
            if(cols != null && cols.Length > 0)
            {
                    cols[0].GetComponent<IInteractable>().Interact();
            }
        }
    }
}

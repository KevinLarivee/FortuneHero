using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.UI.Image;

public class PlayerInteractions : MonoBehaviour
{
    [SerializeField] LayerMask interactable;
    [SerializeField] float interactionRadius = 4f;
    [SerializeField] float enterRadius = 20f;

    Collider[] cols;

    public bool isPaused = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
        if ((cols = Physics.OverlapSphere(transform.position, enterRadius, interactable)) != null)
        {
            foreach(Collider c in cols)
            {
                c.GetComponent<IInteractable>().Enter();
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
                var temp = cols.Where(c => Vector3.Distance(transform.position, c.transform.position) >= interactionRadius).ToArray();
                if(temp != null && temp.Length > 0)
                    temp[0].GetComponent<IInteractable>().Interact();
            }
        }
    }
}

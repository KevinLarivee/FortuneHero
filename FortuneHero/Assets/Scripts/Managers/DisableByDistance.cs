using Unity.VisualScripting;
using UnityEngine;

public class DisableByDistance : MonoBehaviour
{
    [SerializeField] float distanceToDisable = 50f;

    void Start()
    {
        
    }

    void Update()
    {
        var currentDistance = Vector3.Distance(PlayerComponent.Instance.transform.position, transform.position);
        if(currentDistance > distanceToDisable)
        {
            //Faire wtv cque t'as besoin
            transform.gameObject.SetActive(false);
        }
    }
}

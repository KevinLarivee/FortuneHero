using System.Collections;
using UnityEngine;

public class TriggerOutside : MonoBehaviour
{
    public int outsideDamage = 5;
    public string target = "Player";

    float groundY = 0f;
    HealthComponent targetHealth = null;

    private void OnEnable()
    {
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        RaycastHit hit;
        if (Physics.Raycast(origin, Vector3.down, out hit, 100f, LayerMask.GetMask("default")))
            groundY = hit.point.y;
        StartCoroutine(NearZoneCoroutine());
    }

    private void Update()
    {
        if(transform.position.y != groundY)
            transform.position = new Vector3(transform.position.x, groundY, transform.position.z);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(target))
        {
            targetHealth = null;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(target))
        {
            targetHealth = other.GetComponent<HealthComponent>();
        }
    }
    IEnumerator NearZoneCoroutine()
    {
        while (isActiveAndEnabled)
        {
            targetHealth?.Hit(outsideDamage);
            yield return new WaitForSeconds(1f);
        }
    }
}

using System.Collections;
using UnityEngine;

public class FireTrapComponent : MonoBehaviour
{
    [SerializeField] float inactiveTime = 1f;
    [SerializeField] float activeTime = 1f;
    [SerializeField] float activeDistance = 20f;
    PlayerMovement player;
    FireComponent flame;
    Coroutine fireCycle;
    float distance = 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        flame = GetComponentInChildren<FireComponent>();
        flame.StopFire();
        player = PlayerMovement.Instance;
        Debug.Log(player);
    }

    // Update is called once per frame
    void Update()
    {
        distance = Vector3.Distance(player.transform.position, transform.position);
        if (distance <= activeDistance && fireCycle == null)
        {
            fireCycle = StartCoroutine(FireCycle());
        }
    }

    IEnumerator FireCycle()
    {
        while (distance <= activeDistance)
        {
            yield return new WaitForSeconds(inactiveTime);
            flame.PlayFire();
            yield return new WaitForSeconds(activeTime);
            flame.StopFire();
        }
        fireCycle = null;
    }
}

using NaughtyAttributes;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public class CSquareEvent
{
    public Collider self;
    public Collider other;

    public CSquareEvent(Collider self, Collider other)
    {
        this.self = self;
        this.other = other;
    }
}
[RequireComponent(typeof(Collider), typeof(Rigidbody))]
public class TriggerProjectile : MonoBehaviour
{
    public UnityEvent<CSquareEvent> onTrigger;
    public float speed = 8f;
    public bool slowStart = false;
    [ShowIf(nameof(slowStart))] public float accelerationTime = 0.5f;
    Collider selfCollider;
    Rigidbody rb;
    bool isDestroyed = false;
    float startSpeed = 0f;
    private void Awake()
    {
        selfCollider = GetComponent<Collider>();
        rb = GetComponent<Rigidbody>();
    }
    private void OnEnable()
    {
        if (!slowStart)
            startSpeed = speed;
        StartCoroutine(MoveProjectile());
    }

    IEnumerator MoveProjectile()
    {
        float elapsedTime = 0f;
        while(startSpeed < speed)
        {
            float t = elapsedTime / accelerationTime;
            startSpeed = Mathf.Lerp(0f, speed, t);

            rb.linearVelocity = startSpeed * transform.forward;

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        rb.linearVelocity = speed * transform.forward;
        //while (!isDestroyed)
        //{
        //    transform.Translate(speed * Time.deltaTime * transform.forward);
        //    yield return null;
        //}
    }

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log(other.name);
        if(!isDestroyed)
            onTrigger?.Invoke(new(selfCollider, other));
        isDestroyed = true;
        Destroy(gameObject);
    }
}

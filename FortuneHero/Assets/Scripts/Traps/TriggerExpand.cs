using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TriggerExpand : MonoBehaviour
{
    //S'assurer que le collider exclude tout ce qui n'est pas une target

    public UnityEvent<Collider> onTrigger;
    public float distance = 20f;
    public float delay = 0.32f;
    public float duration = 1f;
    Vector3 initialSize;
    float initialRadius;
    bool isDestroyed = false;
    BoxCollider boxCollider;
    SphereCollider sphereCollider;
    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        sphereCollider = GetComponent<SphereCollider>();
        if(boxCollider != null)
            initialSize = boxCollider.size;
        else
            initialRadius = sphereCollider.radius;
    }
    private void OnEnable()
    {
        if(boxCollider != null)
            boxCollider.size = initialSize;
        else
            sphereCollider.radius = initialRadius;
        StartCoroutine(Expanding());
    }
    IEnumerator Expanding()
    {
        yield return new WaitForSeconds(delay);
        float elapsed = delay;
        while (elapsed < duration)
        {
            float t = elapsed / duration;
            float currentLength = Mathf.Lerp(0f, distance, t);

            if (boxCollider != null)
                boxCollider.size = new Vector3(initialSize.x, initialSize.y, currentLength);
            else
                sphereCollider.radius = currentLength;

            elapsed += Time.deltaTime;
            yield return null;
        }
        if (boxCollider != null)
            boxCollider.size = new Vector3(initialSize.x, initialSize.y, distance);
        else
            sphereCollider.radius = distance;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isDestroyed)
            onTrigger?.Invoke(other);
        isDestroyed = true;
        Destroy(gameObject);
    }
}

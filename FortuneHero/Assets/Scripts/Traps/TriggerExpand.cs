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
    Vector3 initialScale;
    bool isDestroyed = false;
    BoxCollider selfCollider;
    private void Awake()
    {
        selfCollider = GetComponent<BoxCollider>();
        initialScale = transform.localScale;
    }
    private void OnEnable()
    {
        transform.localScale = initialScale;
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

            selfCollider.size = new Vector3(initialScale.x, initialScale.y, currentLength);

            elapsed += Time.deltaTime;
            yield return null;
        }
        selfCollider.size = new Vector3(initialScale.x, initialScale.y, distance);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isDestroyed)
            onTrigger?.Invoke(other);
        isDestroyed = true;
        Destroy(gameObject);
    }
}

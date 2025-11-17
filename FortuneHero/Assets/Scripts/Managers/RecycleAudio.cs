using UnityEngine;

public class RecycleAudio : MonoBehaviour, IPoolable
{
    public ObjectPoolComponent Pool { get; set; }

    float elapsed = 0f;
    float soundTime;
    public void Recycle(float time)
    {
        elapsed = 0f;
        soundTime = time;
    }
    void Update()
    {
        elapsed += Time.deltaTime;
        if (elapsed > soundTime)
        {
            Pool.PutObject(gameObject);
        }
    }
}

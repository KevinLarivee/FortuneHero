using UnityEngine;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;
    [SerializeField] ObjectPoolComponent pool;
    void Awake()
    {
        if (instance == null)
            instance = this;

    }

    public void PlaySFX(AudioClip clip, Transform sourceTransform)
    {
        GameObject audioObject = pool.GetObject();
        audioObject.SetActive(true);
        AudioSource audioSource = audioObject.GetComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.transform.position = sourceTransform.position;
        audioSource.Play();
        audioObject.GetComponent<RecycleAudio>().Recycle(audioSource.clip.length);
    }
}

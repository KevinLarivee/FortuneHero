using UnityEngine;
using UnityEngine.Audio;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;
    public static SFXManager Instance { get { return instance; } }

    [SerializeField] ObjectPoolComponent pool;
    void Awake()
    {
        if (instance == null)
            instance = this;

        DontDestroyOnLoad(gameObject);
        DontDestroyOnLoad(pool);
    }

    public void PlaySFX(AudioClip clip, Transform sourceTransform, AudioMixerGroup group)
    {
        GameObject audioObject = pool.GetObject();
        audioObject.SetActive(true);
        AudioSource audioSource = audioObject.GetComponent<AudioSource>();
        audioSource.clip = clip;
        audioSource.outputAudioMixerGroup = group;
        audioSource.transform.position = sourceTransform.position;
        audioSource.Play();
        audioObject.GetComponent<RecycleAudio>().Recycle(audioSource.clip.length);
    }
}

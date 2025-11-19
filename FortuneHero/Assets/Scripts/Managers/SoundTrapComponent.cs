using UnityEngine;

public class SoundTrapComponent : MonoBehaviour
{
    AudioSource audioSource;

    void Start()
    {
       audioSource = GetComponent<AudioSource>();
    }

    // Appelée par l’animation
    public void PlayTrapSound()
    {
        audioSource.Play();
    }
}

using UnityEngine;

public class SoundManager : MonoBehaviour
{
    [SerializeField] AudioClip playSound;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SFXManager.instance.PlaySFX(playSound, transform);
    }
}

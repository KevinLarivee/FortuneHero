using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{

    public static AudioManager instance;
    public static AudioManager Instance { get { return instance; } }

    public AudioMixer audioMixer; // AudioMixer ici
    private float currentVolume = 0f; // volume actuel dependant du son qui est lancer dans la scene

    [SerializeField] string loadingScreenName;
    GameObject loadingScreen;

    void Awake()
    {
        if(loadingScreenName != "")
            SceneManager.LoadScene(loadingScreenName, LoadSceneMode.Additive);
    }
    void Start()
    {
        instance = this;
        // Récupère le volume au démarrage avec un playerpref pour le garder
        currentVolume = PlayerPrefs.GetFloat("MasterVolume", 0f);
        audioMixer.SetFloat("MasterVolume", currentVolume);
        
        loadingScreen = FindFirstObjectByType<LoadManager>().gameObject;
        DontDestroyOnLoad(loadingScreen);
        //loadingScreen.SetActive(false);
    }

    public void SetVolume(float volume)
    {
        // volume = slider value entre 0 et 1
        currentVolume = Mathf.Log10(Mathf.Clamp(volume, 0.001f, 1f)) * 20f; // converti en dB
        audioMixer.SetFloat("MasterVolume", currentVolume);
        PlayerPrefs.SetFloat("MasterVolume", currentVolume);
    }

    public float GetVolume()
    {
        return currentVolume;
    }
}

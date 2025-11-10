using UnityEngine;
using UnityEngine.UI;

public class VolumeSlider : MonoBehaviour
{
    public Slider slider;

    void Start()
    {
        // Met la scrollbar à la bonne position selon le volume de mes scene qui sont sauvegardé
        float savedVolume = PlayerPrefs.GetFloat("MasterVolume", 0f);
        float normalizedVolume = Mathf.InverseLerp(-80f, 0f, savedVolume);
        slider.value = normalizedVolume;

        slider.onValueChanged.AddListener(OnVolumeChanged);
    }

    void OnVolumeChanged(float value)
    {
        // Convertir slider.value qui est 0-1 pour le volume
        AudioManager.instance.SetVolume(value);
    }
}

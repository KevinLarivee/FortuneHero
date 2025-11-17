using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem.XInput;
using UnityEngine.UI;

public class MouseSensativity : MonoBehaviour
{
    [SerializeField] private Slider sensitivitySlider; // si tu veux assigner manuellement
    public const string PrefKey = "MouseSensitivity";

    void Start()
    {
        float saved = PlayerPrefs.GetFloat(PrefKey, 1f);

        // Récupère la valeur sauvegardée sinon valeur par Défaut 1 si rien.
        sensitivitySlider.value = saved;
        // Listener et appliquation initiale
        sensitivitySlider.onValueChanged.AddListener(OnSliderChanged);
    }

    void OnSliderChanged(float normalizedValue)
    {
        if(PlayerComponent.Instance != null)
            PlayerComponent.Instance.ApplySensitivity(normalizedValue);
        PlayerPrefs.SetFloat(PrefKey, normalizedValue);
    }
}
using UnityEngine;
using UnityEngine.UI;

public class VictoireUI : MonoBehaviour
{

    public GameObject victoirePanel;
    public Button lobbyButton;

    void Awake()
    {
        // Pas de DontDestroyOnLoad pour pas reconstruir le UI par scène.
        HideVictoireUI();
    }

    void Start()
    {
        lobbyButton.onClick.AddListener(OnReturnToLobby);

    }

    //public void ShowVictoireUI(bool isBossFight)
    //{
    //    if (victoirePanel == null) return;
    //    victoirePanel.SetActive(true);
    //    lobbyButton.gameObject.SetActive(isBossFight);
    //    Cursor.lockState = CursorLockMode.None;
    //}

    public void HideVictoireUI()
    {
        if (victoirePanel == null) return;
        victoirePanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnReturnToLobby()
    {
        if (victoirePanel != null)
            victoirePanel.SetActive(false);

        // Re-lock le curseur
        Cursor.lockState = CursorLockMode.Locked;
        LoadManager.Instance.Load("LobbyScene");
    }
}

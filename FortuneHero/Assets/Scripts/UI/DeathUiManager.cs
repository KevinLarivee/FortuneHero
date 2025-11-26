using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DeathUiManager : MonoBehaviour
{



    public GameObject deathPanel;
    public Button restartBossButton;
    public Button restartLevelButton;
    public Button lobbyButton;

    void Awake()
    {
        // Pas de DontDestroyOnLoad pour pas reconstruir le UI par scène.
        HideDeathUI();
    }

    void Start()
    {
        restartBossButton.onClick.AddListener(OnRestartBoss);
        restartLevelButton.onClick.AddListener(OnRestartLevel);
        lobbyButton.onClick.AddListener(OnReturnToLobby);
    }

    void OnEnable()
    {
        if(GameManager.Instance.isInBossFight == true)
            restartBossButton.gameObject.SetActive(true);
        else
            restartBossButton.gameObject.SetActive(false);
    }

    public void HideDeathUI()
    {

        if (deathPanel == null) return;
        deathPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
    }

    private void OnRestartLevel()
    {
        // Cache le panel de mort avant de recharger la scène
        if (deathPanel != null)
            deathPanel.SetActive(false);

        // Re-lock le curseur
        //Cursor.lockState = CursorLockMode.Locked;
        GameManager.Instance.RestartLevel();
    }

    private void OnRestartBoss()
    {
        if (deathPanel != null)
            deathPanel.SetActive(false);

        //Cursor.lockState = CursorLockMode.Locked;
        GameManager.Instance.RestartBoss();
    }

    private void OnReturnToLobby()
    {
        if (deathPanel != null)
            deathPanel.SetActive(false);

        //Cursor.lockState = CursorLockMode.Locked;
        LoadManager.Instance.Load("LobbyScene");
        GameManager.Instance.ReturnToLobby();
    }
}

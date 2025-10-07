using UnityEngine;
using UnityEngine.UI;

public class DeathUiManager : MonoBehaviour
{
    public static DeathUiManager Instance { get; private set; }

    public GameObject deathPanel; 
    public Button restartLevelButton;
    public Button restartBossButton;
    public Button lobbyButton;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        // Pas de DontDestroyOnLoad pour pas reconstruir le UI par scène.
        HideDeathUI();
    }

    void Start()
    {
        restartLevelButton.onClick.AddListener(OnRestartLevel);
        restartBossButton.onClick.AddListener(OnRestartBoss);
        lobbyButton.onClick.AddListener(OnReturnToLobby);

    }

    public void ShowDeathUI(bool isBossFight)
    {
        if (deathPanel == null) return;
        deathPanel.SetActive(true);
        restartBossButton.gameObject.SetActive(isBossFight);
        Cursor.lockState = CursorLockMode.None;
    }

    public void HideDeathUI()
    {
        if (deathPanel == null) return;
        deathPanel.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void OnRestartLevel()
    {
        GameManager.Instance.RestartLevel();
    }

    private void OnRestartBoss()
    {
        GameManager.Instance.RestartBoss();
    }

    private void OnReturnToLobby()
    {
        GameManager.Instance.ReturnToLobby();
    }
}

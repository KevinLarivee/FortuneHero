using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class DeathUiManager : MonoBehaviour
{



    public GameObject deathPanel; 
    public Button restartLevelButton;
    public Button restartBossButton;
    public Button lobbyButton;

    void Awake()
    {
        // Pas de DontDestroyOnLoad pour pas reconstruir le UI par scène.
        HideDeathUI();
    }

    void Start()
    {
        restartLevelButton.onClick.AddListener(OnRestartLevel);
        restartBossButton.onClick.AddListener(OnRestartBoss);
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
        GameManager.Instance.RestartLevel();
    }

    private void OnRestartBoss()
    {
        GameManager.Instance.RestartBoss();
    }

    private void OnReturnToLobby()
    {
        LoadManager.Instance.Load("LobbyScene");
    }
}

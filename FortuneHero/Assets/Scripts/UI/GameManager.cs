using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject PlayerPrefab;
    public Transform spawnPoint;
    [SerializeField] string loadScene = "Niveau1";

    public static GameManager Instance { get; private set; }

    [SerializeField] GameObject panelDeath;
    [SerializeField] GameObject panelVictory;


    public bool isInBossFight = false; 

    void Awake()
    {

        Instance = this;
        //DontDestroyOnLoad(gameObject);
        if(spawnPoint == null) spawnPoint = transform;
        Instantiate(PlayerPrefab, spawnPoint.position, spawnPoint.rotation);
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void OnPlayerDeath()
    {
        PauseMenu.Instance.SetPauseAllowed(false);
        Debug.Log("Test");
        panelDeath.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
    }

    public void OnPlayerWin()
    {
        Debug.Log("Test");
        PauseMenu.Instance.SetPauseAllowed(false);
        PlayerComponent.Instance.PausePlayer(true);
        panelVictory.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
    }
    public void RestartLevel()
    {
        LoadManager.Instance.Load(loadScene);
    }

    public void ReturnToLobby()
    {
        LoadManager.Instance.Load("LobbyScene");
    }


    public void RestartBoss()
    {
        LoadManager.Instance.Load(loadScene);
    }
}

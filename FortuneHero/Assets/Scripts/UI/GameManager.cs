using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject PlayerPrefab;
    public Transform spawnPoint;
    [SerializeField] string loadScene = "Niveau1";
    [SerializeField] string loadBoss = "Boss1Arena";

    public static GameManager Instance { get; private set; }

    [SerializeField] GameObject panelDeath;
    [SerializeField] GameObject panelVictory;


    public bool isInBossFight = false;
    int initialCoins = 0;

    void Awake()
    {

        Instance = this;
        //DontDestroyOnLoad(gameObject);
        if(spawnPoint == null) spawnPoint = transform;
        Instantiate(PlayerPrefab, spawnPoint.position, spawnPoint.rotation);
        Cursor.lockState = CursorLockMode.Locked;
        initialCoins = PlayerPrefs.GetInt("coins");
    }

    public void OnPlayerDeath()
    {
        PauseMenu.Instance.SetPauseAllowed(false);
        Debug.Log("Test");
        panelDeath.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        if(!isInBossFight)
        {
            PlayerPrefs.SetInt("coins", 0);
            PlayerPrefs.SetInt("coins", 0);
            PlayerPrefs.SetInt("Level", 1);
            PlayerPrefs.SetInt("XP", 0);
            PlayerComponent.Instance.skill.ResetAllSkills();
        }
    }

    public void OnPlayerWin()
    {
        Debug.Log("Test");
        PauseMenu.Instance.SetPauseAllowed(false);
        PlayerComponent.Instance.PausePlayer(true);
        panelVictory.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        PlayerPrefs.SetInt("coins", 0);
    }
    public void RestartLevel()
    {
        PlayerPrefs.SetInt("coins", 0);
        PlayerPrefs.SetInt("Level", 1);
        PlayerPrefs.SetInt("XP", 0);
        PlayerComponent.Instance.skill.ResetAllSkills();
        LoadManager.Instance.Load(loadScene);
    }

    public void ReturnToLobby()
    {
        PlayerPrefs.SetInt("coins", 0);
        PlayerPrefs.SetInt("coins", 0);
        PlayerPrefs.SetInt("Level", 1);
        PlayerPrefs.SetInt("XP", 0);
        PlayerComponent.Instance.skill.ResetAllSkills();
        LoadManager.Instance.Load("LobbyScene");
    }


    public void RestartBoss()
    {
        PlayerPrefs.SetInt("coins", initialCoins);
        LoadManager.Instance.Load(loadBoss);
    }
}

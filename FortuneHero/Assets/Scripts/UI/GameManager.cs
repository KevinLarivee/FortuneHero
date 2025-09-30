using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject PlayerPrefab;
    public Transform spawnPoint;
    public static GameManager Instance { get; private set; }


    public bool isInBossFight = false; 

    void Awake()
    {
        Instance = this;
        //DontDestroyOnLoad(gameObject);
        if(spawnPoint == null) spawnPoint = transform;
        Instantiate(PlayerPrefab, spawnPoint.position, spawnPoint.rotation);
    }

    public void OnPlayerDeath()
    {
        // Appelle le UI manager pour afficher l'écran de mort
        DeathUiManager.Instance.ShowDeathUI(isInBossFight);
    }

    public void RestartLevel()
    {
        SceneManager.LoadScene("");
    }

    public void ReturnToLobby()
    {
        SceneManager.LoadScene("MainScene");
    }

    
    public void RestartBoss()
    {
        //jsp
    }
}

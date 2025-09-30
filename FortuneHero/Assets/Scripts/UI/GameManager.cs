using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }


    public bool isInBossFight = false; 

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
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

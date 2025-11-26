using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    static PauseMenu instance;
    public static PauseMenu Instance { get { return instance; } }
    [SerializeField] GameObject pauseMenuUI;
    [SerializeField] GameObject panelParametres;
    [SerializeField] GameObject skillMenuUI;
    bool isPaused = false;
    bool canPause = true;
    float currentTimeScale;

    PlayerComponent player;

    private void Start()
    {
        instance = this;
        panelParametres.GetComponent<MenuOption>().previous = Retour;
        player = PlayerComponent.Instance;
        pauseMenuUI.SetActive(false);
    }
    public void OnPause(InputAction.CallbackContext context)
    {
        if (!canPause) return;
        if (context.performed) 
        {
            if (isPaused)
                ResumeGame(context.control.name);
            else
                PauseGame(context.control.name);
        }
    }

    public void PauseGame(string input = "")
    {
        (input == "f" ? skillMenuUI : pauseMenuUI).SetActive(true); 
        currentTimeScale = Time.timeScale;
        Time.timeScale = 0f;         
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        player.PausePlayer(true);
    }

    public void ResumeGame(string input = "")
    {
        (input == "f" ? skillMenuUI : pauseMenuUI).SetActive(false); 
        Time.timeScale = currentTimeScale;         
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        player.PausePlayer(false);
    }

    public void SetPauseAllowed(bool value)
    {
        canPause = value;
    }

    public void ReturnToLobby()
    {
        Time.timeScale = 1f;
        LoadManager.Instance.Load("LobbyScene");
        pauseMenuUI.SetActive(false);
    }
    public void ReturnToMenu()
    {
        LoadManager.Instance.Load("UIScene");
        pauseMenuUI.SetActive(false);
    }

    public void Options()
    {
        panelParametres.SetActive(true);
    }

    public void Retour()
    {
        panelParametres.SetActive(false);
    }
}

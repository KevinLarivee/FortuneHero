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
    private bool isPaused = false;
    private bool canPause = true;
    [SerializeField] GameObject panelParametres;
    [SerializeField] GameObject skillMenuUI;

    PlayerComponent player;

    private void Start()
    {
        instance = this;
        panelParametres.GetComponent<MenuOption>().previous = Retour;
        player = PlayerComponent.Instance;
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
        Time.timeScale = 0f;         
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        player.PausePlayer(true);
    }

    public void ResumeGame(string input = "")
    {
        (input == "f" ? skillMenuUI : pauseMenuUI).SetActive(false); 
        Time.timeScale = 1f;         
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

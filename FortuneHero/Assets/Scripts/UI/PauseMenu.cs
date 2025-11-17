using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject pauseMenuUI; 
    private bool isPaused = false;
    [SerializeField] GameObject panelParametres;
    [SerializeField] GameObject skillMenuUI;

    PlayerComponent player;

    private void Start()
    {
        panelParametres.GetComponent<MenuOption>().previous = Retour;
        player = PlayerComponent.Instance;
    }
    public void OnPause(InputAction.CallbackContext context)
    {
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

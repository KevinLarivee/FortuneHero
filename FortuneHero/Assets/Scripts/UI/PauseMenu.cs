using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField]  GameObject pauseMenuUI; 
    private bool isPaused = false;
    [SerializeField]  GameObject panelParametres;

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
                ResumeGame();
            else
                PauseGame();
        }
    }

    private void PauseGame()
    {
        pauseMenuUI.SetActive(true); 
        Time.timeScale = 0f;         
        isPaused = true;
        Cursor.lockState = CursorLockMode.None;
        player.PausePlayer(true);
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false); 
        Time.timeScale = 1f;         
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;
        player.PausePlayer(false);
    }

    public void ReturnToLobby()
    {
        Time.timeScale = 1f;
        LoadManager.Instance.Load("Test");
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

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI; 
    private bool isPaused = false;

    [SerializeField] private Button boutonResumeGame;
    [SerializeField] private Button boutonOptions;
    [SerializeField] private Button boutonRetournerLobby;
    [SerializeField] private GameObject panelParametres;

    private void Start()
    {
        boutonResumeGame.onClick.AddListener(ResumeGame);
        boutonOptions.onClick.AddListener(Options);
        boutonRetournerLobby.onClick.AddListener(ReturnToLobby);
        panelParametres.GetComponent<MenuOption>().previous = Retour;

    }
    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.started) 
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
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false); 
        Time.timeScale = 1f;         
        isPaused = false;
        Cursor.lockState = CursorLockMode.Locked;

    }

    public void ReturnToLobby()
    {
        Time.timeScale = 1f;          
        SceneManager.LoadScene("MainMenu"); 
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

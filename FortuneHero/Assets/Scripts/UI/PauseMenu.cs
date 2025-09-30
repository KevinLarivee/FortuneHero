using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject pauseMenuUI; 
    private bool isPaused = false;

    public void OnPause(InputAction.CallbackContext context)
    {
        if (context.performed) // seulement quand l'action est déclenchée
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
    }

    public void ResumeGame()
    {
        pauseMenuUI.SetActive(false); 
        Time.timeScale = 1f;         
        isPaused = false;
    }

    public void ReturnToLobby()
    {
        Time.timeScale = 1f;          
        SceneManager.LoadScene(""); 
    }
}

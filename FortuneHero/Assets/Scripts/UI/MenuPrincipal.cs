using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuPrincipal : MonoBehaviour
{
    [SerializeField] private Button boutonJouer;
    [SerializeField] private Button boutonOptions;
    [SerializeField] private Button boutonQuitter;
    [SerializeField] private GameObject panelParametres;
    [SerializeField] private GameObject panelMenuPrincipal;
    private void Start()
    {
        boutonJouer.onClick.AddListener(Jouer);
        boutonOptions.onClick.AddListener(Options);
        boutonQuitter.onClick.AddListener(Quitter);
        panelParametres.GetComponent<MenuOption>().previous = Retour;
    }

    public void Jouer()
    {
        SceneManager.LoadScene("MainScene");
    }

    public void Options()
    {
        panelParametres.SetActive(true);
    }

    public void Quitter()
    {
        #if UNITY_STANDALONE
            Application.Quit();
        #endif
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
    public void Retour()
    {
        panelParametres.SetActive(false);
        panelMenuPrincipal.SetActive(true);
    }
}

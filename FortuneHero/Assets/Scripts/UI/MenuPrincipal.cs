using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuPrincipal : MonoBehaviour
{
    [SerializeField]  Button boutonJouer;
    [SerializeField]  Button boutonOptions;
    [SerializeField]  Button boutonQuitter;
    [SerializeField]  GameObject panelParametres;
     void Start()
    {
        boutonJouer.onClick.AddListener(Jouer);
        boutonOptions.onClick.AddListener(Options);
        boutonQuitter.onClick.AddListener(Quitter);
        panelParametres.GetComponent<MenuOption>().previous = Retour;
    }

    public void Jouer()
    {
        LoadManager.Instance.Load("LobbyScene");
        //SceneManager.LoadScene("MainScene");
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
    }
}

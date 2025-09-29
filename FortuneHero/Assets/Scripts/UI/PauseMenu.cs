using UnityEngine;

public class PauseMenu : MonoBehaviour
{

    [SerializeField] private GameObject panelOptions;

    private bool estEnPause = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) //option
        {
            if (estEnPause)
                Reprendre();
            else
                MettreEnPause();
        }
    }

    public void MettreEnPause()
    {
        panelOptions.SetActive(true);
        Time.timeScale = 0f;
        estEnPause = true;
    }

    public void Reprendre()
    {
        panelOptions.SetActive(false);
        Time.timeScale = 1f;
        estEnPause = false;
    }
}

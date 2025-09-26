using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CanvasRenderer))]
public class DoorComponent : MonoBehaviour, IInteractable
{
    [SerializeField] string sceneToLoad = "Lobby";
    [SerializeField] int levelRequirement = 0;

    LoadManager loadManager = LoadManager.Instance;

    TextMeshPro text;

    bool canEnter = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //Clé à revoir...
        if(levelRequirement > PlayerPrefs.GetInt("Progression"))
        {
            canEnter = false;
            text.text = "Compléter d'abord le niveau " + levelRequirement/*(levelRequirement == 0 ? "tutoriel" : levelRequirement)*/;
        }
        else
        {
            canEnter = true;
            //À revoir...
            text.text = "Appuyer sur " + PlayerPrefs.GetString("InteractKey") + " pour aller au " + sceneToLoad;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Enter()
    {
        text.canvas.gameObject.SetActive(true);
    }

    public void Exit()
    {
        text.canvas.gameObject.SetActive(false);
    }

    public void Interact()
    {
        if(canEnter)
            loadManager.Load(sceneToLoad);
    }
}

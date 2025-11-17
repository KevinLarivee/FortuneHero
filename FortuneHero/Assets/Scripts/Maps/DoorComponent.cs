using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DoorComponent : MonoBehaviour, IInteractable
{
    [SerializeField] string sceneToLoad = "Lobby";
    [SerializeField] int levelRequirement = 0;

    //TextMeshProUGUI text;

    bool canEnter = true;

    public float exitTime { get; set; } = 5f;

    float elapsedTime = 0f;
    bool isExit = true;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        //text = GetComponentInChildren<TextMeshProUGUI>();
        //Clé à revoir...
        if(levelRequirement > PlayerPrefs.GetInt("Progression"))
        {
            canEnter = false;
            //text.text = "Compléter d'abord le niveau " + levelRequirement/*(levelRequirement == 0 ? "tutoriel" : levelRequirement)*/;
        }
        else
        {
            canEnter = true;
            //À revoir...
            //text.text = "Appuyer sur " + PlayerPrefs.GetString("InteractKey") + " pour aller au " + sceneToLoad;
        }
        Exit();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isExit)
        {
            elapsedTime += Time.deltaTime;

            if (elapsedTime > exitTime)
            {
                Exit();
            }
        }
    }

    public void Enter()
    {
        //text.gameObject.SetActive(true);
        elapsedTime = 0f;
        isExit = false;
    }

    public void Exit()
    {
        //text.gameObject.SetActive(false);
        isExit = true;
    }

    public void Interact()
    {
        if(canEnter)
            LoadManager.Instance.Load(sceneToLoad);
    }
}

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class MenuOption : MonoBehaviour
{
    [SerializeField]  Button retour;


    public UnityAction previous;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        retour.onClick.AddListener(previous);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

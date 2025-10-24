using TMPro;
using UnityEngine;

public class DebugBehaviour : MonoBehaviour
{
    [SerializeField] BehaviourTree bt;
    TextMeshProUGUI debugText;

    void Start()
    {
        debugText = GetComponent<TextMeshProUGUI>();
    }
    void Update()
    {
        if (bt != null && debugText != null)
        {
            debugText.SetText("Action: " + bt.activeNode);
        }
    }
}

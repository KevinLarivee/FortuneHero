using TMPro;
using UnityEngine;

public class DebugBehaviour : MonoBehaviour
{
    [SerializeField] TrackPlayerComponent tracker;
    [SerializeField] BehaviourTree bt;
    TextMeshProUGUI debugText;

    void Start()
    {
        debugText = GetComponent<TextMeshProUGUI>();
    }
    void Update()
    {
        if (bt != null && debugText != null && bt.activeNode != null)
        {
            debugText.SetText("Action: " + (bt.activeNode as Behaviour_Composite).compositeInstanceID);
        }
        else
        {
            string temp = "Stats:\n";
            foreach ((string key, Stat stat)in tracker.stats)
            {
                temp += key + ": " + stat.value * stat.multiplier + "\n";
            }
            debugText.SetText(temp);
        }
    }
}

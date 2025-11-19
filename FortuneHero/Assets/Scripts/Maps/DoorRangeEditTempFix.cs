using UnityEngine;

public class DoorRangeEditTempFix : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var playerInteractions = PlayerComponent.Instance.GetComponent<PlayerInteractions>();
        if (playerInteractions != null)
        {
            playerInteractions.enterRadius = 2f;
            playerInteractions.exitRadius = 3f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

using Unity.VisualScripting;
using UnityEngine;

public class LavaBossComponent : BossComponent
{
    [SerializeField] GameObject platforms;


    void Start()
    {
        float lowestY = float.MaxValue;
        foreach (Transform platform in platforms.GetComponentsInChildren<Transform>())
        {
            if (platform.position.y < lowestY)
            {
                lowestY = platform.position.y;
            }
        }
        trackPlayer.yThreshold = lowestY;

        trackPlayer.PlayerY(RemovePlatforms);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void RemovePlatforms()
    {
        platforms.SetActive(false);
    }
}

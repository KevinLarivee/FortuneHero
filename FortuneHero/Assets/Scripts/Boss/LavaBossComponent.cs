using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class LavaBossComponent : BossComponent
{
    [SerializeField] GameObject platforms;
    [SerializeField] GameObject lavaFalls;
    [SerializeField] GameObject SlashCollision;
    [SerializeField] ParticleSystem[] slashs;
    [SerializeField] GameObject groundSmash;
    [SerializeField] FireTrapComponent fireBreath;

    int currentSlash = 0;

    void Start()
    {
        //fireBreath = GetComponentInChildren<FireTrapComponent>();
        fireBreath.GetComponent<AimToTargetComponent>().target = PlayerComponent.Instance.transform;
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

    public void FireBreathStart()
    {
        fireBreath.gameObject.SetActive(true);
    }
    public void FireBreathStop()
    {
        fireBreath.gameObject.SetActive(false);
    }
    public void EmitSlash()
    {
        if(currentSlash < slashs.Length)
        {
            slashs[currentSlash].Emit(1);
            Instantiate(SlashCollision, slashs[currentSlash].transform);
            currentSlash++;
        }
        else
        {
            foreach (ParticleSystem slash in slashs)
            {
                slash.Emit(1);
                Instantiate(SlashCollision, slash.transform);
            }
            currentSlash = 0;
        }
    }
    public void GroundSmash()
    {
        Vector3 origin = transform.position + Vector3.up / 10f;
        Vector3 pos = origin;
        if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, 100f, LayerMask.GetMask("Default", "Water")))
        {
            pos = hit.point;
        }
        Instantiate(groundSmash, pos, Quaternion.identity);
    }
}

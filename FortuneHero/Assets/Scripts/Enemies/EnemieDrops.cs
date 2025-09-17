using UnityEngine;

public class EnemieDrops : MonoBehaviour
{
    [SerializeField] bool isded = false;
    [SerializeField] bool isdropped = false;
    int nbXp = 0;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isded && !isdropped)
        {
            //drop a faire
            Debug.Log("im ded");
            isdropped = true;
        }

    }
}

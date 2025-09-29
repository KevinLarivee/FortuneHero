using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class EnemyDrops : MonoBehaviour
{
    [SerializeField] bool isdropped = false;
    [SerializeField] GameObject XpOrb;
    [SerializeField] GameObject CoinsOrb;

    [Header("Paramètres de drop")]
    [SerializeField] int XpCount = 3;
    [SerializeField] int CoinsCount = 3;

    [Header("Paramètres de position")]
    [SerializeField] float spread = 1.5f;
    [SerializeField] float heightOffset = 0.5f;

    Dictionary<GameObject, int> drops = new Dictionary<GameObject, int>();



    private void Start()
    {
        drops.Add(XpOrb, XpCount);
        drops.Add(CoinsOrb, CoinsCount);
    }

    void Update()
    {

    }
    //public void SpawnDrops()
    //{
    //    int count = XpCount + CoinsCount; 


    //    for (int i = 0; i < count; i++)
    //    {
    //        Vector3 pos = transform.position + Random.insideUnitSphere * spread; // source: https://docs.unity3d.com/ScriptReference/Random-insideUnitSphere.html
    //        pos.y = transform.position.y + 0.5f;

    //        GameObject orb = Instantiate(XpOrb, pos, Quaternion.identity); //object pool?


    //    }
    //    for (int i = 0; i < count; i++)
    //    {
    //        Vector3 pos = transform.position + Random.insideUnitSphere * spread;
    //        pos.y = transform.position.y + 0.5f;

    //        GameObject orb = Instantiate(CoinsOrb, pos, Quaternion.identity); //object pool?


    //    }
    //}
    public void SpawnDrops()
    {
        Vector3 basePos = transform.position;

        foreach (var d in drops)
        {
            if (d.Key == null || d.Value <= 0) continue;

            for (int i = 0; i < d.Value; i++)
            {
                Vector2 rnd = Random.insideUnitCircle * spread;
                Vector3 pos = new Vector3(basePos.x + rnd.x, basePos.y + heightOffset, basePos.z + rnd.y);
                Instantiate(d.Key, pos, Quaternion.identity);
            }
        }
    }
    // Appelle ceci depuis ton système de vie quand l’ennemi meurt
    //public void Die()
    //{
    //    isded = true;
    //}
}

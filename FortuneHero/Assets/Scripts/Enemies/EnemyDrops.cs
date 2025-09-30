using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using System;

public class EnemyDrops : MonoBehaviour
{
    [Serializable]
    public class Drop
    {
        public GameObject ObjectToDrop;
        public int dropSize = 10;
    }
    [SerializeField] List<Drop> drops;
    
    [Header("Paramètres de position")]
    [SerializeField] float spread = 1.5f;
    [SerializeField] float heightOffset = 0.5f;

  



    private void Start()
    {
       
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
            for (int i = 0; i < d.dropSize; i++)
            {
                Vector2 rnd = UnityEngine.Random.insideUnitCircle * spread;
                Vector3 pos = new Vector3(basePos.x + rnd.x, basePos.y + heightOffset, basePos.z + rnd.y);
                Instantiate(d.ObjectToDrop, pos, Quaternion.identity);
            }
        }
    }
    // Appelle ceci depuis ton système de vie quand l’ennemi meurt
    //public void Die()
    //{
    //    isded = true;
    //}
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class TTL : MonoBehaviour//, IPoolable
{
    [SerializeField] float timeToLive = 3f;
    float initialTime;

    //public ObjectPoolComponent Pool { get; set; }

    //void OnEnable()
    void Awake()
    {
        initialTime = Time.time;
    }

    void Update()
    {
        //if(Pool != null & IsServer) // Seul le serveur devrait être capable de détruire (Despawn) un objet
        //{
        //    if (Time.time > initialTime + timeToLive)
        //    {
        //        Pool.PutObject(gameObject);
        //    }
        //}
        //if (Pool != null)
        //{
            if (Time.time > initialTime + timeToLive)
            {
                //Pool.PutObject(gameObject);
                Destroy(gameObject);
            }

        //}
    }
}

using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class EnemieDrops : MonoBehaviour
{
    [SerializeField] bool isded = false;
    [SerializeField] bool isdropped = false;
    [SerializeField] GameObject DropModel;
    //Dictionary<GameObject, int> dropModels;

    [Header("Paramètres de drop")]
    [SerializeField] int XpCount = 3;
    [SerializeField] int xpPerOrb = 5;
    [SerializeField] float spread = 1.5f;

    void Update()
    {
        if (isded && !isdropped)
        {
            isdropped = true;

            if (DropModel == null)
            {
                Debug.LogWarning("[EnemieDrops] DropModel n'est pas assigné.");
            }
            else
            {
                int count = Mathf.Max(1, XpCount);
                int perOrb = Mathf.Max(1, xpPerOrb);

                for (int i = 0; i < count; i++)
                {
                    Vector3 pos = transform.position;
                    + Random.insideUnitSphere * spread; // source: https://docs.unity3d.com/ScriptReference/Random-insideUnitSphere.html
                    pos.y = transform.position.y + 0.5f;

                    GameObject orb = Instantiate(DropModel, pos, Quaternion.identity);
                    var xp = orb.GetComponent<XpOrb>();
                    if (xp != null) xp.amount = perOrb;
                }
            }

            gameObject.SetActive(false);
        }
    }

    // Appelle ceci depuis ton système de vie quand l’ennemi meurt
    public void Die()
    {
        isded = true;
    }
}

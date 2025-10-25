using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ShopComponent : MonoBehaviour
{
    [SerializeField] Transform itemSpot1;
    [SerializeField] Transform itemSpot2;
    [SerializeField] Transform itemSpot3;
    List<PowerUp> meleePowerUps;
    List<PowerUp> distancePowerUps;
    List<PowerUp> defencePowerUps;

    void Start()
    {
        InitializeShop();
    }

    void Update()
    {

    }

    void InitializeShop()
    {
        List<PowerUp> availablePowerUps = PlayerActions.Instance.powerUps; //Faire apparaitre 3 powerUps random (1 par categorie) 
        meleePowerUps = availablePowerUps.Where(p => p.Type == PowerUpTypes.Melee).ToList();
        distancePowerUps = availablePowerUps.Where(p => p.Type == PowerUpTypes.Distance).ToList();
        defencePowerUps = availablePowerUps.Where(p => p.Type == PowerUpTypes.Defence).ToList();

        int meleeRand = Random.Range(0, meleePowerUps.Count);
        int distRand = Random.Range(0, distancePowerUps.Count);
        int defRand = Random.Range(0, defencePowerUps.Count);
        PowerUp meleePower = meleePowerUps[meleeRand];
        PowerUp distPower = distancePowerUps[distRand];
        //PowerUp defPower = defencePowerUps[defRand];

        GameObject obj = Instantiate(meleePower.ShopPrefab, itemSpot1.position, Quaternion.identity);
        Instantiate(distPower.ShopPrefab, itemSpot2.position, Quaternion.identity);
        //Instantiate(defPower.ShopPrefab, itemSpot3, false);

        StartCoroutine(Play(obj)); //Delay pour le aoe pcq il etait pas sync

    }

    IEnumerator Play(GameObject obj)
    {
        yield return new WaitForSeconds(1);
        obj.GetComponent<ParticleSystem>().Play(true);
    }
}

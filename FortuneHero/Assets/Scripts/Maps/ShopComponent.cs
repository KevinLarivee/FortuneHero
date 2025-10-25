using JetBrains.Annotations;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.ProBuilder.MeshOperations;
using UnityEngine.UI;

public class ShopComponent : MonoBehaviour
{
    [Header("ItemCanva")]
    [SerializeField] GameObject ItemCanva;
    [SerializeField] RawImage itemImage;
    [SerializeField] TextMeshProUGUI itemName;
    [SerializeField] TextMeshProUGUI itemDescription;
    [SerializeField] TextMeshProUGUI itemPrice;


    [Header("Items")]
    [SerializeField] Transform itemSpot1;
    [SerializeField] Transform itemSpot2;
    [SerializeField] Transform itemSpot3;
    List<PowerUp> currentShopPowerUps = new();
    List<PowerUp> meleePowerUps;
    List<PowerUp> distancePowerUps;
    List<PowerUp> defencePowerUps;
    GameObject item1;
    GameObject item2;
    GameObject item3;

    UIWeaponSelector selector;
    Coroutine distanceCoroutine;

    void Start()
    {
        selector = FindAnyObjectByType<UIWeaponSelector>();
        ItemCanva.SetActive(false);
        InitializeShop();
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
        currentShopPowerUps.Add(meleePower); //Les ajouter a la liste des powers ups available dans le shop
        currentShopPowerUps.Add(distPower);
        //currentShopPowerUps.Add(defPower);

        item1 = Instantiate(meleePower.ShopPrefab, itemSpot1.position, Quaternion.identity);
        item2 = Instantiate(distPower.ShopPrefab, itemSpot2.position, Quaternion.identity);
        //item3 = Instantiate(defPower.ShopPrefab, itemSpot3, false);

        StartCoroutine(Play(item1)); //Delay pour le aoe pcq il etait pas sync (pas ideal pour si c pas le aoe mais sa change pas grand chose apart opti)
    }

    IEnumerator Play(GameObject obj)
    {
        yield return new WaitForSeconds(1);
        obj.GetComponent<ParticleSystem>().Play(true);
    }

    IEnumerator CheckDistances()
    {
        while (true)
        {
            Vector3 playerPos = PlayerActions.Instance.transform.position;
            float dist1 = Vector3.Distance(playerPos, itemSpot1.position);
            float dist2 = Vector3.Distance(playerPos, itemSpot2.position);
            float dist3 = Vector3.Distance(playerPos, itemSpot3.position);

            if (item1 != null && dist1 < dist2)
                ChangeCanvaValues(currentShopPowerUps[0]);
            else if (item2 != null && dist2 < dist1 && dist2 < dist3)
                ChangeCanvaValues(currentShopPowerUps[1]);
            else if (item3 != null && dist3 < dist2) 
                ChangeCanvaValues(currentShopPowerUps[2]);

            yield return new WaitForSeconds(0.1f);
        }
    }
    private void ChangeCanvaValues(PowerUp power)
    {
        //itemImage = LoadImage();
        itemName.text = power.Name;
        itemDescription.text = power.Description;
        itemPrice.text = power.Price.ToString();
    }
    private void BuyPowerUp(GameObject obj, PowerUp power)
    {
        Destroy(obj);
        PlayerComponent.Instance.GetXpAndCoins(0, -power.Price);
        selector.GainPowerUp(power);
    }


    //Trigger pour afficher le details des items
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            ItemCanva.SetActive(true);
            distanceCoroutine = StartCoroutine(CheckDistances());
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (distanceCoroutine != null)
            {
                Debug.Log("Disable");
                ItemCanva.SetActive(false);
                StopCoroutine(distanceCoroutine);
                distanceCoroutine = null;
            }
        }
    }
}

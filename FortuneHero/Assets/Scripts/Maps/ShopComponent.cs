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

public class ShopComponent : MonoBehaviour, IInteractable
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
    GameObject currentItem;
    PowerUp currentPower;

    [SerializeField] float detectionRadius = 2f;
    UIWeaponSelector selector;
    bool isInRange = false;
    bool canBuy = false;

    public float exitTime { get; set; } //IInteractable

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
        PowerUp defPower = defencePowerUps[defRand];
        currentShopPowerUps.Add(meleePower); //Les ajouter a la liste des powers ups available dans le shop
        currentShopPowerUps.Add(distPower);
        currentShopPowerUps.Add(defPower);

        item1 = Instantiate(meleePower.ShopPrefab, itemSpot1.position, Quaternion.identity);
        item2 = Instantiate(distPower.ShopPrefab, itemSpot2.position, Quaternion.identity);
        item3 = Instantiate(defPower.ShopPrefab, itemSpot3.position, Quaternion.identity);

        StartCoroutine(Play(item1)); //Delay pour le aoe pcq il etait pas sync (pas ideal pour si c pas le aoe mais sa change pas grand chose apart opti)
    }

    IEnumerator Play(GameObject obj)
    {
        yield return new WaitForSeconds(1);
        obj.GetComponent<ParticleSystem>().Play(true);
    }

    IEnumerator CheckDistances() //Checker quel item est plus proche du joueur et afficher l'info correspondant
    {
        while (isInRange)
        {
            Vector3 playerPos = PlayerActions.Instance.transform.position;
            float dist1 = Vector3.Distance(playerPos, itemSpot1.position);
            float dist2 = Vector3.Distance(playerPos, itemSpot2.position);
            float dist3 = Vector3.Distance(playerPos, itemSpot3.position);

            if (dist1 < dist2 && dist1 < detectionRadius) //Si l'item a pas deja ete acheter, s'il est assez proche (pcq 1 collider, donc items peuvent etre loin)
                                                                           //et s'il plus proche que l'autre item
            {
                if(item1 != null)
                {
                    currentItem = item1;
                    currentPower = currentShopPowerUps[0];
                    ItemCanva.SetActive(true); //repete le code pcq il faut que le canva s'active juste quand les conditions sont vrai pour un des trois, sinon non
                    ChangeCanvaValues(currentPower); //repete le code pcq je veux pas appeler la methode chaque frame quand le joueur est dans range, juste quand il en a besoin
                }
                else
                    ItemCanva.SetActive(false);
            }
            else if (dist2 < dist1 && dist2 < dist3 && dist2 < detectionRadius)
            {
                if(item2 != null)
                {
                    currentItem = item2;
                    currentPower = currentShopPowerUps[1];
                    ItemCanva.SetActive(true);
                    ChangeCanvaValues(currentPower);
                }
                
            }
            else if (dist3 < dist2 && dist3 < detectionRadius)
            {
                if(item3 != null)
                {
                    currentItem = item3;
                    currentPower = currentShopPowerUps[2];
                    ItemCanva.SetActive(true);
                    ChangeCanvaValues(currentPower);
                }
                else
                    ItemCanva.SetActive(false);
            }

            yield return new WaitForSeconds(0.1f);
        }
    }
    private void ChangeCanvaValues(PowerUp power)
    {
        itemImage.texture = power.Image.texture;
        itemName.text = power.Name;
        itemDescription.text = power.Description;
        if (PlayerPrefs.GetInt("coins") >= power.Price)
        {
            canBuy = true;
            itemPrice.text = power.Price.ToString();
        }
        else
        {
            canBuy = false;
            itemPrice.text = "Not enough coins. You need: " + power.Price.ToString() + " coins";
        }

    }
    private void BuyPowerUp(GameObject obj, PowerUp power)
    {
        //check si j'ai assez de cash
        if(canBuy)
        {
            Destroy(obj);
            PlayerComponent.Instance.UpdateCoins(-power.Price);
            selector.GainPowerUp(power);
        }
    }


    //Trigger pour afficher le details des items
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = true;
            StartCoroutine(CheckDistances());
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            isInRange = false;
            currentItem = null;
            currentPower = null;
            ItemCanva.SetActive(false);
        }
    }

    public void Enter() { }

    public void Exit() { }

    public void Interact()
    {
        if (isInRange)
        {
            BuyPowerUp(currentItem, currentPower);
            Debug.Log("Buy that shit");
        }
    }
}

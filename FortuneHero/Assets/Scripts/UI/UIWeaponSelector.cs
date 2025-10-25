using NUnit.Framework;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIWeaponSelector : MonoBehaviour
{
    [SerializeField] Color selectedColor = Color.yellow;
    [SerializeField] Color defaultColor = Color.white;
    [SerializeField] RawImage melee;
    [SerializeField] RawImage distance;
    [SerializeField] RawImage defence;
    Texture meleeDefaultTexture;
    Texture distDefaultTexture;
    Texture defDefaultTexture;

    Dictionary<int, PowerUp> currentPowerUps = new();
    int currentSelected = 0;


    void Start()
    {
        //melee.gameObject.SetActive(false);
        //distance.gameObject.SetActive(false);
        //defence.gameObject.SetActive(false);
       /* meleeDefaultTexture = melee.texture;
        distDefaultTexture = distance.texture;
        defDefaultTexture = defence.texture;*/
    }

    public void OnSelectSlot(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        int key = int.Parse(context.control.name);

        if (currentSelected != 0)
            currentPowerUps[currentSelected].Image.color = defaultColor;
        
        currentSelected = key;
        currentPowerUps[currentSelected].Image.color = selectedColor;

        if (!currentPowerUps.ContainsValue(currentPowerUps[currentSelected]))
            currentPowerUps[currentSelected].Action?.Invoke();
        UsePowerUp(currentSelected);
    }

    void UsePowerUp(int key)
    {
        currentPowerUps.Remove(key);
        switch (key) //Change l'image du slot pour l'image du type (par defaut)
        {
            case 1:
                melee.texture = meleeDefaultTexture;
                break;
            case 2:
                distance.texture = distDefaultTexture;
                break;
            case 3:
                defence.texture = defDefaultTexture;
                break;
        }

    }

    public void GainPowerUp(PowerUp power)
    {
        currentSelected = (int)power.Type + 1; //(ex.: Melee = 1, Distance = 2, etc.)

        if (!currentPowerUps.ContainsValue(power))
        {
            currentPowerUps.Add(currentSelected, power);
            switch (power.Type) //Change l'image du slot pour l'image du P-U
            {
                case PowerUpTypes.Melee:
                    melee.texture = power.Image.texture;
                    break;
                case PowerUpTypes.Distance:
                    distance.texture = power.Image.texture;
                    break;
                case PowerUpTypes.Defence:
                    defence.texture = power.Image.texture;
                    break;
            }
        }
    }    


    //public void UnlockSlot(int slotIndex)
    //{
    //    switch (slotIndex)
    //    {
    //        case 1:
    //            swordSlot.gameObject.SetActive(true);
    //            break;
    //        case 2:
    //            bowSlot.gameObject.SetActive(true);
    //            break;
    //        case 3:
    //            powerSlot.gameObject.SetActive(true);
    //            break;
    //    }
    //}
}









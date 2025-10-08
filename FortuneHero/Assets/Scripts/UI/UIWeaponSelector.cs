using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UIWeaponSelector : MonoBehaviour
{
    [SerializeField] RawImage melee;
    [SerializeField] RawImage distance;
    [SerializeField] RawImage defense;

    [SerializeField] Color selectedColor = Color.yellow;
    [SerializeField] Color defaultColor = Color.white;

    RawImage currentSelected;


    void Start()
    {
        melee.gameObject.SetActive(false);
        distance.gameObject.SetActive(false);
        defense.gameObject.SetActive(false);
    }

    public void OnSelectSlot(InputAction.CallbackContext context)
    {
        if (!context.performed)
            return;

        string key = context.control.name;

        switch (key)
        {
            case "1":
                if (melee.enabled)
                    SelectSlot(melee);
                break;
            case "2":
                if (distance.enabled)
                    SelectSlot(distance);
                break;
            case "3":
                if (defense.enabled)
                    SelectSlot(defense);
                break;
        }
    }

    private void SelectSlot(RawImage slot)
    {
        if (currentSelected != null)
            currentSelected.color = defaultColor;

        currentSelected = slot;
        currentSelected.color = selectedColor;
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









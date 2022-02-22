using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemUi : MonoBehaviour
{
    public PlayerControls playerControls;
    public bool equipped;
    public int weight=1;
    public string itemName = "N/A";
    [Space] public string camelisedItemName;
    public Sprite sprite;
    [Space] public Button button;
    [TextArea(15,20)] public string itemDesc;

    void Start() 
    {
        string temp = "";
        for (int i=0 ; i<itemName.Length ; i++)
        {
            char letter = itemName[i];
            if (char.IsUpper(letter))
                temp += " " + char.ToUpper(letter);
            else
                if (i == 0)
                    temp += char.ToUpper(letter);
                else
                    temp += letter;
        }
        camelisedItemName = temp;
    }

    private bool CanEquip(int itemWeight)
    {
        if ((playerControls.currentWeight + itemWeight) <= (playerControls.maxWeight + playerControls.extraWeight) )
            return true;
        playerControls.TooHeavy();
        return ( (playerControls.currentWeight + itemWeight) <= (playerControls.maxWeight + playerControls.extraWeight) );
    }

    private void Equip(int itemWeight)
    {
        playerControls.currentWeight += itemWeight;
        playerControls.weightText.text = playerControls.currentWeight + "/" + (playerControls.maxWeight + playerControls.extraWeight);
    }
    private void Unequip(int itemWeight)
    {
        playerControls.currentWeight -= itemWeight;
        playerControls.weightText.text = playerControls.currentWeight + "/" + (playerControls.maxWeight + playerControls.extraWeight);
    }

    public void TOGGLE_ITEM()
    {
        // ALREADY EQUIPPED, UNEQUIP
        if (equipped)
        {
            equipped = false;
            switch (itemName)
            {
                case "quickCharm":
                    playerControls.quickCharm = false;
                    break;
                case "chuggerCharm":
                    playerControls.chuggerCharm = false;
                    break;
                case "crisisCharm":
                    playerControls.crisisCharm = false;
                    break;
                case "dualCharm":
                    playerControls.dualCharm = false;
                    playerControls.maxPokemonOut--;
                    break;
                default:
                    Debug.LogError("ItemUi.itemName is not yet registered to a matching item");
                    break;
            }
            Unequip(this.weight);
            if (sprite != null)
                playerControls.UnequipItem(sprite);
            else
                Debug.LogError("itemUi.sprite is NULL ", this.gameObject);
            // todo - call to player
        }
        // NOT EQUIP, EQUIP
        else if (!equipped && CanEquip(this.weight) )
        {
            equipped = true;
            switch (itemName)
            {
                case "quickCharm":
                    playerControls.quickCharm = true;
                    break;
                case "chuggerCharm":
                    playerControls.chuggerCharm = true;
                    break;
                case "crisisCharm":
                    playerControls.crisisCharm = true;
                    break;
                case "dualCharm":
                    playerControls.dualCharm = true;
                    playerControls.maxPokemonOut++;
                    break;
                default:
                    Debug.LogError("ItemUi.itemName is not yet registered to a matching item");
                    break;
            }

            Equip(this.weight);

            if (sprite != null)
                playerControls.EquipItem(sprite);
            else
                Debug.LogError("itemUi.sprite is NULL ", this.gameObject);
            // todo - call to player
        }
        
        EquipItemPref();
    }

    public void EquipItemPref()
    {
        if (playerControls != null)
        {
            if (playerControls.equippedItemNames.Contains(itemName))
                playerControls.equippedItemNames.Remove(itemName);
            else
                playerControls.equippedItemNames.Add(itemName);
        }
        else
        {
            Debug.LogError("PlayerControl is not set", this.gameObject);
        }
    }
}

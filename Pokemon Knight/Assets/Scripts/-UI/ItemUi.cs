using UnityEngine;
using UnityEngine.UI;

public class ItemUi : MonoBehaviour
{
    public PlayerControls playerControls;
    public bool equipped;
    public int weight=1;
    public string itemName = "N/A";
    public Sprite sprite;
    [Space] public Button button;
    [TextArea(15,20)] public string itemDesc;


    private bool CanEquip(int itemWeight)
    {
        if ((playerControls.currentWeight + itemWeight) <= playerControls.maxWeight )
            return true;
        Debug.LogError("TOO HEAVY = [" + playerControls.currentWeight + "]  [" + itemWeight + "]");
        return ( (playerControls.currentWeight + itemWeight) <= playerControls.maxWeight );
    }

    private void Equip(int itemWeight)
    {
        playerControls.currentWeight += itemWeight;
        playerControls.weightText.text = playerControls.currentWeight + "/" + playerControls.maxWeight;
    }
    private void Unequip(int itemWeight)
    {
        playerControls.currentWeight -= itemWeight;
        playerControls.weightText.text = playerControls.currentWeight + "/" + playerControls.maxWeight;
    }

    public void TOGGLE_ITEM()
    {
        // ALREADY EQUIPPED, UNEQUIP
        if (equipped)
        {
            equipped = false;
            switch (itemName)
            {
                case "speedScarf":
                    playerControls.speedScarf = false;
                    break;
                case "amberNecklace":
                    playerControls.amberNecklace = false;
                    break;
                case "furyBracelet":
                    playerControls.furyBracelet = false;
                    break;
                case "amethystCharm":
                    playerControls.amethystCharm = false;
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
                case "speedScarf":
                    playerControls.speedScarf = true;
                    break;
                case "amberNecklace":
                    playerControls.amberNecklace = true;
                    break;
                case "furyBracelet":
                    playerControls.furyBracelet = true;
                    break;
                case "amethystCharm":
                    playerControls.amethystCharm = true;
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

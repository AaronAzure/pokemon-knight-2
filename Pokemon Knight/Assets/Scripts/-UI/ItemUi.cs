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
                default:
                    Debug.LogError("ItemUi.itemName is not yet registered to a matching item");
                    break;
            }

            if (sprite != null)
                playerControls.UnequipItem(sprite);
            else
                Debug.LogError("itemUi.sprite is NULL ", this.gameObject);
            // todo - call to player
        }
        // NOT EQUIP, EQUIP
        else
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
                default:
                    Debug.LogError("ItemUi.itemName is not yet registered to a matching item");
                    break;
            }

            if (sprite != null)
                playerControls.EquipItem(sprite);
            else
                Debug.LogError("itemUi.sprite is NULL ", this.gameObject);
            // todo - call to player
        }
    }
}

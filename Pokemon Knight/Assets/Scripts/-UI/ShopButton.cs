// using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine;

public class ShopButton : MonoBehaviour, ISelectHandler
{
    public enum PurchaseType { pokemonType , itemType , spareKeychain };


    public DialogueBox dialogueBox;
    public string purchaseName;
    private int gameNumber;
    public int purchaseCost;
    [Space] public bool isPokemonItem=true;
    [Space] public PurchaseType type=PurchaseType.pokemonType;
    [Space] public Ally ally;
    [Space] public ItemUi item;

    [Space] [SerializeField] protected RectTransform contentPanel;
    [Space] [SerializeField] protected RectTransform panel;
    [SerializeField] private Vector2 anchorOffset;



    // Start is called before the first frame update
    void Start()
    {
        gameNumber = PlayerPrefsElite.GetInt("gameNumber");
        if (PlayerPrefsElite.VerifyArray("purchaseNames" + gameNumber))
        {
            List<string> purchaseNames = new List<string>(PlayerPrefsElite.GetStringArray("purchaseNames" + gameNumber));
            if (purchaseNames.Contains(purchaseName))
                this.gameObject.SetActive(false);
        }
        else
            PlayerPrefsElite.SetStringArray("purchaseNames" + gameNumber, new string[0]);
    }

    public void PURCHASE()
    {
        // CANNOT BUY
        if (dialogueBox == null || !dialogueBox.PlayerHasEnoughCandy(purchaseCost))
            return;
        
        List<string> purchaseNames = new List<string>(PlayerPrefsElite.GetStringArray("purchaseNames" + gameNumber));
        purchaseNames.Add(purchaseName);
        PlayerPrefsElite.SetStringArray("purchaseNames" + gameNumber, purchaseNames.ToArray());

        if (type == PurchaseType.pokemonType && dialogueBox != null)
            dialogueBox.UnlockPokemon(purchaseName, purchaseCost);
        else if (type == PurchaseType.spareKeychain && dialogueBox != null)
            dialogueBox.PurchaseKeychain(purchaseName, purchaseCost);
        else if (type == PurchaseType.itemType && dialogueBox != null)
            dialogueBox.Purchaseitem(purchaseName, purchaseCost);
        
        this.gameObject.SetActive(false);
    }


    public void OnSelect(BaseEventData eventData) 
    {
        contentPanel.anchoredPosition = new Vector2( contentPanel.anchoredPosition.x , Mathf.Abs(panel.anchoredPosition.y));
        contentPanel.anchoredPosition += anchorOffset;
        // Debug.Log(panel.position + " " + panel.anchoredPosition + " " + panel.localPosition);

        if (dialogueBox != null)
        {
            switch (type)
            {
                case PurchaseType.pokemonType:
                    dialogueBox.header.text = ally.moveName;
                    dialogueBox.desc.text = ally.moveDesc;
                    dialogueBox.costTxt.text = purchaseCost.ToString();
                    break;
                case PurchaseType.itemType:
                    dialogueBox.header.text = item.camelisedItemName;
                    dialogueBox.desc.text = item.itemDesc;
                    dialogueBox.costTxt.text = purchaseCost.ToString();
                    break;
                case PurchaseType.spareKeychain:
                    dialogueBox.header.text = "Spare Keychain";
                    dialogueBox.desc.text = "Increases max carry weight for items by 1.";
                    dialogueBox.costTxt.text = purchaseCost.ToString();
                    break;
            }
        }
    }
}

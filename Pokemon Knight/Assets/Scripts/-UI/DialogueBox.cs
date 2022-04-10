using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using TMPro;

public class DialogueBox : MonoBehaviour
{
    private PlayerControls playerControls;
    public Button[] defaultButtons;

    public TextMeshProUGUI header;
    public TextMeshProUGUI desc;
    public TextMeshProUGUI costTxt;
    public TextMeshProUGUI atkPower;
    public TextMeshProUGUI cooldown;
    public TextMeshProUGUI weight;
    


    void Start()
    {
        this.gameObject.SetActive(false);
    }
    
    public void SELECT_DEFAULT_BUTTON()
    {
        foreach (Button defaultButton in defaultButtons)
        {
            if (defaultButton != null && defaultButton.gameObject.activeSelf)
            {
                defaultButton.Select();
                break;
            }
        }
    }

    public void OpenDialogue(PlayerControls player)
    {
        if (playerControls == null)
            playerControls = player;

        this.gameObject.SetActive(true);
        
        SELECT_DEFAULT_BUTTON();
    }

    public bool PlayerHasEnoughCandy(int cost)
    {
        return (playerControls != null && playerControls.currency >= cost);
    }
    
    public void CloseDialogue()
    {
        this.gameObject.SetActive(false);
        
        if (playerControls != null)
        {
            playerControls.inCutscene = false;
            playerControls.dialogue = null;
        }
    }



    public void UnlockPokemon(string pokemonName, int cost)
    {
        playerControls.CaughtAPokemon(pokemonName);
        playerControls.currency -= cost;
        playerControls.currencyTxt.text = playerControls.currency.ToString();

        Debug.Log("saving...");
        playerControls.SaveState(false);

        // SELECT_DEFAULT_BUTTON();
        CloseDialogue();
    }



    public void PurchaseKeychain(string name, int cost)
    {
        playerControls.extraWeight++;
        playerControls.currency -= cost;
        playerControls.currencyTxt.text = playerControls.currency.ToString();
        List<string> keychains = new List<string>( 
            PlayerPrefsElite.GetStringArray("spareKeychain" + PlayerPrefsElite.GetInt("gameNumber"))
        );
        keychains.Add(name);

        PlayerPrefsElite.SetStringArray("spareKeychain" + PlayerPrefsElite.GetInt("gameNumber"), keychains.ToArray());

        playerControls.SaveState(false);
        playerControls.ShowUpgradeAcquired(true);

        CloseDialogue();
    }


    public void Purchaseitem(string name, int cost)
    {
        playerControls.currency -= cost;
        playerControls.currencyTxt.text = playerControls.currency.ToString();

        List<string> temp = new List<string>( 
            PlayerPrefsElite.GetStringArray("itemsObtained" + PlayerPrefsElite.GetInt("gameNumber")) 
        );
        temp.Add(name);
        PlayerPrefsElite.SetStringArray("itemsObtained" + PlayerPrefsElite.GetInt("gameNumber"), temp.ToArray());
        
        playerControls.CheckObtainedItems();

        playerControls.SaveState(false);

        CloseDialogue();
    }
    
}

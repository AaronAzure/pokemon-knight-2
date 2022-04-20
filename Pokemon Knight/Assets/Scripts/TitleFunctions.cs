using System.Collections;
// using System.Collections.Generic;
using Com.LuisPedroFonseca.ProCamera2D;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

using Rewired;
using TMPro;

public class TitleFunctions : MonoBehaviour
{
    private Rewired.Player player;

    [SerializeField] private ProCamera2DTransitionsFX transitionCam;
    [SerializeField] private GameObject[] toDestroy;
    private bool starting;
    private bool canCancel=false;

    [Space] [SerializeField] private Animator anim;
    [SerializeField] private Button firstSaveGameButton;
    [SerializeField] private Button firstStartMenuButton;
    // [Space] [SerializeField] private Button[] newGameButtons;
    [SerializeField] private TextMeshProUGUI[] fileNames;
    
    [Space] [SerializeField] private Button loadedGameFileButton;
    [SerializeField] private TMP_InputField nameField;
    [SerializeField] private TextMeshProUGUI fileNumTxt;
    [SerializeField] private TextMeshProUGUI fileNameTxt;
    [Space] [SerializeField] private TextMeshProUGUI enteredNameTxt;
    [SerializeField] private Button doneNamingButton;

    [Space] public int fileNumber;
    public Button selectedFile;
    public TextMeshProUGUI controlsTxt;
    // public string fileName;
    public bool nintendoControls=true;

    private void Awake() 
    {
        for (int i=0; i<10 ; i++)
            if (!PlayerPrefsElite.VerifyString("gameName" + i))
                PlayerPrefsElite.SetString("gameName" + i, "");
    }

    public void CHANGE_CONTROLS()
    {
        nintendoControls = !nintendoControls;
        // PlayerPrefsElite.SetBoolean("nintendoControls", nintendoControls);
        if (controlsTxt != null)
            if (nintendoControls)
                controlsTxt.text = "Nintendo";
            else
                controlsTxt.text = "Xbox";
    }

    void Start() 
    {
        player = ReInput.players.GetPlayer(0);

        PlayerPrefsElite.SetBoolean("nintendoControls", nintendoControls);
        if (controlsTxt != null)
            controlsTxt.text = "Nintendo";

        // GAME STARTED GETS NEW GAME BUTTON
        for (int i=0 ; i<fileNames.Length ; i++)
        {
            if (PlayerPrefsElite.VerifyBoolean("game-file-" + i.ToString()) 
                && PlayerPrefsElite.GetBoolean("game-file-" + i.ToString())
                && PlayerPrefsElite.VerifyInt("playerLevel" + i.ToString()))
                {
                    fileNames[i].text = "Lv." + (PlayerPrefsElite.GetInt("playerLevel" + i))
                         + " " + PlayerPrefsElite.GetString("gameName" + i);
                    fileNames[i].fontSize = 45;
                }
        }
        canCancel = false;
    }

    private void Update() 
    {
        if ( canCancel && player.GetButtonDown("B"))
        {
            if (EventSystem.current.currentSelectedGameObject == nameField.gameObject)
                doneNamingButton.Select();
            else
                anim.SetTrigger("back");
            // SelectDefaultButton();
        }
    }
    public void SELECT_DEFAULT_BUTTON()
    {
        if      (firstSaveGameButton.IsInteractable())
            firstSaveGameButton.Select();
        else if (firstStartMenuButton.IsInteractable())
            firstStartMenuButton.Select();
        else if (loadedGameFileButton.IsInteractable())
            loadedGameFileButton.Select();
    }
    public void SELECT_BUTTON(Button button)
    {
        button.Select();
    }

    public void BACK()
    {
        if (EventSystem.current.currentSelectedGameObject == nameField.gameObject)
            doneNamingButton.Select();
        else
            anim.SetTrigger("back");
    }
    public void CAN_NOT_EXIT() {canCancel = false;}
    public void CAN_EXIT() {canCancel = true;}
    public void SELECT_NEW_GAME_BUTTON()
    {
        anim.SetTrigger("next");
        firstSaveGameButton.Select();
    }

    public void TO_SAVED_GAMES()
    {
        anim.SetTrigger("next");
        // EventSystem.current.SetSelectedGameObject(null);
        canCancel = false;
        anim.SetBool("toSavedGames", true);
        firstSaveGameButton.Select();
    }
    
    public void START_GAME(int gameNumber)
    {
        // PlayerPrefsElite.GetInt("gameNumber");
        gameNumber = fileNumber;
        PlayerPrefsElite.SetInt("gameNumber", gameNumber);
        PlayerPrefsElite.SetBoolean("game-file-" + gameNumber, true);
        PlayerPrefsElite.SetBoolean("nintendoControls", nintendoControls);
        StartCoroutine( FadeToGame() );
    }
    public void FILE_SELECTED(int gameNumber)
    {
        fileNumber = gameNumber;
        if (PlayerPrefsElite.GetInt("playerLevel" + fileNumber) > 0)
        {
            fileNames[fileNumber].text = "Lv." + (PlayerPrefsElite.GetInt("playerLevel" + fileNumber))
                + " " + PlayerPrefsElite.GetString("gameName" + fileNumber);
            fileNameTxt.text = "Lv." + (PlayerPrefsElite.GetInt("playerLevel" + fileNumber))
                + " " + PlayerPrefsElite.GetString("gameName" + fileNumber);
        }
        if (PlayerPrefsElite.VerifyString("gameName" + fileNumber))
            nameField.text = PlayerPrefsElite.GetString("gameName" + fileNumber);
        else
            nameField.text = "";
        fileNumTxt.text = (fileNumber+1).ToString() + ".";
        loadedGameFileButton.Select();
        anim.SetTrigger("next");
        // anim.SetBool("toFile", true);
    }
    public void NAMING_FILE()
    {
        anim.SetTrigger("next");
        nameField.Select();
        if (PlayerPrefsElite.VerifyString("gameName" + fileNumber))
            nameField.text = PlayerPrefsElite.GetString("gameName" + fileNumber);
        else
            nameField.text = "";
    }
    public void FINISH_TYPING()
    {
        doneNamingButton.Select();
    }
    public void SAVE_NAME()
    {
        string newName = enteredNameTxt.text;
        anim.SetTrigger("back");
        loadedGameFileButton.Select();
        PlayerPrefsElite.SetString("gameName" + fileNumber, newName);
        nameField.text = newName;
        if (PlayerPrefsElite.GetInt("playerLevel" + fileNumber) > 0)
        {
            fileNames[fileNumber].text = "Lv." + (PlayerPrefsElite.GetInt("playerLevel" + fileNumber))
                + " " + PlayerPrefsElite.GetString("gameName" + fileNumber);
            fileNameTxt.text = "Lv." + (PlayerPrefsElite.GetInt("playerLevel" + fileNumber))
                + " " + PlayerPrefsElite.GetString("gameName" + fileNumber);
        }
    }
    public void DisplayOptions()
    {

    }
    public void NEW_GAME(int gameNumber)
    {
        gameNumber = fileNumber;
        PlayerPrefsElite.SetBoolean("game-file-" + gameNumber, true);
        PlayerPrefsElite.SetString("gameName" + gameNumber, enteredNameTxt.text);

        PlayerPrefsElite.SetBoolean("canDoubleJump" + gameNumber.ToString(), false);
        PlayerPrefsElite.SetBoolean("canUseUlt" + gameNumber.ToString(), false);
        
        PlayerPrefsElite.SetBoolean("canDash" + gameNumber.ToString(), false);
        
        PlayerPrefsElite.SetInt("playerLevel" + gameNumber.ToString(), 1);
        
        PlayerPrefsElite.SetInt("playerExp" + gameNumber.ToString(), 0);

        PlayerPrefsElite.DeleteKey("checkpointScene" + gameNumber.ToString());
    
        PlayerPrefsElite.DeleteKey("checkpointPos" + gameNumber.ToString());

        // PLAYER EQUIPPED PREFERENCES
        PlayerPrefsElite.DeleteKey("buttonAllocatedPokemons" + gameNumber.ToString());
        PlayerPrefsElite.DeleteKey("equippedItems" + gameNumber.ToString());

        // ACHIEVEMENTS OR UNLOCKABLES
        if (PlayerPrefsElite.VerifyArray("pokemonsCaught" + gameNumber.ToString()))
            PlayerPrefsElite.SetStringArray("pokemonsCaught" + gameNumber.ToString(), new string[0]);    

        if (PlayerPrefsElite.VerifyArray("roomsBeaten" + gameNumber.ToString()))
            PlayerPrefsElite.SetStringArray("roomsBeaten" + gameNumber.ToString(), new string[0]);    

        if (PlayerPrefsElite.VerifyArray("itemsObtained" + gameNumber.ToString()))
            PlayerPrefsElite.SetStringArray("itemsObtained" + gameNumber.ToString(), new string[0]);
        
        if (PlayerPrefsElite.VerifyArray("subwaysCleared" + gameNumber.ToString()))
            PlayerPrefsElite.SetStringArray("subwaysCleared" + gameNumber.ToString(), new string[0]);
        
        if (PlayerPrefsElite.VerifyArray("crystalsBroken" + gameNumber.ToString()))
            PlayerPrefsElite.SetStringArray("crystalsBroken" + gameNumber.ToString(), new string[0]);
        
        if (PlayerPrefsElite.VerifyArray("berriesCollected" + gameNumber.ToString()))
            PlayerPrefsElite.SetStringArray("berriesCollected" + gameNumber.ToString(), new string[0]);
        
        if (PlayerPrefsElite.VerifyArray("spareKeychain" + gameNumber.ToString()))
            PlayerPrefsElite.SetStringArray("spareKeychain" + gameNumber.ToString(), new string[0]);
        
        if (PlayerPrefsElite.VerifyArray("purchaseNames" + gameNumber.ToString()))
            PlayerPrefsElite.SetStringArray("purchaseNames" + gameNumber.ToString(), new string[0]);

        PlayerPrefsElite.SetBoolean("hasLostBag" + PlayerPrefsElite.GetInt("gameNumber"), false);

        PlayerPrefsElite.SetInt("currency" + gameNumber, 0);
        PlayerPrefsElite.SetInt("bulbasaurLv" + gameNumber, 0);
        PlayerPrefsElite.SetInt("squirtleLv" + gameNumber, 0);
        PlayerPrefsElite.SetInt("charmanderLv" + gameNumber, 0);
        PlayerPrefsElite.SetInt("pidgeyLv" + gameNumber, 0);
        PlayerPrefsElite.SetInt("oddishLv" + gameNumber, 0);
        PlayerPrefsElite.SetInt("butterfreeLv" + gameNumber, 0);
        PlayerPrefsElite.SetInt("tangelaLv" + gameNumber, 0);
        PlayerPrefsElite.SetInt("bellsproutLv" + gameNumber, 0);
        PlayerPrefsElite.SetInt("snorlaxLv" + gameNumber, 0);
        PlayerPrefsElite.SetInt("flareonLv" + gameNumber, 0);
        PlayerPrefsElite.SetInt("eeveeLv" + gameNumber, 0);
        PlayerPrefsElite.SetInt("vaporeonLv" + gameNumber, 0);
        
        
        PlayerPrefsElite.SetStringArray("hiddenRooms" + gameNumber, new string[0]);


        PlayerPrefsElite.SetInt("gameNumber", gameNumber);
        PlayerPrefsElite.SetBoolean("nintendoControls", nintendoControls);
        StartCoroutine( FadeToGame() );
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    IEnumerator FadeToGame()
    {
        if (starting)
            yield break;

        starting = true;
        transitionCam.TransitionExit();
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("Forest 000 (House)");
    }
}

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
    [Space] [SerializeField] private Button[] newGameButtons;
    [SerializeField] private TextMeshProUGUI[] fileNames;

    void Start() 
    {
        player = ReInput.players.GetPlayer(0);

        for (int i=0 ; i<4 ; i++)
        {
            // DOESN'T ALREADY EXIST
            if (!PlayerPrefsElite.VerifyBoolean("game-file-" + i.ToString()))
            {
                PlayerPrefsElite.SetBoolean("game-file-" + i.ToString(), false);
                if (newGameButtons != null && newGameButtons.Length > i)
                    newGameButtons[i].gameObject.SetActive(false);
            }
            // ALREADY EXIST
            else if (!PlayerPrefsElite.GetBoolean("game-file-" + i.ToString()))
            {
                if (newGameButtons != null && newGameButtons.Length > i)
                    newGameButtons[i].gameObject.SetActive(false);
            }
        }

        // GAME STARTED GETS NEW GAME BUTTON
        for (int i=0 ; i<fileNames.Length ; i++)
        {
            if (PlayerPrefsElite.VerifyBoolean("game-file-" + i.ToString()) 
                && PlayerPrefsElite.GetBoolean("game-file-" + i.ToString())
                && PlayerPrefsElite.VerifyInt("playerLevel" + i.ToString()))
                {
                    fileNames[i].text = "Lv." + (PlayerPrefsElite.GetInt("playerLevel" + i.ToString())) + "";
                    fileNames[i].fontSize = 45;
                }
        }
    }

    private void Update() 
    {
        if ( canCancel && player.GetButtonDown("B") && anim.GetBool("toSavedGames"))    
            BackToStartMenu();
    }

    public void CAN_EXIT() {canCancel = true;}
    public void SELECT_NEW_GAME_BUTTON()
    {
        firstSaveGameButton.Select();
    }

    public void TO_SAVED_GAMES()
    {
        // EventSystem.current.SetSelectedGameObject(null);
        canCancel = false;
        anim.SetBool("toSavedGames", true);
        firstSaveGameButton.Select();
    }
    public void BackToStartMenu()
    {
        anim.SetBool("toSavedGames", false);
        firstStartMenuButton.Select();
    }
    public void START_GAME(int gameNumber)
    {
        // PlayerPrefsElite.GetInt("gameNumber");
        PlayerPrefsElite.SetInt("gameNumber", gameNumber);
        PlayerPrefsElite.SetBoolean("game-file-" + gameNumber, true);
        StartCoroutine( FadeToGame() );
    }
    public void DisplayOptions()
    {

    }
    public void NEW_GAME(int gameNumber)
    {
        PlayerPrefsElite.SetBoolean("game-file-" + gameNumber, true);

        if (PlayerPrefsElite.VerifyBoolean("canDoubleJump" + gameNumber.ToString()))
            PlayerPrefsElite.SetBoolean("canDoubleJump" + gameNumber.ToString(), false);
            
        if (PlayerPrefsElite.VerifyBoolean("canDash" + gameNumber.ToString()))
            PlayerPrefsElite.SetBoolean("canDash" + gameNumber.ToString(), false);
            
        if (PlayerPrefsElite.VerifyInt("playerLevel" + gameNumber.ToString()))
            PlayerPrefsElite.SetInt("playerLevel" + gameNumber.ToString(), 1);
            
        if (PlayerPrefsElite.VerifyInt("playerExp" + gameNumber.ToString()))
            PlayerPrefsElite.SetInt("playerExp" + gameNumber.ToString(), 0);

        if (PlayerPrefsElite.VerifyString("checkpointScene" + gameNumber.ToString()))
            PlayerPrefsElite.DeleteKey("checkpointScene" + gameNumber.ToString());
        
        if (PlayerPrefsElite.VerifyVector3("checkpointPos" + gameNumber.ToString()))
            PlayerPrefsElite.DeleteKey("checkpointPos" + gameNumber.ToString());

        if (PlayerPrefsElite.VerifyArray("buttonAllocatedPokemons" + gameNumber.ToString()))
            PlayerPrefsElite.DeleteKey("buttonAllocatedPokemons" + gameNumber.ToString());
        
        if (PlayerPrefsElite.VerifyArray("equippedItems" + gameNumber.ToString()))
            PlayerPrefsElite.DeleteKey("equippedItems" + gameNumber.ToString());

        // Non-player related (Wave)
        if (PlayerPrefsElite.VerifyArray("roomsBeaten" + gameNumber.ToString()))
            PlayerPrefsElite.SetStringArray("roomsBeaten" + gameNumber.ToString(), new string[100]);    

        if (PlayerPrefsElite.VerifyArray("roomsBeaten" + gameNumber.ToString()))
            PlayerPrefsElite.SetStringArray("pokemonsCaught" + gameNumber.ToString(), new string[100]);    

        if (PlayerPrefsElite.VerifyArray("itemsObtained" + gameNumber.ToString()))
            PlayerPrefsElite.SetStringArray("itemsObtained" + gameNumber.ToString(), new string[50]);
        
        if (PlayerPrefsElite.VerifyArray("berries" + gameNumber.ToString()))
            PlayerPrefsElite.SetStringArray("berries" + gameNumber.ToString(), new string[20]);
        
        if (PlayerPrefsElite.VerifyArray("spareKeychain" + gameNumber.ToString()))
            PlayerPrefsElite.SetStringArray("spareKeychain" + gameNumber.ToString(), new string[20]);

        PlayerPrefsElite.SetInt("gameNumber", gameNumber);
        StartCoroutine( FadeToGame() );
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    IEnumerator FadeToGame()
    {
        // if (toDestroy.Length > 0)
        //     foreach (GameObject obj in toDestroy)
        //         Destroy(obj);
        // Can only press once
        if (starting)
            yield break;

        starting = true;
        transitionCam.TransitionExit();
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("Forest 000 (House)");
    }
}

using System.Collections;
using System.Collections.Generic;
using Com.LuisPedroFonseca.ProCamera2D;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleFunctions : MonoBehaviour
{
    [SerializeField] private ProCamera2DTransitionsFX transitionCam;
    [SerializeField] private GameObject[] toDestroy;
    private bool starting;

    void Start() 
    {
        if (!PlayerPrefsElite.VerifyArray("roomsBeaten"))
        {
            PlayerPrefsElite.SetStringArray("roomsBeaten", new string[100]);
        }
    }

    public void StartGame()
    {
        StartCoroutine( FadeToGame() );
    }
    public void DisplayOptions()
    {

    }
    public void ResetPrefs()
    {
        if (PlayerPrefsElite.VerifyBoolean("canDoubleJump"))
            PlayerPrefsElite.SetBoolean("canDoubleJump", false);
            
        if (PlayerPrefsElite.VerifyBoolean("canDash"))
            PlayerPrefsElite.SetBoolean("canDash", false);
            
        if (PlayerPrefsElite.VerifyInt("playerLevel"))
            PlayerPrefsElite.SetInt("playerLevel", 1);
            
        if (PlayerPrefsElite.VerifyInt("playerExp"))
            PlayerPrefsElite.SetInt("playerExp", 0);
        
        if (PlayerPrefsElite.VerifyBoolean("item1"))
            PlayerPrefsElite.SetBoolean("item1", false);

        if (PlayerPrefsElite.VerifyString("checkpointScene"))
            PlayerPrefsElite.DeleteKey("checkpointScene");
        
        if (PlayerPrefsElite.VerifyVector3("checkpointPos"))
            PlayerPrefsElite.DeleteKey("checkpointPos");


        if (PlayerPrefsElite.VerifyArray("buttonAllocatedPokemons"))
            PlayerPrefsElite.DeleteKey("buttonAllocatedPokemons");

        // Non-player related (Wave)
        if (PlayerPrefsElite.VerifyArray("roomsBeaten"))
        {
            // Clear
            PlayerPrefsElite.SetStringArray("roomsBeaten", new string[100]);    
        }
        if (PlayerPrefsElite.VerifyArray("roomsBeaten"))
        {
            // Clear
            PlayerPrefsElite.SetStringArray("pokemonsCaught", new string[100]);    
        }

        if (PlayerPrefsElite.VerifyArray("itemsObtained"))
            PlayerPrefsElite.SetStringArray("itemsObtained", new string[50]);

        StartCoroutine( FadeToGame() );
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    IEnumerator FadeToGame()
    {
        if (toDestroy.Length > 0)
            foreach (GameObject obj in toDestroy)
                Destroy(obj);
        // Can only press once
        if (starting)
            yield break;

        starting = true;
        transitionCam.TransitionExit();
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("Forest 000 (House)");
    }
}

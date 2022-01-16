using System.Collections;
using System.Collections.Generic;
using Com.LuisPedroFonseca.ProCamera2D;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TitleFunctions : MonoBehaviour
{
    [SerializeField] private ProCamera2DTransitionsFX transitionCam;
    private bool starting;


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
    }
    public void QuitGame()
    {
        Application.Quit();
    }
    IEnumerator FadeToGame()
    {
        // Can only press once
        if (starting)
            yield break;

        starting = true;
        transitionCam.TransitionExit();
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene("Scene 000");
    }
}

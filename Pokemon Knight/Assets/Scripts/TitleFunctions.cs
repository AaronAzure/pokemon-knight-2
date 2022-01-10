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

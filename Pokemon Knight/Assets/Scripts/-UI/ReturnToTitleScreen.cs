using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ReturnToTitleScreen : MonoBehaviour
{
    [SerializeField] private Animator transitionAnim;
    public MusicManager musicManager;
    public GameObject toDestroy;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ReturnToTitle()
    {
        StartCoroutine( ReturningToTitleScreen() );
    }

    IEnumerator ReturningToTitleScreen()
    {
        if (transitionAnim != null)
            transitionAnim.SetTrigger("toBlack");

        yield return new WaitForSeconds(1f);
        SceneManager.LoadScene("0Title");
        if (toDestroy != null)
            Destroy(toDestroy);
        
        if (musicManager != null)
            musicManager.BackToTitle();

        yield return new WaitForSeconds(0.5f);
        if (transitionAnim != null)
            transitionAnim.SetTrigger("fromBlack");

        yield return new WaitForSeconds(0.5f);
        Destroy(this.gameObject);
    }
}

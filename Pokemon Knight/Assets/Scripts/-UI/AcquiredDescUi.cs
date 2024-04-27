using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AcquiredDescUi : MonoBehaviour
{
    public Animator anim;
    public bool canMoveAfterAnimation;

    [Space] public PlayerControls player;
    public Image acqImg;
    public TextMeshProUGUI headerTxt;
    public TextMeshProUGUI descTxt;
    // [Space] public Animator subsequentDesc;
    public AudioSource soundEffect;

    private void OnEnable() 
    {
        if (soundEffect != null)
            soundEffect.Play();
        StartCoroutine( player.AllowReadDescriptionOfAcquiredTime() );
    }
    

    public void PAUSE()
    {
        player.PAUSE_GAME();
    }


    public void RESUME()
    {
        this.gameObject.SetActive(false);

        Time.timeScale = 1;
        player.CLOSE_DESC_AND_RESUME(!canMoveAfterAnimation);
    }
}

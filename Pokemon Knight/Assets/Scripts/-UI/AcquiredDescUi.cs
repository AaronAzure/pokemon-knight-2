using UnityEngine;

public class AcquiredDescUi : MonoBehaviour
{
    public PlayerControls player;
    public Animator subsequentDesc;
    
    public void PAUSE()
    {
        player.PAUSE_GAME();
    }

    
    public void RESUME()
    {
        this.gameObject.SetActive(false);

        if (subsequentDesc == null)
        {
            Time.timeScale = 1;
            player.CLOSE_DESC_AND_RESUME();
        }
        else
        {
            subsequentDesc.gameObject.SetActive(true);
            player.descAnim = subsequentDesc;
        }
    }
}

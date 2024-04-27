using UnityEngine;

public class PowerupScreen : MonoBehaviour
{
    [SerializeField] private PlayerControls player;

    public void ResumeMovement()
    {
        player.FinishedCutscene();
        this.gameObject.SetActive(false);
    }
    public void UNPAUSE()
    {
        Time.timeScale = 1;
        this.gameObject.SetActive(false);
        if (player != null)
            player.Resume();
    }

    public void SELECT_DEFAULT_STATION(){
        player.SELECT_DEFAULT_STATION();
    }
}

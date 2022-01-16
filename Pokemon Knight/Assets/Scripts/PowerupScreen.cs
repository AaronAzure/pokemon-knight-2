using UnityEngine;

public class PowerupScreen : MonoBehaviour
{
    [SerializeField] private PlayerControls player;

    public void ResumeMovement()
    {
        player.FinishedCutscene();
        this.gameObject.SetActive(false);
    }
    public void Unpause()
    {
        Time.timeScale = 1;
        this.gameObject.SetActive(false);
    }
}

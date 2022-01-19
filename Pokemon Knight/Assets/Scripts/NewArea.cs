using UnityEngine;

public class NewArea : MonoBehaviour
{
    public string nextScene;
    public Vector2 nextPos;
    public bool isDoorExit;

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.tag == "Player" && Application.CanStreamedLevelBeLoaded(nextScene))
        {
            PlayerControls player = other.GetComponent<PlayerControls>();
            player.SetNextArea(nextScene, nextPos, isDoorExit);
        }
    }
}

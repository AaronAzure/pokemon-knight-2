using UnityEngine;

public class NewArea : MonoBehaviour
{
    public string nextScene;
    public float xPos;
    public float yPos;
    public bool walkLeft;

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.tag == "Player" && Application.CanStreamedLevelBeLoaded(nextScene))
        {
            PlayerControls player = other.GetComponent<PlayerControls>();
            player.SetNextArea(nextScene, xPos, yPos, walkLeft);
        }
    }
}

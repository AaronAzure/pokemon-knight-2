using UnityEngine;

public class Subway : MonoBehaviour
{
    private PlayerControls playerControls;


    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))
        {
            if (playerControls == null)
                playerControls = other.GetComponent<PlayerControls>();
            
            if (playerControls != null)
            {
                playerControls.canTakeSubway = true;
                // playerControls.newScenePos = playerControls.transform.position + new Vector3(0,0.2f);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))
        {
            if (playerControls == null)
                playerControls = other.GetComponent<PlayerControls>();
            
            if (playerControls != null)
                playerControls.canTakeSubway = false;
        }
    }
}

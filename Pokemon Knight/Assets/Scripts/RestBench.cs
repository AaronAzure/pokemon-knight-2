using UnityEngine;

public class RestBench : MonoBehaviour
{
    private PlayerControls playerControls;


    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))
        {
            if (playerControls == null)
                playerControls = other.GetComponent<PlayerControls>();
            
            if (playerControls != null && !playerControls.inCutscene)
            {
                playerControls.canRest = true;
                playerControls.restBench = this.transform;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))
        {
            if (playerControls == null)
                playerControls = other.GetComponent<PlayerControls>();
            
            if (playerControls != null && !playerControls.inCutscene)
            {
                playerControls.canRest = false;
                playerControls.restBench = this.transform;
            }
        }
    }
}

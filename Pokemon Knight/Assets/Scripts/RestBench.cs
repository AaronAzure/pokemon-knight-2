using UnityEngine;

public class RestBench : MonoBehaviour
{
    [SerializeField] private PlayerControls playerControls;
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))
        {
            if (playerControls == null)
                playerControls = other.GetComponent<PlayerControls>();
            
            if (playerControls != null)
                playerControls.canRest = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))
        {
            if (playerControls == null)
                playerControls = other.GetComponent<PlayerControls>();
            
            if (playerControls != null)
                playerControls.canRest = false;
        }
    }
}

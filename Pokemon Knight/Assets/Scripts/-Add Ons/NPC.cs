using UnityEngine;

public class NPC : MonoBehaviour
{
    private PlayerControls playerControls;
    public DialogueBox dialogueBox;



    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))
        {
            if (playerControls == null)
                playerControls = other.GetComponent<PlayerControls>();
            
            playerControls.dialogue = dialogueBox;
            playerControls.talkingToNpc = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))
        {
            if (playerControls == null)
                playerControls = other.GetComponent<PlayerControls>();
            
            playerControls.dialogue = null;
            playerControls.talkingToNpc = false;
        }
    }
}

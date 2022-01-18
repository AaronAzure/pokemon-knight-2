using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    public string newSceneName;
    public Vector2 newPos;
    [Tooltip("true = moving left\nfalse = moving right")] public bool moveLeftFromDoor;

    private PlayerControls playerControls;


    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))
        {
            if (playerControls == null)
                playerControls = other.GetComponent<PlayerControls>();
            
            if (playerControls != null)
            {
                playerControls.canEnter = true;
                playerControls.newSceneName = this.newSceneName;
                playerControls.newScenePos = this.newPos;
                playerControls.moveLeftFromDoor = this.moveLeftFromDoor;
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
                playerControls.canEnter = false;
        }
    }
}

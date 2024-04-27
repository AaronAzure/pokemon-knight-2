using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInArea : MonoBehaviour
{
    public Enemy enemy;
    
    
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player") && enemy != null)    
        {
            enemy.playerInBossRoom = true;
        }
    }
    
    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.CompareTag("Player") && enemy != null)    
        {
            enemy.playerInBossRoom = false;
        }
    }
}

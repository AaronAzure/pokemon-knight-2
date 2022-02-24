using UnityEngine;

public class EnemyCloseRange : MonoBehaviour
{
    [SerializeField] private Enemy enemy;

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player") && enemy != null)
        {
            enemy.playerInCloseRange = true;    
        }
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.CompareTag("Player") && enemy != null)
        {
            enemy.playerInCloseRange = false;    
        }
    }
}

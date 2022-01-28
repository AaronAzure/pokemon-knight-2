using UnityEngine;

public class EnemyFieldOfVision : MonoBehaviour
{
    [SerializeField] private Enemy enemy;

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player") && enemy != null)
        {
            enemy.playerInField = true;    
        }
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.CompareTag("Player") && enemy != null)
        {
            enemy.playerInSight = false;    
            enemy.playerInField = false;    
            enemy.alert.SetActive(false);
        }
    }
}

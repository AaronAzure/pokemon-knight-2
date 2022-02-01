using UnityEngine;

public class EnemyFieldOfVision : MonoBehaviour
{
    [SerializeField] private Enemy enemy;
    [SerializeField] private bool delaySearch;

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player") && enemy != null)
        {
            enemy.playerInField = true;    
            enemy.CallChildOnTargetFound();
        }
    }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.CompareTag("Player") && enemy != null)
        {
            if (!delaySearch)
            {
                enemy.playerInSight = false;    
                enemy.playerInField = false;    
                enemy.alert.SetActive(false);
            }
            else
            {
                enemy.CallChildOnTargetLost();
            }
        }
    }
}

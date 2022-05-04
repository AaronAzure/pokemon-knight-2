using UnityEngine;

public class EnemyFieldOfVision : MonoBehaviour
{
    [SerializeField] private Enemy enemy;
    [SerializeField] private bool neverLoseSight;
    [Space] [SerializeField] private bool delaySearch;
    [Space] [SerializeField] private bool hideAlert=true;
    // [Space] private bool onTriggerStay2D;

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player") && enemy != null)
        {
            enemy.playerInField = true;    
            enemy.keepSearching = true;
            enemy.CallChildOnTargetFound();
        }
    }

    // private void OnTriggerStay2D(Collider2D other) 
    // {
    //     if (onTriggerStay2D && other.CompareTag("Player") && enemy != null)
    //     {
    //         enemy.playerInField = true;    
    //         enemy.CallChildOnTargetFound();
    //     }
    // }

    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.CompareTag("Player") && enemy != null)
        {
            if (!neverLoseSight)
            {
                if (!delaySearch)
                {
                    enemy.playerInSight = false;    
                    enemy.playerInField = false;    
                    if (hideAlert)
                        enemy.alert.SetActive(false);
                }
                else
                {
                    enemy.CallChildOnTargetLost();
                    enemy.keepSearching = false;
                }
            }
        }
    }
}

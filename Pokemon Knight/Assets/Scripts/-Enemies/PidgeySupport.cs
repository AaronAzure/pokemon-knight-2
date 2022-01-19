using UnityEngine;

public class PidgeySupport : MonoBehaviour
{   
    [SerializeField] private Pidgey pidgey;


    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player") && pidgey != null)
        {
            if (pidgey.target == null)
                pidgey.target = other.transform;
            pidgey.playerInRange = true;
            pidgey.anim.speed = pidgey.chaseSpeed;
        }    
    }
    
    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.CompareTag("Player") && pidgey != null)
        {
            pidgey.playerInRange = false;
            pidgey.anim.speed = 1;
        }    
    }
}

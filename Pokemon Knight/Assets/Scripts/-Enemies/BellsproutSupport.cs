using UnityEngine;

public class BellsproutSupport : MonoBehaviour
{
    [SerializeField] private Bellsprout bellsprout;
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player") && bellsprout != null)
        {
            if (bellsprout.target == null)
                bellsprout.target = other.transform;
            bellsprout.playerInRange = true;
            bellsprout.anim.SetTrigger("walking");
            if (bellsprout.canUseBuffs)
                bellsprout.anim.SetTrigger("growth");

            bellsprout.anim.speed = bellsprout.chaseSpeed;
        }    
    }
    
    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.CompareTag("Player") && bellsprout != null)
        {
            bellsprout.playerInRange = false;
            bellsprout.anim.speed = 1;
        }    
    }
}

using UnityEngine;

public class BellsproutSupport : MonoBehaviour
{
    [SerializeField] private Bellsprout bellsprout;
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player") && bellsprout != null)
        {
            bellsprout.target = other.transform;
            bellsprout.chasing = true;
            bellsprout.anim.speed = bellsprout.chaseSpeed;
        }    
    }
    
    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.CompareTag("Player") && bellsprout != null)
        {
            bellsprout.chasing = false;
            bellsprout.anim.speed = 1;
        }    
    }
}

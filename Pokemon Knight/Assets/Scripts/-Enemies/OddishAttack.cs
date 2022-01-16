using UnityEngine;

public class OddishAttack : MonoBehaviour
{
    [SerializeField] private Oddish oddish;

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))
            oddish.canSeePlayer = true;
    }
    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))
            oddish.canSeePlayer = false;
    }
}

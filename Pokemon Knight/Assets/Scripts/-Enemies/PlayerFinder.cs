using UnityEngine;

public class PlayerFinder : MonoBehaviour
{
    [SerializeField] private Weepinbell weepinbell;
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player") && weepinbell != null)
        {
            if (weepinbell.target == null)
                weepinbell.target = other.transform;
            // weepinbell.anim.SetTrigger("walking");
        }    
    }
}

using UnityEngine;

public class Water : MonoBehaviour
{
    [SerializeField] private BuoyancyEffector2D buo;
    private PlayerControls pc;

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))    
        {
            if (pc == null)
                pc = other.GetComponent<PlayerControls>();
            
            if (pc.canSwim)
                buo.density = 1;
            else 
                buo.density = 2;

            this.enabled = false;
        }
    }
}

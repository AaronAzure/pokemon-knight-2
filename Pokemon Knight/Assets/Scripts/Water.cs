using UnityEngine;

public class Water : MonoBehaviour
{
    [SerializeField] private BuoyancyEffector2D[] buo;
    private PlayerControls pc;

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))    
        {
            if (pc == null)
                pc = other.GetComponent<PlayerControls>();
            

            if (pc.canSwim)
                foreach (BuoyancyEffector2D b in buo)
                    b.density = 1;
            else 
                foreach (BuoyancyEffector2D b in buo)
                    b.density = 1.5f;


            this.enabled = false;
        }
    }
}

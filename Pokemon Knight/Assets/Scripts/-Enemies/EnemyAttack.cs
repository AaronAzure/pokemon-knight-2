using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public int atkDmg=3;
    public float kbForce;


    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))    
        {
            // HAS KNOCKBACK
            if (kbForce > 0)
                other.GetComponent<PlayerControls>().TakeDamage(atkDmg, this.transform, kbForce);
            // NO KNOCKBACK
            else
                other.GetComponent<PlayerControls>().TakeDamage(atkDmg);
        }
    }
}

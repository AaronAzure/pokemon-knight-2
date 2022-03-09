using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public int atkDmg=3;
    public float kbForce;
    [Space] public bool ignoreInvincible;



    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))    
        {
            // HAS KNOCKBACK
            other.GetComponent<PlayerControls>().TakeDamage(atkDmg, this.transform, kbForce, ignoreInvincible);
            //// if (kbForce > 0)
            // // NO KNOCKBACK
            //// else
            ////     other.GetComponent<PlayerControls>().TakeDamage(atkDmg);
        }
    }
}

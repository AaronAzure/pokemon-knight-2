using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public int atkDmg=10;
    public float kbForce=10;
    public GameObject explosion;
    public EnemyProjectile extraProjectile;
    public Rigidbody2D body;
    public Vector2 direction;
    public float speed;

    void Start()
    {
        if (body != null)
            body.velocity = direction * speed;
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))    
        {
            other.GetComponent<PlayerControls>().TakeDamage(atkDmg, this.transform, kbForce);
            if (explosion != null)
                Instantiate(explosion, this.transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
        if (other.CompareTag("Ground"))    
        {
            if (extraProjectile != null)
            {
                var obj = Instantiate(extraProjectile, this.transform.position, extraProjectile.transform.rotation);
                obj.direction = Vector2.down;
                
                obj = Instantiate(extraProjectile, this.transform.position, extraProjectile.transform.rotation);
                obj.direction = Vector2.up;
                
                obj = Instantiate(extraProjectile, this.transform.position, extraProjectile.transform.rotation);
                obj.direction = Vector2.left;
                
                obj = Instantiate(extraProjectile, this.transform.position, extraProjectile.transform.rotation);
                obj.direction = Vector2.right;
            }
            

            if (explosion != null)
                Instantiate(explosion, this.transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
}

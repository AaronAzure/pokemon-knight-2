using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public int atkDmg=10;
    public float kbForce=10;
    [Space] public bool destoryOnPlayerCollision;
    [Space] public bool destoryOnWallCollision;
    public GameObject explosion;
    public Rigidbody2D body;
    public Vector2 direction;
    public float speed;
    [Space] public bool sleepEffect;
    [Space] public float sleepDelay;
    public Animator anim;

    void Start()
    {
        if (body != null)
            body.velocity = direction * speed;
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (destoryOnPlayerCollision && other.CompareTag("Player"))    
        {
            PlayerControls player = other.GetComponent<PlayerControls>();
            player.TakeDamage(atkDmg, this.transform, kbForce);
            if (explosion != null)
                Instantiate(explosion, this.transform.position, Quaternion.identity);
            if (sleepEffect)
                player.PutToSleep(sleepDelay);
            Destroy(this.gameObject);
        }
        if (destoryOnWallCollision && other.CompareTag("Ground"))    
        {
            if (explosion != null)
                Instantiate(explosion, this.transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
}

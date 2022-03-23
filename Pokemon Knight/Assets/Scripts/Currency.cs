using System.Collections;
using UnityEngine;

public class Currency : MonoBehaviour
{
    public int value=1;
    public Rigidbody2D body;
    public Transform target;
    [SerializeField] private float speed = 2.5f;
    private float origGrav;
    private bool canMagnetise=false;
    public bool cannotMagnetise=false;


    private void Start() 
    {
        if (body != null)
            origGrav = body.gravityScale;
        
        if (canMagnetise)
            this.enabled = false;
        StartCoroutine( Magnetise() );
    }

    private void FixedUpdate() 
    {
        if (canMagnetise && target != null)
        {
            float step = speed * Time.fixedDeltaTime;
            transform.position = Vector2.MoveTowards(transform.position, target.position, step);    
            if (body.gravityScale != 0)
                body.gravityScale = 0;
        }
    }
    
    public void CollectedBy(PlayerControls player) 
    {
        player.GainCandy(value);
        Destroy(this.gameObject);
    }

    IEnumerator Magnetise()
    {
        yield return new WaitForSeconds(1);
        if (target != null)
            body.velocity = Vector2.zero;
        canMagnetise = true;
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Magnet"))    
        {
            target = other.transform;
            if (canMagnetise)
            {
                body.velocity = Vector2.zero;
                body.gravityScale = 0;
            }
        }
    }
    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.CompareTag("Magnet"))    
        {
            target = null;
            if (canMagnetise)
                body.gravityScale = origGrav;
        }
    }
}

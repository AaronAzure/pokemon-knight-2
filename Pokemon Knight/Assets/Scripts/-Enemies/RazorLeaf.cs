using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RazorLeaf : MonoBehaviour
{
    public int atkDmg=30;
    public GameObject explosion;
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
            other.GetComponent<PlayerControls>().TakeDamage(atkDmg);
            if (explosion != null)
                Instantiate(explosion, this.transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
        if (other.CompareTag("Ground"))    
        {
            if (explosion != null)
                Instantiate(explosion, this.transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }

}

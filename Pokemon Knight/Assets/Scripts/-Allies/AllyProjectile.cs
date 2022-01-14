using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyProjectile : MonoBehaviour
{
    public int atkDmg;
    public int atkForce;    // knockback force

    [SerializeField] private GameObject trailObj;
    [SerializeField] private GameObject explosionObj;

    public float velocity=0;
    public Rigidbody2D body;


    private void Start() 
    {
        StartCoroutine(UnparentTrail());
        if (body != null && velocity != 0)
            body.velocity = Vector2.right * velocity;
    }
    public IEnumerator UnparentTrail()
    {
        yield return new WaitForSeconds(0.5f);
        if (trailObj != null) 
            trailObj.transform.parent = null;

        yield return new WaitForEndOfFrame();
        if (this.gameObject != null)
            Destroy(this.gameObject);
    }
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Ground") || other.CompareTag("Tree") || other.CompareTag("Enemy"))
        {
            if (trailObj != null) 
                trailObj.transform.parent = null;
            if (other.tag == "Enemy")
            {
                Component[] scripts = other.GetComponents(typeof(Enemy));
                foreach (var script in scripts)
                {
                    script.GetComponent<Enemy>().TakeDamage(atkDmg, this.transform, atkForce);
                    // if (explosionObj != null)
                    // {
                    //     var obj = Instantiate(explosionObj, script.gameObject.transform.position, Quaternion.identity);
                    //     Destroy(obj.gameObject, 0.5f);
                    // }
                }
            }
            if (explosionObj != null)
            {
                var obj = Instantiate(explosionObj, this.transform.position, Quaternion.identity);
                Destroy(obj.gameObject, 0.5f);
            }
            Destroy(this.gameObject);
        }
    }

}

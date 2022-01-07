using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyAttack : MonoBehaviour
{
    // public Transform atkPos;
    // public LayerMask whatIsEnemy;
    // public float atkRange;
    public int atkDmg;
    public int atkForce;    // knockback force

    [SerializeField] private GameObject physicalAtkObj;
    [SerializeField] private bool physicalAtk;

    public float velocity=0;
    public Rigidbody2D body;


    private void Start() 
    {
        if (body != null && velocity != 0)
            body.velocity = Vector2.right * velocity;
    }
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.tag == "Enemy")
        {
            // Debug.Log("-- logging " + other.name);
            Component[] scripts = other.GetComponents(typeof(Enemy));
            foreach (var script in scripts)
            {
                // Debug.Log(script.name + "  -  " + script.GetType());
                script.GetComponent<Enemy>().TakeDamage(atkDmg, this.transform, atkForce);
                if (physicalAtk && physicalAtkObj != null)
                {
                    var obj = Instantiate(physicalAtkObj, script.gameObject.transform.position, Quaternion.identity);
                    Destroy(obj.gameObject, 0.5f);
                }
                // script.SendMessage("TakeDamage", atkPower, SendMessageOptions.DontRequireReceiver); //! SendMessage = calling methods from unknown classes
            }
        }
    }

}

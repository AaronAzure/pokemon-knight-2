using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ally : MonoBehaviour
{
    public AllyAttack hitbox;  // Separate gameobject with collider
    public int atkDmg;
    public int atkForce;
    public float outTime = 0.5f;    // Time pokemon appears in the overworld
    public float resummonTime = 0.5f;    // Delay before calling pokemon again

    [Space] public Rigidbody2D body;


    // Start is called before the first frame update
    void Start()
    {
        if (hitbox != null)
        {
            hitbox.atkDmg = this.atkDmg;
            hitbox.atkForce = this.atkForce;
        }

        StartCoroutine( BackToBall() );
    }

    protected IEnumerator BackToBall()
    {
        yield return new WaitForSeconds(outTime);
        Destroy(this.gameObject);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metapod : Enemy
{
    [Space] [Header("Metapod")]
    // public float distanceDetect=2f;
    public LayerMask whatIsPlayer;
    private bool once;


    // Start is called before the first frame update
    public override void Start()
    {
        Setup();
        if (body != null) body.gravityScale = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!once)
        {
            // RaycastHit2D playerInfo = Physics2D.Linecast(this.transform.position, Vector2.down, 10, whatIsPlayer);
            RaycastHit2D playerInfo = Physics2D.Linecast(this.transform.position, this.transform.position + new Vector3(0,-10), whatIsPlayer);
            RaycastHit2D playerInfoRight = Physics2D.Linecast(
                this.transform.position + new Vector3(1,0), this.transform.position + new Vector3(1,-10), whatIsPlayer);
            RaycastHit2D playerInfoLeft = Physics2D.Linecast(
                this.transform.position + new Vector3(-1,0), this.transform.position + new Vector3(-1,-10), whatIsPlayer);

            // Player underneath or been hit
            if (playerInfo || playerInfoRight || playerInfoLeft || body.velocity != Vector2.zero)
            {
                once = true;
                body.gravityScale = 3;
            }
        }

    }
}

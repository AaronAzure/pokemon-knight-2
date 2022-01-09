using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Caterpie : Enemy
{
    public float moveSpeedf=5;
    public float distanceDetect=2f;
    [SerializeField] private bool movingRight = false;
    public Transform groundDetection;
    public float forwardDetect=2f;
    public Transform face;


    void FixedUpdate() 
    {
        if (!receivingKnockback)
            body.velocity = new Vector2(-moveSpeed, body.velocity.y);
        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, distanceDetect, whatIsGround);
        RaycastHit2D frontInfo = Physics2D.Raycast(groundDetection.position, Vector2.left, distanceDetect, whatIsGround);

        //* If at edge, then turn around
        if (!groundInfo || frontInfo)
        // if (!groundInfo)
            Flip();
    }

    private void Flip()
    {
        model.transform.localScale = new Vector3(-model.transform.localScale.x, model.transform.localScale.y, 1);
        moveSpeed *= -1;
    }

    // private void OnDrawGizmosSelected() 
    // {
    //     Gizmos.color = Color.blue;
    //     Gizmos.DrawLine(groundDetection.position, groundDetection.position + new Vector3(0,-distanceDetect));
    //     Gizmos.DrawLine(face.position, face.position + new Vector3(-forwardDetect,0));
    // }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Caterpie : Enemy
{
    public float moveSpeed=5;
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
        if (model.transform.eulerAngles.y != 0)
            model.transform.eulerAngles = new Vector3(0, 0);
        else
            model.transform.eulerAngles = new Vector3(0, 180);
        moveSpeed *= -1;
    }

    private void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(groundDetection.position, groundDetection.position + new Vector3(0,-distanceDetect));
        Gizmos.DrawLine(face.position, face.position + new Vector3(-forwardDetect,0));
    }
}

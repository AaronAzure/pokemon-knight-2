using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metapod : Enemy
{
    [Space] [Header("Metapod")] 
    public float distanceDetect=2f;
    public Transform groundDetection;
    [SerializeField] private LayerMask whatIsPlayer;
    

    void Start() {
        
    }

    void FixedUpdate() 
    {
        if (!receivingKnockback)
            body.velocity = new Vector2(-moveSpeed, body.velocity.y);
        RaycastHit2D playerDetect = Physics2D.Raycast(groundDetection.position, Vector2.down, distanceDetect, whatIsPlayer);

        //* If at edge, then turn around
        if (playerDetect)
            Fall();
    }

    private void Fall()
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

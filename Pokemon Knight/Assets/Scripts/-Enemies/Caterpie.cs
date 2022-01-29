﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Caterpie : Enemy
{
    [Space] [Header("Caterpie")] public float moveSpeed=5;
    public float distanceDetect=2f;
    public Transform groundDetection;
    [SerializeField] private LayerMask whatIsTree;
    public float forwardDetect=2f;
    public Transform face;

    

    void FixedUpdate() 
    {
        if (!receivingKnockback)
        {
            if (model.transform.eulerAngles.y != 0)
                body.velocity = new Vector2( moveSpeed, body.velocity.y);
            else
                body.velocity = new Vector2(-moveSpeed, body.velocity.y);
        }
        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, distanceDetect, whatIsGround);
        RaycastHit2D frontInfo = Physics2D.Raycast(groundDetection.position, Vector2.left, distanceDetect, whatIsGround);
        // RaycastHit2D treeInfo = Physics2D.Raycast(groundDetection.position, Vector2.left, distanceDetect, whatIsTree);

        //* If at edge, then turn around
        if (body.velocity.y >= 0 && (!groundInfo || frontInfo))
            Flip();
    }

    private void Flip()
    {
        if (!canFlip)
            return;
        if (model.transform.eulerAngles.y != 0)
            model.transform.eulerAngles = new Vector3(0, 0);
        else
            model.transform.eulerAngles = new Vector3(0, 180);
        // moveSpeed *= -1;
        StartCoroutine( ResetFlipTimer() );
    }


    private void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(groundDetection.position, groundDetection.position + new Vector3(0,-distanceDetect));
        Gizmos.DrawLine(face.position, face.position + new Vector3(-forwardDetect,0));
    }
}

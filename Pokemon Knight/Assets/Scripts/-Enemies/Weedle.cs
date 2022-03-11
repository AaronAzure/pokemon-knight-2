using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weedle : Enemy
{
    [Space] [Header("Weedle")] public float moveSpeed=2;
    public float distanceDetect=2f;
    public Transform groundDetection;
    public float forwardDetect=1f;
    public Transform face;

    public EnemyProjectile poisonSting;
    public Transform shotPos;

    private LayerMask finalMask;    // detect Player, Ground, ignores Enemy, Bounds
    private Transform target;
    private RaycastHit2D playerInfo;
    private bool attacking;
    private Vector3 trajectory;


    public override void Setup()
    {
        finalMask = (whatIsPlayer | whatIsGround);
        if (alert != null) 
            alert.gameObject.SetActive(false);
        if (target == null && playerControls != null)
            target = playerControls.transform;
        
        if (Random.Range(0,2) == 0)
        {
            movingLeft = true; 
            model.transform.eulerAngles = new Vector3(0, 0);
        }
        else
        {
            movingRight = true; 
            model.transform.eulerAngles = new Vector3(0, 180);
        }
    }

    public override void CallChildOnTargetFound()
    {
        // mainAnim.SetBool("inSight", true);
    }
    public override void CallChildOnTargetLost()
    {
        attacking = false;
        if (alert != null)
            alert.SetActive(false);
        mainAnim.SetBool("inSight", false);
    }
    

    void FixedUpdate() 
    {
        if (!playerInSight)
        {
            if (!receivingKnockback && hp > 0 && !cannotMove)
            {
                if (movingLeft)
                    body.velocity = new Vector2(-moveSpeed, body.velocity.y);
                else if (movingRight)
                    body.velocity = new Vector2( moveSpeed, body.velocity.y);
            }
        }
        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, distanceDetect, whatIsGround);
        RaycastHit2D frontInfo;
        if (model.transform.eulerAngles.y > 0) // right
            frontInfo = Physics2D.Raycast(groundDetection.position, Vector2.right, forwardDetect, whatIsGround);
        else // left
            frontInfo = Physics2D.Raycast(groundDetection.position, Vector2.left, forwardDetect, whatIsGround);

        //* If at edge, then turn around
        if (body.velocity.y >= 0 && (!groundInfo || frontInfo))
            Flip();

        if (target != null && playerInField)
        {
            Vector3 lineOfSight = (target.position + new Vector3(0, 1)) - (this.transform.position + new Vector3(0, 0.5f));
            playerInfo = Physics2D.Linecast(this.transform.position + new Vector3(0, 1),
                this.transform.position + new Vector3(0, 1) + lineOfSight, finalMask);
            if (playerInfo.collider != null && playerInfo.collider.gameObject.CompareTag("Player"))
            {
                attacking = true;
                mainAnim.SetBool("inSight", true);
                if (alert != null) 
                    alert.gameObject.SetActive(true);
            }
            else if (playerInfo.collider != null && !playerInfo.collider.gameObject.CompareTag("Player"))
            {
                CallChildOnTargetLost();
            }

        }
        else if (target != null && !playerInField)
        {
            CallChildOnTargetLost();
        }
    }

    private void Flip()
    {
        if (!canFlip)
            return;

        if (movingRight)
        {
            model.transform.eulerAngles = new Vector3(0, 0);
            movingRight = false;
            movingLeft = true;
        }
        else if (movingLeft)
        {
            model.transform.eulerAngles = new Vector3(0, 180);
            movingRight = true;
            movingLeft = false;
        }
        StartCoroutine( ResetFlipTimer() );
    }


    private void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(groundDetection.position, groundDetection.position + new Vector3(0,-distanceDetect));
        Gizmos.DrawLine(face.position, face.position + new Vector3(-forwardDetect,0));

        // Gizmos.color = Color.magenta;
        // Gizmos.DrawWireCube(feetPos.position, feetBox);

        Gizmos.color = Color.magenta;
        if (target != null)
        {
            Vector3 lineOfSight = (target.position + new Vector3(0, 1)) - (this.transform.position + new Vector3(0, 0.5f));
            Gizmos.DrawLine(this.transform.position + new Vector3(0, 0.5f),
                this.transform.position + new Vector3(0, 0.5f) + lineOfSight);
        }
    }

    public void ATTACKING()
    {
        body.velocity = new Vector2(0,body.velocity.y);
    }
    public void CALCULATE_TRAJECTORY()
    {
        if (target != null)
            trajectory = (target.position + Vector3.up) - shotPos.position;
    }
    public void POISON_STING()
    {
        if (poisonSting != null && hp > 0 && playerInField)
        {
            LookAtPlayer();
            CALCULATE_TRAJECTORY();
            var obj = Instantiate(poisonSting, shotPos.position, poisonSting.transform.rotation);
            obj.body.gravityScale = 0;
            obj.transform.rotation = Quaternion.LookRotation(trajectory);
            obj.direction = trajectory.normalized;
            obj.atkDmg = projectileDmg + calcExtraProjectileDmg;
        }
    }
}

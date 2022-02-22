using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bellsprout : Enemy
{
    [Space] [Header("Bellsprout")]  
    public Animator anim;
    public float moveSpeed=2;
    public float chaseSpeed=5;
    public float maxSpeed=7.5f;
    public float distanceDetect=1f;
    public Transform groundDetection;


    [Header("Attacks")]
    public Transform target;
    public bool chasing;
    public bool playerInRange;
    private Coroutine co;
    private LayerMask finalMask;    // detect Player, Ground, ignores Enemy, Bounds



    public override void Setup()
    {
        co = StartCoroutine( DoSomething() );
        finalMask = (whatIsPlayer | whatIsGround);
        if (alert != null) alert.gameObject.SetActive(false);
    }

    // Start is called before the first frame update
    void FixedUpdate() 
    {
        // Wandering around
        if (!chasing)
        {
            if (!receivingKnockback && hp > 0 && !cannotMove)
            {
                if (movingLeft)
                    body.velocity = new Vector2(-moveSpeed, body.velocity.y);
                else if (movingRight)
                    body.velocity = new Vector2(moveSpeed, body.velocity.y);
            }
            RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, distanceDetect, whatIsGround);
            RaycastHit2D frontInfo;
            if (model.transform.eulerAngles.y > 0) // right
                frontInfo = Physics2D.Raycast(groundDetection.position, Vector2.right, distanceDetect, whatIsGround);
            else // left
                frontInfo = Physics2D.Raycast(groundDetection.position, Vector2.left, distanceDetect, whatIsGround);

            if (movingRight)
                frontInfo = Physics2D.Raycast(groundDetection.position, Vector2.right, distanceDetect, whatIsGround);
            
            //* If at edge, then turn around
            if (body.velocity.y >= 0 && (!groundInfo || frontInfo))
                Flip();
        }
        // Chasing PLayer
        else if (!receivingKnockback && hp > 0)
        {
            //* If at edge, then turn around
            if (target.position.x > this.transform.position.x)  // player is to the right
            {
                body.AddForce(Vector2.right * chaseSpeed * Time.fixedDeltaTime, ForceMode2D.Impulse);
                float cappedSpeed = Mathf.Min(body.velocity.x, maxSpeed);

                body.velocity = new Vector2(cappedSpeed, body.velocity.y);
                model.transform.eulerAngles = new Vector3(0, 180);  // face right
            }
            else
            {
                body.AddForce(Vector2.left * chaseSpeed * Time.fixedDeltaTime, ForceMode2D.Impulse);
                float cappedSpeed = Mathf.Max(body.velocity.x, -maxSpeed);

                body.velocity = new Vector2(cappedSpeed, body.velocity.y);
                model.transform.eulerAngles = new Vector3(0, 0);  // face left
            }
        }

        if (target != null && playerInRange)
        {
            Vector3 lineOfSight = (target.position + new Vector3(0, 1)) - (this.transform.position + new Vector3(0, 1));
            RaycastHit2D playerInfo = Physics2D.Linecast(this.transform.position + new Vector3(0, 1),
                this.transform.position + new Vector3(0, 1) + lineOfSight, finalMask);
            if (playerInfo.collider != null && playerInfo.collider.gameObject.CompareTag("Player"))
            {
                chasing = true;
                if (alert != null) alert.gameObject.SetActive(true);
                anim.speed = chaseSpeed;
            }
            else
            {
                chasing = false;
                if (alert != null) alert.gameObject.SetActive(false);
            }

        }
        else
        {
            chasing = false;
            if (alert != null) alert.gameObject.SetActive(false);
        }
        
    }
    private void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.yellow;
        if (target != null)
        {
            Vector3 lineOfSight = (target.position + new Vector3(0, 1)) - (this.transform.position + new Vector3(0, 1));
            Gizmos.DrawLine(this.transform.position + new Vector3(0, 1),
                this.transform.position + new Vector3(0, 1) + lineOfSight);
        }
    }

    private void Flip()
    {
        if (!canFlip)
            return;
        if (movingLeft || movingRight)
        {
            if (model.transform.eulerAngles.y != 0)
            {
                model.transform.eulerAngles = new Vector3(0, 0);    // left
                movingRight = false;
                movingLeft = true;
            }
            else
            {
                model.transform.eulerAngles = new Vector3(0, 180);  // right
                movingLeft = false;
                movingRight = true;
            }
        }
        StartCoroutine( ResetFlipTimer() );
    }

    void AdjustAnim(string triggerName)
    {
        anim.SetTrigger(triggerName);
        if (triggerName == "walking")
        {
            anim.speed = moveSpeed;
        }
        else
            anim.speed = 1;
    }

    IEnumerator DoSomething()
    {
        yield return new WaitForSeconds(2);
        if (!chasing)
        {
            switch (Random.Range(0,2))
            {
                // Move right
                case 0:
                    movingRight = true;
                    movingLeft = false;
                    model.transform.eulerAngles = new Vector3(0, 180);
                    break;
                // Move left
                case 1:
                    movingRight = false;
                    movingLeft = true;
                    model.transform.eulerAngles = new Vector3(0, 0);
                    break;
            }
            AdjustAnim("walking");
        }

        yield return new WaitForSeconds(2);
        if (!chasing)
        {
            AdjustAnim("idling");
            movingRight = false;
            movingLeft = false;
            body.velocity = new Vector2(0, body.velocity.y);
        }

        co = StartCoroutine(DoSomething());
    }

}

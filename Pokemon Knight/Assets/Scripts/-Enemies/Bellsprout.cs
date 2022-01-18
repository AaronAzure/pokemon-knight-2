using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bellsprout : Enemy
{
    [Space] [Header("Bellsprout")]  
    public Animator anim;
    public float moveSpeed=2;
    public float chaseSpeed=4;
    public float maxSpeed=6;
    public float distanceDetect=1f;
    public Transform groundDetection;
    // [SerializeField] private LayerMask whatIsTree;
    // public Transform face;
    private bool canFlip = true;
    private bool movingLeft;
    private bool movingRight;


    [Header("Attacks")]
    public Transform target;
    public bool chasing;
    private Coroutine co;


    public override void Setup()
    {
        co = StartCoroutine( DoSomething() );
    }



    // Start is called before the first frame update
    void FixedUpdate() 
    {
        // Wandering around
        if (!chasing)
        {
            if (!receivingKnockback)
            {
                if (movingLeft)
                    body.velocity = new Vector2(-moveSpeed, body.velocity.y);
                else if (movingRight)
                    body.velocity = new Vector2(moveSpeed, body.velocity.y);
            }
            RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, distanceDetect, whatIsGround);
            RaycastHit2D frontInfo = Physics2D.Raycast(groundDetection.position, Vector2.left, distanceDetect, whatIsGround);

            if (movingRight)
                frontInfo = Physics2D.Raycast(groundDetection.position, Vector2.right, distanceDetect, whatIsGround);
            
            //* If at edge, then turn around
            if (body.velocity.y >= 0 && (!groundInfo || frontInfo))
                Flip();
        }
        // Chasing PLayer
        else if (!receivingKnockback)
        {
            //* If at edge, then turn around
            if (target.position.x > this.transform.position.x)  // player is to the right
            {
                // body.velocity = new Vector2(chaseSpeed, body.velocity.y);
                body.AddForce(Vector2.right * chaseSpeed * Time.fixedDeltaTime, ForceMode2D.Impulse);
                float cappedSpeed = Mathf.Min(body.velocity.x, maxSpeed);
                body.velocity = new Vector2(cappedSpeed, body.velocity.y);
                model.transform.eulerAngles = new Vector3(0, 180);  // face right
            }
            else
            {
                // body.velocity = new Vector2(-chaseSpeed, body.velocity.y);
                body.AddForce(Vector2.left * chaseSpeed * Time.fixedDeltaTime, ForceMode2D.Impulse);
                float cappedSpeed = Mathf.Max(body.velocity.x, -maxSpeed);
                body.velocity = new Vector2(cappedSpeed, body.velocity.y);
                model.transform.eulerAngles = new Vector3(0, 0);  // face left
            }
        }

    }

    private void Flip()
    {
        if (!canFlip)
            return;
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
        StartCoroutine( ResetFlipTimer() );
    }
    IEnumerator ResetFlipTimer()
    {
        canFlip = false;
        yield return new WaitForSeconds(0.5f);
        canFlip = true;
    }

    void AdjustAnim(string triggerName)
    {
        anim.SetTrigger(triggerName);
        if (triggerName == "walking")
        {
            if (!chasing)
                anim.speed = moveSpeed;
            else
                anim.speed = chaseSpeed;
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
        }

        co = StartCoroutine(DoSomething());
    }

}
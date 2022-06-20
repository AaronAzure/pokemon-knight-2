using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Nidoran : Enemy
{
    [Space] [Header("Nidoran")]  
    public Animator anim;
    public float moveSpeed=2;
    public float chaseSpeed=5;
    public float maxSpeed=6f;
    public bool canJump=true;
    public float jumpHeight=16;
    public float distanceDetect=1f;
    public Transform groundDetection;

    [SerializeField] private bool grounded=true;
    [SerializeField] private Transform feetPos;
    [SerializeField] private Vector2 feetBox;


    [Header("Attacks")]
    public Transform target;
    public bool chasing;
    private Coroutine co;
    [SerializeField] private Coroutine targetLostCo;
    private LayerMask finalMask;    // detect Player, Ground, ignores Enemy, Bounds
    private RaycastHit2D playerInfo;
    [Space] public bool speedVariant;



    public override void Setup()
    {
        co = StartCoroutine( DoSomething() );
        finalMask = (whatIsPlayer | whatIsGround);
        if (alert != null) 
            alert.gameObject.SetActive(false);
        if (target == null && playerControls != null)
            target = playerControls.transform;
        if (speedVariant)
            maxSpeed *= Random.Range(0.9f, 1.1f);
    }

    public override void CallChildOnTargetFound()
    {
        anim.SetBool("isWalking", true);
    }
    public override void CallChildOnTargetLost()
    {
        if (targetLostCo == null)
            targetLostCo = StartCoroutine( TryToFindTarget(1.5f) );
    }
    IEnumerator TryToFindTarget(float duration=2)
    {
        yield return new WaitForSeconds(duration);
        chasing = false;
        alert.SetActive(false);

        targetLostCo = null;
    }

    // Start is called before the first frame update
    void FixedUpdate() 
    {
        grounded = (Physics2D.OverlapBox(feetPos.position, feetBox, 0, whatIsGround) && body.velocity.y <= 0);
        // Wandering around
        if (!chasing)
        {
            if (!receivingKnockback && hp > 0)
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
            
            //* If at edge, then turn around
            if (body.velocity.y >= 0 && (!groundInfo || frontInfo))
                Flip();
        }
        // Chasing PLayer
        else if (!receivingKnockback && hp > 0 && chasing)
        {
            //* JUMP OVER EDGES OR WALLS
            RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, distanceDetect, whatIsGround);
            RaycastHit2D frontInfo;

            if (model.transform.eulerAngles.y > 0) // right
                frontInfo = Physics2D.Raycast(groundDetection.position, Vector2.right, distanceDetect * 2, whatIsGround);
            else // left
                frontInfo = Physics2D.Raycast(groundDetection.position, Vector2.left, distanceDetect * 2, whatIsGround);

            if (canJump && (!groundInfo || frontInfo) && IsBelowTarget())
                if (model.transform.eulerAngles.y > 0 && !PlayerIsToTheLeft())
                    Jump();
                else if (model.transform.eulerAngles.y == 0 && PlayerIsToTheLeft())
                    Jump();

            //* Chase player while on ground (COMMIT TO JUMP)
            if (grounded)
            {
                if (target.position.x > this.transform.position.x)  // player is to the right
                {
                    body.AddForce(Vector2.right * chaseSpeed * Time.fixedDeltaTime, ForceMode2D.Impulse);
                    // float cappedSpeed = Mathf.Min(body.velocity.x, maxSpeed);
                    // body.velocity = new Vector2(cappedSpeed, body.velocity.y);
                    model.transform.eulerAngles = new Vector3(0, 180);  // face right
                }
                else
                {
                    body.AddForce(Vector2.left * chaseSpeed * Time.fixedDeltaTime, ForceMode2D.Impulse);
                    // float cappedSpeed = Mathf.Max(body.velocity.x, -maxSpeed);
                    // body.velocity = new Vector2(cappedSpeed, body.velocity.y);
                    model.transform.eulerAngles = new Vector3(0, 0);  // face left
                }
				body.velocity = Vector2.ClampMagnitude(body.velocity, maxSpeed);
            }
            else 
            {
                if (model.transform.eulerAngles.y > 0)  // right
                    body.velocity = new Vector2(maxSpeed, body.velocity.y);
                else
                    body.velocity = new Vector2(-maxSpeed, body.velocity.y);
            }
            
        }

        if (target != null && playerInField)
        {
            Vector3 lineOfSight = (target.position + new Vector3(0, 1)) - (this.transform.position + new Vector3(0, 1));
            playerInfo = Physics2D.Linecast(this.transform.position + new Vector3(0, 1),
                this.transform.position + new Vector3(0, 1) + lineOfSight, finalMask);
            if (playerInfo.collider != null && playerInfo.collider.gameObject.CompareTag("Player"))
            {
                chasing = true;
                if (alert != null) alert.gameObject.SetActive(true);
                anim.speed = Mathf.Min(2, chaseSpeed);
                if (targetLostCo != null)
                {
                    StopCoroutine( targetLostCo );
                    targetLostCo = null;
                }
            }
            else if (playerInfo.collider != null && !playerInfo.collider.gameObject.CompareTag("Player") && grounded)
            {
                CallChildOnTargetLost();
            }

        }
        else if (target != null && !playerInField && grounded)
        {
            CallChildOnTargetLost();
        }

        if (grounded && body.velocity.y == 0)
        {
            anim.SetBool("isFalling", false);
        }
        else if (!grounded && body.velocity.y < -0.1f)
        {
            anim.SetBool("isFalling", true);
        }
        
    }
    private void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(feetPos.position, feetBox);

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

    private void Jump()
    {
        if (grounded && hp > 0)
        {
            // Debug.Log(this.transform.position.y >= (target.transform.position.y + 1f));
            StartCoroutine( ResetJumpCounter() );
            grounded = false;
            anim.SetTrigger("jump");
            anim.SetBool("isFalling", false);
            body.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
            if (model.transform.eulerAngles.y > 0) // right
                body.velocity = new Vector2(maxSpeed, body.velocity.y);
            else //left
                body.velocity = new Vector2(-maxSpeed, body.velocity.y);
        }
    }

    bool IsBelowTarget()
    {
        // return (this.transform.position.y - target.transform.position.y) <= 0;
        return (this.transform.position.y - target.transform.position.y) < -0.1f;
    }

    IEnumerator ResetJumpCounter()
    {
        canJump = false;

        yield return new WaitForSeconds(0.5f);
        canJump = true;
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
            anim.SetBool("isWalking", true);
        }

        yield return new WaitForSeconds(2);
        if (!chasing)
        {
            anim.SetBool("isWalking", false);
            movingRight = false;
            movingLeft = false;
            body.velocity = new Vector2(0, body.velocity.y);
        }

        co = StartCoroutine(DoSomething());
    }

}

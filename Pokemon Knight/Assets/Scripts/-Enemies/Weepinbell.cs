using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weepinbell : Enemy
{
    [Space] [Header("Weepinbell")]  
    public float moveSpeed=2;
    public float jumpForce=10;
    public bool canPivot=true;
    public bool canJump=true;
    private Coroutine jumpAandTurnCo;
    private Coroutine targetLostCo;

    private LayerMask finalMask;
    public Transform target;
    [Space] public Transform groundDetection;
    public float distanceDetect=1.5f;
    [Space] [SerializeField] private RazorLeaf razorLeaf;
    [SerializeField] private Transform razorLeafSpawn;
    public bool keepAttacking;
    public bool attacked;
    public Vector2 fieldOfVision;
    public Vector3 offset;
    public Vector3 lineOfSight;
    [Space] public int missCount=1;
    public bool moving;

    private RaycastHit2D playerInfo;
    // private RaycastHit2D frontInfo;
    

    public override void Setup()
    {
        missCount = 1;
        finalMask = (whatIsPlayer | whatIsGround);
        if (alert != null) 
            alert.gameObject.SetActive(false);
            
        if (target == null && playerControls != null)
            target = playerControls.transform;

        if (alwaysAttackPlayer)
        {
            isTargeting = true;
            alert.gameObject.SetActive(true);
            LookAtTarget();
        }
    }

    public override void CallChildOnTargetFound()
    {
        // keepAttacking = true;
    }
    public override void CallChildOnTargetLost()
    {
        if (targetLostCo == null)
            targetLostCo = StartCoroutine( TryToFindTarget(1.5f) );
    }
    IEnumerator TryToFindTarget(float duration=2)
    {
        yield return new WaitForSeconds(duration);
        keepAttacking = false;
        alert.SetActive(false);

        targetLostCo = null;
    }

    // Start is called before the first frame update
    void FixedUpdate() 
    {
        //* JUMP AROUND
        if (!keepAttacking && !alwaysAttackPlayer)
        {

        }
        //* PURSUE PLAYER
        else if (!receivingKnockback && hp > 0)
        {
            if (alwaysAttackPlayer && !alert.activeSelf)
                alert.SetActive(true);

            if (isTargeting && !moving)
                LookAtPlayer();

            //* GIVE CHASE
            if (moving)
            {
                if (movingLeft)
                    body.velocity = new Vector2(-moveSpeed, body.velocity.y);
                else if (movingRight)
                    body.velocity = new Vector2( moveSpeed, body.velocity.y);

                //* JUMP OVER EDGES OR WALLS
                RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, distanceDetect, whatIsGround);
                RaycastHit2D frontInfo;

                if (model.transform.eulerAngles.y > 0) // right
                    frontInfo = Physics2D.Raycast(groundDetection.position, Vector2.right, distanceDetect * 2, whatIsGround);
                else // left
                    frontInfo = Physics2D.Raycast(groundDetection.position, Vector2.left, distanceDetect * 2, whatIsGround);

                if ((!groundInfo || frontInfo) && IsBelowTarget())
                    Jump();
            }
        }

        //* GROUND DETECTION
        if (!canJump && body.velocity.y == 0 && IsGrounded())
            canJump = true;

        //* MONITOR FOR PLAYER
        if (!alwaysAttackPlayer)
        {
            if (target != null && playerInField)
            {
                Vector3 lineOfSight = (target.position + new Vector3(0, 1)) - (this.transform.position + new Vector3(0, 1));
                playerInfo = Physics2D.Linecast(this.transform.position + new Vector3(0, 1),
                    this.transform.position + new Vector3(0, 1) + lineOfSight, finalMask);

                //* PLAYER IS IN SIGHT, AND WITHIN FOV
                if (playerInfo.collider != null && playerInfo.collider.gameObject.CompareTag("Player"))
                {
                    keepAttacking = true;
                    isTargeting = true;
                    if (alert != null) alert.gameObject.SetActive(true);
                    if (targetLostCo != null)
                    {
                        StopCoroutine( targetLostCo );
                        targetLostCo = null;
                    }
                }
                //* PLAYER IS OUT OF SIGHT, BUT STILL WITHIN FOV
                else if (playerInfo.collider != null && !playerInfo.collider.gameObject.CompareTag("Player") && IsGrounded())
                {
                    CallChildOnTargetLost();
                }

            }
            //* PLAYER IS OUT OF FOV
            else if (target != null && !playerInField && IsGrounded())
                CallChildOnTargetLost();
        }
        
    }
    private void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(this.transform.position + (Vector3) feetOffset, feetSize);

        Gizmos.color = Color.yellow;
        if (target != null)
        {
            Vector3 lineOfSight = (target.position + new Vector3(0, 1)) - (this.transform.position + new Vector3(0, 1));
            Gizmos.DrawLine(this.transform.position + new Vector3(0, 1),
                this.transform.position + new Vector3(0, 1) + lineOfSight);
        }
    }


    // todo ---------------------- ANIMATION EVENTS ---------------------------

    public void STOP_FOLLOWING()
    {
        isTargeting = false;
    }
    public void CONTINUE_FOLLOWING()
    {
        isTargeting = true;
        LookAtTarget();
    }
    public void NEXT_ACTION()
    {
        if ((keepAttacking || alwaysAttackPlayer) && missCount % 3 != 0)
        {
            mainAnim.SetTrigger("attack");
            JumpChance(target.position.y - this.transform.position.y < 0);
        }
        // CHASE
        else if ((keepAttacking || alwaysAttackPlayer))
        {
            if (model.transform.eulerAngles.y != 0)    // right
            {
                movingRight = true;
                movingLeft = false;
            }
            else    // left
            {
                movingRight = false;
                movingLeft = true;
            }
            moving = true;
            mainAnim.SetBool("isWalking", moving);
            missCount = 1;
        }
        else if (!keepAttacking && jumpAandTurnCo == null && canPivot && canJump)
            jumpAandTurnCo = StartCoroutine( JumpAandTurnCo() );

    }
    public void JumpChance(bool guaranteed=false)
    {
        if (!canJump || !IsGrounded())
            return;

        if (guaranteed || Random.Range(0, 3) == 0)
        {
            canJump = false;
            body.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        }
    }

    // JUMP AND TURN AROUND
    IEnumerator JumpAandTurnCo()
    {
        yield return new WaitForSeconds(3);
        // DONT JUMP IF ALREADY JUMPED
        if (!canJump || keepAttacking)
        {
            jumpAandTurnCo = null;
            yield break;
        }
        canJump = false;

        if (!playerInSight)
            body.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        
        yield return new WaitForSeconds(0.5f);
        if (keepAttacking)
        {
            jumpAandTurnCo = null;
            yield break;
        }
        if (!playerInSight)
            Flip();
        jumpAandTurnCo = null;
    }

    private void Flip()
    {
        if (model.transform.eulerAngles.y != 0)
            model.transform.eulerAngles = new Vector3(0, 0);    // left
        else
            model.transform.eulerAngles = new Vector3(0, 180);  // right
    }

    private void Jump()
    {
        if (IsGrounded() && body.velocity.y == 0 && hp > 0)
        {
            mainAnim.SetTrigger("jump");
            mainAnim.SetBool("isFalling", false);
            canJump = false;
            body.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    bool IsBelowTarget()
    {
        return (this.transform.position.y - target.transform.position.y) < -0.1f;
    }


    public void RAZOR_LEAF()
    {
        lineOfSight = (target.position + new Vector3(0, 1)) - (this.transform.position + new Vector3(0, 1));
        if (razorLeafSpawn != null && hp > 0)
        {
            var obj = Instantiate(razorLeaf, razorLeafSpawn.position, razorLeaf.transform.rotation);
            obj.weepinbell = this;
            obj.atkDmg = projectileDmg + calcExtraProjectileDmg;
            obj.direction = lineOfSight.normalized;
        }
    }

    public void STOP_MOVING()
    {
        moving = false;
        movingLeft = false;
        movingRight = false;
        mainAnim.SetBool("isWalking", moving);
        if (body.bodyType != RigidbodyType2D.Static)
            body.velocity = new Vector2(0, body.velocity.y);
    }

}

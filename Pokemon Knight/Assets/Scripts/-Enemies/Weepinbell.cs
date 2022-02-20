using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weepinbell : Enemy
{
    [Space] [Header("Weepinbell")]  
    public Animator anim;
    public float moveSpeed=2;
    public float jumpForce=10;
    // public float chaseSpeed=4;
    // public float maxSpeed=5f;
    private LayerMask finalMask;
    public Transform target;
    [Space] public Transform groundDetection;
    public float distanceDetect=1.5f;
    [Space] [SerializeField] private RazorLeaf razorLeaf;
    [SerializeField] private Transform razorLeafSpawn;
    public bool attacked;
    public Vector2 fieldOfVision;
    public Vector3 offset;
    public Vector3 lineOfSight;
    [Space] public int missCount;
    private bool moving;

    private RaycastHit2D groundInfo;
    private RaycastHit2D frontInfo;
    

    public override void Setup()
    {
        finalMask = (whatIsPlayer | whatIsGround);
        if (alert != null) alert.gameObject.SetActive(false);
        
        if (GameObject.Find("PLAYER") != null && target == null)
            target = GameObject.Find("PLAYER").gameObject.transform;

        if (alwaysAttackPlayer)
        {
            if (alert != null)
                alert.gameObject.SetActive(true);
            LookAtTarget();
            StartCoroutine( RestBeforeNextAttack() );
        }
    }

    void FixedUpdate() 
    {
        if (!alwaysAttackPlayer)
        {
            bool detection = false;
            if (model.transform.rotation.y > 0)
                detection = Physics2D.OverlapBox(this.transform.position - offset, fieldOfVision, 0, whatIsPlayer);
            else
                detection = Physics2D.OverlapBox(this.transform.position + offset, fieldOfVision, 0, whatIsPlayer);
            if (target != null && detection)
            {

                lineOfSight = (target.position + new Vector3(0, 1)) - (this.transform.position + new Vector3(0, 1));
                RaycastHit2D playerInfo = Physics2D.Linecast(this.transform.position,
                    this.transform.position + new Vector3(0, 1) + lineOfSight, finalMask);

                if (playerInfo.collider != null && playerInfo.collider.gameObject.CompareTag("Player"))
                {
                    if (alert != null) alert.gameObject.SetActive(true);
                    LookAtTarget();
                    if (!attacked)
                    {
                        attacked = true;
                        anim.SetTrigger("attack");
                        moving = false; anim.SetBool("isWalking", moving);
                        JumpAndTarget();
                    }
                }
                else
                {
                    if (alert != null) alert.gameObject.SetActive(false);
                }

            }
        }
        if (body.velocity.y <= 0 && IsGrounded())
        {
            body.velocity *= new Vector2(0,1);
        }
        
        groundInfo = Physics2D.Raycast(this.transform.position, Vector2.down, 0.5f, whatIsGround);

        if (hp > 0 && !receivingKnockback && moving)
        {
            if (model.transform.eulerAngles.y == 0) // LEFT
                body.velocity = new Vector2(-moveSpeed, body.velocity.y);
            else    // RIGHT
                body.velocity = new Vector2(moveSpeed, body.velocity.y);

            if (model.transform.eulerAngles.y == 0 && DistanceFromTarget() > 3f)
                LookAtTarget();
            else if (model.transform.eulerAngles.y > 0 && DistanceFromTarget() < -3f)
                LookAtTarget();
            
            if (model.transform.eulerAngles.y > 0)    // right
                frontInfo = Physics2D.Raycast(groundDetection.position, Vector2.right, distanceDetect, whatIsGround);
            else    // left
                frontInfo = Physics2D.Raycast(groundDetection.position, Vector2.left, distanceDetect, whatIsGround);

            if (frontInfo && body.velocity.y == 0)
                Jump();
        }
    }

    private void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.yellow;
        if (target != null)
        {
            Gizmos.DrawLine(this.transform.position + new Vector3(0, 1),
                this.transform.position + new Vector3(0, 1) + lineOfSight);
        }
        if (model.transform.rotation.y > 0)
            Gizmos.DrawWireCube(this.transform.position - offset , fieldOfVision);
        else
            Gizmos.DrawWireCube(this.transform.position + offset, fieldOfVision);
        
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(this.transform.position + (Vector3) feetOffset, feetSize);

    }

    public void Jump()
    {
        if (groundInfo && body.velocity.y == 0)
            return;
            
        if (playerControls.transform.position.x < this.transform.position.x)
            body.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        else
            body.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
    }
    public void JumpAndTarget(bool guaranteed=false)
    {
        if (!groundInfo)
            return;
        if (guaranteed || Random.Range(0, 3) == 0)
        {
            LookAtPlayer();
            if (playerControls.transform.position.x < this.transform.position.x)
                body.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
            else
                body.AddForce(new Vector2(0, jumpForce), ForceMode2D.Impulse);
        }
    }

    public void RAZOR_LEAF()
    {
        if (alwaysAttackPlayer)
            lineOfSight = (target.position + new Vector3(0, 1)) - (this.transform.position + new Vector3(0, 1));
        if (razorLeafSpawn != null && hp > 0)
        {
            var obj = Instantiate(razorLeaf, razorLeafSpawn.position, razorLeaf.transform.rotation);
            obj.weepinbell = this;
            obj.atkDmg = projectileDmg + calcExtraProjectileDmg;
            obj.direction = lineOfSight.normalized;
        }
    }

    public void NEXT_ACTION()
    {
        if (alwaysAttackPlayer)
            StartCoroutine( RestBeforeNextAttack() );
    }


    IEnumerator RestBeforeNextAttack()
    {
        yield return new WaitForSeconds(2);
        while (!groundInfo)
        {
            yield return new WaitForSeconds(0.5f);
        }
        anim.SetTrigger("attack");
        moving = false; anim.SetBool("isWalking", moving);
        JumpAndTarget(true);
    }

    private void LookAtTarget()
    {
        if (target != null)
        {
            if (target.position.x > this.transform.position.x)  // player is to the right
                model.transform.eulerAngles = new Vector3(0, 180);  // face right
            else
                model.transform.eulerAngles = new Vector3(0, 0);  // face left
        }
    }

    public void CanAttackAgain()
    {
        StartCoroutine( Cooldown() );
    }

    private float DistanceFromTarget()
    {
        if (target != null)
            return target.transform.position.x - this.transform.position.x; // (>0) = to the right, (<0) = to the left
        return 0;
    }

    public void STOP_MOVING()
    {
        moving = false;
        anim.SetBool("isWalking", moving);

        attacked = false;
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(0.5f);
        if (missCount % 3 == 0)
        {
            moving = true;
            anim.SetBool("isWalking", moving);
        }
        else
            attacked = false;
    }

}

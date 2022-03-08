using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Victreebel : Enemy
{
    [Space] [Header("Victreebel")]  
    public float leapForce=20;
    public float jumpForce=10;
    private bool jumpLeft=false;
    private bool goingToJump;
    private LayerMask finalMask;
    public Transform target;
    [Space] public Transform groundDetection;
    public float distanceDetect=0.5f;
    [Space] [SerializeField] private RazorLeaf razorLeaf;
    [Space] [SerializeField] private EnemyProjectile leafTornado;
    [SerializeField] private Transform razorLeafSpawn;
    public int atkPattern=1;
    public int newAtkPattern=3;
    public int maxAtkPattern=6;
    public bool hasAattacked;
    public Vector3 lineOfSight;
    [Space] public BoxCollider2D fovCol;
    // private bool moving;

    private RaycastHit2D groundInfo;
    

    public override void Setup()
    {
        finalMask = (whatIsPlayer | whatIsGround);
        if (alert != null) alert.gameObject.SetActive(false);
        maxAtkPattern = Random.Range(4,7);
        
        if (GameObject.Find("PLAYER") != null && target == null)
            target = GameObject.Find("PLAYER").gameObject.transform;

        if (leafTornado != null)
            leafTornado.atkDmg = Mathf.RoundToInt( (this.projectileDmg + this.calcExtraProjectileDmg) / 2);

        if (!isInRoom)
        {
            fovCol.size *= new Vector2(0.5f,1);
            fovCol.offset *= new Vector2(0.5f,1);
        }
    }

    void FixedUpdate() 
    {
        if (hp > 0)
        {
            if (target != null && playerInField)
            {
                if (isTargeting)
                    LookAtTarget();

                lineOfSight = (target.position + new Vector3(0, 1)) - (this.transform.position + new Vector3(0, 1));
                RaycastHit2D playerInfo = Physics2D.Linecast(this.transform.position,
                    this.transform.position + new Vector3(0, 1) + lineOfSight, finalMask);

                // if (playerInfo.collider != null && playerInfo.collider.gameObject.CompareTag("Player"))
                // {
                //     if (alert != null) 
                //         alert.gameObject.SetActive(true);
                //     LookAtTarget();
                // }
                // else
                // {
                //     if (alert != null) alert.gameObject.SetActive(false);
                // }

            }
            if (!receivingKnockback && !goingToJump && body.velocity.y <= 0 && IsGrounded())
            {
                body.velocity *= new Vector2(0,1);
            }
            else if (goingToJump && body.velocity.y <= 0 && IsGrounded())
            {
                mainAnim.SetTrigger("idle");
            }
            
            groundInfo = Physics2D.Raycast(this.transform.position, Vector2.down, 0.5f, whatIsGround);
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
        
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(this.transform.position + (Vector3) feetOffset, feetSize);

    }


    public void DONE_JUMPING()
    {
        goingToJump = false;
    }
    public void STOP_FOLLOWING()
    {
        isTargeting = false;
    }
    public void CONTINUE_FOLLOWING()
    {
        isTargeting = true;
        LookAtTarget();
    }
    public void CHECK_PLAYER_CURRENT_LOCATION()
    {
        goingToJump = true;
        jumpLeft = PlayerIsToTheLeft();
    }

    public void JUMP_TOWARDS_PLAYER()
    {
            
        if (jumpLeft)
            body.AddForce(new Vector2(-Random.Range(leapForce-8,leapForce), 
                Random.Range(jumpForce, jumpForce + 6)), ForceMode2D.Impulse);
        else
            body.AddForce(new Vector2( Random.Range(leapForce-8,leapForce), 
                Random.Range(jumpForce, jumpForce + 6)), ForceMode2D.Impulse);
    }

    public void RAZOR_LEAF()
    {
        if (alwaysAttackPlayer)
            lineOfSight = (target.position + new Vector3(0, 1)) - (this.transform.position + new Vector3(0, 1));
        if (razorLeafSpawn != null && hp > 0)
        {
            var obj = Instantiate(razorLeaf, razorLeafSpawn.position, razorLeaf.transform.rotation);
            obj.atkDmg = projectileDmg + calcExtraProjectileDmg;
            obj.direction = lineOfSight.normalized;
        }
    }

    public void NEXT_ACTION()
    {
        if (playerInField)
        {
            if (canUseBuffs)
                mainAnim.SetTrigger("growth");
            else if (atkPattern < newAtkPattern)
            {
                atkPattern++;
                mainAnim.SetTrigger("attack");
            }
            else if (atkPattern != maxAtkPattern)
            {
                atkPattern++;
                mainAnim.SetTrigger("jump");
            }
            else
            {
                atkPattern = 1;
                maxAtkPattern = Random.Range(4,7);
                mainAnim.SetTrigger("ult");
            }
        }
            // StartCoroutine( RestBeforeNextAttack() );
    }

    public void FLIP()
    {
        if (model.transform.eulerAngles.y > 0)  // right
            model.transform.eulerAngles = new Vector3(0,0);
        else  // left
            model.transform.eulerAngles = new Vector3(0,180);

        if (movingLeft)
            { movingLeft = false; movingRight = true; }
        else if (movingRight)
            { movingLeft = true; movingRight = false; }
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


}

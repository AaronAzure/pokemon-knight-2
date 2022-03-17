using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vileplume : Enemy
{
    [Space] [Header("Vileplume")]  
    public float moveSpeed=3;
    public float jumpForce=15;
    private bool jumpLeft=false;
    private bool goingToJump;
    private LayerMask finalMask;
    public Transform target;
    public bool targetFound;
    public bool stopSearching;
    public bool moving;
    [Space] public Transform groundDetection;
    public float distanceDetect=0.5f;
    [Space] [SerializeField] private EnemyAttack poisonPowder;
    [Space] [SerializeField] private EnemyProjectile sludgeBomb;
    [Space] [SerializeField] private EnemyProjectile leafTornado;
    [SerializeField] private Transform atkPos;
    public int atkPattern=1;
    public int newAtkPattern=3;
    public int maxAtkPattern=6;
    public float trajectory;
    public Vector3 lineOfSight;
    [Space] public BoxCollider2D fovCol;
    private Coroutine targetLostCo;
    // private bool moving;
    private bool canUsePoisonPowder=true;
    private RaycastHit2D groundInfo;
    

    public override void Setup()
    {
        finalMask = (whatIsPlayer | whatIsGround);
        if (alert != null) alert.gameObject.SetActive(false);
        maxAtkPattern = Random.Range(4,7);
        
        if (target == null)
            target = playerControls.transform;

        if (leafTornado != null)
            leafTornado.atkDmg = Mathf.RoundToInt( (this.projectileDmg + this.calcExtraProjectileDmg) / 2);

        // if (!isInRoom)
        // {
        //     fovCol.size *= new Vector2(0.5f,1);
        //     fovCol.offset *= new Vector2(0.5f,1);
        // }
    }

    public override void CallChildOnTargetLost()
    {
        if (targetLostCo == null)
            targetLostCo = StartCoroutine( TryToFindTarget(5f) );
    }
    public override void CallChildOnDamaged() 
    {
        if (alert != null) 
            alert.SetActive(true);
        targetFound = true;
        if (targetLostCo != null)
        {
            StopCoroutine( targetLostCo );
            targetLostCo = null;
        }
    }
    IEnumerator TryToFindTarget(float duration=2)
    {
        yield return new WaitForSeconds(duration);
        targetFound = false;
        stopSearching = false;
        alert.SetActive(false);

        targetLostCo = null;
    }

    void FixedUpdate() 
    {
        if (hp > 0)
        {
            if (!stopSearching)
            {
                if (target != null && playerInField)
                {
                    // lineOfSight = (target.position + new Vector3(0, 1)) - (this.transform.position + new Vector3(0, 1));
                    RaycastHit2D playerInfo = Physics2D.Linecast(this.transform.position + new Vector3(0, 1),
                        target.position + new Vector3(0, 1), finalMask);

                    if (playerInfo.collider != null && playerInfo.collider.gameObject.CompareTag("Player"))
                    {
                        if (alert != null) 
                            alert.gameObject.SetActive(true);
                        targetFound = true;
                        if (targetLostCo != null)
                        {
                            StopCoroutine( targetLostCo );
                            targetLostCo = null;
                        }
                    }
                    //* PLAYER IS OUT OF SIGHT, BUT STILL WITHIN FOV
                    else if (playerInfo.collider != null && !playerInfo.collider.gameObject.CompareTag("Player") && IsGrounded() 
                        && body.velocity.y <= 0)
                        CallChildOnTargetLost();
                    if (isTargeting)
                        LookAtTarget();
                }
                //* PLAYER IS OUT OF FOV
                else if (target != null && !playerInField && IsGrounded() 
                    && body.velocity.y <= 0)
                    CallChildOnTargetLost();
            }

            if (moving && !receivingKnockback)
            {
                if (movingLeft)
                {
                    if (!PlayerIsToTheLeft())
                    {
                        STOP_MOVING();
                        CONTINUE_FOLLOWING();
                    }
                    body.velocity = new Vector2(-moveSpeed, body.velocity.y);

                }
                else if (movingRight)
                {
                    if (PlayerIsToTheLeft())
                    {
                        STOP_MOVING();
                        CONTINUE_FOLLOWING();
                    }
                    body.velocity = new Vector2(+moveSpeed, body.velocity.y);
                }

                RaycastHit2D frontInfo;

                if (model.transform.eulerAngles.y > 0) // right
                    frontInfo = Physics2D.Raycast(groundDetection.position, Vector2.right, distanceDetect * 2, whatIsGround);
                else // left
                    frontInfo = Physics2D.Raycast(groundDetection.position, Vector2.left, distanceDetect * 2, whatIsGround);

                if ((!groundInfo || frontInfo) && IsBelowTarget() && body.velocity.y == 0)
                    Jump();
            }

            groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, distanceDetect, whatIsGround);

            if (!receivingKnockback && !moving && body.velocity.y <= 0 && IsGrounded())
                body.velocity *= new Vector2(0,1);
            // else if (goingToJump && body.velocity.y <= 0 && IsGrounded())
            //     mainAnim.SetTrigger("idle");
            
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


    bool IsBelowTarget()
    {
        return (this.transform.position.y - target.transform.position.y) < -0.1f;
    }

    public void NEXT_ACTION()
    {
        if (playerInField && targetFound)
        {
            if (canUseBuffs)
                mainAnim.SetTrigger("growth");
            else if (atkPattern < maxAtkPattern)
            {
                atkPattern++;
                if (playerInCloseRange && canUsePoisonPowder)
                {
                    mainAnim.SetTrigger("poisonPowder");
                    StartCoroutine( PoisonPowderCooldown() );
                }
                else
                {
                    mainAnim.SetTrigger("sludgeBomb");
                }
            }
            else if (atkPattern != maxAtkPattern + 1)
            {
                atkPattern++;
                WalkTowards();
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
    IEnumerator PoisonPowderCooldown()
    {
        canUsePoisonPowder = false;
        yield return new WaitForSeconds(6);
        canUsePoisonPowder = true;
    }

    private void Jump()
    {
        if (IsGrounded() && body.velocity.y == 0 && hp > 0)
        {
            // mainAnim.SetTrigger("jump");
            body.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
        }
    }

    public void WalkTowards()
    {
        STOP_FOLLOWING();
        mainAnim.SetBool("isWalking", true);
        LookAtTarget();
        if (PlayerIsToTheLeft())
        {
            movingLeft = true;
            movingRight = false;
        }
        else
        {
            movingLeft = false;
            movingRight = true;
        }
        moving = true;
    }

    public void DONE_JUMPING()
    {
        goingToJump = false;
    }
    public void STOP_MOVING()
    {
        moving = false;
        mainAnim.SetBool("isWalking", false);
    }
    public void STOP_FOLLOWING()
    {
        isTargeting = false;
        stopSearching = true;
        if (targetLostCo != null)
        {
            StopCoroutine( targetLostCo );
            targetLostCo = null;
        }
    }
    public void CONTINUE_FOLLOWING()
    {
        isTargeting = true;
        stopSearching = false;
        LookAtTarget();
    }
    public void CHECK_PLAYER_CURRENT_LOCATION()
    {
        goingToJump = true;
        jumpLeft = PlayerIsToTheLeft();
    }

    public void JUMP_CHANCE()
    {
        if (!IsGrounded())
            return;

        if (Random.Range(0, 3) == 0)
            body.AddForce(new Vector2(0, jumpForce + 5), ForceMode2D.Impulse);
    }

    public void ULT_ATTACK(int version)
    {
        if (hp > 0)
        {
            if (sludgeBomb != null && atkPos != null)
            {
                if (version == 0)
                {
                    for (int i=0 ; i<7 ; i++)
                    {
                        var obj = Instantiate(sludgeBomb, atkPos.position, sludgeBomb.transform.rotation);
                        obj.body.gravityScale = 3;
                        obj.atkDmg = projectileDmg + calcExtraProjectileDmg;
                        obj.direction = new Vector2(-18 + 6*i, 17.5f);
                    }
                }
                else
                {
                    for (int i=0 ; i<6 ; i++)
                    {
                        var obj = Instantiate(sludgeBomb, atkPos.position, sludgeBomb.transform.rotation);
                        obj.body.gravityScale = 3;
                        obj.atkDmg = projectileDmg + calcExtraProjectileDmg;
                        obj.direction = new Vector2(-15 + 6*i, 17.5f);
                    }
                }
            }
        }
    }
    public void SLUDGE_BOMB()
    {
        if (hp > 0)
        {
            if (sludgeBomb != null && atkPos != null)
            {
                var obj = Instantiate(sludgeBomb, atkPos.position, sludgeBomb.transform.rotation);
                obj.body.gravityScale = 3;
                obj.atkDmg = projectileDmg + calcExtraProjectileDmg;
                trajectory = CalculateTrajectory();
                float extraHeight = CalculateExtraHeight();
                obj.direction = new Vector2(trajectory * -1.1f, Random.Range(12,16) + extraHeight);
            }
        }
    }
    public void POISON_POWDER()
    {
        if (hp > 0)
        {
            body.velocity = new Vector2(0, body.velocity.y);
            var obj = Instantiate(poisonPowder, atkPos.position, poisonPowder.transform.rotation);
            obj.atkDmg = Mathf.RoundToInt((projectileDmg + calcExtraProjectileDmg) / 2);
            Destroy(obj, 4.5f);
        }
    }

    private float CalculateTrajectory()
    {
        return (this.transform.position.x - target.position.x);
    }
    private float CalculateExtraHeight()
    {
        return Mathf.Max(0, target.position.y - this.transform.position.y);
    }


    // public void FLIP()
    // {
    //     if (model.transform.eulerAngles.y > 0)  // right
    //         model.transform.eulerAngles = new Vector3(0,0);
    //     else  // left
    //         model.transform.eulerAngles = new Vector3(0,180);

    //     if (movingLeft)
    //         { movingLeft = false; movingRight = true; }
    //     else if (movingRight)
    //         { movingLeft = true; movingRight = false; }
    // }
}

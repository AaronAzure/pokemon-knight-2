using System.Collections;
// using System.Collections.Generic;
using UnityEngine;

public class Gastly : Enemy
{
    [Space] [Header("Gastly")]  
    public float moveSpeed=2;
    public float chaseSpeed=4;
    public float maxSpeed=5f;
    public float chargeSpeed=10;
    private LayerMask finalMask;
    public Transform target;
    public bool chasing;
    // public bool charging;
    // private float chargeX;
    // private float chargeY;
    public bool playerInRange;
    
    [Space] public LayerMask whatIsItself;

    // [SerializeField] private EnemyAttack furyAttack;
    [SerializeField] private GameObject glint;
    public Vector3 lineOfSight;
    [SerializeField] private GameObject spawnedHolder;
    private Coroutine co;
    [SerializeField] private Coroutine targetLostCo;
    private RaycastHit2D playerInfo;

    private float timer=0;
    public float flipTimer=2;
    private float closeTime=0;
    public float furyAttackTimer=1;
    private int origAtkDmg;
    public bool performingFuryAttack;


    public override void Setup()
    {
        finalMask = (whatIsPlayer | whatIsGround);
        if (alert != null) alert.gameObject.SetActive(false);
        if (spawnedHolder != null) 
            spawnedHolder.transform.parent = null;
        if (target == null)
            target = playerControls.transform;

        origAtkDmg = contactDmg;

        if (Random.Range(0,2) == 0)
            movingLeft = true;
        else
        {
            movingRight = true;
            model.transform.eulerAngles = new Vector3(0, 180);
        }

        // if (mainAnim != null)
        //     mainAnim.Play("beedrill-idle-anim", -1, Random.Range(0f,1f));

        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Enemy"), LayerMask.NameToLayer("Ground"));
    }

    public override void CallChildOnIncreaseSpd()
    {
        moveSpeed *= 1.5f;
        chaseSpeed *= 1.5f;
        maxSpeed *= 1.5f;
            
    }
    public override void CallChildOnRevertSpd()
    {
        moveSpeed /= 1.5f;
        chaseSpeed /= 1.5f;
        maxSpeed /= 1.5f;
    }

    public override void CallChildOnTargetFound()
    {
        // mainAnim.SetBool("isWalking", true);
    }
    public override void CallChildOnTargetLost()
    {
        if (targetLostCo == null)
            targetLostCo = StartCoroutine( TryToFindTarget(2.5f) );
    }
    IEnumerator TryToFindTarget(float duration=2)
    {
        yield return new WaitForSeconds(duration);
        chasing = false;
        alert.SetActive(false);
        body.velocity = Vector2.zero;

        targetLostCo = null;
    }

    // Start is called before the first frame update
    void FixedUpdate() 
    {
        if (!performingFuryAttack && !performingBuff)
        {
            if (!inCutscene && !isMiniBoss)
            {
                // Chasing PLayer
                if (chasing)
                {
                    if (hp > 0 && !receivingKnockback)
                    {
                        Vector3 dir = (target.position + new Vector3(0,1) - this.transform.position).normalized;
                        //* If at edge, then turn around
                        body.AddForce(dir * 5 * chaseSpeed * Time.fixedDeltaTime, ForceMode2D.Impulse);
                        CapVelocity();
                    }
                    // Player is close enough to perform a strong attack
                    if (playerInCloseRange)
                    {
                        closeTime += Time.fixedDeltaTime;
                        if (closeTime > furyAttackTimer)
                        {
                            // performingFuryAttack = true;
                            closeTime = 0;
                            // mainAnim.SetTrigger("furyAttack");
                        }
                    }
                    else
                        closeTime = 0;
                }
                // wandering
                else
                {
                    if (movingLeft)
                    {
                        body.velocity = new Vector2(-moveSpeed,0);
                        if (model.transform.eulerAngles.y != 0)
                            model.transform.eulerAngles = new Vector3(0, 0);
                    }
                    else if (movingRight)
                    {
                        body.velocity = new Vector2(+moveSpeed,0);
                        if (model.transform.eulerAngles.y != 180)
                            model.transform.eulerAngles = new Vector3(0, 180);
                    }

                    if (timer < flipTimer)
                        timer += Time.fixedDeltaTime;
                    else
                    {
                        timer = 0;
                        Flip();
                    }
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
                    if (canUseBuffs)
                    {
                        mainAnim.speed = 1;
                        mainAnim.SetTrigger("buff");
                        performingBuff = true;
                        body.velocity = Vector2.zero;
                        timer = 0;
                    }
                    if (alert != null) alert.gameObject.SetActive(true);
                    mainAnim.speed = Mathf.Min(2, chaseSpeed);
                    if (targetLostCo != null)
                    {
                        StopCoroutine( targetLostCo );
                        targetLostCo = null;
                    }
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
        // else if (charging)
        // {
        //     body.velocity = new Vector2(chargeX, chargeY);
        // }
    }


    private void CapVelocity()
    {
        float cappedSpeedX = 0;
        float cappedSpeedY = 0;

        // chasing right (positive velocity)
        if (target.position.x > this.transform.position.x)  // player is to the right
        {
            cappedSpeedX = Mathf.Min(body.velocity.x, maxSpeed);
            model.transform.eulerAngles = new Vector3(0, 180);  // face right
        }
        // chasing left (negative velocity)
        else
        {
            cappedSpeedX = Mathf.Max(body.velocity.x, -maxSpeed);
            model.transform.eulerAngles = new Vector3(0, 0);  // face left
        }

        // chasing up (positive velocity)
        if (target.position.y > this.transform.position.y)  // player is to the right
        {
            cappedSpeedY = Mathf.Min(body.velocity.y, maxSpeed);
        }
        // chasing down (negative velocity)
        else
        {
            cappedSpeedY = Mathf.Max(body.velocity.y, -maxSpeed);
        }

        body.velocity = new Vector2(cappedSpeedX, cappedSpeedY);
    }
    private void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.yellow;
        if (target != null)
        {
            // Vector3 lineOfSight = (target.position + new Vector3(0, 1)) - (this.transform.position);
            Gizmos.DrawLine(this.transform.position,
                this.transform.position + new Vector3(0, 1) + lineOfSight);
        }
    }
    private void Flip()
    {
        if (movingLeft)
        {
            movingLeft = false;
            movingRight = true;
            model.transform.eulerAngles = new Vector3(0, 180);
        }
        else if (movingRight)
        {
            movingRight = false;
            movingLeft = true;
            model.transform.eulerAngles = new Vector3(0, 0);
        }
    }


    public void STOP()
    {
        if (hp > 0)
        {
            body.velocity = Vector2.zero;
            // charging = false;
        }
    }
    public void FINISHED_PERFORMING_FURY_ATTACL()
    {
        performingFuryAttack = false;
        contactDmg = origAtkDmg;
    }
    public void CHARGE()
    {
        contactDmg = projectileDmg + calcExtraProjectileDmg;

        // chargeX = 0;
        // chargeY = 0;
        // if (model.transform.rotation.y > 0)
        //     chargeX = chargeSpeed;
        // else
        //     chargeX = -chargeSpeed;
        
        // if (target.position.y - this.transform.position.y > 2)   // player above beedrill
        //     chargeY = 3;
        // else if (target.position.y - this.transform.position.y < -2)   // player below beedrill
        //     chargeY = -3;
        
        // // body.velocity = new Vector2(chargeX, chargeY);
        // charging = true;
    }
    // private void OnCollisionEnter2D(Collision2D other) 
    // {
    //     if (other.gameObject.CompareTag("Ground"))    
    //     {
    //         Physics2D.IgnoreCollision(other.collider, this.col);
    //     }
    // }

    public void AGILITY()
    {
        StartCoroutine( ResetBuff(5,5, Stat.spd) );
    }
}

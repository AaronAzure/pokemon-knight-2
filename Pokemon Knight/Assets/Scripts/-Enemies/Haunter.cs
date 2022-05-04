using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Haunter : Enemy
{
    public enum Variation { chaser, hexer };
    
    [Space] [Header("Haunter")]  
    public Variation variant;
    
    
    [Space] [Header("Chaser")]  
    public float moveSpeed=2;
    public float chaseSpeed=4;
    public float maxSpeed=5f;
    // public float chargeSpeed=10;
    private LayerMask finalMask;
    public Transform target;
    public bool chasing;
    public bool teleporting;
    public float distanceAway=3;
    public bool playerInRange;
    public EnemyProjectile shadowPunch;
    public float shadowPunchForce=15f;
    public float shadowPunchTimer=1;
    public Transform shadowPunchPos;
    private bool performingShadowPunch;
    private bool returningShadowPunch;
    public float returnPunchDuration=1.5f;
    public float shadowPunchDuration=0.5f;
    private float timeElapsed=0;
    private Vector3 startPos;



    [Space] [Header("Hexer")]  
    public bool performingHex;
    [SerializeField] private EnemyProjectile hex;
    public float hexTimer=0.5f;
    // public bool licking;
    public float teleportAgain=1.5f;
    public float flipTimer=3f;
    [SerializeField] private float lickMoveForce=1;
    [SerializeField] private EnemyAttack lickAtk;


    // [Space] [Header("Willo Wisp")]  
    // [SerializeField] private Transform[] wispSpawns;
    // [SerializeField] private EnemyProjectile wispAtk;
    // [SerializeField] private List<EnemyProjectile> wisps;
    // [SerializeField] private float wispFireDelay=0.8f;
    // [SerializeField] private float wispSpeed=7.5f;
    // private bool shootingWisp;


    

    [Space] [Header("Misc")]  
    // [SerializeField] private GameObject glint;
    [SerializeField] private bool variantSpeed;
    [SerializeField] private GameObject spawnedHolder;
    private Coroutine co;
    [SerializeField] private Coroutine targetLostCo;
    private RaycastHit2D playerInfo;
    private bool sceneLoaded=false;

    private float timer=0;
    private float closeTime=0;
    private float farTime=0;
    public float teleportTimer=1;
    private int origAtkDmg;
    private bool keepStalking;


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


        if (lickAtk != null)
        {
            lickAtk.atkDmg = secondDmg;
            lickAtk.kbForce = contactKb;
        }
        if (variantSpeed)
        {
            moveSpeed += Random.Range(0.9f, 1.2f);
            chaseSpeed += Random.Range(0.9f, 1.2f);
            maxSpeed += Random.Range(0.9f, 1.2f);
            lickMoveForce += Random.Range(0.9f, 1.2f);
            teleportAgain += Random.Range(0.9f, 1.2f);
        }
        StartCoroutine( LoadingIn() );

        if (shadowPunch != null)
        {
            shadowPunch.transform.parent = this.transform.parent.transform;
            shadowPunch.atkDmg = secondDmg + secondExtraDmg;
            shadowPunch.kbForce = contactKb;
        }
        if (hex != null)
        {
            hex.atkDmg = projectileDmg + calcExtraProjectileDmg;
            hex.kbForce = contactKb;
        }
        if (variant == Variation.hexer)
        {
            kbDefense /= 4f;
            moveSpeed = 1;
            chaseSpeed = 1;
            maxSpeed = 2;
            spawnedHolder.transform.parent = null;
        }
    }

    IEnumerator LoadingIn()
    {
        yield return new WaitForSeconds(0.5f);
        sceneLoaded = true;
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
        if (performingBuff || performingHex || performingShadowPunch)
            return;
        // if (targetLostCo == null)
        //     targetLostCo = StartCoroutine( TryToFindTarget(2f) );
    }
    public override void CallChildOnDeath()
    {
        mainAnim.speed = 0;
        mainAnim.SetTrigger("reset");
        StopAllCoroutines();
        if (shadowPunch != null)
            Destroy( shadowPunch.gameObject );
        if (spawnedHolder != null)
            Destroy( spawnedHolder.gameObject );
    }
    public override void CallChildOnDropLoot()
    {
        // if (!Physics2D.Linecast(transform.position - new Vector3(0.1f,0), transform.position + new Vector3(0.1f,0),
        //     whatIsGround))
        // if (!Physics2D.BoxCast(this.transform.position, new Vector2(0.1f, 0.1f), 0, Vector2.zero, 0, whatIsGround))
        // Collider[] cols = Physics.OverlapSphere(transform.position, 0.5f, whatIsGround);
        // foreach(Collider col in cols)
        // {
        //     Debug.Log(col.name);
        // }
        if (!Physics2D.OverlapBox(transform.position, new Vector2(0.2f, 0.2f), 0, whatIsGround))
        {
            if (!isBoss && !isMiniBoss)
                loot.DropLoot( Mathf.FloorToInt(lv / 10) );
            else
                loot.DropLoot();
        }
    }
    
    private void DontLoseTrackOfPlayer()
    {
        // chasing = true;
        // if (targetLostCo != null)
        // {
        //     StopCoroutine( targetLostCo );
        //     targetLostCo = null;
        // }
        // if (alert != null) alert.gameObject.SetActive(true);
    }

    // todo ----------------------------------------------------------------------------------------------------
    void FixedUpdate() 
    {
        if (dead || !sceneLoaded) {}
        else if (variant == Variation.chaser)   //* Variation.chaser
        {
            if (!performingShadowPunch && !performingBuff)
            {
                if (!inCutscene && !isMiniBoss)
                {
                    // Chasing PLayer
                    if (chasing && !teleporting)
                    {
                        if (hp > 0 && !receivingKnockback)
                        {
                            Vector3 dir = (target.position + new Vector3(0,1) - this.transform.position).normalized;
                            //* If at edge, then turn around
                            body.AddForce(dir * 5 * chaseSpeed * Time.fixedDeltaTime, ForceMode2D.Impulse);
                            CapVelocity();
                        }
                        // Player is close enough to perform a strong attack
                        if (!playerInCloseRange)
                        {
                            closeTime = 0;
                            farTime += Time.fixedDeltaTime;
                            if (farTime > teleportTimer)
                            {
                                teleporting = true;
                                farTime = 0;
                                mainAnim.speed = 1;
                                mainAnim.SetTrigger("teleport");
                            }
                        }
                        else
                        {
                            farTime = 0;
                            closeTime += Time.fixedDeltaTime;
                            if (closeTime > shadowPunchTimer)
                            {
                                performingShadowPunch = true;
                                closeTime = 0;
                                mainAnim.speed = 1;
                                mainAnim.SetTrigger("shadowPunch");
                                shadowPunch.transform.position = shadowPunchPos.position;
                                body.velocity = Vector2.zero;
                            }
                        }
                    }
                    // wandering
                    else if (!teleporting)
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
            else if (returningShadowPunch)
            {
                timeElapsed += Time.fixedDeltaTime / returnPunchDuration;
                shadowPunch.transform.position = Vector3.Lerp(startPos, shadowPunchPos.position, timeElapsed);
            }
        }
        else if (variant == Variation.hexer)
        {
            if (chasing && !performingHex)
            {
                if (!dead && !receivingKnockback)
                {
                    Vector3 dir = (target.position + new Vector3(0,1) - this.transform.position).normalized;
                    //* If at edge, then turn around
                    body.AddForce(dir * 5 * chaseSpeed * Time.fixedDeltaTime, ForceMode2D.Impulse);
                    CapVelocity();
                }
                // Player is close enough to perform a strong attack
                if (!playerInCloseRange)
                {
                    closeTime = 0;
                    farTime += Time.fixedDeltaTime;
                    if (farTime > teleportTimer)
                    {
                        teleporting = true;
                        farTime = 0;
                        mainAnim.speed = 1;
                        mainAnim.SetTrigger("teleport");
                    }
                }
                else
                {
                    farTime = 0;
                    closeTime += Time.fixedDeltaTime;
                    if (closeTime > hexTimer)
                    {
                        // performingHex = true;
                        closeTime = 0;
                        mainAnim.speed = 1;
                        body.velocity = Vector2.zero;
                        mainAnim.SetTrigger("hex");
                    }
                }
                // mainAnim.SetBool("isWisping", true);
            }
            else if (!chasing)
            {
                if (timer < flipTimer)
                    timer += Time.fixedDeltaTime;
                else
                {
                    timer = 0;
                    if (model.transform.eulerAngles.y == 0)
                        model.transform.eulerAngles = new Vector3(0, 180);
                    else if (model.transform.eulerAngles.y == 180)
                        model.transform.eulerAngles = new Vector3(0, 0);
                }
            }
            // else if (licking)
            // {
            //     LookAtTarget();
            // }
            
            if (!keepStalking && target != null && (playerInField || keepSearching))
            {
                Vector3 lineOfSight = (target.position + new Vector3(0, 1)) - (this.transform.position + new Vector3(0, 1));
                playerInfo = Physics2D.Linecast(this.transform.position + new Vector3(0, 1),
                    this.transform.position + new Vector3(0, 1) + lineOfSight, finalMask);
                if (playerInfo.collider != null && playerInfo.collider.gameObject.CompareTag("Player"))
                {
                    chasing = true;
                    keepStalking = true;
                    if (canUseBuffs)
                    {
                        mainAnim.speed = 1;
                        mainAnim.SetTrigger("buff");
                        performingBuff = true;
                        body.velocity = Vector2.zero;
                        timer = 0;
                    }
                    if (alert != null) alert.gameObject.SetActive(true);
                }

            }
        }
        // else if (variant == Variation.licker)
        // {
        //     if (chasing && !teleporting && !licking)
        //     {
        //         teleporting = true;
        //         closeTime = 0;
        //         mainAnim.speed = 1;
        //         mainAnim.SetTrigger("teleport");
        //     }
        //     else if (!chasing)
        //     {
        //         if (timer < flipTimer)
        //             timer += Time.fixedDeltaTime;
        //         else
        //         {
        //             timer = 0;
        //             if (model.transform.eulerAngles.y == 0)
        //                 model.transform.eulerAngles = new Vector3(0, 180);
        //             else if (model.transform.eulerAngles.y == 180)
        //                 model.transform.eulerAngles = new Vector3(0, 0);
        //         }
        //     }
        //     if (target != null && playerInField)
        //     {
        //         Vector3 lineOfSight = (target.position + new Vector3(0, 1)) - (this.transform.position + new Vector3(0, 1));
        //         playerInfo = Physics2D.Linecast(this.transform.position + new Vector3(0, 1),
        //             this.transform.position + new Vector3(0, 1) + lineOfSight, finalMask);
        //         if (playerInfo.collider != null && playerInfo.collider.gameObject.CompareTag("Player"))
        //         {
        //             chasing = true;
        //             if (canUseBuffs)
        //             {
        //                 mainAnim.speed = 1;
        //                 mainAnim.SetTrigger("buff");
        //                 performingBuff = true;
        //                 body.velocity = Vector2.zero;
        //                 timer = 0;
        //             }
        //             if (alert != null) alert.gameObject.SetActive(true);
        //             mainAnim.speed = Mathf.Min(2, chaseSpeed);
        //         }
        //         else if (playerInfo.collider != null && !playerInfo.collider.gameObject.CompareTag("Player"))
        //         {
        //             CallChildOnTargetLost();
        //         }

        //     }
        //     else if (target != null && !playerInField)
        //     {
        //         CallChildOnTargetLost();
        //     }
        // }
        // else if (charging)
        // {
        //     body.velocity = new Vector2(chargeX, chargeY);
        // }
    }

    // todo ----------------------------------------------------------------------------------------------------


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
            // Vector3 los = (target.position + new Vector3(0, 1)) - (this.transform.position);
            Gizmos.DrawLine(this.transform.position, target.transform.position + Vector3.up);
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



    public void TELEPORT()
    {   
        if (target == null || hp <= 0)
            return;
        
        body.velocity = Vector2.zero;


        int rng = Random.Range(0,5);
        if (variant == Variation.hexer)
            rng = Mathf.Max(1, rng);

        switch (rng)  // 5
        {
            case 0:
                this.transform.position = target.position + new Vector3(0, distanceAway + 1);
                // if (RaycastHit2D)
                break;
            case 1:
                this.transform.position = target.position + new Vector3(distanceAway, 1);
                // if (RaycastHit2D)
                break;
            case 2:
                this.transform.position = target.position + new Vector3(-distanceAway, 1);
                // if (RaycastHit2D)
                break;
            case 3:
                this.transform.position = target.position + new Vector3(distanceAway, distanceAway + 1);
                // if (RaycastHit2D)
                break;
            case 4:
                this.transform.position = target.position + new Vector3(-distanceAway, distanceAway + 1);
                // if (RaycastHit2D)
                break;
        }
        LookAtPlayer();
    }


    public IEnumerator SHADOW_PUNCH()
    {
        if (shadowPunch != null && !dead)
        {
            shadowPunch.gameObject.SetActive(true);
            performingShadowPunch = true;

            yield return new WaitForSeconds(0.5f);
            LookAtTarget();
            shadowPunch.transform.position = shadowPunchPos.position;

            yield return new WaitForSeconds(0.5f);
            Vector2 dir = ((target.position + Vector3.up) - shadowPunchPos.position).normalized;
            if (model.transform.eulerAngles.y == 0 && dir.x > 0) // facing left
                dir = Vector2.left;
            else if (model.transform.eulerAngles.y != 0 && dir.x < 0) // facing right
                dir = Vector2.right;
            shadowPunch.body.AddForce(shadowPunchForce * dir, ForceMode2D.Impulse);
            
            yield return new WaitForSeconds(shadowPunchDuration);
            shadowPunch.body.velocity = Vector2.zero;
            
            yield return new WaitForSeconds(0.25f);
            timeElapsed = 0;
            startPos = shadowPunch.transform.position;
            returningShadowPunch = true;
            
            yield return new WaitForSeconds(returnPunchDuration);
            returningShadowPunch = false;
            performingShadowPunch = false;
            closeTime = 0;
            farTime = 0;
            mainAnim.SetTrigger("done");
            shadowPunch.gameObject.SetActive(false);
        }
    }

    public void HEX()
    {
        if (!dead && hex != null)
        {
            var obj = Instantiate(hex, target.position + Vector3.up, hex.transform.rotation, spawnedHolder.transform);
        }
    }


    IEnumerator TELEPORT_AGAIN()
    {
        yield return new WaitForSeconds(teleportAgain);
        
        mainAnim.speed = 1;
        mainAnim.SetTrigger("teleport");
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CreativeSpore.SuperTilemapEditor;

public class Gastly : Enemy
{
    public enum Variation { chaser, licker, willoWisp };
    
    [Space] [Header("Gastly")]  
    public Variation variant;
    // public TilemapChunk map;
    public STETilemap map;
    
    
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


    [Space] [Header("Licker")]  
    public bool licking;
    public float teleportAgain=1.5f;
    public float flipTimer=3f;
    [SerializeField] private float lickMoveForce=1;
    [SerializeField] private EnemyAttack lickAtk;


    [Space] [Header("Willo Wisp")]  
    [SerializeField] private GameObject wispHolder;
    [SerializeField] private Transform[] wispSpawns;
    [SerializeField] private EnemyProjectile wispAtk;
    [SerializeField] private List<EnemyProjectile> wisps;
    [SerializeField] private float wispFireDelay=0.8f;
    [SerializeField] private float wispSpeed=7.5f;
    private bool shootingWisp;


    

    [Space] [Header("Misc")]  
    // [SerializeField] private GameObject glint;
    [SerializeField] private bool variantSpeed;
    public Vector3 lineOfSight;
    [SerializeField] private GameObject spawnedHolder;
    private Coroutine co;
    [SerializeField] private Coroutine targetLostCo;
    private RaycastHit2D playerInfo;
    [SerializeField] private bool sceneLoaded=false;

    private float timer=0;
    private float closeTime=0;
    public float teleportTimer=1;
    private int origAtkDmg;
    public bool performingFuryAttack;
    [SerializeField] private int nMap;


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

        if (wispAtk != null)
        {
            wispAtk.atkDmg = projectileDmg + calcExtraProjectileDmg;
            wispAtk.kbForce = contactKb;
            wisps = new List<EnemyProjectile>();
        }
        if (variant == Variation.willoWisp)
        {
            kbDefense /= 4f;
            wispHolder.SetActive(true);
        }
        else
            wispHolder.SetActive(false);
    }

    IEnumerator LoadingIn()
    {
        yield return new WaitForSeconds(0.5f);
        sceneLoaded = true;
        playerInCloseRange  = false;
        playerInField       = false;
        playerInSight       = false;
        chasing             = false;
        licking             = false;
        targetLostCo        = null;
    }



	public override void CallChilByOther()
	{
		mainAnim.SetTrigger("intro");
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
            targetLostCo = StartCoroutine( TryToFindTarget(2f) );
    }
    public override void CallChildOnDeath()
    {
        mainAnim.speed = 0;
        mainAnim.SetTrigger("reset");
        StopAllCoroutines();
        foreach (EnemyProjectile wisp in wisps)
            if (wisp != null)
                Destroy( wisp.gameObject );
    }
    public override void CallChildOnDropLoot(bool attackedByPlayer=true)
    {
        bool insideWall = false;
        if (map != null)
        {
            int n = map.GetChunkCount();
            for (int i=0 ; i<n ; i++)
            {

                if (map.GetChunk(i).GetBounds().Contains(transform.position))
                {
                    Debug.Log(map.GetChunk(i).name);
                    insideWall = true;
                    break;
                }

                // Collider2D[] bounds = map.GetChunk(i).GetComponents<Collider2D>();
                // for (int j=0 ; j<bounds.Length ; j++)
                // {
                //     if (bounds[j].bounds.Contains(transform.position))
                //     {
                //         insideWall = true;
                //         break;
                //     }
                // }
                if (insideWall)
                    break;
            }
            if (!insideWall)
            {
                if (!isBoss && !isMiniBoss)
                    loot.DropLoot( Mathf.FloorToInt(lv / 10) );
                else
                    loot.DropLoot();
            }
        }
        else
            if (!isBoss && !isMiniBoss)
                loot.DropLoot( Mathf.FloorToInt(lv / 10) );
            else
                loot.DropLoot();
    }
    
    IEnumerator TryToFindTarget(float duration=2)
    {
        yield return new WaitForSeconds(duration);
        chasing = false;
        licking = false;
        playerInField = false;
        alert.SetActive(false);
        body.velocity = Vector2.zero;
        mainAnim.SetBool("isWisping", false);

        targetLostCo = null;
    }


    // todo ----------------------------------------------------------------------------------------------------
    void FixedUpdate() 
    {
        if (!sceneLoaded) {}
        else if (variant == Variation.chaser)   //* Variation.chaser
        {
            if (!performingBuff)
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
                            closeTime += Time.fixedDeltaTime;
                            if (closeTime > teleportTimer)
                            {
                                teleporting = true;
                                closeTime = 0;
                                mainAnim.speed = 1;
                                mainAnim.SetTrigger("teleport");
                            }
                        }
                        else
                            closeTime = 0;
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
        }
        else if (variant == Variation.willoWisp)
        {
            if (chasing && !licking)
            {
                licking = true;
                closeTime = 0;
                mainAnim.speed = 1;
                mainAnim.SetTrigger("wisp");
                mainAnim.SetBool("isWisping", true);
            }
            else if (!cannotMove && !chasing)
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
            else if (licking)
            {
                LookAtTarget();
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
        else if (variant == Variation.licker)
        {
            if (chasing && !teleporting && !licking)
            {
                teleporting = true;
                closeTime = 0;
                mainAnim.speed = 1;
                mainAnim.SetTrigger("teleport");
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
            // Vector3 lineOfSight = (target.position + new Vector3(0, 1)) - (this.transform.position);
            Gizmos.DrawLine(this.transform.position,
                this.transform.position + new Vector3(0, 1) + lineOfSight);
        }
        if (map != null && map.GetChunkCount() > nMap && map.GetChunk(nMap) != null)
        {
            map.GetChunk(nMap).DrawColliders();
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
    public void TELEPORT()
    {   
        if (target == null || hp <= 0)
            return;
        
        body.velocity = Vector2.zero;


        int rng = Random.Range(0,5);
        if (variant == Variation.licker)
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
        chasing = true;
        if (targetLostCo != null)
        {
            StopCoroutine( targetLostCo );
            targetLostCo = null;
        }
        SlowlyMove();
    }

    private void SlowlyMove()
    {
        if (variant != Variation.licker)
            return;

        Vector2 dir = ((target.position + Vector3.up) - transform.position).normalized;
        body.AddForce(dir * lickMoveForce, ForceMode2D.Impulse);
    }

    public void LICK()
    {
        if (variant == Variation.licker) 
        {
            licking = true;
            LookAtPlayer();
            mainAnim.SetTrigger("lick");
        }
        
    }



    public IEnumerator SUMMON_WISP()
    {
        if (shootingWisp || !chasing)
            yield break;
        shootingWisp = true;

        wisps.Clear();
        for (int i=0 ; i<wispSpawns.Length ; i++)
        {
            yield return new WaitForSeconds(0.5f);
            var obj = Instantiate(wispAtk, wispSpawns[i].position, wispAtk.transform.rotation, wispSpawns[i].transform);
            wisps.Add(obj);
        }

        if (wisps != null && wisps.Count > 0)
        {
            yield return new WaitForSeconds(1);
            EnemyProjectile[] temp = wisps.ToArray();
            foreach (EnemyProjectile wisp in temp)
            {
                yield return new WaitForSeconds(wispFireDelay);
                if (wisp != null)
                {
                    wisp.transform.parent = null;
                    Vector2 dir = ((target.position + Vector3.up) - wisp.transform.position).normalized;
                    wisp.body.AddForce(dir * wispSpeed, ForceMode2D.Impulse);
                    wisps.Remove(wisp);
                }
            }
        }
        shootingWisp = false;
    }


    IEnumerator TELEPORT_AGAIN()
    {
        yield return new WaitForSeconds(teleportAgain);
        
        mainAnim.speed = 1;
        mainAnim.SetTrigger("teleport");
    }

    public void AGILITY()
    {
        StartCoroutine( ResetBuff(5,5, Stat.spd) );
    }
}

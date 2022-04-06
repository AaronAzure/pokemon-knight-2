using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Pidgey : Enemy
{
    [Space] [Header("Pidgey")]  
    public Animator anim;
    public float moveSpeed=2;
    public float chaseSpeed=4;
    public float maxSpeed=5f;
    private LayerMask finalMask;
    public Transform target;
    public bool chasing;
    public bool playerInRange;
    private float timer;
    private float flipTimer=3;

    private bool once;
    private Coroutine targetLostCo;



    [Space] [Header("Miniboss attacks")]
    [SerializeField] private EnemyProjectile gust;
    [SerializeField] private GameObject glint;
    public Vector3 lineOfSight;
    [SerializeField] private AIDestinationSetter aiDest;
    [SerializeField] private AIPath aiPath;
    [SerializeField] private GameObject spawnedHolder;
    private Coroutine co;



    [Space] [Header("Debug")]
    [SerializeField] private string rayCast;



    public override void Setup()
    {
        finalMask = (whatIsPlayer | whatIsGround);
        if (alert != null) alert.gameObject.SetActive(false);
        if (spawnedHolder != null) 
            spawnedHolder.transform.parent = null;
            
        target = playerControls.gameObject.transform;
        if (isMiniBoss)
        {
            co = StartCoroutine( Attack(7.5f) );
            chasing = true;
            anim.speed = chaseSpeed;
            if (aiDest != null)
                aiDest.target = this.target;
            if (aiPath != null)
                aiPath.canMove = false;
        }
        else
        {
            // target = playerControls.gameObject.transform;
        }
    }

    public override void CallChildOnBossFightStart()
    {
        if (aiPath != null)
            aiPath.canMove = true;
    }
    public override void CallChildOnBossDeath()
    {
        if (co != null)
            StopCoroutine(co);
        if (aiPath != null)
            aiPath.canMove = false;
        body.gravityScale = 3;
        Destroy(spawnedHolder);
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
    public override void CallChildOnHalfHealth()
    {
        if (isMiniBoss)
        {
            aiPath.maxSpeed *= 1.25f;
            // aiPath.maxAcceleration *= 1.25f;
        }
    }

    public override void CallChildOnTargetLost()
    {
        if (targetLostCo == null)
            targetLostCo = StartCoroutine( TryToFindTarget(1f) );
    }
    IEnumerator TryToFindTarget(float duration=2)
    {
        yield return new WaitForSeconds(duration);
        chasing = false;
        alert.SetActive(false);
        body.velocity = Vector2.zero;
        playerInField = false;
        playerInSight = false;

        targetLostCo = null;
        timer = 0;
        anim.speed = 1;
    }



    // Start is called before the first frame update
    void FixedUpdate() 
    {
        // Wandering around ( MOB )
        if (!inCutscene && !isMiniBoss)
        {
            if (!chasing || inAnimation) { 
                body.velocity = Vector2.zero;

                if (timer < flipTimer)
                    timer += Time.fixedDeltaTime;
                else
                {
                    timer = 0;
                    Flip();
                }
            }
            // Chasing PLayer
            else if (!receivingKnockback)
            {
                Vector3 dir = (target.position + new Vector3(0,1) - this.transform.position).normalized;
                //* If at edge, then turn around
                body.AddForce(dir * 5 * chaseSpeed * Time.fixedDeltaTime, ForceMode2D.Impulse);
                CapVelocity();
            }

            if (target != null && playerInField)
            {
                // if (canUseBuffs)
                // {
                //     canUseBuffs = false;
                //     anim.SetTrigger("agility");
                //     inAnimation = true;
                // }
                
                lineOfSight = (target.position + new Vector3(0, 1)) - (this.transform.position);
                RaycastHit2D playerInfo = Physics2D.Linecast(this.transform.position,
                    this.transform.position + new Vector3(0, 1) + lineOfSight, finalMask);
                if (rayCast != null && playerInfo.collider != null)
                    rayCast = playerInfo.collider.name;
                if (playerInfo.collider != null && playerInfo.collider.gameObject.CompareTag("Player"))
                {
                    chasing = true;
                    anim.speed = chaseSpeed;
                    
                    //* BUFF
                    if (canUseBuffs)
                    {
                        canUseBuffs = false;
                        anim.SetTrigger("agility");
                        inAnimation = true;
                    }

                    if (alert != null) alert.gameObject.SetActive(true);
                    if (targetLostCo != null)
                    {
                        StopCoroutine( targetLostCo );
                        targetLostCo = null;
                    }
                }
                else if (playerInfo.collider != null)
                {
                    CallChildOnTargetLost();
                    // chasing = false;
                    // if (alert != null) alert.gameObject.SetActive(false);
                }
            }
            // else if (alert != null) alert.gameObject.SetActive(false);
            // else if (target != null && !playerInField && chasing)
            // {
            //     CallChildOnTargetLost();
            // }
        }
        if (isMiniBoss && hp > 0)
            LookAtTarget();
    }


    private void Flip()
    {
        if (model.transform.eulerAngles.y != 0) // currently facing right
            model.transform.eulerAngles = new Vector3(0, 0);
        else     // currently facing left
            model.transform.eulerAngles = new Vector3(0, 180);
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

    public void AGILITY()
    {
        StartCoroutine( ResetBuff(5,5, Stat.spd) );
    }

    IEnumerator Attack(float delay=5f)
    {
        yield return new WaitForSeconds(delay);
        if (glint != null) {
            glint.SetActive(false); 
            glint.SetActive(true);
        }
        anim.speed = 1;
        anim.SetTrigger("attack");
        aiPath.canMove = false;
    }
    
    public void Gust()
    {
        if (gust != null)
        {
            var obj = Instantiate(gust, this.transform.position - new Vector3(0,1),
                gust.transform.rotation, spawnedHolder.transform);
            obj.atkDmg = projectileDmg + calcExtraProjectileDmg;
        }
    }

    public void ResumeMovement()
    {
        aiPath.canMove = true;
        anim.speed = chaseSpeed;
        if (hpImg.fillAmount < 0.5f)
            co = StartCoroutine( Attack(3.5f) );
        else
            co = StartCoroutine( Attack() );
    }
}

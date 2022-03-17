using System.Collections;
using UnityEngine;

public class Butterfree : Enemy
{
    [Space] [Header("Butterfree")]  
    public float moveSpeed=2;
    public float chaseSpeed=4;
    public float maxSpeed=5f;
    public float chargeSpeed=10;
    private LayerMask finalMask;
    public Transform target;
    public bool chasing;
    public bool charging;
    private float chargeX;
    private float chargeY;
    public bool playerInRange;
    

    [Header("Attacks")]
    [SerializeField] private GameObject glint;
    [SerializeField] private EnemyAttack poisonPowder;
    [SerializeField] private Transform poisonPowderPos;
    public Vector3 lineOfSight;
    [SerializeField] private GameObject spawnedHolder;
    private Coroutine co;
    [SerializeField] private Coroutine targetLostCo;
    private RaycastHit2D playerInfo;

    private float timer=0;
    public float flipTimer=2;
    private float closeTime=0;
    public float poisonPowderAttackTimer=1;
    private int origAtkDmg;
    public bool performingPoisonPowder;


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
        if (!performingPoisonPowder && !performingBuff)
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

                        if (canUseBuffs)
                        {
                            // INCREASE_DEF();
                            mainAnim.speed = 1;
                            mainAnim.SetTrigger("harden");
                            performingBuff = true;
                            body.velocity = Vector2.zero;
                        }
                    }
                    // Player is close enough to perform a strong attack
                    if (playerInCloseRange)
                    {
                        closeTime += Time.fixedDeltaTime;
                        if (closeTime > poisonPowderAttackTimer)
                        {
                            performingPoisonPowder = true;
                            closeTime = 0;
                            body.velocity = Vector2.zero;
                            mainAnim.speed = 1;
                            mainAnim.SetTrigger("poisonPowder");
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
                    if (alert != null) alert.gameObject.SetActive(true);

                    if (!performingPoisonPowder)
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
            Vector3 tempSight = (target.position + new Vector3(0, 1)) - (this.transform.position);
            Gizmos.DrawLine(this.transform.position,
                this.transform.position + new Vector3(0, 1) + tempSight);
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
            charging = false;
        }
    }
    public void FINISHED_PERFORMING_POISON_POWDER()
    {
        performingPoisonPowder = false;
        contactDmg = origAtkDmg;
    }
    public void POISON_POWDER()
    {
        if (hp > 0)
        {
            var obj = Instantiate(poisonPowder, poisonPowderPos.position, Quaternion.identity);
            obj.atkDmg = projectileDmg + calcExtraProjectileDmg;
            
            Destroy(obj.gameObject, 4.5f);
        }
    }

    public void AGILITY()
    {
        StartCoroutine( ResetBuff(5,5, Stat.spd) );
    }



    public void CAN_MOVE() { }
}

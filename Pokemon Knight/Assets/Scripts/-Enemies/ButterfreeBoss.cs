using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButterfreeBoss : Enemy
{
    [Space] [Header("Butterfree")]  public float moveSpeed=7.5f;
    public float chaseSpeed=3.5f;
    public float maxSpeed=5f;
    public float dashSpeed=50;
    public GameObject player;
    private Transform target;
    private Vector3 targetPos;
    private int count;
    private int newAttackPattern=5;
    private bool callOnce;

    [Header("Attacks")]
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject glint;
    [SerializeField] private EnemyAttack stunSpore;
    [SerializeField] private Transform stunSporePos;
    private int atkCount;
    private bool canMove=true;
    public bool performingPoisonPowder;
    [SerializeField] private GameObject spawnHolder;
    private bool tackledAgain;
    [SerializeField] private GameObject tackleEffect;


    public override void Setup()
    {
        int gameNumber = PlayerPrefsElite.GetInt("gameNumber");
        if (PlayerPrefsElite.VerifyBoolean("canDoubleJump" + gameNumber) && PlayerPrefsElite.GetBoolean("canDoubleJump" + gameNumber))
            Destroy(this.gameObject);
        if (statusBar != null)
            statusBar.SetActive(false);
        playerControls = GameObject.Find("PLAYER").GetComponent<PlayerControls>();
        target = playerControls.transform;
        
        if (spawnHolder != null)
            spawnHolder.transform.parent = null;
    }

    public override void CallChildOnBossFightStart()
    {
        player = playerControls.gameObject;
        count = 0;
        StartCoroutine( TrackPlayer() );
    }
    public override void CallChildOnRageCutsceneFinished()
    {
        count = newAttackPattern;
        atkCount = 0;
    }

    public override void CallChildOnRage() 
    {
        count = 0;
        newAttackPattern = 4;
        moveSpeed *= 1.25f;
        // chaseSpeed *= 1.5f;
        maxSpeed *= 1.25f;
    }
    public override void CallChildOnBossDeath() 
    {
        if (spawnHolder != null)
            Destroy(spawnHolder);
        StopAllCoroutines();
    }

    void FixedUpdate()
    {
        if (!inCutscene && !inRageCutscene && canMove && hp > 0)
        {
            if (count < newAttackPattern)
            {
                // //Find direction
                // Vector3 dir = (targetPos - body.transform.position).normalized;
                // //Check if we need to follow object then do so 
                // // if (!receivingKnockback)
                // if (Vector3.Distance(targetPos, body.transform.position) > 0.5f)
                //     body.AddForce(dir * moveSpeed * Time.fixedDeltaTime, ForceMode2D.Impulse);
                // else {
                //     LocatePlayer();
                // }
                Vector3 dir = (target.position + new Vector3(0,1) - this.transform.position).normalized;
                //* If at edge, then turn around
                body.AddForce(dir * 5 * chaseSpeed * Time.fixedDeltaTime, ForceMode2D.Impulse);
                CapVelocity();
            }
        }
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


    private void LocatePlayer()
    {
        targetPos = player.transform.position + new Vector3(0,1);
        if (targetPos.x - this.transform.position.x > 0)
        {
            // Debug.Log("looking right");
            model.transform.eulerAngles = new Vector3(0,180);
        }
        else if (targetPos.x - this.transform.position.x < 0)
        {
            // Debug.Log("looking left");
            model.transform.eulerAngles = new Vector3(0,0);
        }
    }

    IEnumerator TrackPlayer()
    {
        yield return new WaitForSeconds(1);
        LocatePlayer();
        if (!inCutscene)
            count++;

        // Keep chasing
        if (count < newAttackPattern)
            StartCoroutine( TrackPlayer() );
        else
            ChooseAttack();
    }

    void ChooseAttack()
    {
        atkCount++;
        if (atkCount % 2 == 0)
            StartCoroutine( Tackle() );
        else
            StartCoroutine( PoisonPowder() );
    }


    IEnumerator Tackle()
    {
        // CHARGING UP
        body.velocity = Vector2.zero;

        glint.SetActive(false);
        // yield return new WaitForEndOfFrame();
        glint.SetActive(true);
        LocatePlayer();
        
		// LOCATE TARGET
        yield return new WaitForSeconds(0.3f);
        LocatePlayer();
        
		// DASH
        yield return new WaitForSeconds(0.5f);
        int tempDmg = contactDmg;
        contactDmg = secondDmg;

        Vector2 dir = (targetPos - body.transform.position).normalized;
        if (!inCutscene) 
        {
            tackleEffect.SetActive(false);
            tackleEffect.SetActive(true);
            tackleEffect.transform.rotation = Quaternion.LookRotation(-dir, Vector3.up);
            body.AddForce(dir*dashSpeed, ForceMode2D.Impulse);
        }

        // resting
        yield return new WaitForSeconds(0.6f);
		contactDmg = tempDmg;

        // cannotRecieveKb = false;
        body.velocity = Vector2.zero;
        if (inRage && !tackledAgain)
        {
            StartCoroutine( Tackle() );
            tackledAgain = true;
        }
        else
        {
            tackledAgain = false;
            count = 0;
            LocatePlayer();
            StartCoroutine( TrackPlayer() );
        }
    }
    IEnumerator PoisonPowder()
    {
        if (inRage)
        {
            Harden();
            yield return new WaitForSeconds(1.25f);
        }
        anim.SetTrigger("stunSpore");
        body.velocity = Vector2.zero;
        yield return new WaitForSeconds(0.5f);
        if (!inCutscene) 
        {
            var obj = Instantiate(stunSpore, stunSporePos.position, Quaternion.identity, spawnHolder.transform);
            obj.atkDmg = projectileDmg + calcExtraProjectileDmg;
            if (inRage)
                obj.transform.localScale *= 1.5f;
            Destroy(obj.gameObject, 4.5f);
        }

        // resting
        yield return new WaitForSeconds(0.75f);
        count = 0;
        LocatePlayer();
        StartCoroutine( TrackPlayer() );
    }

    private void Harden()
    {
        if (mainAnim != null)
        {
            body.velocity = Vector2.zero;
            mainAnim.SetTrigger("harden");
            canMove = false;
        }
    }
    public void INCREASE_DEF_CO()
    {
        StartCoroutine( ResetBuff(6, 4, Stat.def) );
    }
    public void CAN_MOVE()
    {
        canMove = true;
        // count = 0;
        // LocatePlayer();
        // StartCoroutine( TrackPlayer() );
    }

    // IEnumerator HardenCo()
    // {
    //     Harden();

    //     yield return new WaitForSeconds(10);
    //     StartCoroutine( HardenCo() );
    // }
}

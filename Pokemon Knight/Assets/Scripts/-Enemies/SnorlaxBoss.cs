using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnorlaxBoss : Enemy
{
    [Space] [Header("Snorlax")]  public float moveSpeed=2.5f;
    // public float dashSpeed=50;
    public float jumpHeight=20;
    private Transform target;
    private int count;
    private int newAttackPattern=5;
    private bool performingNextAtk;
    private bool canAtk;
    [SerializeField] private Transform groundDetect;

    [Header("Attacks")]
    private Coroutine co;
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject glint;
    [SerializeField] private GameObject bodySlamExplosion;
    private int atkCount;
    private bool bodySlamming;
    private RaycastHit2D groundInfo;

    public override void Setup()
    {
        // int gameNumber = PlayerPrefsElite.GetInt("gameNumber");
        // if (PlayerPrefsElite.VerifyBoolean("canSecondAtk" + gameNumber) && PlayerPrefsElite.GetBoolean("canSecondAtk" + gameNumber))
        //     Destroy(this.gameObject);
        if (statusBar != null)
            statusBar.SetActive(false);
        playerControls = GameObject.Find("PLAYER").GetComponent<PlayerControls>();
        target = playerControls.transform;
    }

    public override void CallChildOnBossFightStart()
    {
        count = 0;
        canAtk = true;
    }
    public override void CallChildOnRage()
    {
        performingNextAtk = false;
        body.gravityScale = 3;
        anim.speed = 1;
        anim.SetTrigger("reset");
        if (co != null)
            StopCoroutine(co);
    }
    public override void CallChildOnDeath()
    {
        performingNextAtk = false;
        body.gravityScale = 3;
        anim.speed = 1;
        anim.SetTrigger("reset");
        if (co != null)
            StopCoroutine(co);
    }


    void FixedUpdate()
    {
        if (!inCutscene && canAtk)
        {
            if (bodySlamming)
            {
                groundInfo = Physics2D.Raycast(groundDetect.position, Vector2.down, 0.5f, whatIsGround);
                if (groundInfo && body.velocity.y == 0)
                {
                    if (bodySlamExplosion != null)
                    {
                        bodySlamExplosion.SetActive(false);
                        bodySlamExplosion.SetActive(true);
                    }
                    bodySlamming = false;
                    body.velocity = Vector2.zero;
                    co = StartCoroutine( GetUp() );
                }
            }
            else if (!performingNextAtk)
            {
                performingNextAtk = true;
                co = StartCoroutine( BodySlam() );
            }
        }
    }

    private void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(groundDetect.position, groundDetect.position + new Vector3(0,-0.5f));
    }

    IEnumerator GetUp()
    {
        if (!inRage)
        {
            yield return new WaitForSeconds(1);
            anim.speed = 1;
        }
        else
        {
            yield return new WaitForSeconds(0.25f);
            anim.speed = 1.5f;
        }
        anim.SetTrigger("getUp");
        body.velocity = Vector2.zero;
    }
    IEnumerator BodySlam()
    {
        body.velocity = Vector2.zero;
        if (!inRage)
            yield return new WaitForSeconds(1.5f);
        else
        {
            yield return new WaitForSeconds(0.5f);
            anim.speed = 1.5f;
        }
        LookAtPlayer();

        float xTargetPos = 0;
        if (playerControls != null)
            xTargetPos = (playerControls.transform.position.x - this.transform.position.x);
        xTargetPos += Random.Range(-1,2);

        anim.SetTrigger("bodySlam");
        body.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
        body.AddForce(Vector2.right * xTargetPos, ForceMode2D.Impulse);
    }


    public void PERFORMED_ACTION()
    {
        performingNextAtk = false;
        body.gravityScale = 3;
        anim.speed = 1;
    }
    public void BODY_SLAMMING()
    {
        bodySlamming = true;
    }
}

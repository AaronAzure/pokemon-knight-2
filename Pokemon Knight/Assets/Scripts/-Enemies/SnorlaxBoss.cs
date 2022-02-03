using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnorlaxBoss : Enemy
{
    [Space] [Header("Snorlax")]  public float moveSpeed=2.5f;
    // public float dashSpeed=50;
    public float jumpHeight=20;
    private Transform target;
    private int atkCount;
    private int newAttackPattern=3;
    private bool performingNextAtk;
    private bool canAtk;
    [SerializeField] private Transform groundDetect;
    [SerializeField] private ParticleSystem yawnEffect;
    [SerializeField] private EnemyProjectile yawnAtk;
    [SerializeField] private Transform yawnPos;


    [Header("Attacks")]
    private Coroutine co;
    private int[] bodySlamOffset={-2,0,2};
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject glint;
    [SerializeField] private GameObject bodySlamExplosion;
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

    public override void CallChildOnRoar()
    {
        if (yawnEffect != null)
            yawnEffect.Stop(true, ParticleSystemStopBehavior.StopEmitting);
    }
    public override void CallChildOnBossFightStart()
    {
        atkCount = 0;
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
                if (atkCount  != newAttackPattern)
                    co = StartCoroutine( BodySlam() );
                else
                    anim.SetTrigger("yawn");
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
            yield return new WaitForSeconds(0.75f);
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
            yield return new WaitForSeconds(1f);
        else
        {
            yield return new WaitForSeconds(0.5f);
            anim.speed = 1.5f;
        }
        LookAtPlayer();

        float xTargetPos = 0;
        if (playerControls != null)
            xTargetPos = (playerControls.transform.position.x - this.transform.position.x);
        xTargetPos += bodySlamOffset[ Random.Range(0, bodySlamOffset.Length) ];

        anim.SetTrigger("bodySlam");
        body.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
        body.AddForce(Vector2.right * xTargetPos, ForceMode2D.Impulse);
    }


    public void PERFORMED_ACTION()
    {
        performingNextAtk = false;
        body.gravityScale = 3;
        anim.speed = 1;
        atkCount++;
        if (atkCount > newAttackPattern)
            atkCount = 0;
    }
    public void BODY_SLAMMING()
    {
        bodySlamming = true;
    }
    public void YAWN()
    {
        var obj = Instantiate(yawnAtk, yawnPos.position, yawnAtk.transform.rotation);
        LookAtPlayer();
        Vector2 dir = (playerControls.transform.position + new Vector3(0,2f) - yawnPos.position).normalized;
        obj.direction = dir;

        if (inRage)
        {

        }
    }
}
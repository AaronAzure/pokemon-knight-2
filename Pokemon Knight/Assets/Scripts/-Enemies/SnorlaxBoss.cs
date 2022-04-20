using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnorlaxBoss : Enemy
{
    [Space] [Header("Snorlax")]  public float moveSpeed=2.5f;
    // public float dashSpeed=50;
    public float jumpHeight=20;
    private Vector3 target;
    private int atkCount;
    private int newAttackPattern=3;
    private bool performingNextAtk;
    private bool canAtk;
    [SerializeField] private Transform groundDetect;
    [SerializeField] private ParticleSystem yawnEffect;
    [SerializeField] private EnemyProjectile yawnAtk;
    [SerializeField] private Transform yawnPos;
    [SerializeField] private GameObject spawnHolder;


    [Header("Attacks")]
    private Coroutine co;
    private int[] bodySlamOffset={-2,0,2};
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject glint;
    [SerializeField] private GameObject bodySlamExplosion;
    private bool bodySlamming;
    private RaycastHit2D groundInfo;
    private int yawnCount;
    private int gigaImpactCount;
    public float gigaImpactForce = 30;
    public float gigaImpactDuration = 1f;

    public override void Setup()
    {
        if (PlayerPrefsElite.VerifyBoolean("canUseUlt" + PlayerPrefsElite.GetInt("gameNumber")))
            if (PlayerPrefsElite.GetBoolean("canUseUlt" + PlayerPrefsElite.GetInt("gameNumber")))
            {
                if (bossRoom != null)
                    bossRoom.Walls(false);
                Destroy( this.gameObject );
            }
        if (statusBar != null)
            statusBar.SetActive(false);
        playerControls = GameObject.Find("PLAYER").GetComponent<PlayerControls>();
        target = playerControls.transform.position;
        
        if (spawnHolder != null)
            spawnHolder.transform.parent = null;
    }
    public override void CallChildOnDamaged()
    {
        //* CALLED ONCE
        if (anim != null)
            anim.SetTrigger("attacked");
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
        bodySlamming = false;
        anim.SetTrigger("reset");
        if (co != null)
            StopCoroutine(co);
    }
    public override void CallChildOnBossDeath()
    {
        performingNextAtk = false;
        body.gravityScale = 3;
        anim.speed = 0.3f;
        anim.SetTrigger("reset");
        if (co != null)
            StopCoroutine(co);
        StopAllCoroutines();
        if (spawnHolder != null)
            Destroy( spawnHolder );
    }
    public override void CallChildOnRageCutsceneFinished()
    {
        atkCount = 0;
        gigaImpactCount = 0;
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
                if (inRage && atkCount == gigaImpactCount)
                    co = StartCoroutine( GigaImpact() );
                else if (atkCount  != newAttackPattern)
                    co = StartCoroutine( BodySlam() );
                else
                {
                    LookAtPlayer();
                    anim.SetTrigger("yawn");
                }
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
        yield return new WaitForSeconds(0.25f);
        if (!inRage)
            anim.speed = 1;
        else
            anim.speed = 1.5f;
        anim.SetTrigger("getUp");
        body.velocity = Vector2.zero;
    }
    IEnumerator BodySlam()
    {
        body.velocity = Vector2.zero;
        if (!inRage)
            yield return new WaitForSeconds(0.75f);
        else
        {
            yield return new WaitForSeconds(0.5f);
            anim.speed = 1.3f;
        }
        LookAtPlayer();

        float xTargetPos = 0;
        if (playerControls != null)
            xTargetPos = (playerControls.transform.position.x - this.transform.position.x);
        xTargetPos += bodySlamOffset[ Random.Range(0, bodySlamOffset.Length) ];
        // contactDmg = 50;

        anim.SetTrigger("bodySlam");
        body.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
        body.AddForce(Vector2.right * xTargetPos, ForceMode2D.Impulse);
    }
    IEnumerator GigaImpact()
    {
        LookAtPlayer();
        anim.SetTrigger("gigaImpact");
        anim.speed = 1;
        
        yield return new WaitForSeconds(1.5f);
        
        this.transform.position += new Vector3(0, 0.3f); // AVOID STOP ON GROUND
        int tempDmg = contactDmg;
        contactDmg = secondDmg;

        if (IsLookingLeft())
            body.AddForce(Vector2.left * gigaImpactForce, ForceMode2D.Impulse);
        else
            body.AddForce(Vector2.right * gigaImpactForce, ForceMode2D.Impulse);

        yield return new WaitForSeconds( gigaImpactDuration );
        contactDmg = tempDmg;
        body.velocity = new Vector2(0, body.velocity.y);
    }


    public void PERFORMED_ACTION()
    {
        performingNextAtk = false;
        body.gravityScale = 3;
        anim.speed = 1;
        // contactDmg = origContactDmg;
        atkCount++;
        if (atkCount > newAttackPattern)
        {
            atkCount = 0;
            if (inRage)
                gigaImpactCount = Random.Range(0, newAttackPattern);
        }
    }
    public void BODY_SLAMMING()
    {
        bodySlamming = true;
    }

    public void YAWN_AGAIN()
    {
        // if (inRage && yawnCount == 0)
        // {
        //     yawnCount++;
        //     LookAtPlayer();
        //     anim.SetTrigger("yawn");
        // }
        // else if (inRage && yawnCount > 0)
        // {
        //     yawnCount = 0;
        //     PERFORMED_ACTION();
        // }
        // else if (!inRage)
        // {
        // }
        PERFORMED_ACTION();
    }
    public void YAWN()
    {
        LookAtPlayer();
        var obj = Instantiate(yawnAtk, yawnPos.position, yawnAtk.transform.rotation);
        if (spawnHolder != null)
            obj.transform.parent = spawnHolder.transform;
        Vector2 dir = (playerControls.transform.position + new Vector3(0,2f) - yawnPos.position).normalized;
        obj.direction = dir;

        if (obj.anim != null && Random.Range(0,2) == 0)
            obj.anim.SetTrigger("reverse");
    }
}

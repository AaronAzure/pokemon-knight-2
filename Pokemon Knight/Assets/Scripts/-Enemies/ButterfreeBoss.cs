using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButterfreeBoss : Enemy
{
    [Space] [Header("Butterfree")]  public float moveSpeed=7.5f;
    public float dashSpeed=50;
    public GameObject player;
    private Vector3 target;
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


    public override void Setup()
    {
        int gameNumber = PlayerPrefsElite.GetInt("gameNumber");
        if (PlayerPrefsElite.VerifyBoolean("canDoubleJump" + gameNumber) && PlayerPrefsElite.GetBoolean("canDoubleJump" + gameNumber))
            Destroy(this.gameObject);
        if (statusBar != null)
            statusBar.SetActive(false);
        playerControls = GameObject.Find("PLAYER").GetComponent<PlayerControls>();
    }

    public override void CallChildOnBossFightStart()
    {
        player = playerControls.gameObject;
        count = 0;
        StartCoroutine( TrackPlayer() );
    }
    public override void CallChildOnRageCutsceneFinished()
    {
        atkCount = 0;
    }

    void FixedUpdate()
    {
        if (!inCutscene && !inRageCutscene && canMove)
        {
            if (count < newAttackPattern)
            {
                //Find direction
                Vector3 dir = (target - body.transform.position).normalized;
                //Check if we need to follow object then do so 
                // if (!receivingKnockback)
                if (Vector3.Distance(target, body.transform.position) > 0.5f)
                    body.AddForce(dir * moveSpeed * Time.fixedDeltaTime, ForceMode2D.Impulse);
                else {
                    LocatePlayer();
                }
            }
        }
    }

    public override void CallChildOnRage() 
    {
        count = 0;
        newAttackPattern = 3;
        moveSpeed *= 1.5f;
    }

    private void LocatePlayer()
    {
        target = player.transform.position + new Vector3(0,1);
        if (target.x - this.transform.position.x > 0)
        {
            // Debug.Log("looking right");
            model.transform.eulerAngles = new Vector3(0,180);
        }
        else if (target.x - this.transform.position.x < 0)
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
        // charging up
        body.velocity = Vector2.zero;

        glint.SetActive(false);
        yield return new WaitForEndOfFrame();
        glint.SetActive(true);
        
        if (inRage)
            yield return new WaitForSeconds(0.5f);
        else
            yield return new WaitForSeconds(0.8f);
        // cannotRecieveKb = true;
        Vector3 dir = (target - body.transform.position).normalized;
        if (!inCutscene) 
            body.AddForce(dir*dashSpeed, ForceMode2D.Impulse);

        // resting
        yield return new WaitForSeconds(0.3f);
        // cannotRecieveKb = false;
        count = 0;
        LocatePlayer();
        StartCoroutine( TrackPlayer() );
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
            var obj = Instantiate(stunSpore, stunSporePos.position, Quaternion.identity);
            obj.atkDmg = projectileDmg + calcExtraProjectileDmg;
            if (inRage)
                obj.transform.localScale *= 1.75f;
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

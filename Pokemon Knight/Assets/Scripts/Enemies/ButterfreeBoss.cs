using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButterfreeBoss : Enemy
{
    public float moveSpeed=7.5f;
    public float dashSpeed=50;
    public GameObject player;
    private Vector3 target;
    private int count;
    private int newAttackPattern=5;
    private float localX;
    private bool callOnce;

    [Header("Attacks")]
    [SerializeField] private Animator anim;
    [SerializeField] private GameObject glint;
    [SerializeField] private GameObject stunSpore;
    [SerializeField] private Transform stunSporePos;
    private int atkCount;

    // Start is called before the first frame update
    void Start()
    {
        count = 0;
        StartCoroutine( BossIntro() );
        StartCoroutine( TrackPlayer() );
        localX = model.transform.localScale.x;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!inCutscene)
        {
            if (count < newAttackPattern)
            {
                //Find direction
                Vector3 dir = (target - body.transform.position).normalized;
                //Check if we need to follow object then do so 
                // if (!receivingKnockback)
                if (Vector3.Distance(target, body.transform.position) > 0.5f)
                    body.MovePosition(body.transform.position + dir * moveSpeed * Time.fixedDeltaTime);
                else {
                    LocatePlayer();
                }
            }
        }
        if (inRage && !callOnce)
        {
            callOnce = true;
            count = 0;
            newAttackPattern = 3;
            moveSpeed = 12.5f;
        }
    }

    private void LocatePlayer()
    {
        target = player.transform.position + new Vector3(0,3);
        if (target.x - this.transform.position.x > 0)
        {
            // Debug.Log("looking right");
            //! model.transform.localScale = new Vector3(-localX, localX, 1);
            model.transform.eulerAngles = new Vector3(0,180);
        }
        else if (target.x - this.transform.position.x < 0)
        {
            // Debug.Log("looking left");
            model.transform.eulerAngles = new Vector3(0,0);
            //! model.transform.localScale = new Vector3(localX, localX, 1);
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
            StartCoroutine( StunSpore() );
    }

    IEnumerator Tackle()
    {
        // charging up
        body.velocity = Vector2.zero;
        glint.SetActive(true);
        if (inRage)
            yield return new WaitForSeconds(0.5f);
        else
            yield return new WaitForSeconds(1);
        Vector3 dir = (target - body.transform.position).normalized;
        if (!inCutscene) 
            body.AddForce(dir*dashSpeed, ForceMode2D.Impulse);

        // resting
        yield return new WaitForSeconds(0.5f);
        glint.SetActive(false);
        count = 0;
        LocatePlayer();
        StartCoroutine( TrackPlayer() );
    }
    IEnumerator StunSpore()
    {
        anim.SetTrigger("stunSpore");
        body.velocity = Vector2.zero;
        yield return new WaitForSeconds(0.5f);
        if (!inCutscene) 
        {
            var obj = Instantiate(stunSpore, stunSporePos.position, Quaternion.identity);
            if (inRage)
                obj.transform.localScale *= 1.75f;
            Destroy(obj.gameObject, 4);
        }

        // resting
        yield return new WaitForSeconds(2f);
        count = 0;
        LocatePlayer();
        StartCoroutine( TrackPlayer() );
    }
}

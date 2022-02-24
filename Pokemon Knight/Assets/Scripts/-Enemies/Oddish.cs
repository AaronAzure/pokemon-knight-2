using UnityEngine;
using System.Collections;


public class Oddish : Enemy
{
    [Space] [Header("Oddish")]  public float moveSpeed=2;
    public float distanceDetect=1f;
    public Transform groundDetection;
    public float forwardDetect=1f;
    public Transform face;


    [Header("Attacks")]
    [SerializeField] private Animator anim;
    [SerializeField] private EnemyAttack stunSpore;
    [SerializeField] private Transform stunSporePos;
    public bool canSeePlayer;
    public bool canAtk = true;
    private Coroutine co;


    // [Space] [Header("Buffs")]
    private bool canUseGrowth=true;

    [Space] [Header("Miniboss attacks")]
    // [SerializeField] private GameObject glint;
    [SerializeField] private GameObject spawnedHolder;
    [SerializeField] private EnemyProjectile sludgeBomb;
    [SerializeField] private Transform sludgeBombPos;
    [Space] [SerializeField] private Transform target;
    private float trajectory;
    private int attackCount;


    public override void Setup()
    {
        if (spawnedHolder != null) 
            spawnedHolder.transform.parent = null;
            
        if (isMiniBoss)
        {
            if (target == null)
                target = GameObject.Find("PLAYER").transform;
            // co = StartCoroutine( Attack(7.5f) );
        }
        else
        {
            co = StartCoroutine( DoSomething() );
        }
    }

    public override void CallChildOnBossFightStart()
    {
        anim.SetTrigger("sludgeBomb");
    }

    public override void CallChildOnBossDeath()
    {
        if (co != null)
            StopCoroutine(co);
        body.gravityScale = 3;
        Destroy(spawnedHolder);
    }

    void FixedUpdate() 
    {
        if (!inCutscene && !isMiniBoss)
        {
            if (!receivingKnockback && hp > 0)
            {
                if (movingLeft)
                    body.velocity = new Vector2(-moveSpeed, body.velocity.y);
                else if (movingRight)
                    body.velocity = new Vector2(moveSpeed, body.velocity.y);

            }

            if (canSeePlayer && canAtk)
            {
                canAtk = false;
                StartCoroutine(Attack());
            }
            RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, distanceDetect, whatIsGround);
            RaycastHit2D frontInfo;
            if (model.transform.eulerAngles.y > 0) // right
                frontInfo = Physics2D.Raycast(groundDetection.position, Vector2.right, distanceDetect, whatIsGround);
            else // left
                frontInfo = Physics2D.Raycast(groundDetection.position, Vector2.left, distanceDetect, whatIsGround);

            if (movingRight)
                frontInfo = Physics2D.Raycast(groundDetection.position, Vector2.right, distanceDetect, whatIsGround);
            
            //* If at edge, then turn around
            if (body.velocity.y >= 0 && (!groundInfo || frontInfo))
                Flip();
        }
        else
        {

        }
    }

    void AdjustAnim(string triggerName)
    {
        anim.SetTrigger(triggerName);
        if (triggerName == "walking")
            anim.speed = moveSpeed;
        else
            anim.speed = 1;
    }

    IEnumerator DoSomething()
    {
        yield return new WaitForSeconds(2);
        switch (Random.Range(0,2))
        {
            // Move right
            case 0:
                movingRight = true;
                movingLeft = false;
                model.transform.eulerAngles = new Vector3(0, 180);
                break;
            // Move left
            case 1:
                movingRight = false;
                movingLeft = true;
                model.transform.eulerAngles = new Vector3(0, 0);
                break;
        }
        AdjustAnim("walking");

        yield return new WaitForSeconds(2);
        AdjustAnim("idling");
        movingRight = false;
        movingLeft = false;
        body.velocity = new Vector2(0, body.velocity.y);

        canAtk = true;
        co = StartCoroutine(DoSomething());
    }

    private void Flip()
    {
        if (!canFlip)
            return;
        if (model.transform.eulerAngles.y != 0)
        {
            model.transform.eulerAngles = new Vector3(0, 0);    // left
            movingRight = false;
            movingLeft = true;
        }
        else
        {
            model.transform.eulerAngles = new Vector3(0, 180);  // right
            movingLeft = false;
            movingRight = true;
        }
        StartCoroutine( ResetFlipTimer() );
    }
    public IEnumerator Attack()
    {
        yield return new WaitForSeconds(0.2f);
        StopCoroutine(co);
        movingLeft = false;
        movingRight = false;
        canAtk = false;
        if (canUseBuffs && canUseGrowth)
            AdjustAnim("growth");
        else
            AdjustAnim("attacking");

        yield return new WaitForSeconds(1f);
        co = StartCoroutine( DoSomething() );
    }

    public void STOP_MOVING()
    {
        if (hp > 0)
            body.velocity = new Vector2(0, body.velocity.y);
    }

    public void GROWTH()
    {
        if (hp > 0 && canUseBuffs)
        {
            canUseBuffs = false;
            StartCoroutine( GrowthTimer() );
        }
    }
    IEnumerator GrowthTimer()
    {
        IncreaseAtk();
        yield return new WaitForSeconds(1);
        canAtk = true;

        yield return new WaitForSeconds(4);
        RevertAtk();

        yield return new WaitForSeconds(5);
        canUseBuffs = true;
    }
    public void POISON_POWDER()
    {
        if (hp > 0)
        {
            body.velocity = new Vector2(0, body.velocity.y);
            var obj = Instantiate(stunSpore, stunSporePos.position, stunSporePos.transform.rotation);
            obj.atkDmg = projectileDmg + calcExtraProjectileDmg;
            Destroy(obj, 4.5f);
        }
    }





    // todo ---------- M I N I  B O S S ----------

    public void SLUDGE_BOMB()
    {
        if (hp > 0)
        {
            if (sludgeBomb != null && sludgeBombPos != null)
            {
                var obj = Instantiate(sludgeBomb, sludgeBombPos.position, sludgeBomb.transform.rotation);
                obj.body.gravityScale = 3;
                obj.atkDmg = projectileDmg + calcExtraProjectileDmg;
                obj.direction = new Vector2(-trajectory, Random.Range(14,21));
                if (spawnedHolder != null)
                    obj.transform.parent = spawnedHolder.transform;
            }
        }
    }

    public void FACE_TARGET()
    {
        if (this.transform.position.x > target.position.x)
            model.transform.eulerAngles = new Vector3(0, 0);
        else
            model.transform.eulerAngles = new Vector3(0, 180);
        
        trajectory = CalculateTrajectory();
    }

    private float CalculateTrajectory()
    {
        if (attackCount == 1)
            return (this.transform.position.x - target.position.x);
        return (this.transform.position.x - target.position.x) + Random.Range(-1f,1f);
    }

    public void ATTACK_COUNT()
    {
        if (attackCount < 3)
        {
            attackCount++;
        }
        else 
        {
            // anim.Play("animState", -1, 0);
            anim.SetTrigger("reset");
            co = StartCoroutine( RestBeforeNextVolley() );
        }
    }

    IEnumerator RestBeforeNextVolley()
    {
        if (hpImg.fillAmount <= 0.5f)
            yield return new WaitForSeconds(1.5f);
        else
            yield return new WaitForSeconds(3);
        if (hp > 0)
            anim.SetTrigger("sludgeBomb");
        attackCount = 0;
    }



    private void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(groundDetection.position, groundDetection.position + new Vector3(0,-distanceDetect));
        Gizmos.DrawLine(face.position, face.position + new Vector3(-forwardDetect,0));
    }
}

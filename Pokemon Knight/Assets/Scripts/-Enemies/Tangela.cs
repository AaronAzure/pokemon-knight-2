using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tangela : Enemy
{
    [Space] [Header("Tangela")]  public float moveSpeed=2;
    public float distanceDetect=1f;
    public Transform groundDetection;
    [SerializeField] private LayerMask whatIsTree;
    public float forwardDetect=1f;
    public Transform face;
    


    [Space] public EnemyProjectile absorbObj;
    private Transform target;
    private float multiplier=-1.1f;
    private float trajectory;
    private LayerMask finalMask;

    [Space] [SerializeField] private GameObject spawnedHolder;
    [SerializeField] private Transform[] absorbSpawns;
    private Coroutine co;

    private bool massAbsorbAtk;
    private int sameAtkPattern;
    private bool canMove=true;
    public bool attacking;

    public override void Setup()
    {
        if (GameObject.Find("PLAYER") != null)
            target = GameObject.Find("PLAYER").transform;

        if (alert != null) 
            alert.gameObject.SetActive(false);

        finalMask = (whatIsPlayer | whatIsGround);
        
        if (spawnedHolder != null)
            spawnedHolder.transform.parent = null;

        if (isMiniBoss)
            canMove = false;
    }

        
    public override void CallChildOnBossFightStart()
    {
        co = StartCoroutine( ChooseAttack(0.5f) );
        canMove = true;
        massAbsorbAtk = (Random.Range(0,2) == 0 ? true : false);
    }
    public override void CallChildOnDeath()
    {
        if (spawnedHolder != null)
            Destroy(spawnedHolder);
    }
    public override void CallChildOnBossDeath()
    {
        if (co != null)
            StopCoroutine(co);
        if (spawnedHolder != null)
            Destroy(spawnedHolder);
    }

    private void FixedUpdate() 
    {
        if (!isMiniBoss)
        {
            RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, distanceDetect, whatIsGround);
            RaycastHit2D frontInfo;
            if (model.transform.eulerAngles.y > 0)    // right
                frontInfo = Physics2D.Raycast(groundDetection.position, Vector2.right, distanceDetect, whatIsGround);
            else    // left
                frontInfo = Physics2D.Raycast(groundDetection.position, Vector2.left, distanceDetect, whatIsGround);

            if (hp > 0 && !receivingKnockback)
            {
                if (movingLeft)
                {
                    body.velocity = new Vector2(-moveSpeed, body.velocity.y);
                    model.transform.eulerAngles = new Vector3(0, 0);
                }
                else if (movingRight)
                {
                    body.velocity = new Vector2(moveSpeed, body.velocity.y);
                    model.transform.eulerAngles = new Vector3(0, 180);
                }
            }
                
            if ((!groundInfo || frontInfo) && canFlip && body.velocity.y >= 0 && (movingLeft || movingRight))
                Flip();

            if (playerInField && target != null)
            {
                Vector3 lineOfSight = (target.position + new Vector3(0, 1)) - (this.transform.position + new Vector3(0, 1));
                RaycastHit2D playerInfo = Physics2D.Linecast(this.transform.position + new Vector3(0, 1),
                    this.transform.position + new Vector3(0, 1) + lineOfSight, finalMask);

                if (playerInfo.collider != null && playerInfo.collider.gameObject.CompareTag("Player"))
                {
                    playerInSight = true;
                    // alert.SetActive(true);
                    if (!trigger)
                    {
                        trigger = true;
                        mainAnim.SetTrigger("attack");
                    }
                }
                else
                {
                    playerInSight = false;
                }
            }    
        }
        else
        {
            RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, distanceDetect, whatIsGround);
            RaycastHit2D frontInfo;
            if (model.transform.eulerAngles.y > 0)    // right
                frontInfo = Physics2D.Raycast(groundDetection.position, Vector2.right, distanceDetect, whatIsGround);
            else    // left
                frontInfo = Physics2D.Raycast(groundDetection.position, Vector2.left, distanceDetect, whatIsGround);

            if (hp > 0 && !receivingKnockback)
            {
                if (movingLeft)
                {
                    body.velocity = new Vector2(-moveSpeed, body.velocity.y);
                    model.transform.eulerAngles = new Vector3(0, 0);
                }
                else if (movingRight)
                {
                    body.velocity = new Vector2(moveSpeed, body.velocity.y);
                    model.transform.eulerAngles = new Vector3(0, 180);
                }
            }
                
            if ((!groundInfo || frontInfo) && canFlip && body.velocity.y >= 0 && (movingLeft || movingRight))
                Flip();
        }
    }
    private void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.yellow;
        if (playerInField && target != null)
        {
            Vector3 lineOfSight = (target.position + new Vector3(0, 1)) - (this.transform.position + new Vector3(0, 1));
            Gizmos.DrawLine(this.transform.position + new Vector3(0, 1),
                this.transform.position + new Vector3(0, 1) + lineOfSight);
        }
    }

    public void MiniBossChainAbsorb()
    {
        if (hp > 0)
        {
            if (absorbObj != null && target != null)
            {
                var obj = Instantiate(absorbObj, target.position + new Vector3(0,1), absorbObj.transform.rotation);
                obj.moveMaster = this;
                if (obj.anim != null) 
                    if (hpImg.fillAmount <= 0.5f)
                        obj.anim.speed = 3;
                    else
                        obj.anim.speed = 2;

                obj.transform.parent = spawnedHolder.transform;
                obj.atkDmg = projectileDmg + calcExtraProjectileDmg;
            }
        }
    }
    public void MiniBossBurstAbsorb(int n=4)
    {
        List<int> pos = new List<int>();
        List<int> ints = new List<int>();
        for (int i=0 ; i<absorbSpawns.Length ; i++)
            ints.Add(i);

        for (int i=0 ; i<n ; i++)
        {
            int x = ints[ Random.Range(0, ints.Count) ];
            ints.Remove(x);
            pos.Add(x);
        }

        if (hp > 0)
        {
            if (absorbObj != null && target != null)
            {
                for (int i=0 ; i<pos.Count ; i++)
                {
                    var obj = Instantiate(absorbObj, absorbSpawns[ pos[i] ].position, absorbObj.transform.rotation);
                    obj.moveMaster = this;
                    obj.transform.parent = spawnedHolder.transform;
                    obj.atkDmg = projectileDmg + calcExtraProjectileDmg;
                }
            }
        }
    }
    public void ABSORB()
    {
        if (hp > 0 && !isMiniBoss)
        {
            if (absorbObj != null && target != null)
            {
                var obj = Instantiate(absorbObj, target.position + new Vector3(0,1), absorbObj.transform.rotation);
                obj.moveMaster = this;
                obj.transform.parent = spawnedHolder.transform;
                obj.atkDmg = projectileDmg + calcExtraProjectileDmg;
            }
        }
    }

    public void BACK_TO_WALKING()
    {
        if (!isMiniBoss)
        {
            trigger = false;
            mainAnim.SetTrigger("walk");
        }
    }
    public void NEXT_ACTION()
    {
        if (!isMiniBoss)
        {
            if (playerInSight)
            {
                mainAnim.SetTrigger("attack");
            }
            else if (Random.Range(0,2) == 0)
            {
                trigger = false;
                mainAnim.SetTrigger("walk");
                WALK();
            }
        }
        else if (canMove && !attacking)
        {
            if (Random.Range(0,2) == 0)
            {
                trigger = false;
                mainAnim.SetTrigger("walk");
                WALK();
            }
        }
    }

    public void WALK()
    {
        if (Random.Range(0,2) == 0)
        {
            if (hp > 0)
            {
                movingLeft = true;
                movingRight = false;
            }
        }
        else
        {
            if (hp > 0)
            {
                movingRight = true;
                movingLeft = false;
            }
        }
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

    public void STOP()
    {
        if (hp > 0)
            body.velocity = new Vector2(0, body.velocity.y);
        movingLeft = false;
        movingRight = false;
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
        return (this.transform.position.x - target.position.x);
    }

    IEnumerator ChooseAttack(float delay=1)
    {
        yield return new WaitForSeconds(delay);

        if (massAbsorbAtk)
        {
            //* Less likely to repeat the same attack
            if (Random.Range(0,2*sameAtkPattern) == 0)
                if (hpImg.fillAmount <= 0.5f)
                    co = StartCoroutine( MultiAbsorbSimultaneous(6) );
                else
                    co = StartCoroutine( MultiAbsorbSimultaneous(4) );
            
            //* More likely to perform a different attack
            else
                if (hpImg.fillAmount <= 0.5f)
                    co = StartCoroutine( MultiAbsorbOverTime(6) );
                else
                    co = StartCoroutine( MultiAbsorbOverTime(4) );
        }
        else
        {
            //* Less likely to repeat the same attack
            if (Random.Range(0,2*sameAtkPattern) == 0)
                co = StartCoroutine( MultiAbsorbOverTime(4) );
            
            //* More likely to perform a different attack
            else
                if (hpImg.fillAmount <= 0.5f)
                    co = StartCoroutine( MultiAbsorbSimultaneous(6) );
                else
                    co = StartCoroutine( MultiAbsorbSimultaneous(4) );
        }
    }
    IEnumerator MultiAbsorbOverTime(int n=4)
    {
        massAbsorbAtk = true;
        mainAnim.SetTrigger("attack");
        for (int i=0 ; i<n ; i++)
        {
            yield return new WaitForSeconds(0.5f);
            MiniBossChainAbsorb();
        }
        co = StartCoroutine( ChooseAttack(2) );
    }
    IEnumerator MultiAbsorbSimultaneous(int n=4)
    {
        massAbsorbAtk = false;
        mainAnim.SetTrigger("attack");
        if (n > absorbSpawns.Length)
            n = absorbSpawns.Length;
        MiniBossBurstAbsorb(n);

        yield return new WaitForSeconds(3);
        co = StartCoroutine( ChooseAttack(0.5f) );
    }

}

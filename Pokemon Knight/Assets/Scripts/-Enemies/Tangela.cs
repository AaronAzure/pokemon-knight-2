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


    public override void Setup()
    {
        // StartCoroutine( Attack() );
        if (GameObject.Find("PLAYER") != null)
            target = GameObject.Find("PLAYER").transform;

        finalMask = (whatIsPlayer | whatIsGround);
    }

    private void FixedUpdate() 
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
                alert.SetActive(true);
                if (!trigger)
                {
                    trigger = true;
                    mainAnim.SetTrigger("attack");
                }
            }
            else
            {
                playerInSight = false;
                alert.SetActive(false);
            }
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

    public void ABSORB()
    {
        if (hp > 0)
        {
            if (absorbObj != null && target != null)
            {
                var obj = Instantiate(absorbObj, target.position + new Vector3(0,1), absorbObj.transform.rotation);
                obj.moveMaster = this;
                obj.atkDmg = projectileDmg + calcExtraProjectileDmg;
            }
        }
    }

    public void BACK_TO_WALKING()
    {
        trigger = false;
        mainAnim.SetTrigger("walk");
    }
    public void NEXT_ACTION()
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
}

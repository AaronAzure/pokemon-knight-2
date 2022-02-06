using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gloom : Enemy
{
    [Space] [Header("Gloom")]  public float moveSpeed=2;
    public float distanceDetect=1f;
    public Transform groundDetection;
    [SerializeField] private LayerMask whatIsTree;
    public float forwardDetect=1f;
    public Transform face;
    private bool movingLeft;
    private bool movingRight;
    [SerializeField] private EnemyProjectile sludgeBomb;
    [SerializeField] private Transform sludgeBombPos;
    

    [Space] [SerializeField] private Transform target;

    [SerializeField] private float multiplier=-1.1f;
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
            
        if ((!groundInfo || frontInfo) && canFlip && body.velocity.y >= 0)
        {
            canFlip = false;
            WalkTheOtherWay();
            StartCoroutine( ResetFlipTimer() );
        }

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

    public void SLUDGE_BOMB()
    {
        if (hp > 0)
        {
            if (sludgeBomb != null && sludgeBombPos != null)
            {
                var obj = Instantiate(sludgeBomb, sludgeBombPos.position, sludgeBomb.transform.rotation);
                obj.body.gravityScale = 3;
                obj.atkDmg += totalExtraDmg;
                obj.direction = new Vector2(multiplier * trajectory, Random.Range(12,16));
            }
        }
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
        }

    }

    public void WALK()
    {
        if (Random.Range(0,2) == 0)
        {
            if (hp > 0)
                body.velocity = new Vector2(-moveSpeed, body.velocity.y);
            model.transform.eulerAngles = new Vector3(0, 0);
        }
        else
        {
            if (hp > 0)
                body.velocity = new Vector2(moveSpeed, body.velocity.y);
            model.transform.eulerAngles = new Vector3(0, 180);
        }
    }
    public void STOP()
    {
        if (hp > 0)
            body.velocity = new Vector2(0, body.velocity.y);
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

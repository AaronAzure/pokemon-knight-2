using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class Pidgey : Enemy
{
    [Space] [Header("Pidgey")]  
    public Animator anim;
    public float moveSpeed=2;
    public float chaseSpeed=4;
    public float maxSpeed=5f;
    [SerializeField] private LayerMask whatIsPlayer;
    private LayerMask finalMask;
    public Transform target;
    public bool chasing;
    public bool playerInRange;
    
    [Space] [SerializeField] private GameObject alert;
    private bool once;

    [Space] [Header("Miniboss attacks")]
    [SerializeField] private EnemyProjectile gust;
    [SerializeField] private GameObject glint;
    public Vector3 lineOfSight;
    [SerializeField] private AIDestinationSetter aiDest;
    [SerializeField] private AIPath aiPath;
    private Coroutine co;


    public override void Setup()
    {
        finalMask = (whatIsPlayer | whatIsGround);
        if (alert != null) alert.gameObject.SetActive(false);
            
        if (isMiniBoss)
        {
            target = playerControls.gameObject.transform;
            co = StartCoroutine( Gust(5) );
            chasing = true;
            anim.speed = chaseSpeed;
            if (aiDest != null)
                aiDest.target = this.target;
            if (aiPath != null)
                aiPath.canMove = false;
        }
        else
        {
            // StartCoroutine( Wait() );
        }
    }

    public override void CallChild()
    {
        if (aiPath != null)
            aiPath.canMove = true;
    }
    public override void CallChild2()
    {
        StopCoroutine(co);
        if (aiPath != null)
            aiPath.canMove = false;
        body.gravityScale = 3;
    }


    // Start is called before the first frame update
    void FixedUpdate() 
    {
        // Wandering around
        if (!inCutscene && !isMiniBoss)
        {
            if (!chasing) { body.velocity = Vector2.zero; }
            // Chasing PLayer
            else if (!receivingKnockback)
            {
                Vector3 dir = (target.position + new Vector3(0,1) - this.transform.position).normalized;
                //* If at edge, then turn around
                body.AddForce(dir * 5 * chaseSpeed * Time.fixedDeltaTime, ForceMode2D.Impulse);
                CapVelocity();
            }

            // if (!isMiniBoss)
            // {
                if (target != null && playerInRange)
                {
                        lineOfSight = (target.position + new Vector3(0, 1)) - (this.transform.position);
                        RaycastHit2D playerInfo = Physics2D.Linecast(this.transform.position,
                            this.transform.position + new Vector3(0, 1) + lineOfSight, finalMask);
                        if (playerInfo.collider != null && playerInfo.collider.gameObject.CompareTag("Player"))
                        {
                            chasing = true;
                            anim.speed = chaseSpeed;
                            if (alert != null) alert.gameObject.SetActive(true);
                        }
                        else
                        {
                            chasing = false;
                            if (alert != null) alert.gameObject.SetActive(false);
                        }
                }
                else if (alert != null) alert.gameObject.SetActive(false);
            // }
        }
        if (isMiniBoss)
            LookAtTarget();
    }

    private void LookAtTarget()
    {
        if (target.position.x > this.transform.position.x)  // player is to the right
            model.transform.eulerAngles = new Vector3(0, 180);  // face right
        // chasing left (negative velocity)
        else
            model.transform.eulerAngles = new Vector3(0, 0);  // face left
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
    private void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.yellow;
        if (target != null)
        {
            // Vector3 lineOfSight = (target.position + new Vector3(0, 1)) - (this.transform.position);
            Gizmos.DrawLine(this.transform.position,
                this.transform.position + new Vector3(0, 1) + lineOfSight);
        }
    }


    IEnumerator Gust(float delay=2.5f)
    {
        yield return new WaitForSeconds(delay);
        if (glint != null) {
            glint.SetActive(false); 
            glint.SetActive(true);
        }

        yield return new WaitForSeconds(0.5f);
        if (gust != null)
        {
            lineOfSight = (target.position + new Vector3(0, 1)) - (this.transform.position);
            var obj = Instantiate(gust, this.transform.position, gust.transform.rotation);
            obj.direction = lineOfSight.normalized;
        }

        yield return new WaitForSeconds(0.5f);
        
        co = StartCoroutine( Gust() );
    }
}
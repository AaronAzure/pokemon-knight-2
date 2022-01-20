using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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


    public override void Setup()
    {
        finalMask = (whatIsPlayer | whatIsGround);
        if (alert != null) alert.gameObject.SetActive(false);
        StartCoroutine( Wait() );
    }



    // Start is called before the first frame update
    void FixedUpdate() 
    {
        // Wandering around
        if (!chasing)
        {

        }
        // Chasing PLayer
        else if (!receivingKnockback)
        {
            Vector3 dir = (target.position + new Vector3(0,1) - this.transform.position).normalized;
            //* If at edge, then turn around
            body.AddForce(dir * 5 * chaseSpeed * Time.fixedDeltaTime, ForceMode2D.Impulse);
            CapVelocity();
        }

        if (target != null && playerInRange)
        {
            Vector3 lineOfSight = (target.position + new Vector3(0, 1)) - (this.transform.position);
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
            Vector3 lineOfSight = (target.position + new Vector3(0, 1)) - (this.transform.position);
            Gizmos.DrawLine(this.transform.position,
                this.transform.position + new Vector3(0, 1) + lineOfSight);
        }
    }

    IEnumerator Wait()
    {
        yield return new WaitForSeconds(1);
        if (!chasing)
        {
            anim.speed = 1;
                body.velocity = Vector2.zero;
            // if (!once)
            // {
            //     once  = true;
            //     yield return new WaitForEndOfFrame();
            // }
            // if (body.velocity.x > 0) 
            // {
            //     body.AddForce(Vector2.left * moveSpeed, ForceMode2D.Impulse);
            //     model.transform.eulerAngles = new Vector3(0, 0);  // face left
            // }
            // else 
            // {
            //     body.AddForce(Vector2.right * moveSpeed * 2, ForceMode2D.Impulse);
            //     // body.AddForce(Vector2.right * moveSpeed, ForceMode2D.Impulse);
            //     model.transform.eulerAngles = new Vector3(0, 180);  // face right
            // }
        }
        // else
        //     once  = false;

        StartCoroutine( Wait() );
    }

}

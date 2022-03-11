using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class BellsproutBoss : Enemy
{
    [Space] [Header("Bellsprout")]  
    public Animator anim;
    public float moveSpeed=2;
    public float chaseSpeed=5;
    public float maxSpeed=7.5f;
    public float distanceDetect=1f;
    public Transform groundDetection;


    [Space] [Header("Attacks")]
    public Transform target;
    public bool chasing;
    public bool playerInRange;
    private Coroutine co;
    private LayerMask finalMask;    // detect Player, Ground, ignores Enemy, Bounds

    
    [Space] [Header("Pathfinding")]
    public float activationDist=20f;
    public float pathUpdateSec=0.5f;
    public float nextWayPointDist=1f;
    public float nextWayPointDistY=3.5f;
    public float maxWayPointDistDiff=2f;
    public float jumpHeightReq=0.8f;
    public float distToJumpReq=0.8f;
    public float jumpForce=12f;
    public bool followEnabled = true;
    public bool directionLookEnabled = true;

    public Path path;
    public int currentWaypoint = 2;
    public bool isGrounded;
    public Seeker seeker;
    [Space] public Vector2 moveDir;

    public List<Vector3> paths;

    public bool jumpLeft;
    public bool jumped;
    
    
    [Space] [Header("Mini boss")]
    [SerializeField] private GameObject spawnedHolder;
    [Space] [SerializeField] private RazorLeaf razorLeaf;
    [Space] [SerializeField] private EnemyProjectile stunSpore;
    [SerializeField] private Transform razorLeafSpawn;
    public bool lookAtTarget;
    
    

    public override void Setup()
    {
        if (seeker == null)
            seeker = GetComponent<Seeker>();
        finalMask = (whatIsPlayer | whatIsGround);
        if (alert != null) alert.gameObject.SetActive(false);

        target = playerControls.transform;

        if (spawnedHolder != null) 
            spawnedHolder.transform.parent = null;

        UpdatePath();
        InvokeRepeating("UpdatePath", 0f, pathUpdateSec);
    }

    public override void CallChildOnBossFightStart()
    {
        chasing = true;
        anim.SetTrigger("walking");
        anim.speed = chaseSpeed;
        co = StartCoroutine( NextActionCo() );
    }
    public override void CallChildOnBossDeath()
    {
        if (co != null)
            StopCoroutine(co);
        anim.SetTrigger("idling");
        body.gravityScale = 3;
        Destroy(spawnedHolder);
    }

    private void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireCube(feetOffset + body.position, feetSize);

        Gizmos.color = Color.red;
        if (path != null && currentWaypoint < path.vectorPath.Count)
            Gizmos.DrawSphere(path.vectorPath[currentWaypoint], 0.25f);
    }

    private void FixedUpdate()
    {
        if (lookAtTarget && !chasing)
            LookAtTarget();
        if (hp > 0 && chasing && TargetInDistance() && followEnabled)
            PathFollow();
    }

    private void UpdatePath()
    {
        if (followEnabled && seeker.IsDone())
            seeker.StartPath(body.position, target.position, OnPathComplete);
    }

    private void PathFollow()
    {
        // Reached end of path
        if (path == null || currentWaypoint >= path.vectorPath.Count)
        {
            UpdatePath();
            return;
        }

        // See if colliding with anything
        isGrounded = IsGrounded();
        
        //* Direction Calculation
        moveDir = ((Vector2)path.vectorPath[currentWaypoint] - body.position);  // + = right , - = left
        paths = path.vectorPath;
        
        // UpdatePath();

        // Next Waypoint
        float xDistance = Mathf.Abs(body.position.x - path.vectorPath[currentWaypoint].x);
        // float yDistance = Mathf.Abs(body.position.y - path.vectorPath[currentWaypoint].y);
        if (xDistance < nextWayPointDist)
        {
            UpdatePath();
            for (int i=Mathf.Max(1,currentWaypoint) ; i<path.vectorPath.Count ; i++)
            {
                if (Mathf.Abs( path.vectorPath[i].x - path.vectorPath[currentWaypoint].x) >= maxWayPointDistDiff)
                {
                    currentWaypoint = i; 
                    break;
                }
            }
        }


        //* Jump
        if (isGrounded && body.velocity.y == 0)
        {
            if (moveDir.y > jumpHeightReq && (
                (moveDir.x >= 0 && moveDir.x < distToJumpReq) || moveDir.x <= 0 && moveDir.x > -distToJumpReq)
            )
            {
                body.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                jumped = true;
                if (moveDir.x > 0)  // right
                    jumpLeft = false;
                else  // left
                    jumpLeft = true;
            }
        }

        //* Movement
        if (isGrounded && body.velocity.y <= 0)
            if (moveDir.x > 0)  // right
                body.velocity = new Vector2(chaseSpeed, body.velocity.y);
            else  // left
                body.velocity = new Vector2(-chaseSpeed, body.velocity.y);
        else if (!isGrounded && jumped)
            if (jumpLeft) // left
                body.velocity = new Vector2(-chaseSpeed, body.velocity.y);
            else     // right
                body.velocity = new Vector2( chaseSpeed, body.velocity.y);

        if (jumped && isGrounded && body.velocity.y <= 0)
            jumped = false;
        
        // Direction Graphics Handling
        LookWhereYoureGoing();
    }

    private void LookWhereYoureGoing()
    {
        if (body.velocity.x > 0.05f)    // right
            model.transform.eulerAngles = new Vector3(0,180);
        else if (body.velocity.x < -0.05f)
            model.transform.eulerAngles = new Vector3(0,0);
    }

    private bool TargetInDistance()
    {
        return Vector2.Distance(transform.position, target.transform.position) < activationDist;
    }

    private void OnPathComplete(Path p)
    {
        if (!p.error)
        {
            path = p;
            currentWaypoint = 2;
        }
    }

    IEnumerator NextActionCo()
    {
        if (hpImg.fillAmount <= 0.5f)
        {
            yield return new WaitForSeconds(3);
            anim.speed = 1.5f;
        }
        else
        {
            yield return new WaitForSeconds(5);
            anim.speed = 1;
        }
        
        chasing = false;
        body.velocity = new Vector2(0, body.velocity.y);
        if (Random.Range(0,3) == 0)
        {
            anim.SetTrigger("stunSpore");
        }
        else
        {
            anim.SetTrigger("razorLeaf");
            lookAtTarget = true;
        }
    }

    public void STUN_SPORE()
    {
        if (stunSpore != null)
        {
            var obj = Instantiate(stunSpore, this.transform.position, 
                stunSpore.transform.rotation, spawnedHolder.transform);
            obj.atkDmg = (projectileDmg + calcExtraProjectileDmg);
            obj.transform.localScale *= 1.5f;
        }
    }
    public void RAZOR_LEAF()
    {
        Vector2 lineOfSight = (target.position + new Vector3(0, 1)) - (this.transform.position + new Vector3(0, 1));
        if (razorLeafSpawn != null && hp > 0)
        {
            var obj = Instantiate(razorLeaf, razorLeafSpawn.position, 
                razorLeaf.transform.rotation, spawnedHolder.transform);
            obj.atkDmg = projectileDmg + calcExtraProjectileDmg;
            obj.direction = lineOfSight.normalized;
        }
    }

    public void RESUME_CHASE()
    {
        anim.SetTrigger("walking");
        anim.speed = chaseSpeed;
        lookAtTarget = true;
        chasing = true;
        co = StartCoroutine( NextActionCo() );
    }

    private void Flip()
    {
        if (!canFlip)
            return;
        if (movingLeft || movingRight)
        {
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
        }
        StartCoroutine( ResetFlipTimer() );
    }
}

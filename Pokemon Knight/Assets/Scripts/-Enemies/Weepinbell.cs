using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weepinbell : Enemy
{
    [Space] [Header("Weepinbell")]  
    public Animator anim;
    public float moveSpeed=2;
    public float jumpForce=10;
    // public float chaseSpeed=4;
    // public float maxSpeed=5f;
    private LayerMask finalMask;
    public Transform target;
    // public bool canAtk;
    [SerializeField] private RazorLeaf razorLeaf;
    [SerializeField] private Transform razorLeafSpawn;
    public bool attacked;
    public Vector2 fieldOfVision;
    public Vector3 offset;
    public Vector3 lineOfSight;
    

    public override void Setup()
    {
        finalMask = (whatIsPlayer | whatIsGround);
        if (alert != null) alert.gameObject.SetActive(false);
        
        if (GameObject.Find("PLAYER") != null && target == null)
            target = GameObject.Find("PLAYER").gameObject.transform;

        if (alwaysAttackPlayer)
        {
            if (alert != null)
                alert.gameObject.SetActive(true);
            LookAtTarget();
            StartCoroutine( RestBeforeNextAttack() );
        }
    }

    void FixedUpdate() 
    {
        if (!alwaysAttackPlayer)
        {
            bool detection = Physics2D.OverlapBox(this.transform.position + offset, fieldOfVision, 0, whatIsPlayer);
            if (target != null && detection)
            {

                lineOfSight = (target.position + new Vector3(0, 1)) - (this.transform.position + new Vector3(0, 1));
                RaycastHit2D playerInfo = Physics2D.Linecast(this.transform.position,
                    this.transform.position + new Vector3(0, 1) + lineOfSight, finalMask);

                if (playerInfo.collider != null && playerInfo.collider.gameObject.CompareTag("Player"))
                {
                    if (alert != null) alert.gameObject.SetActive(true);
                    LookAtTarget();
                    if (!attacked)
                    {
                        attacked = true;
                        anim.SetTrigger("attack");
                        Jump();
                    }
                }
                else
                {
                    if (alert != null) alert.gameObject.SetActive(false);
                }

            }
        }
    }

    private void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.yellow;
        if (target != null)
        {
            Gizmos.DrawLine(this.transform.position + new Vector3(0, 1),
                this.transform.position + new Vector3(0, 1) + lineOfSight);
        }
        Gizmos.DrawWireCube(this.transform.position + offset, fieldOfVision);
    }

    public void Jump(bool guaranteed=false)
    {
        if (guaranteed || Random.Range(0, 3) == 0)
            body.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
    }

    public void RAZOR_LEAF()
    {
        if (alwaysAttackPlayer)
            lineOfSight = (target.position + new Vector3(0, 1)) - (this.transform.position + new Vector3(0, 1));
        if (razorLeafSpawn != null && hp > 0)
        {
            var obj = Instantiate(razorLeaf, razorLeafSpawn.position, razorLeaf.transform.rotation);
            obj.direction = lineOfSight.normalized;
        }
    }

    public void NEXT_ACTION()
    {
        if (alwaysAttackPlayer)
            StartCoroutine( RestBeforeNextAttack() );
    }


    IEnumerator RestBeforeNextAttack()
    {
        yield return new WaitForSeconds(2);
        anim.SetTrigger("attack");
        Jump(true);
    }

    private void LookAtTarget()
    {
        if (target != null)
        {
            if (target.position.x > this.transform.position.x)  // player is to the right
                model.transform.eulerAngles = new Vector3(0, 180);  // face right
            else
                model.transform.eulerAngles = new Vector3(0, 0);  // face left
        }
    }

    public void CanAttackAgain()
    {
        StartCoroutine( Cooldown() );
    }

    IEnumerator Cooldown()
    {
        yield return new WaitForSeconds(0.5f);
        attacked = false;
    }

}

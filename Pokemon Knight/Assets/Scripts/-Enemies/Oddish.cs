using UnityEngine;
using System.Collections;


public class Oddish : Enemy
{
    [Space] [Header("Oddish")]  public float moveSpeed=2;
    public float distanceDetect=1f;
    public Transform groundDetection;
    [SerializeField] private LayerMask whatIsTree;
    public float forwardDetect=1f;
    public Transform face;
    private bool canFlip = true;
    private bool movingLeft;
    private bool movingRight;
    // [Space] private EnemyAttack stunSporeDmg;


    [Header("Attacks")]
    [SerializeField] private Animator anim;
    [SerializeField] private EnemyAttack stunSpore;
    [SerializeField] private int stunSporeDmg=2;
    [SerializeField] private Transform stunSporePos;
    public bool canSeePlayer;
    public bool canAtk = true;
    private Coroutine co;


    public override void Setup()
    {
        co = StartCoroutine( DoSomething() );
    }

    void FixedUpdate() 
    {
        if (!receivingKnockback)
        {
            if (movingLeft)
                body.velocity = new Vector2(-moveSpeed, body.velocity.y);
            else if (movingRight)
                body.velocity = new Vector2(moveSpeed, body.velocity.y);

        }

        if (canSeePlayer && canAtk)
        {
            canAtk = false;
            StartCoroutine(PoisonPowder());
        }
        RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, distanceDetect, whatIsGround);
        RaycastHit2D frontInfo = Physics2D.Raycast(groundDetection.position, Vector2.left, distanceDetect, whatIsGround);

        if (movingRight)
            frontInfo = Physics2D.Raycast(groundDetection.position, Vector2.right, distanceDetect, whatIsGround);
        
        //* If at edge, then turn around
        if (body.velocity.y >= 0 && (!groundInfo || frontInfo))
            Flip();
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

    IEnumerator ResetFlipTimer()
    {
        canFlip = false;
        yield return new WaitForSeconds(0.5f);
        canFlip = true;
    }

    public IEnumerator PoisonPowder()
    {
        yield return new WaitForSeconds(0.5f);
        StopCoroutine(co);
        AdjustAnim("attacking");
        movingLeft = false;
        movingRight = false;
        canAtk = false;
        var obj = Instantiate(stunSpore, stunSporePos.position, stunSporePos.transform.rotation);
        obj.atkDmg = stunSporeDmg;
        Destroy(obj, 4.5f);

        yield return new WaitForSeconds(1f);
        co = StartCoroutine( DoSomething() );
    }

    private void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(groundDetection.position, groundDetection.position + new Vector3(0,-distanceDetect));
        Gizmos.DrawLine(face.position, face.position + new Vector3(-forwardDetect,0));
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ally : MonoBehaviour
{
    [HideInInspector] public GameObject trainer;    // player who summoned
    [Space] [SerializeField] protected GameObject model;
    [Space] public AllyAttack hitbox;  // Separate gameobject with collider
    public int atkDmg;
    public int atkForce;
    public float outTime = 0.5f;    // Time pokemon appears in the overworld
    public float resummonTime = 0.5f;    // Delay before calling pokemon again

    [Space] public Rigidbody2D body;
    [Space] [Tooltip("PokeballTrail prefab - return back to player")] public FollowTowards trailObj;

    [Header("Flash")]
    [SerializeField] protected SpriteRenderer[] renderers;
    [SerializeField] protected Material flashMat;

    [Header("Physics")] 
    [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private float feetRadius=0.1f;
    private bool once;
    


    // Start is called before the first frame update
    void Start()
    {
        if (hitbox != null)
        {
            hitbox.atkDmg = this.atkDmg;
            hitbox.atkForce = this.atkForce;
        }

        StartCoroutine( BackToBall() );
    }

    void FixedUpdate() 
    {
        // bool grounded = Physics2D.OverlapCircle(this.transform.position, feetRadius, whatIsGround);
        if (!once)
        {
            RaycastHit2D groundInfo = Physics2D.Raycast(this.transform.position, Vector2.down, feetRadius, whatIsGround);
            if (groundInfo)
                body.velocity = Vector2.zero;
        }
    }

    protected IEnumerator BackToBall()
    {
        yield return new WaitForSeconds(outTime);
        int times = 20;
        float x = model.transform.localScale.x / times;
        foreach (SpriteRenderer renderer in renderers)
        {
            if (flashMat != null)
                renderer.material = flashMat;
        }
        for (int i=0 ; i<times ; i++)
        {
            model.transform.localScale -= new Vector3(x,x);
            yield return new WaitForEndOfFrame();
        }
        var returnObj = Instantiate(trailObj, this.transform.position, Quaternion.identity);
        if (trainer != null)
            returnObj.target = trainer.transform;

        Destroy(this.gameObject);
    }
    // protected IEnumerator 
}

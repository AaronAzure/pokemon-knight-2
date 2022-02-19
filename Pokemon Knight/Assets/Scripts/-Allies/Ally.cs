using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ally : MonoBehaviour
{
    [Space] [SerializeField] protected GameObject model;
    [Space] [Tooltip("Collider within obj, not spawned")] public AllyAttack hitbox;  // Separate gameobject with collider
    public int atkDmg;
    public int atkForce;
    [Space] public int extraDmg;
    public int perLevel=1;
    [Space] public float outTime = 0.5f;    // Time pokemon appears in the overworld
    public float resummonTime = 0.5f;    // Delay before calling pokemon again
    // [SerializeField] private int delayTimes=50;
    [Space] public Rigidbody2D body;
    [Space] [Tooltip("PokeballTrail prefab - return back to player")] public FollowTowards trailObj;
    

    // [Header("Called from player")]
    [HideInInspector] public PlayerControls trainer;    // player who summoned
    [HideInInspector] public string button;

    
    [Space] [Header("Move Description")]
    [Tooltip("e.g. Vine whip")] public string moveName;
    public int multiHit=1;
    [TextArea(15,20)] public string moveDesc;


    [Space] [Header("Flash")]
    [SerializeField] protected SpriteRenderer[] renderers;
    [SerializeField] protected Material flashMat;


    [Space] [Header("Physics")] 
    public bool aquatic=false;
    [Space] [SerializeField] private LayerMask whatIsGround;
    [SerializeField] private float feetRadius=0.01f;
    private bool once;
    private bool returning;
    private bool shrinking;
    


    // Start is called before the first frame update
    protected virtual void Start()
    {
        // Strength grows per level
        if (trainer != null)
            atkDmg += ( extraDmg * Mathf.CeilToInt(((trainer.lv - 1) / perLevel)) );
        if (trainer != null && trainer.furyBracelet && trainer.hpImg.fillAmount <= 0.25f)
            atkDmg *= 2;
        else if (trainer != null && trainer.furyBracelet && trainer.hpImg.fillAmount <= 0.5f)
            atkDmg = Mathf.RoundToInt(atkDmg * 1.25f);
        if (trainer != null && hitbox != null && trainer.extraRange)
            hitbox.gameObject.transform.localScale *= 1.5f;

        if (hitbox != null)
        {
            hitbox.atkDmg = this.atkDmg;
            hitbox.atkForce = this.atkForce;
        }
        if (trainer != null && trainer.speedScarf)
            resummonTime *= 0.7f;

        StartCoroutine( BackToBall() );
        Setup();
    }

    protected virtual void Setup() { }

    public virtual string ExtraDesc(int playerLv) { return ""; }


    void LateUpdate() 
    {
        if (!once)
        {
            RaycastHit2D groundInfo = Physics2D.Raycast(this.transform.position, Vector2.down, feetRadius, whatIsGround);
            if (groundInfo && body != null)
                body.velocity = Vector2.zero;
        }
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (!aquatic && other.CompareTag("Underwater"))    
        {
            StartCoroutine( BackToBall() );
        }
    }

    protected virtual void ExtraTrailEffects(FollowTowards ft) {}

    protected IEnumerator BackToBall()
    {
        // CALL ONCE
        if (returning)
            yield break;
        returning = true;

        yield return new WaitForSeconds(outTime);
        int times = 10;
        float x = model.transform.localScale.x / times;
        foreach (SpriteRenderer renderer in renderers)
        {
            if (flashMat != null)
                renderer.material = flashMat;
        }
        yield return new WaitForEndOfFrame();
        for (int i=0 ; i<times ; i++)
        {
            model.transform.localScale -= new Vector3(x,x);
            // yield return new WaitForSeconds(0.01f);
            yield return null;
            // yield return new WaitForEndOfFrame();
        }
        if (trailObj != null)
        {
            var returnObj = Instantiate(trailObj, this.transform.position, Quaternion.identity, null);
            returnObj.button = this.button;
            returnObj.cooldownTime = this.resummonTime;
            ExtraTrailEffects(returnObj);

            if (trainer != null)
            {
                returnObj.player = this.trainer;
                returnObj.target = trainer.transform;
            }
            else
            {
                Debug.LogError(" PlayerControls not assigned to Ally.trainer");
            }
        }

        yield return new WaitForEndOfFrame();
        Destroy(this.gameObject);
    }
}

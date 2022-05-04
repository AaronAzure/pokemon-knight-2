using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class Ally : MonoBehaviour
{
    [Space] [SerializeField] protected GameObject model;
    public Sprite currentForm;
    public string pokemonName;
    
    [Space] public int extraLevel;
    [Space] [SerializeField] protected Animator anim;
    [Space] [Tooltip("Collider within obj, not spawned")] public AllyAttack hitbox;  // Separate gameobject with collider
    public int atkDmg;
    public int atkForce;
    public int spBonus;
    [Space] public int extraDmg;
    public int perLevel=1;
    [Space] public float outTime = 0.5f;    // Time pokemon appears in the overworld
    public float resummonTime = 0.5f;    // Delay before calling pokemon again

    
    [Space] [Header("Evolution")]
    public Ally evolvedForm1;
    public Sprite evolvedSprite1;
    public Ally evolvedForm2;
    public Sprite evolvedSprite2;

    [Space] public Rigidbody2D body;
    [Space] public bool useUlt;
    public float ultOutTime=1;
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
    private bool hitWater;
    private Coroutine co;
    [Space] [SerializeField] protected LayerMask whatIsGround;
    public Vector2 feetBox;
    public Vector2 feetOffset;
    [Space] public float feetRadius=0.01f;
    protected bool once;
    private bool returning=false;
    private bool shrinking;

    [Space][Header("UI")]
    public List<Image> imgs;
    


    // Start is called before the first frame update
    protected virtual void Start()
    {
        // Strength grows per level
        if (trainer != null)
            atkDmg += ( extraDmg * Mathf.CeilToInt(((trainer.lv - 1) + ExtraEnhancedDmg() / perLevel)) );
        if (trainer != null && trainer.crisisCharm && trainer.hpImg.fillAmount <= 0.25f)
            atkDmg *= 2;
        else if (trainer != null && trainer.crisisCharm && trainer.hpImg.fillAmount <= 0.5f)
            atkDmg = Mathf.RoundToInt(atkDmg * 1.25f);
        if (trainer != null && hitbox != null && trainer.extraRange)
            hitbox.gameObject.transform.localScale *= 1.5f;

        if (hitbox != null)
        {
            hitbox.atkDmg = this.atkDmg;
            hitbox.atkForce = this.atkForce;
            hitbox.spBonus = this.spBonus;
        }
        if (useUlt)
            resummonTime = 1;
        if (trainer != null && trainer.quickCharm)
            resummonTime *= trainer.coolDownSpeed;

        WhatEvolution();

        Setup();
        co = StartCoroutine( BackToBallAfterAction() );
    }


    public void SetExtraLevel(int lv=0)
    {
        this.extraLevel = lv;
        if      (IsAtThirdEvolution() && evolvedSprite2 != null)
            currentForm = evolvedSprite2;
        else if (IsAtSecondEvolution() && evolvedSprite1 != null)
            currentForm = evolvedSprite1;
    }
    
    
    public bool IsAtSecondEvolution() { return (extraLevel >= 3); }
    public bool IsAtThirdEvolution() { return (extraLevel >= 6); }

    private void WhatEvolution()
    {
        //* POKEMON EVOLUTION
        if (IsAtThirdEvolution())
        {
            OnThirdEvolution();
            if (evolvedForm2 != null)
            {
                var obj = Instantiate(evolvedForm2, this.transform.position, Quaternion.identity);
                obj.extraLevel = this.extraLevel;
                obj.atkDmg = this.atkDmg;
                obj.atkForce = this.atkForce;
                obj.body.velocity = this.body.velocity;
                obj.trainer = this.trainer;
                obj.button = this.button;

                if (this.transform.eulerAngles.y > 0)
                        obj.transform.eulerAngles = new Vector3(0,-180);

                Destroy(this.gameObject);
            }
        }
        else if (IsAtSecondEvolution())
        {
            OnSecondEvolution();
            if (evolvedForm1 != null)
            {
                var obj = Instantiate(evolvedForm1, this.transform.position, Quaternion.identity);
                obj.extraLevel = this.extraLevel;
                obj.atkDmg = this.atkDmg;
                obj.atkForce = this.atkForce;
                obj.body.velocity = this.body.velocity;
                obj.trainer = this.trainer;
                obj.button = this.button;

                if (this.transform.eulerAngles.y > 0)
                        obj.transform.eulerAngles = new Vector3(0,-180);

                Destroy(this.gameObject);
            }
        }
    }
    protected virtual void OnSecondEvolution() { }
    protected virtual void OnThirdEvolution() { }


    public int ExtraEnhancedDmg()
    {
        int extraEnhancedDmg = 0;
        for (int i=0 ; i<extraLevel ; i++)
        {
            if (i >= 5)
                extraEnhancedDmg += 7;
            else if (i >= 2)
                extraEnhancedDmg += 5;
            else 
                extraEnhancedDmg += 3;
        }

        return extraEnhancedDmg;
    }
    public int EnhanceDmgBonus()
    {
        if (extraLevel >= 5)
            return 7;
        else if (extraLevel >= 2)
            return 5;
        return 3;
    }

    public void ENHANCE_POKEMON(int newLevel)
    {
        extraLevel = newLevel;
        if (IsAtThirdEvolution() && evolvedSprite2 != null)
            foreach (Image img in imgs)
                img.sprite = evolvedSprite2;
        else if (IsAtSecondEvolution() && evolvedSprite1 != null)
            foreach (Image img in imgs)
                img.sprite = evolvedSprite1;
    }


    protected virtual void Setup() { }

    public virtual string ExtraDesc(int playerLv) { return ""; }

    public virtual void CallChildOnLanded() { once = true; }
    public virtual void CallChildIsGrounded() 
    {
        RaycastHit2D groundInfo = Physics2D.Raycast(this.transform.position, Vector2.down, feetRadius, whatIsGround);
        if (groundInfo && body != null && body.velocity.y <= 0)
        {
            body.velocity = Vector2.zero;
            CallChildOnLanded();
        }
    }

    void LateUpdate() 
    {
        if (!once)
            CallChildIsGrounded();
    }

    private void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(this.transform.position, this.transform.position + (Vector3.down * feetRadius));
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(this.transform.position + (Vector3)feetOffset, feetBox);
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (!aquatic && !hitWater && other.CompareTag("Underwater"))
        {
            hitWater = true;
            StopCoroutine(co);
            StartCoroutine( ImmediateBackToBall() );
        }
    }

    protected virtual void ExtraTrailEffects(FollowTowards ft) {}

    public void IMMEDIATE_RETURN_TO_BALL()
    {
        StartCoroutine( ImmediateBackToBall() );
    }

    private IEnumerator ImmediateBackToBall()
    {
        // CALL ONCE

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
            yield return null;
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
    protected IEnumerator BackToBallAfterAction()
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

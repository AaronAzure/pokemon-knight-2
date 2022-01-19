using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public abstract class Enemy : MonoBehaviour
{
    public int lv=2;
    public int defaultLv=5; // Bonus
    [Space] public int maxHp=100;
    public int hp;
    protected PlayerControls playerControls;
    

    [Space]
    [Header("Level Bonus")]
    public int extraHp=2;   // Bonus
    public int extraDmg=2;  // Bonus
    [Space] public int expPossess=5;
    public int extraExp=2;  // Additional bonus
    public int lvBreak=50;  // Additional bonus
    
    //// [SerializeField] private GameObject emptyHolder;
    //// [SerializeField] private TextMeshPro dmgText;

    public Rigidbody2D body;
    public GameObject model;    // sprites holder
    protected bool receivingKnockback;
    public bool cannotRecieveKb;
    [Tooltip("1 = 100% kb, 0 = 0%")] [SerializeField] [Range(0,1f)] protected float kbDefense=1;
    [SerializeField] protected LayerMask whatIsGround;

    //* UI
    public GameObject statusBar;
    public Image hpEffectImg;
    public Image hpImg;
    public TextMeshProUGUI lvText;
    [SerializeField] private float effectSpeed = 0.005f;

    [Space]
    [Header("Boss")]
    [SerializeField] protected bool isBoss;
    [SerializeField] protected GameObject possessedAura;
    [SerializeField] protected GameObject battleRoarObj;
    [SerializeField] protected GameObject rageChargeObj;
    [SerializeField] protected GameObject rageAuraObj;
    protected bool inRage;
    public bool inCutscene; // Can't move
    public bool inRageCutscene; // Can't move
    protected bool startingBossFight; // Can't move
    protected bool isDefeated;
    [Space] [SerializeField] private string powerupName;
    [Space] [SerializeField] private GameObject pokeball;
    [Space] [SerializeField] private GameObject canCatchEffect;


    [Space]
    [Header("Damage Related")]
    public int contactDmg=5;
    public float contactKb=10;
    
    [Space] [SerializeField] private SpriteRenderer[] renderers;
    [SerializeField] private Material defeatedMat;
    [SerializeField] private Material flashMat;
    [SerializeField] private Material origMat;
    [HideInInspector] public BossRoom bossRoom;
    private bool canCatch;
    public bool isSmart;    // Turns if attacked from behind;


    protected void Awake()
    {
        // Bonus Hp per level greater
        if (lv > defaultLv)
            maxHp += (lv - defaultLv) * extraHp;
        if (lv > lvBreak)
            maxHp += (lv - lvBreak) * extraHp;

        hp = maxHp;   
        if (lvText != null)
            lvText.text = "Lv. " + lv; 
    }

    public virtual void Start() 
    {
        if (statusBar != null)
            statusBar.SetActive(false);
        playerControls = GameObject.Find("PLAYER").GetComponent<PlayerControls>();

        if (!isBoss)
            this.gameObject.transform.localScale *= Random.Range(0.95f, 1.05f);

        Setup();
    }

    public virtual void Setup() {}

    protected void LateUpdate() 
    {
        if (hpImg != null && hpEffectImg != null)
        {
            hpImg.fillAmount = (float) hp / (float) maxHp;
            
            //* Hp lost Effect
            if (hpEffectImg.fillAmount > hpImg.fillAmount)
                hpEffectImg.fillAmount -= effectSpeed;
            else 
                hpEffectImg.fillAmount = hpImg.fillAmount;
            
            //* Hp color Effect
            if (hpImg.fillAmount <= 0.25f)
                hpImg.color = new Color(0.8f, 0.01f, 0f);
            else if (hpImg.fillAmount <= 0.5f)
                hpImg.color = new Color(1f, 0.65f, 0f);
            else
                hpImg.color = new Color(0f, 0.85f, 0f);
        }
    }

    public void TakeDamage(int dmg=0, Transform opponent=null, float force=0)
    {
        if (!inCutscene)
        {
            if (hp > 0)
            {
                hp -= dmg;
                statusBar.SetActive(true);
            }

            //// var holder = Instantiate(emptyHolder, new Vector3(transform.position.x, transform.position.y + 2), Quaternion.identity);
            //// Destroy(holder.gameObject, 0.5f);
            //// var obj = Instantiate(dmgText, new Vector3(transform.position.x, transform.position.y + 2), Quaternion.identity, holder.transform);
            //// obj.text = dmg.ToString();
            // if (isSmart)
            // {
            //     Vector3 opponent
            // }

            if (dmg > 0 && dmg < hp)
                StartCoroutine( Flash() );

            if (force > 0 && opponent != null && !cannotRecieveKb)
                StartCoroutine( ApplyKnockback(opponent, force) );

            // Dramatic Boss Finisher
            if (isBoss && hp <= 0 && !isDefeated)
            {
                if (defeatedMat != null)
                {
                    foreach (SpriteRenderer renderer in renderers)
                    {
                        renderer.material = defeatedMat;
                    }
                }
                isDefeated = true;

                StartCoroutine(DramaticSlowmo());
                if (possessedAura != null)
                    possessedAura.SetActive(false);
                inCutscene = true;
                statusBar.SetActive(false);
                body.velocity = Vector2.zero;
                body.gravityScale = 3;
            }
            // Player Gains exp
            if (hp <= 0)
            {
                if (playerControls != null)
                    playerControls.GainExp(expPossess + (extraDmg * Mathf.Max(1, lv - defaultLv)), lv);

                if (!isBoss)
                    Destroy(this.gameObject);
                else if (playerControls != null)
                    playerControls.BossBattleOver();
            }
            // Boss half health
            else if (isBoss && !inRage && (float)hp/(float)maxHp < 0.5f)
            {
                inRage = true;  
                StartCoroutine( ActivateRageMode() );
            }
        }
    }
    IEnumerator DramaticSlowmo()
    {
        Time.timeScale = 0.5f;
        yield return new WaitForSeconds(0.5f);
        Time.timeScale = 1;
        yield return new WaitForSeconds(0.5f);
        rageAuraObj.SetActive(false);
        if (canCatchEffect != null) 
            canCatchEffect.SetActive(true);
        canCatch = true;
    }
    public IEnumerator ApplyKnockback(Transform opponent, float force)
    {
        receivingKnockback = true;
        Vector2 direction = (opponent.position - this.transform.position).normalized;
        direction *= new Vector2(1,0);
        body.AddForce(-direction * force * kbDefense, ForceMode2D.Impulse);
        
        if (!isBoss)
            yield return new WaitForSeconds(0.1f);
        else
            yield return new WaitForSeconds(0.02f);
        body.velocity = Vector2.zero;
        receivingKnockback = false;
    }
    IEnumerator Flash()
    {
        foreach (SpriteRenderer renderer in renderers)
        {
            if (flashMat != null && origMat != null)
                renderer.material = flashMat;
        }
        
        yield return new WaitForSeconds(0.1f);
        foreach (SpriteRenderer renderer in renderers)
        {
            if (flashMat != null && origMat != null)
                renderer.material = origMat;
        }

    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        if (other.gameObject.CompareTag("Player") && hp > 0 && !inCutscene)
        {
            PlayerControls player = other.gameObject.GetComponent<PlayerControls>();
            player.TakeDamage(contactDmg + (extraDmg * Mathf.Max(1, lv - defaultLv)), this.transform, contactKb);
            body.velocity = Vector2.zero;
        }    
        else if (other.gameObject.CompareTag("Player") && canCatch)
        {
            canCatch = false;
            StartCoroutine( BackToBall() );
            Pokeball obj = Instantiate( pokeball, this.transform.position, Quaternion.identity).GetComponent<Pokeball>();
            obj.powerup = this.powerupName;
        }    
    }


    // todo  (BOSS)  ------------------------------------------------------------

    public void StartBossBattle()
    {
        StartCoroutine( BossIntro() );
    }
    protected IEnumerator BossIntro()
    {
        body.velocity = Vector2.zero;
        inCutscene = true;

        yield return new WaitForSeconds(1);
        battleRoarObj.SetActive(true);
        
        yield return new WaitForSeconds(3);
        startingBossFight = true;
        battleRoarObj.SetActive(false);
        inCutscene = false;
    }
    IEnumerator ActivateRageMode()
    {
        body.velocity = Vector2.zero;
        inCutscene = true;
        inRageCutscene = true;

        yield return new WaitForSeconds(0.5f);
        rageChargeObj.SetActive(true);
        yield return new WaitForSeconds(3.75f);
        rageAuraObj.SetActive(true);
        yield return new WaitForSeconds(0.75f);
        inCutscene = false;
        inRageCutscene = false;
    }


    protected IEnumerator BackToBall()
    {
        // yield return new WaitForSeconds(5);
        int times = 40;
        float x = model.transform.localScale.x / times;
        foreach (SpriteRenderer renderer in renderers)
        {
            if (flashMat != null)
                renderer.material = flashMat;
        }
        for (int i=0 ; i<times ; i++)
        {
            model.transform.localScale -= new Vector3(x,x);
            // yield return new WaitForSeconds(0.01f);
            yield return new WaitForEndOfFrame();
        }
        if (bossRoom != null)
            bossRoom.Walls(false);

        // yield return new WaitForSeconds(0.01f);
        yield return new WaitForEndOfFrame();
        Destroy(this.gameObject);
    }

    void OnInspectorUpdate()
    {
        if (lvText != null)
            lvText.text = "Lv. " + lv; 
    }
}
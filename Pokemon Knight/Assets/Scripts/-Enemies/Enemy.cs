using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


public abstract class Enemy : MonoBehaviour
{
    public int lv=2;
    public int maxHp=100;
    public int hp;
    public int expPossess=5;
    protected PlayerControls playerControls;
    

    [Space]
    [Header("Level Bonus")]
    public int extraHp=2;   // Bonus
    public int extraDmg=2;  // Bonus
    public int defaultLv=5; // Bonus
    public int lvBreak=50;  // Additional bonus
    public int extraExp=2;  // Additional bonus
    
    [SerializeField] private GameObject emptyHolder;
    [SerializeField] private TextMeshPro dmgText;

    public Rigidbody2D body;
    public GameObject model;    // sprites holder
    protected bool receivingKnockback;
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
    [SerializeField] protected GameObject battleRoarObj;
    [SerializeField] protected GameObject rageChargeObj;
    [SerializeField] protected GameObject rageAuraObj;
    [SerializeField] protected bool isBoss;
    protected bool inRage;
    protected bool inCutscene;
    protected bool isDefeated;

    [Space] [SerializeField] protected AudioManager audioManager;

    [Space]
    [Header("Damage Related")]
    public int contactDmg=5;
    public float contactKb=30;
    
    [Space] [SerializeField] private SpriteRenderer[] renderers;
    [SerializeField] private Material flashMat;
    [SerializeField] private Material origMat;


    private void Awake()
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

    private void Start() 
    {
        if (statusBar != null)
            statusBar.SetActive(false);
        playerControls = GameObject.Find("PLAYER").GetComponent<PlayerControls>();

        if (!isBoss)
            this.gameObject.transform.localScale *= Random.Range(0.95f, 1.05f);
    }

    private void LateUpdate() 
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

            // var holder = Instantiate(emptyHolder, new Vector3(transform.position.x, transform.position.y + 2), Quaternion.identity);
            // Destroy(holder.gameObject, 0.5f);
            // var obj = Instantiate(dmgText, new Vector3(transform.position.x, transform.position.y + 2), Quaternion.identity, holder.transform);
            // obj.text = dmg.ToString();
            if (dmg > 0)
                StartCoroutine( Flash() );

            if (force > 0 && opponent != null)
                StartCoroutine( ApplyKnockback(opponent, force) );

            if (isBoss && hp <= 0)
            {
                Time.timeScale = 0.5f;
                isDefeated = true;
                inCutscene = true;
                statusBar.SetActive(false);
                body.velocity = Vector2.zero;
                body.gravityScale = 7.5f;
            }
            else if (!isBoss && hp <= 0)
            {
                if (playerControls != null)
                    playerControls.GainExp(expPossess,lv);
                Destroy(this.gameObject);
            }
            // half health
            else if (isBoss && !inRage && (float)hp/(float)maxHp < 0.5f)
            {
                inRage = true;  
                StartCoroutine( ActivateRageMode() );
                if (audioManager != null)
                    StartCoroutine( audioManager.TransitionBossTrack() );
            }
        }
    }

    public IEnumerator ApplyKnockback(Transform opponent, float force)
    {
        receivingKnockback = true;
        Vector2 direction = (opponent.position - this.transform.position).normalized;
        direction *= new Vector2(1,0);
        body.AddForce(-direction * force * kbDefense, ForceMode2D.Impulse);
        
        yield return new WaitForSeconds(0.1f);
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
            player.TakeDamage(contactDmg, this.transform, contactKb);
        }    
    }


    // todo  (BOSS)  ------------------------------------------------------------

    // Cutscene
    protected IEnumerator BossIntro()
    {
        body.velocity = Vector2.zero;
        inCutscene = true;

        yield return new WaitForSeconds(1);
        battleRoarObj.SetActive(true);
        
        yield return new WaitForSeconds(3);
        battleRoarObj.SetActive(false);
        inCutscene = false;
    }
    IEnumerator ActivateRageMode()
    {
        body.velocity = Vector2.zero;
        inCutscene = true;

        yield return new WaitForSeconds(0.5f);
        rageChargeObj.SetActive(true);
        yield return new WaitForSeconds(3.75f);
        rageAuraObj.SetActive(true);
        yield return new WaitForSeconds(0.75f);
        inCutscene = false;
    }

    void OnInspectorUpdate()
    {
        if (lvText != null)
            lvText.text = "Lv. " + lv; 
    }
}

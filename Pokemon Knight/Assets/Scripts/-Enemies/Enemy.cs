using System.Collections;
// using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

#if UNITY_EDITOR

using UnityEditor;

#endif

public abstract class Enemy : MonoBehaviour
{
    public int lv=2;
    protected int defaultLv=1; // Bonus
    [Space] public int maxHp=100;
    public int hp;
    protected PlayerControls playerControls;
    [SerializeField] protected Animator mainAnim;
    

    [Space]
    [Header("Level Bonus")]
    public int extraHp=2;   // Bonus
    public int extraDmg=2;  // Bonus
    public int extraProjectileDmg=0;  // Bonus
    public int perLv=1;  // Bonus
    [SerializeField] protected int totalExtraDmg=0;
    
    protected int origContactDmg;
    protected float origContactKb;
    protected int origTotalExtraDmg;

    [Space] public int expPossess=5;
    public int extraExp=2;  // Additional bonus
    public int lvBreak=50;  // Additional bonus
    
    //// [SerializeField] private GameObject emptyHolder;
    //// [SerializeField] private TextMeshPro dmgText;

    [Space] public Collider2D col;
    public Rigidbody2D body;
    public GameObject model;    // sprites holder
    protected bool receivingKnockback;
    public bool cannotRecieveKb;
    [Tooltip("1 = 100% kb, 0 = 0%")] [SerializeField] [Range(0,1f)] protected float kbDefense=1;
    [SerializeField] protected LayerMask whatIsGround;
    [SerializeField] protected LayerMask whatIsPlayer;


    //* UI
    public GameObject statusBar;
    public Image hpEffectImg;
    public Image hpImg;
    public TextMeshProUGUI lvText;
    [SerializeField] private float effectSpeed = 0.005f;


    [Header("size")]
    public Vector3 origSize;
    public float ShrinkDuration = 0.5f;
    public float t = 0;

    [Space]
    [Header("Boss")]
    public bool isBoss;
    public bool isMiniBoss;
    public GameObject possessedAura;
    public GameObject battleRoarObj;
    public GameObject rageChargeObj;
    [HideInInspector] public bool inRage;
    [HideInInspector] public bool inCutscene; // Can't move
    [HideInInspector] public bool inRageCutscene; // Can't move
    [HideInInspector] public bool isDefeated;
    [Space] [Space] public string powerupName;
    public GameObject pokeball;
    public GameObject canCatchEffect;
    [HideInInspector] public bool mustDmgBeforeFight;
    [HideInInspector] public bool bossBattleBegin;


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

    [Space] [Header("Waves Related")]
    [HideInInspector] public WaveSpawner spawner;

    
    [Space] [Header("Support")]
    public GameObject alert;
    [HideInInspector] protected bool canFlip=true ;
    public bool playerInField;
    public bool playerInSight;
    protected bool trigger;
    public bool alwaysAttackPlayer;


    [Space] [Header("Buffs / Debuffs")]
    [SerializeField] protected bool canUseBuffs;
    [SerializeField] protected Image[] statusConditions;
    [SerializeField] protected int nCondition;
    [Space] [SerializeField] protected Sprite empty;
    [SerializeField] protected Sprite increaseAtk;
    [SerializeField] protected Sprite increaseDef;
    [SerializeField] protected Sprite increaseSpd;
    private int defenseStage;
    protected enum Stat { atk, def, spd };
    protected bool inAnimation;



    public virtual void Start() 
    {
        if (statusBar != null)
            statusBar.SetActive(false);
        if (GameObject.Find("PLAYER") != null)
            playerControls = GameObject.Find("PLAYER").GetComponent<PlayerControls>();

        if (!isBoss)
            model.transform.localScale *= Random.Range(0.9f, 1.1f);
        if (isMiniBoss)
        {
            maxHp *= 3;
            expPossess *= 3;
        }

        Setup();

        // if (lv > lvBreak)
        //     maxHp += (lv - lvBreak) * extraHp;
        if (lv > defaultLv)
            maxHp += Mathf.CeilToInt((extraHp * (lv - defaultLv))/2f);
        if (lv > defaultLv)
            extraDmg += Mathf.FloorToInt((extraDmg * lv - defaultLv)/2f);

        totalExtraDmg = Mathf.Max(0, extraProjectileDmg * Mathf.FloorToInt((float)(lv - defaultLv)/perLv));

        hp = maxHp;   
        if (lvText != null)
            lvText.text = "Lv. " + lv; 

        origSize = model.transform.localScale;
    }

    public virtual void Setup() {}
    public virtual void CallChildOnRoar() {}
    public virtual void CallChildOnBossFightStart() {}
    public virtual void CallChildOnRage() {}
    public virtual void CallChildOnRageCutsceneFinished() {}
    public virtual void CallChildOnDeath() {}
    public virtual void CallChildOnTargetLost() {}  // VIA EnemyFieldOfVision
    public virtual void CallChildOnTargetFound() {}  // VIA EnemyFieldOfVision
    
    public virtual void CallChildOnIncreaseSpd() {}  // VIA EnemyFieldOfVision
    public virtual void CallChildOnRevertSpd() {}  // VIA EnemyFieldOfVision

    // todo ----------------------------------------------------------------------------------------------------

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
        if (hp <= 0 && !isBoss)
        {
            t += Time.deltaTime / ShrinkDuration;

            // Lerp wants the third parameter to go from 0 to 1 over time. 't' will do that for us.
            Vector3 newScale = Vector3.Lerp(origSize, Vector3.zero, t);
            model.transform.localScale = newScale;

            // We're done! We can disable this component to save resources.
            if (model.transform.localScale.x <= 0) {
                Destroy(this.gameObject);
            }
        }
    }


    protected void WalkTheOtherWay()
    {
        //* LOOKING RIGHT
        if (model.transform.eulerAngles.y > 0)
            model.transform.eulerAngles = new Vector3(0, 0);
        else
            model.transform.eulerAngles = new Vector3(0, 180);
        body.velocity = new Vector2(-body.velocity.x, body.velocity.y);
    }

    public IEnumerator ResetFlipTimer()
    {
        canFlip = false;
        yield return new WaitForSeconds(0.5f);
        canFlip = true;
    }

    public void LookAtPlayer()
    {
        if (playerControls == null)
            return;

        if (playerControls.transform.position.x - this.transform.position.x > 0)
            model.transform.eulerAngles = new Vector3(0,180);
        else if (playerControls.transform.position.x - this.transform.position.x < 0)
            model.transform.eulerAngles = new Vector3(0,0);
    }
    public bool PlayerIsToTheLeft()
    {
        if (playerControls == null)
            return false;

        return(playerControls.transform.position.x - this.transform.position.x < 0);
    }

    public void TakeDamage(int dmg=0, Transform opponent=null, float force=0)
    {
        if (!inCutscene)
        {
            if (hp > 0)
            {
                if (defenseStage == 0)
                    hp -= dmg;
                else
                    hp -= Mathf.FloorToInt( Mathf.Pow(0.7f, defenseStage) * dmg);
                statusBar.SetActive(true);
            }

            //// var holder = Instantiate(emptyHolder, new Vector3(transform.position.x, transform.position.y + 2), Quaternion.identity);
            //// Destroy(holder.gameObject, 0.5f);
            //// var obj = Instantiate(dmgText, new Vector3(transform.position.x, transform.position.y + 2), Quaternion.identity, holder.transform);
            //// obj.text = dmg.ToString();

            if (dmg > 0 && dmg < hp)
                StartCoroutine( Flash() );

            if (force > 0 && opponent != null && !cannotRecieveKb)
                StartCoroutine( ApplyKnockback(opponent, force) );

            if (isSmart)
                LookAtPlayer();

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
                if (mainAnim != null)
                    mainAnim.speed = 0.3f;
                CallChildOnDeath();

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
                if (spawner != null && !isBoss)
                    spawner.SpawnedDefeated();

                if (playerControls != null)
                    playerControls.GainExp(expPossess + (extraExp * Mathf.Max(1, lv - defaultLv)), lv);

                if (!isBoss)
                {
                    if (body != null) 
                    {
                        body.gravityScale = 0;
                        body.bodyType = RigidbodyType2D.Static;
                    }
                    if (col != null) col.enabled = false;
                    foreach (SpriteRenderer renderer in renderers)
                        if (flashMat != null)
                            renderer.material = flashMat;
                    // Destroy(this.gameObject);
                    // StartCoroutine( Fainted() );
                }
                else if (playerControls != null)
                    playerControls.BossBattleOver();
            }
            // Boss half health
            else if (isBoss && !isMiniBoss && !inRage && (float)hp/(float)maxHp < 0.5f)
            {
                inRage = true;  
                CallChildOnRage();
                StartCoroutine( ActivateRageMode() );
            }
        }
        if (mustDmgBeforeFight && !bossBattleBegin)
        {
            StartBossBattle();
        }
    }
    IEnumerator DramaticSlowmo()
    {
        Time.timeScale = 0.5f;
        yield return new WaitForSeconds(0.5f);
        Time.timeScale = 1;
        yield return new WaitForSeconds(0.5f);
        if (rageChargeObj != null) rageChargeObj.SetActive(false);
        
        if (canCatchEffect != null) 
            canCatchEffect.SetActive(true);
        canCatch = true;
    }
    public IEnumerator ApplyKnockback(Transform opponent, float force)
    {
        if (hp > 0)
        {
            receivingKnockback = true;
            Vector2 direction = (opponent.position - this.transform.position).normalized;
            body.velocity = -direction * force * kbDefense;
            yield return new WaitForSeconds(0.1f);
            
            if (hp > 0)
                body.velocity = Vector2.zero;
            receivingKnockback = false;
        }
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


    public void PlayerCollision()
    {
        if (hp > 0 && !inCutscene)
        {
            playerControls.TakeDamage(contactDmg + extraDmg, this.transform, contactKb);
            if (!cannotRecieveKb)
                body.velocity = Vector2.zero;
        }    
        else if (canCatch)
        {
            canCatch = false;
            StartCoroutine( BackToBall() );
            Pokeball obj = Instantiate( pokeball, this.transform.position, Quaternion.identity).GetComponent<Pokeball>();
            obj.powerup = this.powerupName;
            if (spawner != null)
                obj.spawner = this.spawner;
            if (bossRoom != null)
                obj.bossRoom = this.bossRoom;
        }    
    }


    // todo  (BOSS)  ------------------------------------------------------------

    public void StartBossBattle(float delay=0.5f)
    {
        if (!bossBattleBegin)
        {
            bossBattleBegin = true;
            StartCoroutine( BossIntro(delay) );
            if (bossRoom != null)
            {
                bossRoom.Walls(true);
            }
        }
    }
    protected IEnumerator BossIntro(float delay)
    {
        inCutscene = true;
        body.velocity = Vector2.zero;

        CallChildOnRoar();
        yield return new WaitForSeconds(delay);
        if (battleRoarObj != null) 
            battleRoarObj.SetActive(true);
        
        yield return new WaitForSeconds(3);
        if (battleRoarObj != null) 
            battleRoarObj.SetActive(false);

        inCutscene = false;
        CallChildOnBossFightStart();
    }
    IEnumerator ActivateRageMode()
    {
        body.velocity = Vector2.zero;
        inCutscene = true;
        inRageCutscene = true;

        yield return new WaitForSeconds(0.5f);
        if (rageChargeObj != null) rageChargeObj.SetActive(true);
        yield return new WaitForSeconds(4.5f);
        CallChildOnRageCutsceneFinished();
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

        // yield return new WaitForSeconds(0.01f);
        yield return new WaitForEndOfFrame();
        Destroy(this.gameObject);
    }

    protected IEnumerator Fainted()
    {
        if (body != null) body.gravityScale = 0;
        if (col != null) col.enabled = false;

        // int times = 40;
        // float x = model.transform.localScale.x / times;
        // foreach (SpriteRenderer renderer in renderers)
        // {
        //     if (flashMat != null)
        //         renderer.material = flashMat;
        // }
        // for (int i=0 ; i<times ; i++)
        // {
        //     model.transform.localScale -= new Vector3(x,x);
        //     yield return new WaitForEndOfFrame();
        // }

        yield return new WaitForSeconds(1);
        // yield return new WaitForEndOfFrame();
        Destroy(this.gameObject);
    }


    public void ANIMATION_OVER()
    {
    inAnimation = false;
    }

    protected IEnumerator ResetBuff(float duration, float duration2, Stat statType)
    {
        canUseBuffs = false;
        if (statType == Stat.atk)
            IncreaseAtk();
        else if (statType == Stat.def)
            IncreaseDef();
        else if (statType == Stat.spd)
            IncreaseSpd();

        yield return new WaitForSeconds(duration);
        if (statType == Stat.atk)
            RevertAtk();
        else if (statType == Stat.def)
            RevertDef();
        else if (statType == Stat.spd)
            RevertSpd();
        yield return new WaitForSeconds(duration2);
        canUseBuffs = true;
    }


    protected void IncreaseAtk()
    {
        origContactDmg = contactDmg;
        origContactKb = contactKb;
        origTotalExtraDmg = totalExtraDmg;
        if (statusBar != null && !statusBar.activeSelf)
            statusBar.SetActive(true);

        contactDmg = Mathf.CeilToInt(1.3f * contactDmg);
        contactKb = Mathf.CeilToInt(1.3f * contactKb);
        totalExtraDmg = Mathf.CeilToInt(1.3f * totalExtraDmg);
        if (nCondition < statusConditions.Length)
            statusConditions[nCondition].sprite = increaseAtk;
        nCondition++;
    }
    protected void IncreaseSpd()
    {
        CallChildOnIncreaseSpd();
        if (statusBar != null && !statusBar.activeSelf)
            statusBar.SetActive(true);

        if (nCondition < statusConditions.Length)
            statusConditions[nCondition].sprite = increaseSpd;
        nCondition++;
    }
    protected void IncreaseDef(int stages=1)
    {
        if (statusBar != null && !statusBar.activeSelf)
            statusBar.SetActive(true);

        defenseStage += stages;
        if (nCondition < statusConditions.Length)
            statusConditions[nCondition].sprite = increaseDef;
        nCondition++;
    }

    protected void RevertAtk()
    {
        contactDmg = origContactDmg;
        contactKb = origContactKb;
        totalExtraDmg = origTotalExtraDmg;
        nCondition--;
        ConditionFinished();
    }
    protected void RevertDef(int stages=1)
    {
        defenseStage -= stages;
        nCondition--;
        ConditionFinished();
    }
    protected void RevertSpd(int stages=1)
    {
        CallChildOnRevertSpd();
        nCondition--;
        ConditionFinished();
    }
    protected void ConditionFinished()
    {
        for (int i=0 ; i<statusConditions.Length - 1 ; i++)
            statusConditions[i].sprite = statusConditions[i+1].sprite;
    }
}

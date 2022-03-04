﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;

#if UNITY_EDITOR

using UnityEditor;

#endif

public abstract class Enemy : MonoBehaviour
{
    private string enemyId;
    public int lv=2;
    protected int defaultLv=1; // Bonus
    private int lvBreak=10;  // Additional bonus
    [HideInInspector] public enum ChapterLevel { one, two, three, four, five, six, seven };
    public ChapterLevel chapterLevel = ChapterLevel.one;
    [Space] public int maxHp=100;
    public int hp;
    protected PlayerControls playerControls;
    [Space] [SerializeField] protected Animator mainAnim;
    private bool inPlayMode;
    

    [Space] [Header("Damage Related")]
    public int contactDmg=5;
    public float contactKb=10;
    public int projectileDmg=5;
    [Space] public bool isSmart;    // Turns if attacked from behind;
    
    [Space] [SerializeField] private SpriteRenderer[] renderers;
    [SerializeField] private Material defeatedMat;
    [SerializeField] private Material flashMat;
    [SerializeField] private Material origMat;
    [HideInInspector] public BossRoom bossRoom;
    private bool canCatch;


    [Space] [Header("Level Bonus")]
    public int extraHp=2;   // Bonus
    public int extraDmg=2;  // Bonus
    public int extraProjectileDmg=0;  // Bonus
    public int perLv=1;  // Bonus
    [SerializeField] protected int calcExtraProjectileDmg=0;
    
    protected int origContactDmg;
    protected float origContactKb;
    protected int origTotalExtraDmg;

    [Space] public int expPossess=5;
    public int extraExp=2;  // Additional bonus
    
    //// [SerializeField] private GameObject emptyHolder;
    //// [SerializeField] private TextMeshPro dmgText;


    [Space] public Collider2D col;
    public Rigidbody2D body;
    public GameObject model;    // sprites holder
    protected bool receivingKnockback=false;
    public bool cannotRecieveKb;
    [Tooltip("1 = 100% kb, 0 = 0%")] [SerializeField] [Range(0,1f)] protected float kbDefense=1;
    [SerializeField] protected LayerMask whatIsGround;
    [SerializeField] protected LayerMask whatIsPlayer;
    [SerializeField] protected LayerMask whatIsBounds;
    [SerializeField] private LayerMask whatIsTree;
    protected LayerMask obstacleMask;


    //* UI
    public GameObject statusBar;
    public Image hpEffectImg;
    public Image hpImg;
    public TextMeshProUGUI lvText;
    [SerializeField] private float effectSpeed = 0.005f;


    [Header("Feet")]
    [HideInInspector] public Vector3 origSize;
    [HideInInspector] public float ShrinkDuration = 0.5f;
    [HideInInspector] public float t = 0;
    [Space] public Vector2 feetSize;
    public Vector2 feetOffset;


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



    [Space] [Header("Waves Related")]
    [HideInInspector] public WaveSpawner spawner;

    
    [Space] [Header("Support / Mechanics")]
    public GameObject alert;
    [HideInInspector] protected bool canFlip=true ;
    public bool playerInField;
    public bool playerInSight;
    public bool playerInCloseRange;
    protected bool trigger;
    public bool alwaysAttackPlayer;
    [Space] public bool cannotMove;
    [SerializeField] protected bool movingLeft;
    [SerializeField] protected bool movingRight;
    private bool downToHalfHp;
    [Space] public bool aquatic;


    [Space] [Header("Buffs / Debuffs")]
    public bool canUseBuffs;
    [SerializeField] protected Image[] statusConditions;
    [SerializeField] protected int nCondition;
    [Space] [SerializeField] protected Sprite empty;
    [SerializeField] protected Sprite increaseAtk;
    [SerializeField] protected Sprite increaseDef;
    [SerializeField] protected Sprite increaseSpd;
    private int defenseStage;
    protected enum Stat { atk, def, spd };
    protected bool inAnimation;


    private enum HpEffect { none, heal, lost };
    private HpEffect healthStatus = HpEffect.none;
    [HideInInspector] public bool spawnedByWave;




    public virtual void Start() 
    {
        if (!isBoss && !spawnedByWave)
        {
            enemyId = SceneManager.GetActiveScene().name + " " + this.name;
            if (PlayerPrefsElite.VerifyArray("enemyDefeated"))
            {
                HashSet<string> names = new HashSet<string>(PlayerPrefsElite.GetStringArray("enemyDefeated"));
                if (names.Contains(enemyId))
                    Destroy(this.gameObject);
            }
        }

        inPlayMode = true;
        if (statusBar != null)
            statusBar.SetActive(false);
        if (GameObject.Find("PLAYER") != null)
            playerControls = GameObject.Find("PLAYER").GetComponent<PlayerControls>();
        else
            Debug.LogError("PLAYER couldn't be found!!");

        if (!isBoss)
            model.transform.localScale *= Random.Range(0.9f, 1.1f);
        if (isMiniBoss)
        {
            maxHp *= 5;
            expPossess *= 5;
        }


        if (lv > defaultLv)
            maxHp += Mathf.CeilToInt((extraHp * (lv - defaultLv))/2f);
        if (lv > defaultLv)
            contactDmg += Mathf.FloorToInt((extraDmg * (lv - defaultLv))/2f);
        if (lv > defaultLv)
            expPossess += Mathf.FloorToInt((extraExp * Mathf.Max(1, lv - defaultLv)) );

        calcExtraProjectileDmg = Mathf.Max(0, extraProjectileDmg * Mathf.FloorToInt((float)(lv - defaultLv)/perLv));

        if (!isBoss || isMiniBoss)
        {
            int nextAreas = (lv / lvBreak);
            for (int i=0 ; i<nextAreas ; i++)
            // for (int i=0 ; i<(int)chapterLevel ; i++)
            {
                maxHp = Mathf.RoundToInt(maxHp * 2f);
                expPossess = Mathf.RoundToInt(expPossess * 2f);
            }
        }

        hp = maxHp;   
        if (lvText != null)
            lvText.text = "Lv. " + lv; 

        origSize = model.transform.localScale;
        obstacleMask = (whatIsGround | whatIsBounds);
        
        Setup();
    }

    public virtual void Setup() {}
    public virtual void CallChildOnRoar() {}
    public virtual void CallChildOnBossFightStart() {}
    public virtual void CallChildOnRage() {}
    public virtual void CallChildOnRageCutsceneFinished() {}
    public virtual void CallChildOnDeath() {}
    public virtual void CallChildOnBossDeath() {}
    public virtual void CallChildOnTargetLost() {}  // VIA EnemyFieldOfVision
    public virtual void CallChildOnTargetFound() {}  // VIA EnemyFieldOfVision
    
    public virtual void CallChildOnHalfHealth() {}  
    public virtual void CallChildOnIncreaseSpd() {}  
    public virtual void CallChildOnRevertSpd() {}  

    // todo ----------------------------------------------------------------------------------------------------

    protected void LateUpdate() 
    {
        if (hpImg != null && hpEffectImg != null)
        {
            float currentHpPercent = ((float) Mathf.Max(0,hp) /(float) maxHp);

            if      (hpImg.fillAmount < currentHpPercent)    //* RECOVERED HEALTH
            {
                hpEffectImg.fillAmount = currentHpPercent;
                healthStatus = HpEffect.heal;
            }
            else if (hpImg.fillAmount > currentHpPercent)    //* LOST HEALTH
            {
                hpImg.fillAmount = currentHpPercent;
                healthStatus = HpEffect.lost;
            }
            
            //* Hp gain Effect
            if (healthStatus == HpEffect.heal)
            {
                if (hpImg.fillAmount < hpEffectImg.fillAmount)
                    hpImg.fillAmount += effectSpeed;
                else 
                {
                    hpImg.fillAmount = hpEffectImg.fillAmount;
                    healthStatus = HpEffect.none;
                }
            }
            //* Hp lost 
            else if (healthStatus == HpEffect.lost)
            {
                if (hp > 0)
                {
                    if (hpEffectImg.fillAmount > hpImg.fillAmount)
                        hpEffectImg.fillAmount -= effectSpeed;
                    else
                    {
                        hpEffectImg.fillAmount = hpImg.fillAmount;
                        healthStatus = HpEffect.none;
                    }
                }
                else if (hpEffectImg.fillAmount > hpImg.fillAmount)
                    hpEffectImg.fillAmount -= (effectSpeed * 5);
            }
                    
            
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
        {
            // Move towards player
            if (movingLeft)
            {
                movingLeft = false; 
                movingRight = true;
            }
            model.transform.eulerAngles = new Vector3(0,180);
        }
        else if (playerControls.transform.position.x - this.transform.position.x < 0)
        {
            // Move towards player
            if (movingRight)
            {
                movingLeft = true; 
                movingRight = false;
            }
            model.transform.eulerAngles = new Vector3(0,0);
        }
    }
    public bool PlayerIsToTheLeft()
    {
        if (playerControls == null)
            return false;

        return(playerControls.transform.position.x - this.transform.position.x < 0);
    }

    public void TakeDamage(int dmg, Vector3 hitPos, float force=0, bool attackedByPlayer=true, int spBonus=0)
    {
        if (!inCutscene && hp > 0)
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

            if (attackedByPlayer && playerControls != null)
                playerControls.FillGauge(spBonus);

            if (!downToHalfHp && hpImg.fillAmount <= 0.5f)
            {
                downToHalfHp = true;
                CallChildOnHalfHealth();
            }

            if (dmg > 0 && dmg < hp)
                StartCoroutine( Flash() );

            if (force > 0 && hitPos != null && !cannotRecieveKb)
                StartCoroutine( ApplyKnockback(hitPos, force) );

            if (isSmart && force > 0)
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
                CallChildOnBossDeath();

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

                if (playerControls != null && attackedByPlayer)
                {
                    playerControls.GainExp(expPossess, lv);
                    playerControls.KilledEnemy(enemyId);
                }

                if (!isBoss)
                {
                    CallChildOnDeath();
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
    public IEnumerator ApplyKnockback(Vector3 hitPos, float force)
    {
        if (hp > 0)
        {
            receivingKnockback = true;
            Vector2 direction = (hitPos - this.transform.position).normalized;
            direction *= new Vector2(1,0);
            body.velocity = -direction * force * kbDefense;
            yield return new WaitForSeconds(0.1f);
            
            if (hp > 0)
                if (body.gravityScale != 0)
                    body.velocity = new Vector2(0, Mathf.Min(0, body.velocity.y));
                else
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

    protected bool IsGrounded()
    {
        return Physics2D.OverlapBox(this.transform.position + (Vector3) feetOffset, feetSize, 0, whatIsGround);
    }
    public void PlayerCollision()
    {
        if (hp > 0 && !inCutscene)
        {
            playerControls.TakeDamage(contactDmg, this.transform, contactKb);
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


    public void INCREASE_ATK()
    {
        if (canUseBuffs)
        {
            canUseBuffs = false;
            StartCoroutine( INCREASE_ATK_CO() );
        }
    }
    IEnumerator INCREASE_ATK_CO()
    {
        canUseBuffs = false;
        IncreaseAtk();

        yield return new WaitForSeconds(5);
        RevertAtk();

        yield return new WaitForSeconds(5);
        canUseBuffs = true;
    }
    public void INCREASE_DEF()
    {
        if (canUseBuffs)
        {
            canUseBuffs = false;
            StartCoroutine( INCREASE_DEF_CO() );
        }
    }
    IEnumerator INCREASE_DEF_CO()
    {
        canUseBuffs = false;
        IncreaseDef();

        yield return new WaitForSeconds(5);
        RevertDef();

        yield return new WaitForSeconds(5);
        canUseBuffs = true;
    }
    public void INCREASE_SPD()
    {
        if (canUseBuffs)
        {
            canUseBuffs = false;
            StartCoroutine( INCREASE_SPD_CO() );
        }
    }
    IEnumerator INCREASE_SPD_CO()
    {
        canUseBuffs = false;
        IncreaseSpd();

        yield return new WaitForSeconds(5);
        RevertSpd();

        yield return new WaitForSeconds(5);
        canUseBuffs = true;
    }

    protected void IncreaseAtk()
    {
        origContactDmg = contactDmg;
        origContactKb = contactKb;
        origTotalExtraDmg = calcExtraProjectileDmg;
        if (statusBar != null && !statusBar.activeSelf)
            statusBar.SetActive(true);

        contactDmg = Mathf.CeilToInt(1.3f * contactDmg);
        contactKb = Mathf.CeilToInt(1.3f * contactKb);
        calcExtraProjectileDmg = Mathf.CeilToInt(1.3f * calcExtraProjectileDmg);
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
        calcExtraProjectileDmg = origTotalExtraDmg;
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

    public void LogCurrentStatus()
    {

        if (!inPlayMode)
        {
            int tempHp = maxHp;
            int tempExp = expPossess;
            if (lv > defaultLv)
                tempHp += Mathf.CeilToInt((extraHp * (lv - defaultLv))/2f);
            if (lv > defaultLv)
                tempExp += Mathf.FloorToInt((extraExp * Mathf.Max(1, lv - defaultLv)) );
            
            if (!isBoss)
            {
                int nextAreas = (lv / lvBreak);
                for (int i=0 ; i<nextAreas ; i++)
                // for (int i=0 ; i<(int)chapterLevel ; i++)
                {
                    tempHp = Mathf.RoundToInt(tempHp * 1.5f);
                    tempExp = Mathf.RoundToInt(tempExp * 1.5f);
                }
            }

            string gap = "          ";
            Debug.Log("<color=yellow>  " + this.name + " <b><i>( Lv. " + this.lv + " )</i></b></color> =   " +
                "<color=#11FF00> maxHP = " + (tempHp) +  gap +
                "</color><color=#FF8000> contactDmg = " + (contactDmg + (Mathf.FloorToInt((extraDmg * (lv - defaultLv))/2f))) + gap +
                "</color><color=#F784FF> projectileDmg = " + 
                (projectileDmg + Mathf.Max(0, extraProjectileDmg * Mathf.FloorToInt((float)(lv - defaultLv)/perLv))) + gap +
                "</color><color=#00E8FF> expGained = " + (tempExp) + "</color>"
            );
        }
        else
        {
            string gap = "          ";
            Debug.Log("<color=yellow>  " + this.name + " <b><i>( Lv. " + this.lv + " )</i></b></color> =   " +
                "<color=#11FF00> maxHP = " + (maxHp) +  gap +
                "</color><color=#FF8000> contactDmg = " + (contactDmg) + gap +
                "</color><color=#F784FF> projectileDmg = " + (projectileDmg + calcExtraProjectileDmg) + gap +
                "</color><color=#00E8FF> expGained = " + (expPossess) + "</color>"
            );
        }
    }
}


[CanEditMultipleObjects] [CustomEditor(typeof(Enemy), true)]
public class EnemyEditor : Editor
{
    public override void OnInspectorGUI()
    {

        Enemy enemyScript = (Enemy)target;

        if (GUILayout.Button("Calculate Status"))
        {
            enemyScript.LogCurrentStatus();
        }
        EditorGUILayout.Space();

        DrawDefaultInspector();
    }
}
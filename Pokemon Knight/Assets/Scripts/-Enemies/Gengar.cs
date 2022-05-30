using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gengar : Enemy
{
    public enum Variation { nightShader, boss };
    
    [Space] [Header("Gengar")]  
    public Variation variant;
    
    
    [Space] [Header("Chaser")]  
    public float moveSpeed=2;
    public float chaseSpeed=4;
    public float maxSpeed=5f;
    // public float chargeSpeed=10;
    private LayerMask finalMask;
    public Transform target;
    public bool chasing;
    public bool teleporting;
    public float distanceAway=3;
    public bool playerInRange;


    [Space] [Header("Licker")]  
    public bool licking;
    public float teleportAgain=1.5f;
    public float flipTimer=3f;
    [SerializeField] private float lickMoveForce=1;
    [SerializeField] private EnemyAttack lickAtk;
	private Vector2 moveDir;


    [Space] [Header("Night shade")]  
    [SerializeField] private EnemyProjectile nightShade;
    private bool isStalker;
    [SerializeField] private int numNightShade=5;
    [SerializeField] private float nightShadeRayHeight=1.5f;
    [SerializeField] private float nightShadeMaxDist=5f;
    [SerializeField] private float nightShadeXOffset=1f;


    [SerializeField] private Transform lockPos;
    [SerializeField] private Vector2 lockOffset;


    [Space] [Header("Willo Wisp")]  
    [SerializeField] private Transform[] wispSpawns;
    [SerializeField] private EnemyProjectile wispAtk;
    [SerializeField] private List<EnemyProjectile> wisps;
    [SerializeField] private float wispFireDelay=0.8f;
    [SerializeField] private float wispSpeed=7.5f;
    private bool shootingWisp;

    
    [Space] [Header("Teleport")]
    [SerializeField] private Transform[] teleportPos;

    

    [Space] [Header("Misc")]  
    // [SerializeField] private GameObject glint;
    [SerializeField] private bool variantSpeed;
    public Vector3 lineOfSight;
    [SerializeField] private GameObject spawnedHolder;
    private Coroutine co;
    [SerializeField] private Coroutine targetLostCo;
    private RaycastHit2D playerInfo;
    private bool sceneLoaded=false;

    private float timer=0;
    private float closeTime=0;
    public float teleportTimer=1;
    private int origAtkDmg;
    public bool performingFuryAttack;


    public override void Setup()
    {
        // finalMask = (whatIsPlayer | whatIsGround);
        // if (alert != null) alert.gameObject.SetActive(false);
        if (spawnedHolder != null) 
            spawnedHolder.transform.parent = null;
        if (target == null)
            target = playerControls.transform;

        int gameNumber = PlayerPrefsElite.GetInt("gameNumber");
        if (PlayerPrefsElite.VerifyBoolean("caughtGengar" + gameNumber))
            if (PlayerPrefsElite.GetBoolean("caughtGengar" + gameNumber))
                this.gameObject.SetActive(false);
        else
            PlayerPrefsElite.SetBoolean("caughtGengar" + gameNumber, false);

        // origAtkDmg = contactDmg;

        // if (Random.Range(0,2) == 0)
        //     movingLeft = true;
        // else
        // {
        //     movingRight = true;
        //     model.transform.eulerAngles = new Vector3(0, 180);
        // }


        if (lickAtk != null)
        {
            lickAtk.atkDmg = secondDmg;
            lickAtk.kbForce = contactKb;
        }
        // if (variantSpeed)
        // {
        //     moveSpeed += Random.Range(0.9f, 1.2f);
        //     chaseSpeed += Random.Range(0.9f, 1.2f);
        //     maxSpeed += Random.Range(0.9f, 1.2f);
        //     lickMoveForce += Random.Range(0.9f, 1.2f);
        //     teleportAgain += Random.Range(0.9f, 1.2f);
        // }

        // if (wispAtk != null)
        // {
        //     wispAtk.atkDmg = projectileDmg + calcExtraProjectileDmg;
        //     wispAtk.kbForce = contactKb;
        //     wisps = new List<EnemyProjectile>();
        // }
        if (nightShade != null)
        {
            nightShade.atkDmg = projectileDmg + calcExtraProjectileDmg;
            nightShade.kbForce = contactKb;
        }
        if (variant == Variation.nightShader)
        {
            // cannotRecieveKb = true;
            mainAnim.SetBool("isStalking", true);
            isStalker = true;
        }
        StartCoroutine( LoadingIn() );
    }

    IEnumerator LoadingIn()
    {
        yield return new WaitForSeconds(0.5f);
        sceneLoaded = true;
    }

    public override void CallChildOnIncreaseSpd()
    {
        moveSpeed *= 1.5f;
        chaseSpeed *= 1.5f;
        maxSpeed *= 1.5f;
            
    }
    public override void CallChildOnRevertSpd()
    {
        moveSpeed /= 1.5f;
        chaseSpeed /= 1.5f;
        maxSpeed /= 1.5f;
    }

    public override void CallChildOnTargetFound()
    {
        // mainAnim.SetBool("isWalking", true);
    }
    public override void CallChildOnDeath()
    {
        mainAnim.speed = 0;
        mainAnim.SetTrigger("reset");
        StopAllCoroutines();
        foreach (EnemyProjectile wisp in wisps)
            if (wisp != null)
                Destroy( wisp.gameObject );
    }
    public override void CallChildOnDropLoot()
    {
        // if (!Physics2D.Linecast(transform.position - new Vector3(0.1f,0), transform.position + new Vector3(0.1f,0),
        //     whatIsGround))
        // if (!Physics2D.BoxCast(this.transform.position, new Vector2(0.1f, 0.1f), 0, Vector2.zero, 0, whatIsGround))
        // Collider[] cols = Physics.OverlapSphere(transform.position, 0.5f, whatIsGround);
        // foreach(Collider col in cols)
        // {
        //     Debug.Log(col.name);
        // }
        if (!Physics2D.OverlapBox(transform.position, new Vector2(0.2f, 0.2f), 0, whatIsGround))
        {
            if (!isBoss && !isMiniBoss)
                loot.DropLoot( Mathf.FloorToInt(lv / 10) );
            else
                loot.DropLoot();
        }
    }

	public override void CallChildOnBossFightStart()
    {
        teleporting = false;
		licking = false;
    }



    // todo ----------------------------------------------------------------------------------------------------
    void FixedUpdate() 
    {
        if (!sceneLoaded || inCutscene) {}
        else if (variant == Variation.nightShader)
        {
            // if (lockPos != null)
            //     transform.position = (Vector2) lockPos.position + lockOffset;
            
            // LookAtTarget();
        }
        else if (variant == Variation.boss)
        {
            if (!teleporting && !licking)
            {
                teleporting = true;
                mainAnim.speed = 1;
                mainAnim.SetTrigger("teleport");
            }
			if (teleporting && !receivingKnockback)
			{
				body.velocity = moveDir * lickMoveForce;
			}
        }
        
    }

    // todo ----------------------------------------------------------------------------------------------------


    private void CapVelocity()
    {
        float cappedSpeedX = 0;
        float cappedSpeedY = 0;

        // chasing right (positive velocity)
        if (target.position.x > this.transform.position.x)  // player is to the right
        {
            cappedSpeedX = Mathf.Min(body.velocity.x, maxSpeed);
            model.transform.eulerAngles = new Vector3(0, 180);  // face right
        }
        // chasing left (negative velocity)
        else
        {
            cappedSpeedX = Mathf.Max(body.velocity.x, -maxSpeed);
            model.transform.eulerAngles = new Vector3(0, 0);  // face left
        }

        // chasing up (positive velocity)
        if (target.position.y > this.transform.position.y)  // player is to the right
        {
            cappedSpeedY = Mathf.Min(body.velocity.y, maxSpeed);
        }
        // chasing down (negative velocity)
        else
        {
            cappedSpeedY = Mathf.Max(body.velocity.y, -maxSpeed);
        }

        body.velocity = new Vector2(cappedSpeedX, cappedSpeedY);
    }
    private void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.yellow;
        if (target != null)
        {
            // Vector3 lineOfSight = (target.position + new Vector3(0, 1)) - (this.transform.position);
            Gizmos.DrawLine(this.transform.position,
                this.transform.position + new Vector3(0, 1) + lineOfSight);
        }
    }
    private void Flip()
    {
        if (movingLeft)
        {
            movingLeft = false;
            movingRight = true;
            model.transform.eulerAngles = new Vector3(0, 180);
        }
        else if (movingRight)
        {
            movingRight = false;
            movingLeft = true;
            model.transform.eulerAngles = new Vector3(0, 0);
        }
    }


    public void STOP()
    {
        if (hp > 0)
        {
            body.velocity = Vector2.zero;
            // charging = false;
        }
    }
    public IEnumerator TELEPORT()
    {   
        if (target == null || hp <= 0)
            yield break;
        
        body.velocity = Vector2.zero;


        int rng = Random.Range(0,5);
        if (variant == Variation.boss)
            rng = Mathf.Max(1, rng);

        switch (rng)  // 5
        {
            case 0:
                this.transform.position = target.position + new Vector3(0, distanceAway + 1);
                break;
            case 1:
                this.transform.position = target.position + new Vector3(distanceAway, 1);
                break;
            case 2:
                this.transform.position = target.position + new Vector3(-distanceAway, 1);
                break;
            case 3:
                this.transform.position = target.position + new Vector3(distanceAway, distanceAway + 1);
                break;
            case 4:
                this.transform.position = target.position + new Vector3(-distanceAway, distanceAway + 1);
                break;
        }
        LookAtPlayer();
        chasing = true;
        if (targetLostCo != null)
        {
            StopCoroutine( targetLostCo );
            targetLostCo = null;
        }
        if (variant == Variation.boss)
		{
			Vector2 dir = ((target.position + Vector3.up) - transform.position).normalized;
			moveDir = dir;

			// body.AddForce(dir * lickMoveForce, ForceMode2D.Impulse);
		}

		if (hpImg.fillAmount <= 0.5f)
		{
			yield return new WaitForSeconds(2f);
		}
		else
		{
			mainAnim.speed = 1;
			yield return new WaitForSeconds(3);
		}
		mainAnim.SetTrigger("teleport");
    }
    
    public void LICK()
    {
        if (variant == Variation.boss) 
        {
            licking = true;
            LookAtPlayer();
            mainAnim.SetTrigger("lick");
        }
        
    }
    void TELEPORT_SPECIFIC()
    {
        if (isStalker)
        {
            float minDist = Mathf.Infinity;
            float secMinDist = Mathf.Infinity;
            int ind=0;
            int secInd=0;
            for (int i=0 ; i<teleportPos.Length ; i++)
            {
                float dist = Vector2.Distance(teleportPos[i].position, target.position);
                if (dist < minDist)
                {
                    secMinDist = minDist;
                    secInd = ind;
                    minDist = dist;
                    ind = i;
                }
                else if (dist < secMinDist && dist != minDist)
                {
                    secMinDist = dist;
                    secInd = i;
                }
            }
            this.transform.position = teleportPos[secInd].position;
            LookAtTarget();
        }
    }

    public void SHOULD_KEEP_STALKING()
    {
        if (downToHalfHp && variant == Variation.nightShader)
        {
            StopAllCoroutines();
            this.gameObject.SetActive(false);
        }
    }

    public void NIGHT_SHADE()
    {
        if (nightShade != null && target != null)
        {
            for (int i=0 ; i<numNightShade ; i++)
            {
                float xOffset = 0;
                if (i % 2 == 0) // even
                    xOffset = i / 2;
                else            // odd
                    xOffset = -(i + 1) / 2;
                xOffset *= nightShadeXOffset;
                Vector2 origin = new Vector2(target.position.x + xOffset, target.position.y + nightShadeRayHeight);
                RaycastHit2D hit = Physics2D.Raycast(origin, Vector2.down, nightShadeMaxDist, whatIsGround);
                if (hit.collider != null)
                {
                    var obj = Instantiate(nightShade, hit.point, nightShade.transform.rotation, spawnedHolder.transform);
                }
            }
        }
    }


    public IEnumerator SUMMON_WISP()
    {
        if (shootingWisp || !chasing)
            yield break;
        shootingWisp = true;

        wisps.Clear();
        for (int i=0 ; i<wispSpawns.Length ; i++)
        {
            yield return new WaitForSeconds(0.5f);
            var obj = Instantiate(wispAtk, wispSpawns[i].position, wispAtk.transform.rotation, wispSpawns[i].transform);
            wisps.Add(obj);
        }

        if (wisps != null && wisps.Count > 0)
        {
            yield return new WaitForSeconds(1);
            EnemyProjectile[] temp = wisps.ToArray();
            foreach (EnemyProjectile wisp in temp)
            {
                yield return new WaitForSeconds(wispFireDelay);
                if (wisp != null)
                {
                    wisp.transform.parent = null;
                    Vector2 dir = ((target.position + Vector3.up) - wisp.transform.position).normalized;
                    wisp.body.AddForce(dir * wispSpeed, ForceMode2D.Impulse);
                    wisps.Remove(wisp);
                }
            }
        }
        shootingWisp = false;
    }


    IEnumerator TELEPORT_AGAIN()
    {
        yield return new WaitForSeconds(teleportAgain);
        
        mainAnim.speed = 1;
        mainAnim.SetTrigger("teleport");
    }

    public void AGILITY()
    {
        StartCoroutine( ResetBuff(5,5, Stat.spd) );
    }
}

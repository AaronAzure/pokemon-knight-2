using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clefairy : Enemy
{
    // [System.Serializable] class MetronomeAttacks 
    // {
    //     public EnemyProjectile projectile;
    //     public int projectileDmg=10;
    //     public int extraDmg=3;
    //     public int projectileKb=10;
    //     public float duration=0.5f;
    // }
    [System.Serializable] class MetronomeAttacks 
    {
        // public EnemyProjectile projectile;
        public int projectileDmg=10;
        public int extraDmg=3;
        public int projectileKb=10;
        public float duration=0f;
    }

    [Space] [Header("Clefairy")]  
    public bool performingMetronome;
    private Transform target;
    private LayerMask finalMask;
    private Coroutine co;
    [SerializeField] private bool cannotFlip;
    [SerializeField] private bool varyingFlipEvery;
    [SerializeField] private float flipEvery=2;
    private float flipTimer;


    [Space] [Header("Projectiles")]  
    [SerializeField] private Transform atkPos;
    [Space] [SerializeField] private GameObject spawnedHolder;
    [SerializeField] private int nProjectiles=4;

    [Space] [SerializeField] private RazorLeaf razorLeafObj;
    [SerializeField] private MetronomeAttacks razorLeafStat;

    [Space] [SerializeField] private EnemyProjectile sludgeBombObj;
    [SerializeField] private MetronomeAttacks sludgeBombStat;
    
    [Space] [SerializeField] private EnemyAttack poisonPowderObj;
    [SerializeField] private MetronomeAttacks poisonPowderStat;

    [Space] [SerializeField] private EnemyProjectile yawnObj;
    [SerializeField] private MetronomeAttacks yawnStat;
    
    [Space] [SerializeField] private EnemyProjectile nightShadeObj;
    [SerializeField] private MetronomeAttacks nightShadeStat;
    [SerializeField] private int numNightShade=3;
    [SerializeField] private float nightShadeRayHeight=1.5f;
    [SerializeField] private float nightShadeMaxDist=5f;
    [SerializeField] private float nightShadeXOffset=1f;
    
    [Space] [SerializeField] private EnemyProjectile poisonStingObj;
    [SerializeField] private MetronomeAttacks poisonStingStat;
    [SerializeField] private int nPoisonSting;
    
    [Space] [SerializeField] private EnemyProjectile waterGunObj;
    [SerializeField] private MetronomeAttacks waterGunStat;

    // [SerializeField] private MetronomeAttacks[] metronomeAttacks;
    // [Space] [SerializeField] private EnemyProjectile flameThrower;

    public override void Setup()
    {
        if (GameObject.Find("PLAYER") != null)
            target = GameObject.Find("PLAYER").transform;

        if (alert != null) 
            alert.gameObject.SetActive(false);

        finalMask = (whatIsPlayer | whatIsGround);
        
        if (spawnedHolder != null)
            spawnedHolder.transform.parent = null;

        if (varyingFlipEvery)
            flipEvery *= Random.Range(0.75f, 1.25f);

    }
    public override void CallChildOnDeath()
    {
        if (spawnedHolder != null)
            Destroy(spawnedHolder);
    }
    public override void CallChildOnBossDeath()
    {
        if (co != null)
            StopCoroutine(co);
        if (spawnedHolder != null)
            Destroy(spawnedHolder);
		mainAnim.SetTrigger("reset");
    }

    public override void CallChildOnBossFightStart()
	{
		mainAnim.SetTrigger("metronome");
	}


    private void FixedUpdate() 
    {
        if (!isMiniBoss)
        {
            
            // RaycastHit2D groundInfo = Physics2D.Raycast(groundDetection.position, Vector2.down, distanceDetect, whatIsGround);
            // RaycastHit2D frontInfo;
            // if (model.transform.eulerAngles.y > 0)    // right
            //     frontInfo = Physics2D.Raycast(groundDetection.position, Vector2.right, distanceDetect, whatIsGround);
            // else    // left
            //     frontInfo = Physics2D.Raycast(groundDetection.position, Vector2.left, distanceDetect, whatIsGround);


            // if (hp > 0 && !receivingKnockback)
            // {
            //     if (movingLeft)
            //     {
            //         body.velocity = new Vector2(-moveSpeed, body.velocity.y);
            //         model.transform.eulerAngles = new Vector3(0, 0);
            //     }
            //     else if (movingRight)
            //     {
            //         body.velocity = new Vector2(moveSpeed, body.velocity.y);
            //         model.transform.eulerAngles = new Vector3(0, 180);
            //     }
            // }
            if (!performingMetronome && !cannotFlip)
            {
                if (flipTimer < flipEvery)
                    flipTimer += Time.fixedDeltaTime;
                // FLIP
                else
                {
                    Flip();
                    flipTimer = 0;
                }
            }
            else
                LookAtTarget();

            if (!receivingKnockback)
            {
                if (!performingMetronome && playerInField && target != null)
                {
                    Vector3 lineOfSight = (target.position + new Vector3(0, 1)) - (this.transform.position + new Vector3(0, 1));
                    RaycastHit2D playerInfo = Physics2D.Linecast(this.transform.position + new Vector3(0, 1),
                        this.transform.position + new Vector3(0, 1) + lineOfSight, finalMask);

                    if (playerInfo.collider != null && playerInfo.collider.gameObject.CompareTag("Player"))
                    {
                        playerInSight = true;
                        flipTimer = 0;
                        if (alert != null) 
                            alert.gameObject.SetActive(true);
                        performingMetronome = true;
                        mainAnim.SetTrigger("metronome");
                    }
                    else
                    {
                        playerInSight = false;
                        if (alert != null) 
                            alert.gameObject.SetActive(false);
                    }
                }
            }    
        }
        else if (!inCutscene)
        {
            if (performingMetronome)
                LookAtTarget();

            // if (!receivingKnockback)
            // {
			// 	mainAnim.SetTrigger("metronome");
            // }    
        }
    }

    private void Flip()
    {
        if (model.transform.eulerAngles.y != 0) // facing right
            model.transform.eulerAngles = new Vector3(0, 0);
        else                // facing left
            model.transform.eulerAngles = new Vector3(0, 180);
    }
    private void OnDrawGizmosSelected() 
    {
        Gizmos.color = Color.yellow;
        if (playerInField && target != null)
        {
            Vector3 lineOfSight = (target.position + new Vector3(0, 1)) - (this.transform.position + new Vector3(0, 1));
            Gizmos.DrawLine(this.transform.position + new Vector3(0, 1),
                this.transform.position + new Vector3(0, 1) + lineOfSight);
        }
    }
    

    public void METRONOME()
    {
        if (dead)
            return;

        if (isMiniBoss && hp <= 0)
            return;

        int rng = Random.Range(0, nProjectiles);
        switch (rng)
        {
            // Razor Leaf
            case 0:
                LookAtTarget();
                if (razorLeafObj != null)
                {
                    var obj = Instantiate(razorLeafObj, atkPos.position, razorLeafObj.transform.rotation);
                    obj.atkDmg = razorLeafStat.projectileDmg +
                        Mathf.Max(0, razorLeafStat.extraDmg * Mathf.FloorToInt((float)(lv - defaultLv)/2));
                    obj.direction = ProjectileDirection();
                }
                StartCoroutine( Done( razorLeafStat.duration ) );
                break;
            // Sludge Bomb
            case 1:
                LookAtTarget();
                if (sludgeBombObj != null)
                {
                    var obj = Instantiate(sludgeBombObj, atkPos.position, sludgeBombObj.transform.rotation);
                    obj.body.gravityScale = 3;
                    obj.atkDmg = sludgeBombStat.projectileDmg +
                        Mathf.Max(0, sludgeBombStat.extraDmg * Mathf.FloorToInt((float)(lv - defaultLv)/2));
                    float trajectory = CalculateTrajectory();
                    float extraHeight = CalculateExtraHeight();
                    obj.direction = new Vector2(trajectory * -1.1f, Random.Range(10,16) + extraHeight);
                }
                StartCoroutine( Done( sludgeBombStat.duration ) );
                break;
            // Poison Powder
            case 2:
                if (poisonPowderObj != null)
                {
                    var obj = Instantiate(poisonPowderObj, atkPos.position, poisonPowderObj.transform.rotation);
                    obj.atkDmg = poisonPowderStat.projectileDmg +
                        Mathf.Max(0, poisonPowderStat.extraDmg * Mathf.FloorToInt((float)(lv - defaultLv)/2));
                }
                StartCoroutine( Done( poisonPowderStat.duration ) );
                break;
            // Yawn
            case 3:
                LookAtTarget();
                if (yawnObj != null)
                {
                    var obj = Instantiate(yawnObj, atkPos.position, yawnObj.transform.rotation);
                    Vector2 dir = ProjectileDirection();
                    obj.direction = dir;
                }
                StartCoroutine( Done( yawnStat.duration ) );
                break;
            // Night Shade
            case 4:
                LookAtTarget();
                NIGHT_SHADE();
                StartCoroutine( Done( yawnStat.duration ) );
                break;
            // Poison Sting
            case 5:
                LookAtTarget();
                POISON_STING();
                StartCoroutine( Done( yawnStat.duration ) );
                break;
            // Water Gun
            case 6:
                LookAtTarget();
                if (waterGunObj != null)
                {
                    var obj = Instantiate(waterGunObj, atkPos.position, waterGunObj.transform.rotation);
                    obj.atkDmg = waterGunStat.projectileDmg +
                        Mathf.Max(0, waterGunStat.extraDmg * Mathf.FloorToInt((float)(lv - defaultLv)/2));
                    obj.direction = ProjectileDirection();
                }
                StartCoroutine( Done( waterGunStat.duration ) );
                break;
        }
    }

    public void POISON_STING()
    {
        if (poisonStingObj != null && hp > 0 && playerInField)
        {
			// bool playerBelowSelf = (target.position.y < transform.position.y);
            for (int i=0 ; i<nPoisonSting ; i++)
            {
                float offset = 0;
                if (i % 2 == 0) // even
                    offset = i / 2;
                else            // odd
                    offset = -(i + 1) / 2;
                LookAtPlayer();
                Vector2 trajectory = ((target.position + Vector3.up) - atkPos.position).normalized;
                
				if (trajectory.y < 0)
					trajectory *= new Vector2(1 ,0);

                trajectory = Quaternion.Euler(0, 0, 15 * offset) * trajectory;
                // vector = Quaternion.Euler(0, -45, 0) * vector;
                var obj = Instantiate(poisonStingObj, atkPos.position, poisonStingObj.transform.rotation);
                obj.body.gravityScale = 0;
                obj.transform.rotation = Quaternion.LookRotation(trajectory);
                obj.direction = trajectory.normalized;
                obj.atkDmg = projectileDmg + calcExtraProjectileDmg;
            }
        }
    }

    public void NIGHT_SHADE()
    {
        if (nightShadeObj != null && target != null)
        {
            nightShadeObj.atkDmg = nightShadeStat.projectileDmg +
                        Mathf.Max(0, nightShadeStat.extraDmg * Mathf.FloorToInt((float)(lv - defaultLv)/2));
            nightShadeObj.kbForce = nightShadeStat.projectileKb;
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
                    var obj = Instantiate(nightShadeObj, hit.point, nightShadeObj.transform.rotation);
                }
            }
        }
    }

    Vector2 ProjectileDirection()
    {
		Vector2 dir = (playerControls.transform.position + new Vector3(0,2f) - atkPos.position).normalized;
		if (dir.y < 0)
			dir *= new Vector2(1,0);
        return dir;
    }

    IEnumerator Done(float duration)
    {
        if (duration != 0)
            yield return new WaitForSeconds(duration);
        else
            yield return new WaitForEndOfFrame();

        mainAnim.SetTrigger("done");
        performingMetronome = false;

        if (alert != null && !playerInSight) 
            alert.gameObject.SetActive(false);
    }

    private float CalculateTrajectory()
    {
        return (this.transform.position.x - target.position.x);
    }
    private float CalculateExtraHeight()
    {
        return Mathf.Max(0, target.position.y - this.transform.position.y);
    }

}

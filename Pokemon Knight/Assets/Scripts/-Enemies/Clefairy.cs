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


	[Space] [Header("Willo Wisp")]  
    [SerializeField] private GameObject wispHolder;
    [SerializeField] private Transform[] wispSpawns;
    [SerializeField] private EnemyProjectile wispAtk;
    [SerializeField] private List<EnemyProjectile> wisps;
    [SerializeField] private float wispFireDelay=0.8f;
    [SerializeField] private float wispSpeed=7.5f;
    private bool shootingWisp;


    [Space] [Header("Projectiles")]  
    [SerializeField] private Transform atkPos;
    [Space] [SerializeField] private GameObject spawnedHolder;
    [SerializeField] private int nProjectiles=4;
    [SerializeField] private int fixedAtk=-1;

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
    
    [Space] [SerializeField] private EnemyAttack bodySlamObj;
    [SerializeField] private bool performingBodySlam;
    [SerializeField] private float bodySlamForce=20;
    [SerializeField] private MetronomeAttacks bodySlamStat;
    
    [Space] [SerializeField] private EnemyProjectile whirlWindObj;
    [SerializeField] private MetronomeAttacks whirlWindStat;

    [Space] [SerializeField] private GameObject teleportEffect;
    [SerializeField] private GameObject teleportBurstEffect;


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

		if (bodySlamObj != null)
		{
			bodySlamObj.atkDmg = Mathf.RoundToInt(contactDmg * 0.8f);
			bodySlamObj.kbForce = bodySlamStat.projectileKb;
		}
		if (whirlWindObj != null)
		{
			whirlWindObj.atkDmg = whirlWindStat.projectileDmg;
			whirlWindObj.atkDmg += (whirlWindStat.extraDmg * CalculateExtraDmg());
			whirlWindObj.kbForce = whirlWindStat.projectileKb;
		}
		if (wispAtk != null)
        {
            wispAtk.atkDmg = projectileDmg + calcExtraProjectileDmg;
            wispAtk.kbForce = contactKb;
            wisps = new List<EnemyProjectile>();
        }

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

        if (wispHolder != null)
			Destroy(wispHolder);

		StopAllCoroutines();
        if (spawnedHolder != null)
            Destroy(spawnedHolder);
		mainAnim.SetTrigger("reset");
    }

    public override void CallChildOnBossFightStart()
	{
		mainAnim.SetTrigger("metronome");
	}

	public override void CallChildOnHalfHealth()
	{
		if (isMiniBoss)
		{
			mainAnim.SetFloat("mSpeed", 2);
		}
	}





    private void FixedUpdate() 
    {
        if (!isMiniBoss)
        {
			if (performingBodySlam && IsGrounded())
			{
				performingBodySlam = false;
				body.velocity = Vector2.zero;
				if (co != null)
            		StopCoroutine(co);
				co = null;
				StartCoroutine(Done(0));
				bodySlamObj.gameObject.SetActive(false);
				bodySlamObj.gameObject.SetActive(true);
			}
            else if (!performingMetronome && !cannotFlip)
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

            if (performingBodySlam && IsGrounded())
			{
				performingBodySlam = false;
				body.velocity = Vector2.zero;
				if (co != null)
            		StopCoroutine(co);
				co = null;
				StartCoroutine(Done(0));
				bodySlamObj.gameObject.SetActive(false);
				bodySlamObj.gameObject.SetActive(true);
			}
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
		if (fixedAtk >= 0)
			rng = fixedAtk;
        switch (rng)
        {
            // Razor Leaf
            case 0:
                LookAtTarget();
                if (razorLeafObj != null)
                {
                    var obj = Instantiate(razorLeafObj, atkPos.position, razorLeafObj.transform.rotation, spawnedHolder.transform);
                    obj.atkDmg = razorLeafStat.projectileDmg +
                        Mathf.Max(0, razorLeafStat.extraDmg * CalculateExtraDmg());
                    obj.direction = ProjectileDirection();
                }
                StartCoroutine( Done( razorLeafStat.duration ) );
                break;
            // Sludge Bomb
            case 1:
                LookAtTarget();
                if (sludgeBombObj != null)
                {
                    var obj = Instantiate(sludgeBombObj, atkPos.position, sludgeBombObj.transform.rotation, spawnedHolder.transform);
                    obj.body.gravityScale = 3;
                    obj.atkDmg = sludgeBombStat.projectileDmg +
                        Mathf.Max(0, sludgeBombStat.extraDmg * CalculateExtraDmg());
                    float trajectory = CalculateTrajectory();
                    float extraHeight = CalculateExtraHeight();
                    obj.direction = new Vector2(trajectory * 1.1f, Random.Range(10,16) + extraHeight);
                }
                StartCoroutine( Done( sludgeBombStat.duration ) );
                break;
            // Poison Powder
            case 2:
                if (poisonPowderObj != null)
                {
                    var obj = Instantiate(poisonPowderObj, atkPos.position, poisonPowderObj.transform.rotation, spawnedHolder.transform);
                    obj.atkDmg = poisonPowderStat.projectileDmg +
                        Mathf.Max(0, poisonPowderStat.extraDmg * CalculateExtraDmg());
                }
                StartCoroutine( Done( poisonPowderStat.duration ) );
                break;
            // Yawn
            case 3:
                LookAtTarget();
                if (yawnObj != null)
                {
                    var obj = Instantiate(yawnObj, atkPos.position, yawnObj.transform.rotation, spawnedHolder.transform);
                    Vector2 dir = ProjectileDirection();
                    obj.direction = dir;
                }
                StartCoroutine( Done( yawnStat.duration ) );
                break;
            // Night Shade
            case 4:
                LookAtTarget();
                NIGHT_SHADE();
                StartCoroutine( Done( nightShadeStat.duration ) );
                break;
            // Poison Sting
            case 5:
                LookAtTarget();
                POISON_STING();
                StartCoroutine( Done( poisonStingStat.duration ) );
                break;
            // Water Gun
            case 6:
                LookAtTarget();
                if (waterGunObj != null)
                {
                    var obj = Instantiate(waterGunObj, atkPos.position, waterGunObj.transform.rotation, spawnedHolder.transform);
                    obj.atkDmg = waterGunStat.projectileDmg +
                        Mathf.Max(0, waterGunStat.extraDmg * CalculateExtraDmg());
                    obj.direction = ProjectileDirection();
                }
                StartCoroutine( Done( waterGunStat.duration ) );
                break;
            // Body Slam
            case 7:
                LookAtTarget();

                if (bodySlamObj != null)
                {
					float trajectory = CalculateTrajectory();
                    float extraHeight = CalculateExtraHeight();
					body.AddForce(new Vector2(trajectory, bodySlamForce + extraHeight), ForceMode2D.Impulse);
					StartCoroutine(BodySlam());
                }
                co = StartCoroutine( Done( bodySlamStat.duration ) );
                break;
            // WhirlWind
            case 8:
                LookAtTarget();

                if (whirlWindObj != null)
                {
					for (int i=0 ; i<6 ; i++)
					{
						Vector2 trajectory = Vector2.right;
						trajectory = Quaternion.Euler(0, 0, 60 * i) * trajectory;
						var obj = Instantiate(whirlWindObj, this.transform.position, whirlWindObj.transform.rotation, spawnedHolder.transform);
						obj.direction = trajectory.normalized;
					}
                }
                StartCoroutine( Done( whirlWindStat.duration ) );
                break;
            // Will o Wisp
            case 9:
                LookAtTarget();

                StartCoroutine( SUMMON_WISP() );
                StartCoroutine( Done( whirlWindStat.duration ) );
                break;
            // Teleport
            case 10:
				Vector2 origin = target.position + new Vector3(0, 0.5f);
				RaycastHit2D uphit = Physics2D.Raycast(origin, Vector2.up, 7, whatIsGround);
                LookAtTarget();

				// CEILING IS TO0 CLOSE
				if (uphit.collider != null)
				{
					RaycastHit2D lefthit = Physics2D.Raycast(origin, Vector2.left, 4, whatIsGround);
					RaycastHit2D righthit = Physics2D.Raycast(origin, Vector2.right, 4, whatIsGround);
					

					if (lefthit.collider == null && righthit.collider == null)
					{
						int r = Random.Range(0, 2) == 1 ? -1 : 1;
						Teleport((Vector3) origin + 4 * Vector3.right * r);
					}

					else if (lefthit.collider == null)
						Teleport((Vector3) origin + 4 * Vector3.left);

					else if (righthit.collider == null)
						Teleport((Vector3) origin + 4 * Vector3.right);

					else if (Mathf.Abs(lefthit.distance) < Mathf.Abs(righthit.distance))
					{
						Debug.Log("right = " + lefthit.distance + "  :  " + righthit.distance);
						Teleport((Vector3) origin + Mathf.Abs(righthit.distance) * Vector3.right + Vector3.left);
					}
					else
					{
						Debug.Log("left = " + lefthit.distance + "  :  " + righthit.distance);
						Teleport((Vector3) origin + Mathf.Abs(lefthit.distance) * Vector3.left + Vector3.right);
					}

				}
				else
				{
					Teleport((Vector3) origin + 6 * Vector3.up, 0.1f);
				}
				// else
				// {
				// 	if (bodySlamObj != null)
				// 	{
				// 		float trajectory = CalculateTrajectory();
				// 		float extraHeight = CalculateExtraHeight();
				// 		body.AddForce(new Vector2(trajectory, bodySlamForce + extraHeight), ForceMode2D.Impulse);
				// 		StartCoroutine(BodySlam());
				// 	}
				// 	co = StartCoroutine( Done( bodySlamStat.duration ) );
				// }

                break;
        }
    }

	public void Teleport( Vector3 telePos , float delay=0f)
	{
		Instantiate( teleportBurstEffect, this.transform.position, teleportBurstEffect.transform.rotation );
		Instantiate( teleportEffect, this.transform.position, teleportEffect.transform.rotation );
		
		transform.position = telePos;
		Instantiate( teleportBurstEffect, this.transform.position, teleportBurstEffect.transform.rotation );
		Instantiate( teleportEffect, this.transform.position, teleportEffect.transform.rotation );

		StartCoroutine( Done( delay ) );
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
				{
					trajectory *= new Vector2(1, 0);
					if (trajectory.x >= 0)
						trajectory.x = 1;
					else
						trajectory.x = -1;
				}

                trajectory = Quaternion.Euler(0, 0, 15 * offset) * trajectory;
                // vector = Quaternion.Euler(0, -45, 0) * vector;
                var obj = Instantiate(poisonStingObj, atkPos.position, poisonStingObj.transform.rotation, spawnedHolder.transform);
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
                        Mathf.Max(0, nightShadeStat.extraDmg * CalculateExtraDmg());
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
                    var obj = Instantiate(nightShadeObj, hit.point, nightShadeObj.transform.rotation, spawnedHolder.transform);
                }
            }
        }
    }


	public IEnumerator SUMMON_WISP()
    {
        if (shootingWisp)
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

    Vector2 ProjectileDirection()
    {
		Vector2 dir = (playerControls.transform.position + new Vector3(0,2f) - atkPos.position).normalized;
		if (dir.y < 0)
		{
			dir *= new Vector2(1,0);
			if (dir.x >= 0)
				dir.x = 1;
			else
				dir.x = -1;
		}
        return dir;
    }


	IEnumerator BodySlam()
	{
		yield return new WaitForSeconds(0.2f);
		performingBodySlam = true;
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
		
		if (co != null)
			co = null;
    }

    private int CalculateExtraDmg()
    {
		return Mathf.FloorToInt((float)(lv - defaultLv)/2);
    }
    private float CalculateTrajectory()
    {
        return (target.position.x - this.transform.position.x);
    }
    private float CalculateExtraHeight()
    {
        return Mathf.Max(0, target.position.y - this.transform.position.y);
    }

}

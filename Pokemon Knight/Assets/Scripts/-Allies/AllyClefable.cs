using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyClefable : Ally
{
	[System.Serializable] class MetronomeAttacks 
    {
        // public AllyProjectile projectile;
        public int dmg=10;
        public int extraDmg=3;
        public int Kb=10;
        public float duration=0f;
        public int spBonus=5;
    }

	[Space] [Header("Clefable")] 
    [SerializeField] private GameObject ultFlash;
    [SerializeField] private Transform atkPos;
    [SerializeField] private Transform targetPos;
    [Space] [SerializeField] private DetectEnemy detection;
    [Space] [SerializeField] private LayerMask whatIsEnemy;
    [Space] [SerializeField] private LayerMask whatIsGhost;
    [SerializeField] private LayerMask finalMask;
    [Space] public Vector2 quickAtkOffset;
    [SerializeField] private GameObject cannotFind;


	[Space] [Header("Projectiles")]  
	private Coroutine co;
    [SerializeField] private int nProjectiles=4;
    [SerializeField] private int fixedAtk=-1;

    [Space] [SerializeField] private AllyProjectile razorLeafObj;
    [SerializeField] private MetronomeAttacks razorLeafStat;

    [Space] [SerializeField] private AllyProjectile sludgeBombObj;
    [SerializeField] private MetronomeAttacks sludgeBombStat;
    
    [Space] [SerializeField] private AllyAttack poisonPowderObj;
    [SerializeField] private MetronomeAttacks poisonPowderStat;

    [Space] [SerializeField] private AllyProjectile waterGunObj;
    [SerializeField] private MetronomeAttacks waterGunStat;
    
    [Space] [SerializeField] private AllyProjectile poisonStingObj;
    [SerializeField] private MetronomeAttacks poisonStingStat;
    [SerializeField] private int nPoisonSting;
    
    [Space] [SerializeField] private AllyAttack bodySlamObj;
    private bool performingBodySlam;
    [SerializeField] private float bodySlamForce=20;
    [SerializeField] private MetronomeAttacks bodySlamStat;
    
    [Space] [SerializeField] private AllyAttack lavaPlumeObj;
    [SerializeField] private MetronomeAttacks lavaPlumeStat;
    
    [Space] [SerializeField] private AllyProjectile whirlWindObj;
    [SerializeField] private MetronomeAttacks whirlWindStat;
    
	[Space] [SerializeField] private AllyProjectile fireBlastObj;
    [SerializeField] private MetronomeAttacks fireBlastStat;

    [Space] [SerializeField] private AllyBuff healPulse;

    [Space] [SerializeField] private GameObject splash;





    protected override void Setup() 
    {
        finalMask = (whatIsEnemy | whatIsGround | whatIsGhost);
        cannotFind.SetActive(false);
        
        if (useUlt && anim != null)
        {
            outTime = ultOutTime;
            // anim.SetTrigger("ult");
			if (ultFlash != null)
				ultFlash.SetActive(true);
			anim.SetFloat("mSpeed", anim.GetFloat("mSpeed") * 1.5f);
        }
        else
        {
            StartCoroutine( Detect() );
        }

		if (bodySlamObj != null)
		{
			bodySlamObj.atkDmg = bodySlamStat.dmg + GetExtraDmg(bodySlamStat.extraDmg);
			bodySlamObj.atkForce = bodySlamStat.Kb;
			bodySlamObj.spBonus = bodySlamStat.spBonus;
		}
    }

    protected override void OnSecondEvolution()
    {
		anim.SetFloat("mSpeed", 2f);
    }
    protected override void OnThirdEvolution()
    {
		anim.SetFloat("mSpeed", 3f);
    }

    public override void CallChildIsGrounded() 
    {
        // RaycastHit2D groundInfo = Physics2D.Raycast(this.transform.position, Vector2.down, feetRadius, whatIsGround);
		bool groundInfo = Physics2D.OverlapBox(this.transform.position + (Vector3)feetOffset, feetBox, 0, whatIsGround);
        if (groundInfo && body != null && body.velocity.y <= 0)
        {
			if (performingBodySlam)
			{
				performingBodySlam = false;
				body.velocity = Vector2.zero;
				// CallChildOnLanded();
				if (co != null)
					StopCoroutine(co);
				co = null;
				StartCoroutine(Done(0.5f));
				bodySlamObj.gameObject.SetActive(true);
			}
			else
			{
				body.velocity = Vector2.zero;
				// CallChildOnLanded();
			}
        }
    }

	void METRONOME()
	{
		if (!useUlt)
		{
        int rng = Random.Range(0, nProjectiles);
		if (fixedAtk >= 0)
			rng = fixedAtk;
        switch (rng)
        {
            // Razor Leaf
            case 0:
                ClosestEnemy();
				LookAtTarget();
                if (razorLeafObj != null)
                {
                    var obj = Instantiate(razorLeafObj, atkPos.position, razorLeafObj.transform.rotation);
					obj.spawnedPos = atkPos.position;
                    obj.atkDmg = razorLeafStat.dmg + GetExtraDmg(razorLeafStat.extraDmg);
                    obj.body.velocity = ProjectileDirection() * obj.velocity;
					obj.spBonus = razorLeafStat.spBonus;
					Debug.Log("METRONOME  =  Razor Leaf ("+obj.atkDmg+")");
                }
                StartCoroutine( Done( razorLeafStat.duration ) );
                break;
            // Sludge Bomb
            case 1:
                ClosestEnemy();
				LookAtTarget();
                if (sludgeBombObj != null)
                {
                    var obj = Instantiate(sludgeBombObj, atkPos.position, sludgeBombObj.transform.rotation);
					obj.spawnedPos = atkPos.position;
                    obj.body.gravityScale = 3;
                    obj.atkDmg = sludgeBombStat.dmg + GetExtraDmg(sludgeBombStat.extraDmg);
                    obj.body.velocity = (FacingRight() ? new Vector2(13, 12) : new Vector2(-13, 12));
					obj.spBonus = sludgeBombStat.spBonus;
					Debug.Log("METRONOME  =  Sludge Bomb ("+obj.atkDmg+")");
                }
                StartCoroutine( Done( sludgeBombStat.duration ) );
                break;
            // Poison Powder
            case 2:
                if (poisonPowderObj != null)
                {
                    var obj = Instantiate(poisonPowderObj, atkPos.position, poisonPowderObj.transform.rotation);
                    obj.atkDmg = poisonPowderStat.dmg + GetExtraDmg(poisonPowderStat.extraDmg);
					obj.spBonus = poisonPowderStat.spBonus;
					Debug.Log("METRONOME  =  Poison Powder ("+obj.atkDmg+")");
                }
                StartCoroutine( Done( poisonPowderStat.duration ) );
                break;
            // Water Gun
            case 3:
                ClosestEnemy();
				LookAtTarget();
                if (waterGunObj != null)
                {
                    var obj = Instantiate(waterGunObj, atkPos.position, waterGunObj.transform.rotation);
					obj.spawnedPos = atkPos.position;
                    obj.atkDmg = waterGunStat.dmg + GetExtraDmg(waterGunStat.extraDmg);
					obj.direction = ProjectileDirection();
					obj.body.velocity = ProjectileDirection() * obj.velocity;
                    // obj.velocity *= (FacingRight() ? 1 : -1);
					obj.spBonus = waterGunStat.spBonus;
					Debug.Log("METRONOME  =  Water Gun ("+obj.atkDmg+")");
                }
                StartCoroutine( Done( waterGunStat.duration ) );
                break;

            // Poison Sting
            case 4:
                ClosestEnemy();
				LookAtTarget();
                PoisonSting();
                StartCoroutine( Done( poisonStingStat.duration ) );
                break;

            // Body Slam
            case 5:
				Debug.Log("METRONOME  =  Body Slam");
                ClosestEnemy();
				LookAtTarget();
				
                if (bodySlamObj != null)
                {
					float trajectory = 0;
					if (targetPos != null)
						trajectory = targetPos.position.x - this.transform.position.x;
                    // float extraHeight = CalculateExtraHeight();
					body.AddForce(new Vector2(trajectory, bodySlamForce), ForceMode2D.Impulse);
					StartCoroutine(BodySlam());
                }
                co = StartCoroutine( Done( bodySlamStat.duration ) );
                break;

            // Lava Plume
            case 6:
				Debug.Log("METRONOME  =  Lava Plume");

                if (lavaPlumeObj != null)
                {
					lavaPlumeObj.atkDmg = lavaPlumeStat.dmg + GetExtraDmg(lavaPlumeStat.extraDmg);
					lavaPlumeObj.atkForce = lavaPlumeStat.Kb;
					lavaPlumeObj.spBonus = lavaPlumeStat.spBonus;
					lavaPlumeObj.gameObject.SetActive(true);
                }
                StartCoroutine( Done( lavaPlumeStat.duration ) );
                break;

            case 7:
				Debug.Log("METRONOME  =  WhirlWind");

                if (whirlWindObj != null)
                {
					whirlWindObj.atkDmg = whirlWindStat.dmg + GetExtraDmg(whirlWindStat.extraDmg);
					whirlWindObj.atkForce = whirlWindStat.Kb;
					whirlWindObj.spBonus = whirlWindStat.spBonus;
					for (int i=0 ; i<6 ; i++)
					{
						Vector2 trajectory = Vector2.right;
						trajectory = Quaternion.Euler(0, 0, 60 * i) * trajectory;
						var obj = Instantiate(whirlWindObj, atkPos.transform.position, whirlWindObj.transform.rotation);
						obj.spawnedPos = atkPos.position;
						obj.direction = trajectory.normalized;
					}
                }
                StartCoroutine( Done( whirlWindStat.duration ) );
                break;

            // Heal Pulse
            case 8:
				Debug.Log("METRONOME  =  Heal Pulse");
                ClosestEnemy();
				if (targetPos != null && healPulse != null)
					Instantiate(healPulse, atkPos.position, healPulse.transform.rotation);
				else if (splash != null)
					splash.SetActive(true);
                StartCoroutine( Done( 0 ) );
                break;

            // Fire Blast
            case 9:
				// Debug.Log("METRONOME  =  Will o Wisp");
                ClosestEnemy();
				LookAtTarget();
				if (fireBlastObj != null)
                {
					fireBlastObj.atkDmg = fireBlastStat.dmg + GetExtraDmg(fireBlastStat.extraDmg);
					fireBlastObj.atkForce = fireBlastStat.Kb;
					fireBlastObj.spBonus = fireBlastStat.spBonus;
					var obj = Instantiate(fireBlastObj, atkPos.transform.position, fireBlastObj.transform.rotation);
					obj.spawnedPos = atkPos.position;
					obj.direction = ProjectileDirection();
					obj.body.velocity = ProjectileDirection() * obj.velocity;
					// obj.velocity *= (FacingRight() ? 1 : -1);
                }
                // StartCoroutine( SUMMON_WISP() );
                StartCoroutine( Done( fireBlastStat.duration ) );
                break;
			default:
				if (splash != null)
					splash.SetActive(true);
				StartCoroutine( Done( 0 ) );
                break;

        }

		}
		else
		{
			if (sludgeBombObj != null)
			{
				sludgeBombObj.atkDmg = sludgeBombStat.dmg + GetExtraDmg(sludgeBombStat.extraDmg);
				sludgeBombObj.body.gravityScale = 3;
				sludgeBombObj.spBonus = 0;
				for (int i=0 ; i<7 ; i++)
				{
					float offset = ((i % 2 == 0) ? (i / 2) : (-(i + 1) / 2));

					var obj = Instantiate(sludgeBombObj, atkPos.position, sludgeBombObj.transform.rotation);
					obj.spawnedPos = atkPos.position;
					obj.body.velocity = new Vector2(offset * 5, 15);
				}
			}
			if (whirlWindObj != null)
			{
				whirlWindObj.atkDmg = whirlWindStat.dmg + GetExtraDmg(whirlWindStat.extraDmg);
				whirlWindObj.atkForce = whirlWindStat.Kb;
				whirlWindObj.spBonus = 0;
				for (int i=0 ; i<6 ; i++)
				{
					Vector2 trajectory = Vector2.right;
					trajectory = Quaternion.Euler(0, 0, 60 * i) * trajectory;
					var obj = Instantiate(whirlWindObj, atkPos.transform.position, whirlWindObj.transform.rotation);
					obj.spawnedPos = atkPos.position;
					obj.direction = trajectory.normalized;
				}
			}
			if (lavaPlumeObj != null)
			{
				lavaPlumeObj.atkDmg = lavaPlumeStat.dmg + GetExtraDmg(lavaPlumeStat.extraDmg);
				lavaPlumeObj.atkForce = lavaPlumeStat.Kb;
				lavaPlumeObj.spBonus = 0;
				lavaPlumeObj.gameObject.SetActive(true);
			}
			StartCoroutine( Done( lavaPlumeStat.duration ) );
		}
	}
	IEnumerator Done(float duration)
    {
        if (duration != 0)
            yield return new WaitForSeconds(duration);
        else
            yield return new WaitForEndOfFrame();

        anim.SetTrigger("done");
    }


	public void PoisonSting()
    {
        if (poisonStingObj && targetPos != null)
        {
			poisonStingObj.atkDmg = poisonPowderStat.dmg + GetExtraDmg(poisonPowderStat.extraDmg);
			poisonStingObj.spBonus = poisonPowderStat.spBonus;
			Debug.Log("METRONOME  =  Poison Sting (" + poisonStingObj.atkDmg + ")");

			// bool playerBelowSelf = (target.position.y < transform.position.y);
            for (int i=0 ; i<nPoisonSting ; i++)
            {
                float offset = 0;
                if (i % 2 == 0) // even
                    offset = i / 2;
                else            // odd
                    offset = -(i + 1) / 2;
                LookAtTarget();
                Vector2 trajectory = ((targetPos.position + Vector3.up) - atkPos.position).normalized;
                
				// if (trajectory.y < 0)
				// {
				// 	trajectory *= new Vector2(1, 0);
				// 	if (trajectory.x >= 0)
				// 		trajectory.x = 1;
				// 	else
				// 		trajectory.x = -1;
				// }

                trajectory = Quaternion.Euler(0, 0, 15 * offset) * trajectory;
                // vector = Quaternion.Euler(0, -45, 0) * vector;
                var obj = Instantiate(poisonStingObj, atkPos.position, poisonStingObj.transform.rotation);
                obj.body.gravityScale = 0;
                obj.transform.rotation = Quaternion.LookRotation(trajectory);
                obj.direction = trajectory.normalized;
                obj.body.velocity = trajectory.normalized;
                // obj.atkDmg = poisonPowderStat.dmg + GetExtraDmg(poisonPowderStat.extraDmg);
            }
        }
    }

	IEnumerator BodySlam()
	{
		yield return new WaitForSeconds(0.1f);
		performingBodySlam = true;
		hitbox.gameObject.SetActive(true);
	}


    IEnumerator Detect()
    {
        yield return new WaitForSeconds(0.1f);
        ClosestEnemy();
    }



	void LookAtTarget()
	{
		if (targetPos != null)
		{
			model.transform.eulerAngles = 
				(targetPos.position.x - this.transform.position.x < 0) 
				? new Vector3(0,180) : new Vector3(0,0);
		}
	}
    private void ClosestEnemy()
    {
        if (detection == null)
        {
            CannotFindTarget();
            return;
        }
        
        float distance = Mathf.Infinity;
        List<Transform> enemies = detection.detected;

        if (enemies == null || enemies.Count == 0)
        {
            CannotFindTarget();
            return;
        }
        
        int ind = -1;
        for (int i=0 ; i<enemies.Count ; i++)
        {
			if (enemies[i] != null)
			{
				float distToSelf = Mathf.Abs(Vector2.Distance(atkPos.position, enemies[i].position));
				if (distToSelf < distance && EnemyInLineOfSight(enemies[i]))
				{
					distance = distToSelf;
					ind = i;
				}
			}
        }
        if (ind == -1)
        {
            CannotFindTarget();
            return;
        }

        targetPos = enemies[ind];
    }
    private bool EnemyInLineOfSight(Transform target)
    {
        RaycastHit2D sightInfo = Physics2D.Linecast(this.transform.position + new Vector3(0, 1),
            target.position + new Vector3(0, 0.3f), finalMask);
        return (sightInfo.collider != null && sightInfo.collider.gameObject.CompareTag("Enemy"));
    }


    private void CannotFindTarget()
    {
        cannotFind.SetActive(true);
    }


	Vector2 ProjectileDirection()
	{
		if (targetPos == null)
			return FacingRight() ? Vector2.right : Vector2.left;
		return (targetPos.position - atkPos.position).normalized;
	}

	private int GetExtraDmg(int extraDmg, int perLv=1)
    {
		return Mathf.Max(0, Mathf.FloorToInt((float)(trainerBonusLv + ExtraEnhancedDmg()) / perLv) * extraDmg);
    }
}

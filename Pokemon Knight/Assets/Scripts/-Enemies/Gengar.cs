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
    public Transform target;
    public bool chasing;
    public bool teleporting;
    private int teleportCount=0;
    private int nTeleport=2;
    public float distanceAway=3;


	[Space] [Header("Attacks")]
	public bool usingNightShade;	//* SET BY ANIMATION
	public bool usingLick;			//* SET BY ANIMATION
	public bool usingShadowBall;	//* SET BY ANIMATION


    [Space] [Header("Licker")]  
    public bool licking;
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


    [Space] [Header("Shadow Ball")]  
    [SerializeField] private Transform shadowBallPos;
    [SerializeField] private EnemyProjectile shadowBallAtk;
    private int nShadowBalls;
    private int shadowBallBurst=3;


    [Space] [Header("Shadow Attack")]  
    [SerializeField] private Transform shadowAtkPos;
    [SerializeField] private EnemyProjectile shadowAtk;
    [SerializeField] private bool shadowCo;
    [SerializeField] private int nShadows=5;
    [SerializeField] private GameObject currentShadow;
    [SerializeField] private float shadowDuration=12.5f;

    
    [Space] [Header("Teleport")]
    [SerializeField] private Transform[] teleportPos;

    

    [Space] [Header("Misc")]  
    // [SerializeField] private GameObject glint;
	private Vector3 startPos;
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private bool variantSpeed;
	[SerializeField] private GameObject shadow;
	[SerializeField] private GameObject shadowLonger;
	[SerializeField] private GameObject shadowTrail;
	private int atkCount;
    public Vector3 lineOfSight;
    [SerializeField] private GameObject spawnedHolder;
    private Coroutine co;
    [SerializeField] private Coroutine targetLostCo;
    private RaycastHit2D playerInfo;
    public bool sceneLoaded=false;
	private bool isActuallyBoss=false;


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
                Destroy(this.gameObject);
        else
            PlayerPrefsElite.SetBoolean("caughtGengar" + gameNumber, false);

        // origAtkDmg = contactDmg;


        if (lickAtk != null)
        {
            lickAtk.atkDmg = secondDmg;
            lickAtk.kbForce = contactKb;
        }
        if (nightShade != null)
        {
            nightShade.atkDmg = secondDmg;
            nightShade.kbForce = contactKb;
        }
        if (variant == Variation.nightShader)
        {
            // cannotRecieveKb = true;
            mainAnim.SetBool("isStalking", true);
            isStalker = true;
        }
		else
		{
			inCutscene = true;
			isSmart = false;
			isActuallyBoss = true;
			mainAnim.speed = 0;
			mainAnim.Play("gengar-intro-anim", -1, 0);
			startPos = new Vector3(transform.position.x, transform.position.y + 2);
		}
        if (shadowBallAtk != null)
		{
			shadowBallAtk.atkDmg = projectileDmg + calcExtraProjectileDmg;
		}
		if (shadowAtk != null)
		{
			shadowAtk.atkDmg = projectileDmg + calcExtraProjectileDmg;
		}
		
		if (this.enabled == true)
			StartCoroutine( LoadingIn() );
    }


	private void OnEnable() 
	{
		LookAtTarget();	
	}
    IEnumerator LoadingIn()
    {
        yield return new WaitForSeconds(0.5f);
        sceneLoaded = true;
    }

    public override void CallChildOnRoar()
	{
		mainAnim.speed = 1;
		mainAnim.Play("gengar-intro-anim", -1, 0);
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
    public override void CallChildOnBossDeath()
    {
        mainAnim.speed = 0;
        mainAnim.SetTrigger("reset");
        StopAllCoroutines();
		Destroy(spawnedHolder);
		Destroy(shadowBallPos.gameObject);
		if (shadowTrail != null)
			shadowTrail.SetActive(false);

		this.gameObject.layer = LayerMask.NameToLayer("Enemy");
		// this.gameObject.layer = enemyLayer;
		this.transform.position = startPos;
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
        else if (!isActuallyBoss)
        {
            // if (lockPos != null)
            //     transform.position = (Vector2) lockPos.position + lockOffset;
            
            // LookAtTarget();
        }
        else if (isActuallyBoss)
        {
			if ((licking || usingLick) && !receivingKnockback)
			{
				licking = true;
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
	public void NEXT_ATTACK(int specific=-1)
	{
		if (!isActuallyBoss && !inCutscene)
			return;

		if (!shadowTrail.activeSelf && currentShadow == null)
			shadowTrail.SetActive(true);

		int rng = Random.Range(0,5);
		if (downToHalfHp || hpImg.fillAmount <= 0.5f)
		{
			downToHalfHp = true;
			// rng = Random.Range(0,6);
			if (!shadowCo)
				rng = 5;
		}
		if (specific != -1)
			rng = specific;

		licking = false;
		mainAnim.SetBool("usingShadowBall", false);
		mainAnim.SetBool("usingNightShade", false);
		mainAnim.SetBool("usingShadows", false);
		mainAnim.SetBool("usingShadowBallBurst", false);

		switch (rng)
		{
			//* NIGHT SHADE
			case 0:
				teleporting = false;
				moveDir = Vector2.zero;
				mainAnim.SetTrigger("teleport");
				mainAnim.SetBool("usingNightShade", true);
				if (hpImg.fillAmount <= 0.5f)
					numNightShade = 9;
				break;

			//* LICK
			case 1:
				teleporting = true;
				mainAnim.SetTrigger("teleport");
				if (hpImg.fillAmount <= 0.5f)
					mainAnim.SetFloat("lickSpeed", 1.5f);
				break;
			
			case 2:
				if (hpImg.fillAmount <= 0.5f)
					nShadowBalls = 1;
				teleporting = false;
				moveDir = Vector2.zero;
				mainAnim.SetTrigger("teleport");
				mainAnim.SetBool("usingShadowBallBurst", true);
				break;
				
			case 5:
				teleporting = false;
				moveDir = Vector2.zero;
				mainAnim.SetTrigger("teleport");
				mainAnim.SetBool("usingShadows", true);
				break;

			//* SHADOW BALL
			default:
				nShadowBalls = 1;
				if (hpImg.fillAmount <= 0.5f)
					nShadowBalls = 3;
				teleporting = false;
				moveDir = Vector2.zero;
				mainAnim.SetTrigger("teleport");
				mainAnim.SetBool("usingShadowBall", true);
				break;
		}
	}

	public IEnumerator SHADOW_BALL(float delay)
	{
		if (shadowBallAtk != null)
		{
			yield return new WaitForSeconds(delay);
			EnemyProjectile[] objs = new EnemyProjectile[nShadowBalls];
			
			for (int i=0 ; i<nShadowBalls ; i++)
			{
				var obj = Instantiate(shadowBallAtk, 
					shadowBallPos.position, shadowBallAtk.transform.rotation, spawnedHolder.transform);
				// obj.transform.parent = shadowBallPos;
				obj.atkDmg = projectileDmg + calcExtraProjectileDmg;

				objs[i] = obj;
				yield return new WaitForSeconds(0.25f);
			}
			
			yield return new WaitForSeconds(1f - (0.25f * nShadowBalls));	LookAtTarget();
			// yield return new WaitForSeconds(0.25f);	LookAtTarget();
			
			int temp = nShadowBalls;
			for (int i=0 ; i<temp ; i++)
			{
				if (objs[i] != null)
				{
					yield return new WaitForSeconds(0.25f); LookAtTarget();
					Vector2 trajectory = ((target.position + Vector3.up) - objs[i].transform.position).normalized;
					objs[i].LaunchAt( trajectory );
					
					nShadowBalls--;
					if (nShadowBalls > 0)
					{
						mainAnim.Play("gengar-shadow-ball-anim", -1, 0.625f);
					}
				}
			}
			
		}
	}
	
	public IEnumerator SHADOW_BALL_BURST(float delay)
	{
		if (shadowBallAtk != null)
		{
			yield return new WaitForSeconds(delay);
			EnemyProjectile[] objs = new EnemyProjectile[shadowBallBurst];
			
			for (int i=0 ; i<shadowBallBurst ; i++)
			{
				var obj = Instantiate(shadowBallAtk, 
					shadowBallPos.position, shadowBallAtk.transform.rotation, spawnedHolder.transform);
				// obj.transform.parent = shadowBallPos;
				obj.atkDmg = projectileDmg + calcExtraProjectileDmg;

				objs[i] = obj;
			}
			
			yield return new WaitForSeconds(1f);	LookAtTarget();
			yield return new WaitForSeconds(0.25f);	LookAtTarget();
			
			for (int i=0 ; i<shadowBallBurst ; i++)
			{
				if (objs[i] != null)
				{
					float offset = 0;
					if (i % 2 == 0) // even
						offset = i / 2;
					else            // odd
						offset = -(i + 1) / 2;
					Vector2 trajectory = ((target.position + Vector3.up) - objs[i].transform.position).normalized;

					trajectory = Quaternion.Euler(0, 0, 30 * offset) * trajectory;
					objs[i].LaunchAt( trajectory );
				}
			}
			
			if (nShadowBalls > 0)
			{
				nShadowBalls--;
				mainAnim.Play("gengar-shadow-ball-burst-anim", -1, 0);
			}
		}
	}

    public IEnumerator TELEPORT()
    {   
        if (target == null || hp <= 0)
            yield break;
        
        body.velocity = Vector2.zero;


        int rng = Random.Range(1,5);

		if (!shadowTrail.activeSelf && currentShadow == null)
			shadowTrail.SetActive(true);

		// if (downToHalfHp)

        switch (rng)  // 5
        {
			// ABOVE
            case 0:
                this.transform.position = target.position + new Vector3(0, distanceAway + 1);
                break;
			// RIGHT
            case 1:
                this.transform.position = target.position + new Vector3(distanceAway, 1);
                break;
			// LEFT
            case 2:
                this.transform.position = target.position + new Vector3(-distanceAway, 1);
                break;
			// TOP RIGHT
            case 3:
                this.transform.position = target.position + new Vector3(distanceAway, distanceAway + 1);
                break;
            case 4:
			// TOP LEFT
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
        if (isActuallyBoss)
		{
			Vector2 dir = ((target.position + Vector3.up) - transform.position).normalized;
			moveDir = dir;
		}

		if (hpImg.fillAmount <= 0.5f)
		{
			if (mainAnim.GetBool("usingShadows"))
			{
				this.transform.position = startPos;
				mainAnim.SetBool("stillTeleporting", false);
				teleportCount = 0;
				nTeleport = Random.Range(0,3);
			}
			// STOP TELEPORTING
			else if (teleportCount >= nTeleport)
			{
				// // if (rng != 0)
				// 	Instantiate(shadowLonger, target.position + new Vector3(0, distanceAway + 1), 
				// 				Quaternion.Euler(0,0,0), spawnedHolder.transform); 
				// // if (rng != 1)
				// 	Instantiate(shadowLonger, target.position + new Vector3(distanceAway, 1), 
				// 				Quaternion.Euler(0,0,0), spawnedHolder.transform); 
				// // if (rng != 2)
				// 	Instantiate(shadowLonger, target.position + new Vector3(-distanceAway, 1), 
				// 				Quaternion.Euler(0,180,0), spawnedHolder.transform); 
				// // if (rng != 3)
				// 	Instantiate(shadowLonger, target.position + new Vector3(distanceAway, distanceAway + 1), 
				// 				Quaternion.Euler(0,0,0), spawnedHolder.transform); 
				// // if (rng != 4)
				// 	Instantiate(shadowLonger, target.position + new Vector3(-distanceAway, distanceAway + 1), 
				// 				Quaternion.Euler(0,180,0), spawnedHolder.transform); 

				mainAnim.SetBool("stillTeleporting", false);
				teleportCount = 0;
				nTeleport = Random.Range(0,3);
			}
			// TELEPORT AGAIN
			else
			{
				// // if (rng != 0)
				// 	Instantiate(shadow, target.position + new Vector3(0, distanceAway + 1), 
				// 				Quaternion.Euler(0,0,0), spawnedHolder.transform); 
				// // if (rng != 1)
				// 	Instantiate(shadow, target.position + new Vector3(distanceAway, 1), 
				// 				Quaternion.Euler(0,0,0), spawnedHolder.transform); 
				// // if (rng != 2)
				// 	Instantiate(shadow, target.position + new Vector3(-distanceAway, 1), 
				// 				Quaternion.Euler(0,180,0), spawnedHolder.transform); 
				// // if (rng != 3)
				// 	Instantiate(shadow, target.position + new Vector3(distanceAway, distanceAway + 1), 
				// 				Quaternion.Euler(0,0,0), spawnedHolder.transform); 
				// // if (rng != 4)
				// 	Instantiate(shadow, target.position + new Vector3(-distanceAway, distanceAway + 1), 
				// 				Quaternion.Euler(0,180,0), spawnedHolder.transform); 
				teleportCount++;
				mainAnim.SetBool("stillTeleporting", true);
				yield return new WaitForSeconds(0.5f);
				mainAnim.SetTrigger("teleport");
			}
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
        if (downToHalfHp && !isActuallyBoss)
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

	IEnumerator SHADOWS()
	{
		shadowCo = true;
		if (shadowAtk != null)
		{
			LookAtTarget();
			var obj = Instantiate(shadowAtk, shadowAtkPos.position, Quaternion.identity, spawnedHolder.transform);
			obj.target = this.target;
			obj.direction = (target.position + new Vector3(0,0.5f) - this.transform.position).normalized;
			currentShadow = obj.gameObject;
			shadowTrail.SetActive(false);

			// bool left = (model.transform.eulerAngles.y == 0);	//
			// List<int> temp = new List<int>();
			// for (int i=0 ; i<11 ; i++)
			// 	temp.Add(i * 30);
			// for (int i=0 ; i<nShadows ; i++)
			// {
			// 	var obj = Instantiate(shadowAtk, shadowAtkPos.position, Angle(left), spawnedHolder.transform);
			// 	int ind = Random.Range(0, temp.Count);
			// 	obj.direction = Quaternion.Euler(0,0, temp[ind]) * Vector2.right;
			// 	temp.RemoveAt(ind);

			// }
		}
		mainAnim.SetBool("usingShadows", false);
		yield return new WaitForSeconds( shadowDuration );
		shadowCo = false;
	}

	Quaternion Angle(bool facingLeft)
	{
		return facingLeft ? Quaternion.Euler(0,0,0) : Quaternion.Euler(0,180,0);
	}

}

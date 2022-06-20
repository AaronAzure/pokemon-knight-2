using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alakazam : Enemy
{
	[System.Serializable] class Damager
	{
		public Enemy enem;
		public int dmg=10;
		public float kb=10;
	}

	[Header("Alakazam")]
	public float moveSpeed=3;
	public float maxSpeed=6;
	public Transform target;
	public GameObject spawnedHolder;

	public EnemyProjectile psybeamAtk;
	public EnemyProjectile psybeamSlowAtk;
	public EnemyProjectile psychoCutAtk;
	public Transform[] teleportPos;

	[Space]
	public GameObject teleportBurstEffect;
	public GameObject teleportEffect;
	private int lastTpInd=-1;



	[Space] [Header("Testing")]
	public int specific = -1;
	public Transform atkPos;
	public Transform atkPos2;
	public bool usePsychoCut;
	public Vector3 psychoCutUp = Vector3.up;
	public ParticleSystem[] ps;


	[Space] [Header("Psychic")]
	public Transform[] spawnPos;
	[Space] [SerializeField] private Damager book;
	[Space] [SerializeField] private Damager plank;
	[Space] [SerializeField] private Damager chair;
	public int nBooks;
	public float delay=5;
	public float bookSpeed=10;
	public bool moving=false;
	public Vector2 dest;
	private List<Enemy> telekinetic = new List<Enemy>();
	private int atkCount;
	[Space] public int mustTeleport=3;
	private bool teleported;




    // Start is called before the first frame update
    public override void Setup()
    {
        if (spawnedHolder != null) 
            spawnedHolder.transform.parent = null;
        if (target == null)
            target = playerControls.transform;

		if (psybeamAtk != null)
		{
			psybeamAtk.chase.target = this.target;
			psybeamAtk.atkDmg = projectileDmg + calcExtraProjectileDmg;
		}
		if (psybeamSlowAtk != null)
		{
			psybeamSlowAtk.chase.target = this.target;
			psybeamSlowAtk.atkDmg = projectileDmg + calcExtraProjectileDmg;
		}
		if (psychoCutAtk != null)
		{
			psychoCutAtk.atkDmg = secondDmg;
		}
		if (book != null)
		{
			book.enem.playerControls	= this.playerControls;
			book.enem.contactDmg		= book.dmg;
			book.enem.contactKb			= book.kb;
		}
		if (plank != null)
		{
			plank.enem.playerControls	= this.playerControls;
			plank.enem.contactDmg		= plank.dmg;
			plank.enem.contactKb		= plank.kb;
		}
		if (chair != null)
		{
			chair.enem.playerControls	= this.playerControls;
			chair.enem.contactDmg		= chair.dmg;
			chair.enem.contactKb		= chair.kb;
		}
		// StartCoroutine( Testing() );
    }


	public override void CallChildOnRoar()
    {
        //* CALLED ONCE
		possessedAura.SetActive(true);
        if (mainAnim != null)
		{
            mainAnim.SetTrigger("attacked");
		}
		foreach (ParticleSystem p in ps)
		{
			p.Stop(true, ParticleSystemStopBehavior.StopEmitting);
		}
    }

	public override void CallChildOnBossFightStart()
	{
		StartCoroutine( ConstantPsychic() );
	}

	public override void CallChildOnRage()
	{
		downToHalfHp = true;
		mainAnim.SetTrigger("done");
		body.velocity = Vector2.zero;
		nBooks = 8;
		moving = false;
		mustTeleport = 2;
		if (mainAnim != null)
		{
			mainAnim.SetTrigger("rage");
			mainAnim.SetBool("inRage", true);
		}
		StopAllCoroutines();
		Unflash();

		foreach (Enemy obj in telekinetic)
			if (obj != null)
				obj.CallChildOnDeactivate();
		telekinetic.Clear();
	}

	public override void CallChildOnRageCutsceneFinished()
	{
		atkCount = mustTeleport;// TELEPORT
		StartCoroutine( ConstantPsychic(2) );
	}

	public override void CallChildOnBossDeath()
    {
        mainAnim.speed /= 2;
        mainAnim.SetTrigger("reset");

		foreach (Enemy obj in telekinetic)
		{
			if (obj != null)
			{
				obj.transform.parent = null;
				obj.CallChildOnDeactivate();
			}
		}
		telekinetic.Clear();

        StopAllCoroutines();
		body.gravityScale = 3;
		Destroy(spawnedHolder);
    }
	





    
    void FixedUpdate()
    {
        if (!inCutscene && moving)
		{
			Vector2 dir = (target.position + new Vector3(0,1.5f) - this.transform.position).normalized;
			body.AddForce(dir * 5 * moveSpeed * Time.fixedDeltaTime, ForceMode2D.Impulse);
			body.velocity = Vector2.ClampMagnitude(body.velocity, maxSpeed);
			if (PlayerIsToTheLeft() && body.velocity.x < 0)
				LookAtTarget();
			else if (!PlayerIsToTheLeft() && body.velocity.x > 0)
				LookAtTarget();
		}
    }


	public void NEXT_ATTACK()
	{
		if (inCutscene)
			return;

		// if (!shadowTrail.activeSelf && currentShadow == null)
		// 	shadowTrail.SetActive(true);

		int rng = Random.Range(0,5);

		if (specific != -1)
			rng = specific;

		// AFTER TELEPORTING (ATTACK)
		if (atkCount > mustTeleport)
		{
			rng = Random.Range(1,3);	// NOT PSYCHO CUT
			atkCount = 0;
		}
		else if (atkCount == mustTeleport)
		{
			rng = -1;
		}

		LookAtTarget();

		switch (rng)
		{
			//* PSYCHO CUT
			case 0:
				mainAnim.SetTrigger("psychoCut");
				
				atkCount++;
				if (teleported)
				{
					teleported = false;
					atkCount = mustTeleport;
				}
				break;

			//* PSYBEAM
			case 1:
				mainAnim.SetTrigger("psybeamSlow");
				
				atkCount++;
				if (teleported)
				{
					teleported = false;
					atkCount = mustTeleport;
				}
				break;

			//* PSYCHIC
			case 2:
				mainAnim.SetTrigger("psychic");
				
				atkCount++;
				if (teleported)
				{
					teleported = false;
					atkCount = mustTeleport;
				}
				break;

			//* MOVE
			case 3:
				mainAnim.SetTrigger("move");
				atkCount++;
				StartCoroutine(MOVE());
				break;

			//* TELEPORT CLOSE
			case 4:
				TELEPORT();
				teleported = true;
				break;

			//* TELPORT AWAY
			default:
				TELEPORT_FAR();
				atkCount = mustTeleport + 1;
				// TELEPORT();
				break;
		}
	}


	// todo ------------------------------------------------------------------------------------------------------

	// public void
	IEnumerator MOVE()
	{
		moving = true;
		yield return new WaitForSeconds(2);
		moving = false;
		body.velocity = Vector2.zero;
		mainAnim.SetTrigger("done");
	}
	
	public IEnumerator PSYBEAM(float delay)
	{
		if (!dead && psybeamAtk != null)
		{
			var obj = Instantiate(psybeamAtk, atkPos2.position, 
					psybeamAtk.transform.rotation, spawnedHolder.transform);
			obj.chase.canMove = false;

			yield return new WaitForSeconds(delay);
			obj.chase.canMove = true;
		}
	}

	Enemy SelectSpawn()
	{
		if (!downToHalfHp)
		{
			switch (Random.Range(0,5))
			{
				case 0:		return book.enem;
				case 1:		return book.enem;
				case 2:		return book.enem;
				case 3:		return plank.enem;
				case 4:		return plank.enem;
				default:	return book.enem;
			}
		}
		switch (Random.Range(0,6))
		{
			case 0:		return book.enem;
			case 1:		return plank.enem;
			case 2:		return plank.enem;
			case 3:		return plank.enem;
			case 4:		return chair.enem;
			case 5:		return chair.enem;
			default:	return book.enem;
		}
	}

	IEnumerator PSYCHIC()
	{
		Enemy[] objs = new Enemy[nBooks];

		for (int i=0 ; i<objs.Length ; i++)
		{
			Vector3 offset = new Vector3(Random.Range(-1f,1f), Random.Range(-1f,1f));
			Enemy psychic = SelectSpawn();
			var obj = Instantiate(psychic, offset + spawnPos[Random.Range(0, spawnPos.Length)].position, 
				Quaternion.identity, spawnedHolder.transform);
			objs[i] = obj;
			telekinetic.Add(obj);
			yield return new WaitForSeconds(0.2f);
		}

		yield return new WaitForSeconds(delay);
		for (int i=0 ; i<objs.Length ; i++)
		{
			if (objs[i] != null)
			{
				Vector2 traj = (target.transform.position + Vector3.up - objs[i].transform.position).normalized;
				objs[i].SpeedUpAnimation(5);

				yield return new WaitForSeconds(0.2f);
				if (objs[i] != null)
				{
					objs[i].CallChildOnLaunch(traj, bookSpeed);
					telekinetic.Remove(objs[i]);
				} 
			}
		}

	}

	IEnumerator ConstantPsychic(int n=1)
	{
		Enemy[] objs = new Enemy[n];

		for (int i=0 ; i<n ; i++)
		{
			Vector3 offset = new Vector3(Random.Range(-1f,1f), Random.Range(-1f,1f));
			Enemy psychic = SelectSpawn();
			var obj = Instantiate(psychic, offset + spawnPos[Random.Range(0, spawnPos.Length)].position, 
				Quaternion.identity, spawnedHolder.transform);
			objs[i] = obj;
			if (obj != null)
				telekinetic.Add(obj);
			yield return new WaitForSeconds(0.2f);
		}

		yield return new WaitForSeconds(delay/2);
		StartCoroutine( ConstantPsychic() );
		yield return new WaitForSeconds(delay/2);
		for (int i=0 ; i<n ; i++)
		{
			if (objs[i] != null)
			{
				Vector2 traj = (target.transform.position + Vector3.up - objs[i].transform.position).normalized;
				objs[i].SpeedUpAnimation(8);

				yield return new WaitForSeconds(0.2f);
				if (objs[i] != null)
				{
					objs[i].CallChildOnLaunch(traj, bookSpeed);
					telekinetic.Remove(objs[i]);
				}
			}
		}


	}

	void PSYCHO_CUT()
	{
		if (!dead && psychoCutAtk != null)
		{
			var obj = Instantiate(psychoCutAtk, atkPos.position, 
				Quaternion.identity, spawnedHolder.transform);
			Vector2 traj = (target.position + new Vector3(0, 1.5f) - this.transform.position).normalized;
			obj.direction = traj;
			obj.transform.rotation = Quaternion.LookRotation(psychoCutUp, traj);
			// psychoCutAtk
		}
		LookAtTarget();
	}

	void TELEPORT()
    {
		float minDist = Mathf.Infinity;
		float secMinDist = Mathf.Infinity;
		float thirdMinDist = Mathf.Infinity;
		int ind=0;
		int secInd=0;
		int thirdInd=0;
		for (int i=0 ; i<teleportPos.Length ; i++)
		{
			float dist = Vector2.Distance(teleportPos[i].position, target.position);
			if (dist < minDist)
			{
				thirdMinDist = secMinDist;
				thirdInd = secInd;

				secMinDist = minDist;
				secInd = ind;

				minDist = dist;
				ind = i;
			}
			else if (dist < secMinDist && dist != minDist)
			{
				thirdMinDist = secMinDist;
				thirdInd = secInd;

				secMinDist = dist;
				secInd = i;
			}
			else if (dist < thirdMinDist && dist != minDist)
			{
				thirdMinDist = dist;
				thirdInd = i;
			}
		}

		ind = thirdInd;
		if (lastTpInd == ind)
			ind = secInd;
		// bool isSec = (Random.Range(0,2) == 0);
		// ind = (isSec ? secInd : thirdInd);
		// if (isSec && lastTpInd == secInd)
		// 	ind = thirdInd;
		// else if (!isSec && lastTpInd == thirdInd)
		// 	ind = secInd;
		
		if (teleportBurstEffect != null)
			Instantiate( teleportBurstEffect, this.transform.position, teleportBurstEffect.transform.rotation );
		if (teleportEffect != null)
			Instantiate( teleportEffect, this.transform.position, teleportEffect.transform.rotation );
		
		// TELEPORT TO DIFFERENT NODE
		this.transform.position = teleportPos[ ind ].position;
		lastTpInd = ind;

		if (teleportBurstEffect != null)
			Instantiate( teleportBurstEffect, this.transform.position, teleportBurstEffect.transform.rotation );
		if (teleportEffect != null)
			Instantiate( teleportEffect, this.transform.position, teleportEffect.transform.rotation );
		
		LookAtTarget();
    }

	void TELEPORT_FAR()
    {
		float maxDist = -1;
		float secMaxDist = -1;
		float thirdMaxDist = -1;
		int ind=0;
		int secInd=0;
		int thirdInd=0;
		for (int i=0 ; i<teleportPos.Length ; i++)
		{
			float dist = Vector2.Distance(teleportPos[i].position, target.position);
			if (dist > maxDist)
			{
				thirdMaxDist = secMaxDist;
				thirdInd = secInd;

				secMaxDist = maxDist;
				secInd = ind;

				maxDist = dist;
				ind = i;
			}
			else if (dist > secMaxDist && dist != maxDist)
			{
				thirdMaxDist = secMaxDist;
				thirdInd = secInd;

				secMaxDist = dist;
				secInd = i;
			}
			else if (dist > thirdMaxDist && dist != maxDist)
			{
				thirdMaxDist = dist;
				thirdInd = i;
			}
			// if (dist > maxDist)
			// {
			// 	maxDist = dist;
			// 	ind = i;
			// }
		}
		
		if (teleportBurstEffect != null)
			Instantiate( teleportBurstEffect, this.transform.position, teleportBurstEffect.transform.rotation );
		if (teleportEffect != null)
			Instantiate( teleportEffect, this.transform.position, teleportEffect.transform.rotation );
		
		// TELEPORT TO DIFFERENT NODE
		if (!downToHalfHp)
		{
			if (lastTpInd == thirdInd)
			{
				this.transform.position = teleportPos[ secInd ].position;
				lastTpInd = secInd;
			}
			else
			{
				this.transform.position = teleportPos[ thirdInd ].position;
				lastTpInd = thirdInd;
			}
		}
		// TELEPORT EVEN FURTHER
		else
		{
			if (lastTpInd == secInd)
			{
				this.transform.position = teleportPos[ ind ].position;
				lastTpInd = ind;
			}
			else
			{
				this.transform.position = teleportPos[ secInd ].position;
				lastTpInd = secInd;
			}
		}

		if (teleportBurstEffect != null)
			Instantiate( teleportBurstEffect, this.transform.position, teleportBurstEffect.transform.rotation );
		if (teleportEffect != null)
			Instantiate( teleportEffect, this.transform.position, teleportEffect.transform.rotation );
		
		LookAtTarget();
    }
}

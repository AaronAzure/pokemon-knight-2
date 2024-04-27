using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class Mew : MonoBehaviour
{
	private Rewired.Player player;
	public Transform target;
	public GameObject model;
	public PlayerControls p1;
	public Animator anim;
	public Rigidbody2D body;
	[Space] [SerializeField] LayerMask whatIsGround;


	[Header("Platformer Mechanics")]
	[Space] public float moveSpeed=3;
	public bool p1Alive=true;
	public float maxSpeed=5;
	public float dashSpeed=7.5f;
	public float farAwayDist=8f;
	public GameObject teleportBeginEffect;
	public GameObject teleportEndEffect;
	public ParticleSystem ps; // nova glow
	public ParticleSystem.MainModule m;
	public AllyAttack hitBox;
	public GameObject glint;
	private bool teleportCooldown;
	private bool canPound=true;
	private bool receivingKnockback;
	public float atkCooldown=1;
	public GameObject glass;

	
	private float distance; 
	private bool dashing; 
	private bool canDash=true; 
	private float moveX; 
	private float moveY; 
	


	[Space] [Header("Testing")]
	public bool autoBot = true;
	public bool inBattle;	// controlled by animator
	public bool inCutscene;	// controlled by animator




	void Awake() 
	{
        // Subscribe to events
        ReInput.ControllerConnectedEvent += OnControllerConnected;
        ReInput.ControllerDisconnectedEvent += OnControllerDisconnected;
    }

    // Start is called before the first frame update
    void Start()
    {
        player = ReInput.players.GetPlayer(1);
		this.transform.parent = null;
		m = ps.main;
		// InvokeRepeating("OutputTime", 1f, 1f);
    }

	bool Attack()
	{
		return (player.GetButton("A") || player.GetButton("Y") || player.GetButton("X"));
	}

    // Update is called once per frame
    void Update()
    {
		// PLAYER TWO ( INPUTS )
        if (!inBattle && !inCutscene && !receivingKnockback && p1Alive && !autoBot && player != null)
		{
			if (!teleportCooldown && player.GetButtonDown("B"))
				TeleportToP1();

			if (canPound && Attack())
			{
				canPound = false;
				anim.SetTrigger("atk");
				StartCoroutine(Pound());
			}

			if (canDash && !dashing && player.GetButtonDown("ZR"))
			{
				dashing = true;
				StartCoroutine(Dash());
			}
		}
    }
    void FixedUpdate()
    {
		if (inBattle || inCutscene) {}
		
		// PLAYER TWO
        else if (p1Alive && !receivingKnockback && !autoBot && player != null)
		{
			if (!dashing)
			{
				moveX = player.GetAxis("Move Horizontal");
				moveY = player.GetAxis("Move Vertical");
				body.velocity = new Vector2(moveX, moveY) * maxSpeed;
			}
			else 
			{
				body.velocity = new Vector2(moveX, moveY) * dashSpeed;
			}

			if 		(moveX < 0)
				model.transform.rotation = Quaternion.Euler(0,180,0);

			else if (moveX > 0)
				model.transform.rotation = Quaternion.Euler(0,0,0);
		}
		// AUTO PILOT (NOT P2)
		else if (!receivingKnockback)
		{
			distance = Vector2.Distance(target.position, this.transform.position);
			Vector2 dir = (target.position - this.transform.position).normalized;
			body.AddForce(dir * moveSpeed * Time.fixedDeltaTime * Mathf.Min(1, distance / 3f), ForceMode2D.Impulse);
			body.velocity = Vector2.ClampMagnitude(body.velocity, maxSpeed);

			if 		(body.velocity.x < 0)
				model.transform.rotation = Quaternion.Euler(0,180,0);

			else if (body.velocity.x > 0)
				model.transform.rotation = Quaternion.Euler(0,0,0);

			if (!teleportCooldown && distance > farAwayDist)
				TeleportToP1();
		}
		anim.SetBool("moving", body.velocity.magnitude > 0);
    }

	IEnumerator Cooldown()
	{
		teleportCooldown = true;
		yield return new WaitForSeconds(0.5f);
		teleportCooldown = false;
	}

	public void EnterBossBattle(Transform bossPos)
	{
		if (anim != null) anim.SetTrigger("battle");
		model.transform.rotation = (bossPos.position.x < transform.position.x) ? 
			Quaternion.Euler(0,180,0) : Quaternion.Euler(0,0,0);
	}
	public IEnumerator StartPurify(Enemy boss)
	{
		inCutscene = true;
		body.velocity = Vector2.zero;
		Transform bossPos = boss.transform;
		bool toRight = (bossPos.position.x - transform.position.x) > 0;
		ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
		
		RaycastHit2D groundInfo = Physics2D.Linecast(bossPos.position + new Vector3(0, 1),
			bossPos.position + new Vector3(1.5f, 1), whatIsGround);
		RaycastHit2D groundInfo2 = Physics2D.Linecast(bossPos.position + new Vector3(0, 1),
			bossPos.position + new Vector3(-1.5f, 1), whatIsGround);

		yield return new WaitForSeconds(1f);
		// Prioritise teleport to closest pos
		if (groundInfo.collider == null && !toRight)
			TeleportToPos(bossPos.position + new Vector3(1.5f, 0));
		else if (groundInfo2.collider == null && toRight)
			TeleportToPos(bossPos.position + new Vector3(-1.5f, 0));
		else if (groundInfo.collider == null && toRight)
			TeleportToPos(bossPos.position + new Vector3(1.5f, 0));
		else if (groundInfo2.collider == null && !toRight)
			TeleportToPos(bossPos.position + new Vector3(-1.5f, 0));
		// Default teleport pos if all else fails
		else
			TeleportToPos(bossPos.position);
		
		model.transform.rotation = (bossPos.position.x < transform.position.x) ? 
			Quaternion.Euler(0,180,0) : Quaternion.Euler(0,0,0);

		yield return new WaitForSeconds(0.25f);
		anim.SetTrigger("purify");

		yield return new WaitForSeconds(2.75f);
		boss.Purify();

		yield return new WaitForSeconds(1.25f);
		ps.Play(true);
	}

	public void TeleportToP1()
	{
		StartCoroutine(Cooldown());

		if (teleportBeginEffect != null)
		{
			teleportBeginEffect.SetActive(false);
			teleportBeginEffect.SetActive(true);
			teleportBeginEffect.transform.position = this.transform.position;
		}

		if (p1 != null)
			this.transform.position = p1.transform.position + Vector3.up;

		if (teleportEndEffect != null)
		{
			teleportEndEffect.SetActive(false);
			teleportEndEffect.SetActive(true);
			teleportEndEffect.transform.position = this.transform.position;
		}
	}
	public void TeleportToPos(Vector3 pos)
	{
		StartCoroutine(Cooldown());

		if (teleportBeginEffect != null)
		{
			teleportBeginEffect.SetActive(false);
			teleportBeginEffect.SetActive(true);
			teleportBeginEffect.transform.position = this.transform.position;
		}

		this.transform.position = pos + Vector3.up;

		if (teleportEndEffect != null)
		{
			teleportEndEffect.SetActive(false);
			teleportEndEffect.SetActive(true);
			teleportEndEffect.transform.position = this.transform.position;
		}
	}

	public void CollectCandyForTrainer(int value)
	{
		if (p1 != null)
			p1.GainCandy(value);
	}

	public void EnterCutscene()
	{
		inCutscene = true;
	}
	public void LeaveCutscene()
	{
		inCutscene = false;
	}

	IEnumerator Pound()
	{
		bool tooFar = Vector2.Distance(target.position, this.transform.position) > farAwayDist;
		if (hitBox != null)
			hitBox.atkDmg = tooFar ? (5 + ((p1.lv - 1) * 3)) / 2 : (5 + ((p1.lv - 1) * 3));
		
		yield return new WaitForSeconds(atkCooldown);
		canPound = true;
		glint.SetActive(false);
		glint.SetActive(true);
	}

	IEnumerator Dash()
	{
		dashing = true;
		canDash = false;
		if (teleportBeginEffect != null)
		{
			teleportBeginEffect.SetActive(false);
			teleportBeginEffect.SetActive(true);
			teleportBeginEffect.transform.position = this.transform.position;
		}
		if (model != null)
			model.SetActive(false);

		yield return new WaitForSeconds(0.5f);
		dashing = false;
		if (teleportEndEffect != null)
		{
			teleportEndEffect.SetActive(false);
			teleportEndEffect.SetActive(true);
			teleportEndEffect.transform.position = this.transform.position;
		}
		if (model != null)
			model.SetActive(true);

		yield return new WaitForSeconds(0.5f);
		canDash = true;
		// if (ps != null)
		// {
		// 	ps.transform.position = this.transform.position;
		// 	m.simulationSpace = ParticleSystemSimulationSpace.Local;
		// 	ps.Play(true);
		// }
	}

	public void STOP()
	{
		body.velocity = Vector2.zero;
	}
	public void PlayerDied()
	{
		TeleportToP1();
		p1Alive = false;
		anim.SetTrigger("playerDied");
	}
	public void PlayerRevived()
	{
		TeleportToP1();
		p1Alive = true;
		anim.SetTrigger("playerRevived");
	}

	private void OnTriggerEnter2D(Collider2D other) 
	{
		if (p1Alive && !autoBot && !receivingKnockback && !dashing)
		{
			if (other.CompareTag("Hurtbox"))
				StartCoroutine( ApplyKnockback(other.transform) );
			else if (other.CompareTag("EnemyProjectile"))
				StartCoroutine( ApplyKnockback(other.transform) );
		}
	}


	public IEnumerator ApplyKnockback(Transform opponent, float force=10)
    {
        receivingKnockback = true;
		anim.SetTrigger("hurt");

        Vector2 direction = (opponent.position - this.transform.position).normalized;
		if (glass != null)
		{
			// glass.SetActive(false);
			// glass.SetActive(true);
			float angleZ = Mathf.Atan2(-direction.y, -direction.x) * Mathf.Rad2Deg;
			// glass.transform.rotation = Quaternion.Euler(0,0,angleZ);
			Instantiate(glass, transform.position, Quaternion.Euler(0,0,angleZ));
		}
        body.velocity = new Vector2(-direction.x * force, -direction.x * force);
        
        yield return new WaitForSeconds(0.15f);
        body.velocity = Vector2.zero;
        receivingKnockback = false;
    }


	// todo ------------------------------------------------------------

    // This function will be called when a controller is connected
    void OnControllerConnected(ControllerStatusChangedEventArgs args) 
	{
		if (args.controllerId == 1 && args.controllerType == Rewired.ControllerType.Joystick)
			autoBot = false;
    }

	// This function will be called when a controller is fully disconnected
	void OnControllerDisconnected(ControllerStatusChangedEventArgs args) 
	{
		if (args.controllerId == 1 && args.controllerType == Rewired.ControllerType.Joystick)
			autoBot = true;
    }

	bool CheckControllers()
	{
		int n = ReInput.controllers.GetControllers(Rewired.ControllerType.Joystick).Length;
		return n > 1;

	}
}

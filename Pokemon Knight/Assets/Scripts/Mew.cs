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


	[Header("Platformer Mechanics")]
	[Space] public float moveSpeed=3;
	public bool p1Alive=true;
	public float maxSpeed=5;
	public float teleportDist=10f;
	public GameObject teleportBeginEffect;
	public GameObject teleportEndEffect;
	public AllyAttack hitBox;
	private bool teleportCooldown;
	private bool canPound=true;
	public float atkCooldown=1;


	[Space] [Header("Testing")]
	public bool autoBot = true;




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
		// InvokeRepeating("OutputTime", 1f, 1f);
    }

    // Update is called once per frame
    void Update()
    {
        if (p1Alive && !autoBot && player != null)
		{
			if (!teleportCooldown && player.GetButtonDown("B"))
				TeleportToP1();

			if (canPound && player.GetButton("A"))
			{
				canPound = false;
				anim.SetTrigger("atk");
				StartCoroutine(Pound());
			}
		}
    }
    void FixedUpdate()
    {
        if (p1Alive && !autoBot && player != null)
		{
			float x = player.GetAxis("Move Horizontal");
			float y = player.GetAxis("Move Vertical");
			body.velocity = new Vector2(x, y) * maxSpeed;

			if 		(x < 0)
				model.transform.rotation = Quaternion.Euler(0,180,0);

			else if (x > 0)
				model.transform.rotation = Quaternion.Euler(0,0,0);
		}
		else 
		{
			float distance = Vector2.Distance(target.position, this.transform.position);
			Vector2 dir = (target.position - this.transform.position).normalized;
			body.AddForce(dir * moveSpeed * Time.fixedDeltaTime * Mathf.Min(1, distance / 4f), ForceMode2D.Impulse);
			body.velocity = Vector2.ClampMagnitude(body.velocity, maxSpeed);

			if 		(body.velocity.x < 0)
				model.transform.rotation = Quaternion.Euler(0,180,0);

			else if (body.velocity.x > 0)
				model.transform.rotation = Quaternion.Euler(0,0,0);

			if (!teleportCooldown && distance > teleportDist)
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

	public void CollectCandyForTrainer(int value)
	{
		if (p1 != null)
		{
			p1.GainCandy(value);
		}
	}

	IEnumerator Pound()
	{
		if (hitBox != null)
			hitBox.atkDmg = 5 + (p1.lv - 1) * 3;
		
		yield return new WaitForSeconds(p1.quickCharm ? atkCooldown * 0.7f : atkCooldown);
		canPound = true;
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

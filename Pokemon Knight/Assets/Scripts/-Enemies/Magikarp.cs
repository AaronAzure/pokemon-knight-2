using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magikarp : Enemy
{
	[Space] [Header("Magikarp")] public float moveSpeed=5;
	public float forwardDetect=1f;
	public Transform face;
	private bool isStopped;
	private float moveTimer;
	private float moveMaxTimer=3f;
	private float stopTimer;
	private float stopMaxTimer=1.5f;


	void FixedUpdate() 
	{
		if (!receivingKnockback)
		{
			if (!isStopped)
			{
				moveTimer += Time.fixedDeltaTime;
				if (moveTimer >= moveMaxTimer)
				{
					isStopped = true;
					moveTimer = 0;
				}
			}
			else
			{
				stopTimer += Time.fixedDeltaTime;
				if (stopTimer >= stopMaxTimer)
				{
					isStopped = false;
					stopTimer = 0;
					Flip();
				}
			}
		}
		if (!receivingKnockback && !isStopped && hp > 0)
		{
			if (model.transform.eulerAngles.y != 0)
				body.velocity = new Vector2( moveSpeed, body.velocity.y);
			else
				body.velocity = new Vector2(-moveSpeed, body.velocity.y);
		}
		RaycastHit2D frontInfo;
		if (model.transform.eulerAngles.y > 0) // right
			frontInfo = Physics2D.Raycast(face.position, Vector2.right, forwardDetect, whatIsGround);
		else // left
			frontInfo = Physics2D.Raycast(face.position, Vector2.left, forwardDetect, whatIsGround);

		//* If at edge, then turn around
		if (body.velocity.y >= 0 && frontInfo && !isStopped)
		{
			moveTimer = moveMaxTimer;
			isStopped = true;
			stopTimer = 0;
		}
	}

	private void Flip()
	{
		if (!canFlip)
			return;
		if (model.transform.eulerAngles.y != 0)
			model.transform.eulerAngles = new Vector3(0, 0);
		else
			model.transform.eulerAngles = new Vector3(0, 180);
		// moveSpeed *= -1;
		StartCoroutine( ResetFlipTimer() );
	}


	private void OnDrawGizmosSelected() 
	{
		Gizmos.color = Color.yellow;
		Gizmos.DrawLine(face.position, face.position + new Vector3(-forwardDetect,0));
	}
}

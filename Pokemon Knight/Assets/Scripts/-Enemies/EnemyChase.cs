using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyChase : MonoBehaviour
{
	public bool canMove=true;
	public Rigidbody2D body;
    public Transform target;
	public float speed=5;
	public float maxSpeed;


    private void FixedUpdate() 
	{
		if (canMove && body != null)
		{
			Vector2 dir = (target.position + new Vector3(0,0.5f) - this.transform.position).normalized;
			body.AddForce(dir * 5 * speed * Time.fixedDeltaTime, ForceMode2D.Impulse);

			if(body.velocity.magnitude > maxSpeed)
			{
				body.velocity = body.velocity.normalized * maxSpeed;
			}
		}
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metapod : Enemy
{
	[Space] [Header("Metapod")]
	// public float distanceDetect=2f;
	private bool once;
	[SerializeField] private float distanceDetect = 10;


	// Start is called before the first frame update
	public override void Setup()
	{
		if (body != null) body.gravityScale = 0;
	}

	// Update is called once per frame
	void FixedUpdate()
	{
		if (!once)
		{
			// RaycastHit2D playerInfo = Physics2D.Linecast(this.transform.position, Vector2.down, 10, whatIsPlayer);
			RaycastHit2D playerInfo = Physics2D.Linecast(this.transform.position, this.transform.position + new Vector3(0,-distanceDetect), whatIsPlayer);
			RaycastHit2D playerInfoRight = Physics2D.Linecast(
				this.transform.position + new Vector3(1,0), this.transform.position + new Vector3(1,-distanceDetect), whatIsPlayer);
			RaycastHit2D playerInfoLeft = Physics2D.Linecast(
				this.transform.position + new Vector3(-1,0), this.transform.position + new Vector3(-1,-distanceDetect), whatIsPlayer);

			// Player underneath or been hit
			if (playerInfo || playerInfoRight || playerInfoLeft || body.velocity != Vector2.zero)
			{
				once = true;
				body.gravityScale = 3;
				if (canUseBuffs)
					StartCoroutine( Harden() );
			}
		}
	}

	IEnumerator Harden()
	{
		if (mainAnim != null)
			mainAnim.SetTrigger("harden");
		// IncreaseDef();
		
		yield return new WaitForSeconds(5);
		RevertDef(2);

		yield return new WaitForSeconds(5);
		StartCoroutine( Harden() );
	}

	private void OnDrawGizmosSelected() 
	{
		Gizmos.color = Color.yellow;

        //RaycastHit2D hitM = Physics2D.Raycast(transform.position, Vector2.down, distanceDetect, whatIsGround);
		//RaycastHit2D hitR = Physics2D.Raycast(transform.position + new Vector3(1,0), Vector2.down, distanceDetect, whatIsGround);
		//RaycastHit2D hitL = Physics2D.Raycast(transform.position - new Vector3(1,0), Vector2.down, distanceDetect, whatIsGround);
		//if (hitM)
		//{
		//	Gizmos.DrawLine(this.transform.position, hitM.point);
		//	Gizmos.color = Color.red;
		//	Gizmos.DrawLine(hitM.point, hitM.point + new Vector2(0,(this.transform.position.y-hitM.point.y)-distanceDetect));
		//	Gizmos.color = Color.yellow;
		//}
		//else
		//	Gizmos.DrawLine(this.transform.position, this.transform.position + new Vector3(0,-distanceDetect));
		//if (hitR)
		//{
		//	Gizmos.DrawLine(this.transform.position + new Vector3(1,0), hitR.point);
		//	Gizmos.color = Color.red;
		//	Gizmos.DrawLine(hitR.point, hitR.point + new Vector2(0,(this.transform.position.y-hitR.point.y)-distanceDetect));
		//	Gizmos.color = Color.yellow;
		//}
		//else
		//	Gizmos.DrawLine(this.transform.position + new Vector3(1,0), this.transform.position + new Vector3(1,-distanceDetect));
		//if (hitL)
		//{
		//	Gizmos.DrawLine(this.transform.position - new Vector3(1,0), hitL.point);
		//	Gizmos.color = Color.red;
		//	Gizmos.DrawLine(hitL.point, hitL.point + new Vector2(0,(this.transform.position.y-hitL.point.y)-distanceDetect));
		//	Gizmos.color = Color.yellow;
		//}
		//else
		//	Gizmos.DrawLine(this.transform.position - new Vector3(1,0), this.transform.position + new Vector3(-1,-distanceDetect));
		
		Gizmos.DrawLine(this.transform.position, this.transform.position + new Vector3(0,-distanceDetect));
		Gizmos.DrawLine(this.transform.position + new Vector3(1,0), this.transform.position + new Vector3(1,-distanceDetect));
		Gizmos.DrawLine(this.transform.position - new Vector3(1,0), this.transform.position + new Vector3(-1,-distanceDetect));

	}
}

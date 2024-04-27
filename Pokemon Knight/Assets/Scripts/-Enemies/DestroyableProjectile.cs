using System.Collections;
using System.Collections.Generic;
using UnityEngine.Rendering;
using UnityEngine;

public class DestroyableProjectile : Enemy
{
	[Space] [Header("destroyable")]
	[SerializeField] private float riseSpeed=3;
	[SerializeField] private float riseDur=2;
	[Space] [SerializeField] private ColliderOnAfter addOn;
	[SerializeField] private SortingGroup sort;
	[SerializeField] private Collider2D playerDetection;
	[SerializeField] private GameObject glowObj;


	public override void Setup()
	{
		if (addOn != null)
			addOn.delayOn = riseDur;
		StartCoroutine( RisingUp() );
	}

	IEnumerator RisingUp()
	{
		if (body != null)
			body.velocity = Vector2.up * riseSpeed;
			
		yield return new WaitForSeconds(riseDur);
		// sort.sortingLayerName = "Player";
		sort.sortingOrder = 20;
		if (mainAnim != null)
			mainAnim.speed *= 2;
		if (body != null)
			body.velocity = Vector2.zero;
	}


	public override void CallChildOnDeath()
	{
		CreateExplosion();
	}
	public override void CallChildOnCollision()
	{
		CreateExplosion();
	}


	public override void CallChildOnLaunch(Vector2 dir, float speed)
	{
		if (body != null)
			body.AddForce(dir * speed, ForceMode2D.Impulse);
		
	}

	public override void CallChildOnDeactivate()
	{
		if (body != null)
			body.gravityScale = 3;
		StopAllCoroutines();
		glowObj.SetActive(false);
		col.enabled = true;
		sort.sortingOrder = 20;
		if (mainAnim != null)
			mainAnim.speed = 0;
		if (playerDetection != null)
			playerDetection.enabled = false;
	}

	private void OnCollisionEnter2D(Collision2D other) 
	{
		if (other.gameObject.CompareTag("Ground"))	
		{
			CreateExplosion();
		}
	}



	void CreateExplosion()
	{
		if (explosion != null)
			Instantiate(explosion, this.transform.position, Quaternion.identity);
		Destroy(this.gameObject);
	}


}

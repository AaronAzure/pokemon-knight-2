using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alakazam : Enemy
{

	[Header("Alakazam")]
	public Transform target;
	public GameObject spawnedHolder;

	public EnemyProjectile psybeamAtk;
	public EnemyProjectile psybeamSlowAtk;


    // Start is called before the first frame update
    public override void Setup()
    {
        if (spawnedHolder != null) 
            spawnedHolder.transform.parent = null;
        if (target == null)
            target = playerControls.transform;
		if (psybeamAtk != null)
			psybeamAtk.chase.target = this.target;
		if (psybeamSlowAtk != null)
			psybeamSlowAtk.chase.target = this.target;
		StartCoroutine( Testing() );
    }

    
    void FixedUpdate()
    {
        
    }

	IEnumerator Testing()
	{
		yield return new WaitForSeconds(5);
		if (Random.Range(0,2) == 0 && psybeamAtk != null)
		{
			var obj = Instantiate(psybeamAtk, this.transform.position, 
				psybeamAtk.transform.rotation, spawnedHolder.transform);
		}
		else if (psybeamSlowAtk != null)
		{
			var obj = Instantiate(psybeamSlowAtk, this.transform.position, 
				psybeamSlowAtk.transform.rotation, spawnedHolder.transform);
		}

		StartCoroutine( Testing() );
	}
}

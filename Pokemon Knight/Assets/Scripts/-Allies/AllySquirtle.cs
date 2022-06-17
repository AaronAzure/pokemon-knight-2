using System.Collections;
using UnityEngine;

public class AllySquirtle : Ally
{
    [Space] [Header("Squirtle")] [SerializeField] private AllyProjectile watergunObj;
    [SerializeField] private Transform atkPos;
	[SerializeField] private int nProjectiles=1;
    protected override void Setup() 
    {
        if (useUlt && anim != null)
        {
            outTime = 0.5f;
            anim.SetTrigger("ult");
            StartCoroutine( UltimateAttack() );
        }
        else
        {
            StartCoroutine( Watergun() );
        }

    }

	protected override void OnSecondEvolution()
	{
		nProjectiles = 3;
	}

	protected override void OnThirdEvolution()
	{
		nProjectiles = 5;
	}

    IEnumerator UltimateAttack()
    {
        yield return new WaitForSeconds(0.2f);
        for (int i=0 ; i<15 ; i++)
        {
            UltimateWatergun(i);
            yield return new WaitForSeconds(0.02f);
        }
    }
    
    void UltimateWatergun(int x)
    {
        float offset = 0;
        if (x % 3 == 1)
            offset = Random.Range(0.1f ,0.25f);
        else if (x % 3 == 2)
            offset = Random.Range(-0.25f ,-0.1f);
        var obj = Instantiate(watergunObj, atkPos.position + new Vector3(0, offset), Quaternion.identity);
        obj.atkDmg = (this.atkDmg / 2);
        obj.atkForce = (this.atkForce);
        obj.spawnedPos = this.transform.position;
        obj.velocity *= 1.5f;
        if (this.transform.localScale.x < 0 || this.transform.eulerAngles.y == 180)    // looking left
            obj.velocity *= -1;
    }
    IEnumerator Watergun()
    {
        yield return new WaitForSeconds(0.25f);
		for (int i=0 ; i<nProjectiles ; i++)
		{
			int offset = (i % 2 == 0) ? (i/2) : -(i+1)/2;
			var obj = Instantiate(watergunObj, atkPos.position, Quaternion.identity);
			obj.atkDmg = i == 0 ? this.atkDmg : Mathf.RoundToInt(this.atkDmg / 3f);
			obj.atkForce = this.atkForce;
			obj.spBonus = this.spBonus;
			obj.spawnedPos = this.transform.position;
			Vector2 traj = Quaternion.Euler(0,0,offset * 7.5f) * Vector2.right;
			obj.direction = traj;
			if (this.transform.localScale.x < 0 || this.transform.eulerAngles.y == 180)    // looking left
				obj.velocity *= -1;
		}

    }
}

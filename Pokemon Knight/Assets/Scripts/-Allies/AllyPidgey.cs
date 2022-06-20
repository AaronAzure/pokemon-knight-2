using System.Collections;
using UnityEngine;

public class AllyPidgey : Ally
{
    [Space] [Header("Pidgey")] 
    [SerializeField] private Transform atkPos;
    [SerializeField] private AllyAttack gust;
    [Space][SerializeField] private AllyProjectile ultAtk;

    protected override void Setup() 
    {
		if (useUlt && anim != null)
		{
            body.velocity = Vector2.zero;
			outTime = 1;
			if (ultAtk != null)
			{
				ultAtk.atkDmg = Mathf.RoundToInt( this.atkDmg * 1.5f );
				ultAtk.atkForce = this.atkForce;
			}
			anim.SetTrigger("ult");
		}
		else
		{
			if (gust != null)
			{
				gust.atkDmg = this.atkDmg;
				gust.atkForce = this.atkForce;
				gust.spBonus = this.spBonus;
			}
			body.velocity *= 0.5f;
		}
    }   

	protected override void OnSecondEvolution()
	{
		if (gust != null)
			gust.gameObject.transform.localScale *= 1.3f;
	}
	protected override void OnThirdEvolution()
	{
		if (gust != null)
			gust.gameObject.transform.localScale *= 1.6f;
	}

    public void Gust()
    {
        if (gust != null)
        {
            body.velocity = Vector2.zero;
            var obj = Instantiate(gust, this.transform.position - new Vector3(0,1), gust.transform.rotation);
            // obj.spawnedPos = this.transform.position;
        }
    }

	public void ULT()
	{
		if (ultAtk != null)
		{
			for (int i=0 ; i<6 ; i++)
			{
				Vector2 trajectory = Vector2.right;
				trajectory = Quaternion.Euler(0, 0, 60 * i) * trajectory;
				var obj = Instantiate(ultAtk, this.transform.position, ultAtk.transform.rotation);
				obj.direction = trajectory.normalized;
			}
		}
	}
}

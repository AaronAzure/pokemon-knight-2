using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyAlakazam : Ally
{
    [Space] [Header("Alakazam")] 
    [SerializeField] private Transform atkPos;
    [SerializeField] private AllyProjectile psychoCut;
    [Space][SerializeField] private AllyProjectile ultAtk;
    [SerializeField] private float angleOffset=15;

    protected override void Setup() 
    {
		if (psychoCut != null)
		{
			psychoCut.atkDmg = this.atkDmg;
			psychoCut.atkForce = this.atkForce;
			psychoCut.spBonus = this.spBonus;
		}
		if (useUlt && anim != null)
		{
			if (psychoCut != null)
				psychoCut.atkDmg = Mathf.RoundToInt(0.8f * psychoCut.atkDmg);
            body.velocity = Vector2.zero;
			outTime = ultOutTime;
			anim.SetTrigger("ult");
		}
		else
		{
			anim.SetBool("atkMode", true);
			body.velocity *= 0.5f;
		}
    }   

	protected override void OnSecondEvolution()
	{
		if (psychoCut != null)
			psychoCut.gameObject.transform.localScale *= 1.3f;
	}
	protected override void OnThirdEvolution()
	{
		if (psychoCut != null)
			psychoCut.gameObject.transform.localScale *= 1.6f;
	}

    public void PSYCHO_CUT()
    {
        if (psychoCut != null)
        {
            body.velocity = Vector2.zero;
            var obj = Instantiate(psychoCut, atkPos.position, psychoCut.transform.rotation);
			obj.spawnedPos = this.transform.position;
            if (!FacingRight())
			{
				obj.transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.left);
				obj.direction = Vector2.left;
			}
			else
				obj.transform.rotation = Quaternion.LookRotation(Vector3.forward, Vector3.right);
        }
    }

	public void ULT(int angle)
	{
		if (psychoCut != null)
        {
            var obj = Instantiate(psychoCut, atkPos.position, psychoCut.transform.rotation);
			obj.spawnedPos = this.transform.position;

			bool facingRight = FacingRight();
			Vector2 traj = facingRight ? Vector2.right : Vector2.left;
			traj = Quaternion.Euler(0, 0, angleOffset * angle) * traj;

			obj.transform.rotation = Quaternion.LookRotation(Vector3.forward, facingRight ? Vector3.right : Vector3.left);
			obj.direction = traj.normalized;
        }
	}
}

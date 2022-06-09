using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyGengar : Ally
{
    protected override void Setup() 
    {
        if (useUlt && anim != null)
        {
            outTime = ultOutTime;
            anim.SetTrigger("ult");
            // hitbox.atkDmg = Mathf.RoundToInt( this.atkDmg / 3f);
			spBonus = 0;
        }
		body.velocity *= 0.5f;

    }

    protected override void OnSecondEvolution()
    {
        if (anim != null)
            anim.speed *= 1.5f;
    }

    protected override void OnThirdEvolution()
    {
        if (anim != null)
            anim.speed *= 2f;
    }
}

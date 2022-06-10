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
            hitbox.atkDmg = Mathf.RoundToInt( hitbox.atkDmg / 2f);
			spBonus = 0;
        }
		body.velocity *= 0.5f;

    }

    protected override void OnSecondEvolution()
    {
		hitbox.atkDmg = Mathf.RoundToInt(hitbox.atkDmg * 1.25f );
        // if (anim != null)
        //     anim.speed *= 1.5f;
    }

    protected override void OnThirdEvolution()
    {
		hitbox.atkDmg = Mathf.RoundToInt(hitbox.atkDmg * 1.75f );
        // if (anim != null)
        //     anim.speed *= 2f;
    }
}

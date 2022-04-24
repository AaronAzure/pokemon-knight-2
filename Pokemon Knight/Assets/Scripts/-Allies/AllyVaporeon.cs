using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyVaporeon : Ally
{
    [Space] [Header("Vaporeon")]
    public AllyAttack ultAtk;
    public GameObject attack;
    [SerializeField] private float hForce=10;
    [SerializeField] private float vForce=5;
    
    protected override void Setup() 
    {
        if (useUlt && anim != null)
        {
            outTime = ultOutTime;
            anim.SetTrigger("ult");
            hitbox.atkDmg = Mathf.RoundToInt( this.atkDmg / 3f);
            
            if (transform.rotation.y == 0)  // right
                body.AddForce(new Vector2(hForce, vForce), ForceMode2D.Impulse);
            else
                body.AddForce(new Vector2(-hForce, vForce), ForceMode2D.Impulse);
        }

    }

    protected override void OnSecondEvolution()
    {
        if (attack != null)
            attack.transform.localScale *= 1.25f;
    }

    protected override void OnThirdEvolution()
    {
        if (attack != null)
            attack.transform.localScale *= 1.6f;
    }

}

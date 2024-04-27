using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyBulbasaur : Ally
{
    [Space] [Header("Ult Atk")]
    public AllyAttack ultAtk;
    protected override void Setup() 
    {
        if (useUlt && anim != null)
        {
            outTime = 0.5f;
            anim.SetTrigger("ult");
            atkDmg *= 4;
            atkForce *= 2;
            ultAtk.atkDmg = this.atkDmg;
            ultAtk.atkForce = this.atkForce;
        }

    }   
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllySnorlax : Ally
{
    [Space] [Header("Ult Atk")]
    public AllyAttack slamHitbox;
    public AllyAttack ultAtk;
    public float jumpHeight;
    public bool stillJumping=true;
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
        else
        {
            slamHitbox.atkDmg = Mathf.RoundToInt( this.atkDmg * 0.8f );
            slamHitbox.atkForce = this.atkForce;
            body.velocity = new Vector2(body.velocity.x, 0);
            body.AddForce(Vector2.up * jumpHeight, ForceMode2D.Impulse);
        }
    }   

    public override void CallChildIsGrounded() 
    {
        if (stillJumping)
            body.velocity = new Vector2(body.velocity.x, jumpHeight);
        bool groundInfo = Physics2D.OverlapBox(this.transform.position + (Vector3)feetOffset, feetBox, 0, whatIsGround);
        if (groundInfo && body != null && body.velocity.y <= 0)
        {
            body.velocity = Vector2.zero;
            CallChildOnLanded();
        }
    }

    public void UnparentBlastRadius()
    {
        if (slamHitbox != null)
            slamHitbox.transform.parent = null;
    }

    public override void CallChildOnLanded() 
    {
        if (anim != null)
            anim.SetTrigger("getUp");
    }

}

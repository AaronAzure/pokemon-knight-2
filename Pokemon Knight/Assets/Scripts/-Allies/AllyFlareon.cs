using UnityEngine;

public class AllyFlareon : Ally
{
    [Header("Flareon")]
    public AllyProjectile fireBlast;
    public AllyAttack lavaPlume1;
    public AllyAttack lavaPlume2;
    public AllyAttack lavaPlume3;


    protected override void Setup() 
    {
        if (useUlt && anim != null)
        {
            outTime = 1f;
            anim.SetTrigger("ult");
            if (fireBlast != null)
            {
                fireBlast.atkDmg = this.atkDmg * this.multiHit;
                fireBlast.atkForce = this.atkForce;
            }
        }
    }

    protected override void OnThirdEvolution()
    {
        if (lavaPlume1 != null)
            lavaPlume1.gameObject.SetActive(false);
        if (lavaPlume3 != null)
        {
            lavaPlume3.atkDmg = this.atkDmg;
            lavaPlume3.atkForce = this.atkForce;
            lavaPlume3.spBonus = this.spBonus;
            lavaPlume3.gameObject.SetActive(true);
        }
    }

    protected override void OnSecondEvolution()
    {
        if (lavaPlume1 != null)
            lavaPlume1.gameObject.SetActive(false);
        if (lavaPlume2 != null)
        {
            lavaPlume2.atkDmg = this.atkDmg;
            lavaPlume2.atkForce = this.atkForce;
            lavaPlume2.spBonus = this.spBonus;
            lavaPlume2.gameObject.SetActive(true);
        }
    }

    // public void FIRE_BLAST()
    // {
    //     if (fireBlast != null)
    //     {
    //         var obj = Instantiate(fireBlast, mouthPos.position, fireBlast.transform.rotation);
    //         obj.explosiveAtk = fireBlast.atkDmg;
    //         obj.explosiveKb = fireBlast.atkForce;
    //         if (this.model.transform.eulerAngles.y > 0) // left
    //             obj.velocity *= -1;
    //     }
    // }
}

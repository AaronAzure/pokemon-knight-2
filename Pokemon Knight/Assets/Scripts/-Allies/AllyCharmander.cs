using UnityEngine;

public class AllyCharmander : Ally
{
    public AllyProjectile fireBlast;
    public Transform mouthPos;
    protected override void Setup() 
    {
        if (useUlt && anim != null)
        {
            outTime = 1f;
            anim.SetTrigger("ult");
            if (fireBlast != null)
            {
                fireBlast.atkDmg = Mathf.RoundToInt( this.atkDmg * this.multiHit * 1.2f);
                fireBlast.atkForce = this.atkForce;
            }
        }
    }   

    public void FIRE_BLAST()
    {
        if (fireBlast != null)
        {
            var obj = Instantiate(fireBlast, mouthPos.position, fireBlast.transform.rotation);
            obj.explosiveAtk = fireBlast.atkDmg;
            obj.explosiveKb = fireBlast.atkForce;
            if (this.model.transform.eulerAngles.y > 0) // left
                obj.velocity *= -1;
        }
    }
}

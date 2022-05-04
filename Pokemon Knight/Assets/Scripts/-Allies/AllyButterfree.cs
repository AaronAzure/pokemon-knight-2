using System.Collections;
using UnityEngine;

public class AllyButterfree : Ally
{
    [Space] [Header("Butterfree")] 
    [SerializeField] private Transform atkPos;
    [SerializeField] private AllyAttack poisonPowder;
    [SerializeField] private AllyAttack poisonPowder2;
    [SerializeField] private AllyAttack poisonPowder3;

    protected override void Setup() 
    {
        if (!useUlt && anim != null)
            anim.SetTrigger("attack");
        else if (useUlt && anim != null)
        {
            anim.SetTrigger("ult");
            outTime = 1.5f;
            if (hitbox != null)
            {
                hitbox.atkDmg = this.atkDmg * 7;
                hitbox.atkForce = this.atkForce;
            }

        }
        if (poisonPowder != null)
        {
            poisonPowder.atkDmg = this.atkDmg;
            poisonPowder.atkForce = this.atkForce;
            poisonPowder.spBonus = this.spBonus;
        }
        body.velocity *= 0.5f;
    }

    protected override void OnSecondEvolution()
    {
        poisonPowder = poisonPowder2;
    }
    protected override void OnThirdEvolution()
    {
        poisonPowder = poisonPowder3;
    }

    protected override void ExtraTrailEffects(FollowTowards ft)
    {
        ft.isButterfree = true;
    }

    public void STOP()
    {
        body.velocity = Vector2.zero;
    }
    public void POISON_POWDER()
    {
        if (poisonPowder != null)
        {
            body.velocity = Vector2.zero;
            Instantiate(poisonPowder, atkPos.position, poisonPowder.transform.rotation);
        }
    }
}

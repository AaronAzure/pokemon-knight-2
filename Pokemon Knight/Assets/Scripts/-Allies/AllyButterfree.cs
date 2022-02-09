using System.Collections;
using UnityEngine;

public class AllyButterfree : Ally
{
    [Space] [Header("Butterfree")] 
    [SerializeField] private Animator anim;
    [SerializeField] private Transform atkPos;
    [SerializeField] private AllyAttack poisonPowder;

    protected override void Setup() 
    {
        if (anim != null)
            anim.SetTrigger("attack");
        if (poisonPowder != null)
        {
            poisonPowder.atkDmg = this.atkDmg;
            poisonPowder.atkForce = this.atkForce;
        }
        body.velocity *= 0.5f;
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

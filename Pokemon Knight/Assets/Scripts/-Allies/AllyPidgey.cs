using System.Collections;
using UnityEngine;

public class AllyPidgey : Ally
{
    [Space] [Header("Pidgey")] 
    [SerializeField] private Transform atkPos;
    [SerializeField] private AllyProjectile gust;

    protected override void Start() 
    {
        if (trainer != null)
            atkDmg += ( extraDmg * (int) ((trainer.lv - 1) / perLevel) );

        if (hitbox != null)
        {
            hitbox.atkDmg = this.atkDmg;
            hitbox.atkForce = this.atkForce;
        }

        body.velocity *= 0.5f;
        StartCoroutine( BackToBall() );
    }   

    public void Gust()
    {
        if (gust != null)
        {
            body.velocity = Vector2.zero;
            Instantiate(gust, this.transform.position - new Vector3(0,1), gust.transform.rotation);
        }
    }
}

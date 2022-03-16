using System.Collections;
using UnityEngine;

public class AllyPidgey : Ally
{
    [Space] [Header("Pidgey")] 
    [SerializeField] private Transform atkPos;
    [SerializeField] private AllyAttack gust;

    protected override void Setup() 
    {
        if (gust != null)
        {
            gust.atkDmg = this.atkDmg;
            gust.atkForce = this.atkForce;
            gust.spBonus = this.spBonus;
        }
        body.velocity *= 0.5f;
    }   

    public void Gust()
    {
        if (gust != null)
        {
            body.velocity = Vector2.zero;
            var obj = Instantiate(gust, this.transform.position - new Vector3(0,1), gust.transform.rotation);
            // obj.spawnedPos = this.transform.position;
        }
    }
}

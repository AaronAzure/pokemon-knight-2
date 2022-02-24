using System.Collections;
using UnityEngine;

public class AllyOddish : Ally
{
    [Space] [Header("Oddish")] 
    [SerializeField] private Transform atkPos;
    [SerializeField] private AllyProjectile sludgeBomb;

    protected override void Setup() 
    {
        if (sludgeBomb != null)
        {
            sludgeBomb.atkDmg = this.atkDmg;
            sludgeBomb.atkForce = this.atkForce;
        }
    }   

    public void SLUDGE_BOMB()   //* ANIMATION EVENT
    {
        if (sludgeBomb != null)
        {
            var obj = Instantiate(sludgeBomb, atkPos.position, sludgeBomb.transform.rotation);
            obj.spawnedPos = this.transform.position;
            if (this.transform.eulerAngles.y > 0) //left
                obj.body.velocity = new Vector2(-13,12);
            else //right
                obj.body.velocity = new Vector2(13,12);
        }
    }
}

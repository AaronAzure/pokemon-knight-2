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
            sludgeBomb.spBonus = this.spBonus;
        }
        if (useUlt && anim != null)
        {
            outTime = 0.5f;
            anim.SetTrigger("ult");
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
    public void SLUDGE_BOMB_ULT(int angle=0)   //* ANIMATION EVENT
    {
        if (sludgeBomb != null)
        {
            var obj = Instantiate(sludgeBomb, atkPos.position, sludgeBomb.transform.rotation);
            obj.spawnedPos = this.transform.position;
            
            int dir = 1;    // right
            if (this.transform.eulerAngles.y > 0) //left
                dir = -1;

            switch (angle)
            {
                case 0:
                    obj.body.velocity = new Vector2(9 * dir, 16);
                    break;
                case 1:
                    obj.body.velocity = new Vector2(11 * dir, 14);
                    break;
                case 2:
                    obj.body.velocity = new Vector2(13 * dir, 12);
                    break;
                case 3:
                    obj.body.velocity = new Vector2(15 * dir, 10);
                    break;
                case 4:
                    obj.body.velocity = new Vector2(17 * dir, 8);
                    break;
                default:
                    obj.body.velocity = new Vector2(13 * dir, 12);
                    break;
            }
        }
    }
}

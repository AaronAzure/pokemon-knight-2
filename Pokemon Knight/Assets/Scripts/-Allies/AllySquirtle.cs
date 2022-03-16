using System.Collections;
using UnityEngine;

public class AllySquirtle : Ally
{
    [Space] [Header("Squirtle")] [SerializeField] private AllyProjectile watergunObj;
    [SerializeField] private Transform atkPos;
    protected override void Setup() 
    {
        if (useUlt && anim != null)
        {
            outTime = 0.5f;
            anim.SetTrigger("ult");
            StartCoroutine( UltimateAttack() );
        }
        else
        {
            StartCoroutine( Watergun() );
        }

    }

    IEnumerator UltimateAttack()
    {
        yield return new WaitForSeconds(0.2f);
        for (int i=0 ; i<15 ; i++)
        {
            UltimateWatergun(i);
            yield return new WaitForSeconds(0.02f);
        }
    }
    
    void UltimateWatergun(int x)
    {
        float offset = 0;
        if (x % 3 == 1)
            offset = Random.Range(0.1f ,0.25f);
        else if (x % 3 == 2)
            offset = Random.Range(-0.25f ,-0.1f);
        var obj = Instantiate(watergunObj, atkPos.position + new Vector3(0, offset), Quaternion.identity);
        obj.atkDmg = (this.atkDmg / 2);
        obj.atkForce = (this.atkForce);
        obj.spawnedPos = this.transform.position;
        obj.velocity *= 1.5f;
        if (this.transform.localScale.x < 0 || this.transform.eulerAngles.y == 180)    // looking left
            obj.velocity *= -1;
    }
    IEnumerator Watergun()
    {
        yield return new WaitForSeconds(0.25f);
        var obj = Instantiate(watergunObj, atkPos.position, Quaternion.identity);
        obj.atkDmg = this.atkDmg;
        obj.atkForce = this.atkForce;
        obj.spBonus = this.spBonus;
        obj.spawnedPos = this.transform.position;
        if (this.transform.localScale.x < 0 || this.transform.eulerAngles.y == 180)    // looking left
            obj.velocity *= -1;

    }
}

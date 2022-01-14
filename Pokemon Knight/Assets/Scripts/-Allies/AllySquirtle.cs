using System.Collections;
using UnityEngine;

public class AllySquirtle : Ally
{
    [Space] [Header("Squirtle")] [SerializeField] private AllyProjectile watergunObj;
    [SerializeField] private Transform atkPos;
    private void Start() 
    {
        StartCoroutine( Watergun() );
        StartCoroutine( BackToBall() );
    }
    IEnumerator Watergun()
    {
        yield return new WaitForSeconds(0.25f);
        var obj = Instantiate(watergunObj, atkPos.position, Quaternion.identity);
        obj.atkDmg = this.atkDmg;
        obj.atkForce = this.atkForce;
        if (this.transform.localScale.x < 0 || this.transform.eulerAngles.y == 180)    // looking left
            obj.velocity *= -1;
    }
}

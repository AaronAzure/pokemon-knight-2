using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyTangela : Ally
{
    [Space] [Header("Tangela")] 
    [SerializeField] private AllyProjectile absorb;
    [SerializeField] private DetectEnemy detection;

    protected override void Setup() 
    {
        if (absorb != null)
        {
            absorb.atkDmg = this.atkDmg;
            absorb.atkForce = this.atkForce;
        }
    }   

    private Transform ClosestEnemy()
    {
        if (detection == null)
            return this.transform;
        
        float distance = Mathf.Infinity;
        List<Transform> enemies = detection.DetectEnemies();

        if (enemies == null || enemies.Count == 0)
            return this.transform;
        
        int ind = 0;
        for (int i=0 ; i<enemies.Count ; i++)
        {
            float distToSelf = Mathf.Abs(Vector2.Distance(this.transform.position + new Vector3(0,1), enemies[i].position));
            if (distToSelf < distance)
            {
                distance = distToSelf;
                ind = i;
            }
        }
        return enemies[ind];
    }

    public void ABSORB()
    {
        if (absorb != null)
        {
            var obj = Instantiate(absorb, ClosestEnemy().position + new Vector3(0,1), absorb.transform.rotation);
            obj.player = trainer;
        }
    }
}

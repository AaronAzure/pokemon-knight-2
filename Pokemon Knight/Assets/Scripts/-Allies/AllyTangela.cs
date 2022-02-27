using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyTangela : Ally
{
    [Space] [Header("Tangela")] 
    [SerializeField] private AllyProjectile absorb;
    [SerializeField] private Transform absorbDefaultPos;
    [Space] [SerializeField] private DetectEnemy detection;
    [SerializeField] private int maxDrain=10;
    [SerializeField] private int totalMaxDrain;
    [SerializeField] private int extraDrainDmg=1;
    [Space] [SerializeField] private LayerMask whatIsEnemy;
    [SerializeField] private LayerMask finalMask;

    protected override void Setup() 
    {
        if (trainer != null)
            totalMaxDrain = maxDrain + ( extraDrainDmg * Mathf.CeilToInt(((trainer.lv - 1) / perLevel)) );

        if (absorb != null)
        {
            absorb.atkDmg = this.atkDmg;
            absorb.atkForce = this.atkForce;
            absorb.maxDrain = this.totalMaxDrain;
        }
        finalMask = (whatIsEnemy | whatIsGround);
    }   

    public override string ExtraDesc(int playerLv)
    {
        totalMaxDrain = maxDrain + ( extraDrainDmg * Mathf.CeilToInt(((playerLv - 1) / perLevel)) );
        string extraDesc = "\n(max of " + totalMaxDrain + " hp).";

        return extraDesc;
    }

    private Transform ClosestEnemy()
    {
        if (detection == null)
            return absorbDefaultPos;
        
        float distance = Mathf.Infinity;
        List<Transform> enemies = detection.DetectEnemies();

        if (enemies == null || enemies.Count == 0)
            return absorbDefaultPos;
        
        int ind = -1;
        for (int i=0 ; i<enemies.Count ; i++)
        {
            float distToSelf = Mathf.Abs(Vector2.Distance(this.transform.position + new Vector3(0,1), enemies[i].position));
            if (distToSelf < distance)
            {
                distance = distToSelf;
                ind = i;
            }
        }
        if (ind == -1)
            return absorbDefaultPos;

        return enemies[ind];
    }
    private bool EnemyInLineOfSight(Transform target)
    {
        Vector3 lineOfSight = (target.position + new Vector3(0, 1)) - (this.transform.position + new Vector3(0, 1));
        RaycastHit2D sightInfo = Physics2D.Linecast(this.transform.position + new Vector3(0, 1),
            this.transform.position + new Vector3(0, 1) + lineOfSight, finalMask);
        return (sightInfo.collider != null && sightInfo.collider.gameObject.CompareTag("Enemy"));
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

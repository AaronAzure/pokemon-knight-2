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
    [Space] [SerializeField] private LayerMask whatIsGhost;
    [SerializeField] private LayerMask finalMask;
    [SerializeField] private GameObject cannotFind;

    protected override void Setup() 
    {
        if (trainer != null)
            totalMaxDrain = maxDrain + ( extraDrainDmg * Mathf.CeilToInt(((trainer.lv - 1) / perLevel)) );

		if (useUlt && anim != null)
        {
            anim.SetTrigger("ult");
			this.atkDmg = Mathf.RoundToInt(this.atkDmg * 1.25f);
        }
        if (absorb != null)
        {
            absorb.atkDmg = this.atkDmg;
            absorb.atkForce = this.atkForce;
            absorb.spBonus = this.spBonus;
            absorb.maxDrain = this.totalMaxDrain;
        }
        finalMask = (whatIsEnemy | whatIsGround | whatIsGhost);
        cannotFind.SetActive(false);
    }

    protected override void OnSecondEvolution()
    {
        if (absorb != null && absorb.anim != null)
        {
            anim.speed /= (2f/3f);
            outTime *= (2f/3f);
        }
    }
    protected override void OnThirdEvolution()
    {
        if (absorb != null && absorb.anim != null)
        {
            anim.speed /= (1f/3f);
            outTime *= (1f/3f);
        }
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
            return null;
        
        float distance = Mathf.Infinity;
        List<Transform> enemies = detection.detected;

        if (enemies == null || enemies.Count == 0)
            return null;
        
        int ind = -1;
        for (int i=0 ; i<enemies.Count ; i++)
        {
			//* TARGET DESTROYED
			if (enemies[i] == null)
				continue;

            float distToSelf = Mathf.Abs(Vector2.Distance(this.transform.position + new Vector3(0,1), enemies[i].position));
            if (distToSelf < distance && EnemyInLineOfSight(enemies[i]))
            {
                distance = distToSelf;
                ind = i;
            }
        }
        if (ind == -1)
            return null;

        return enemies[ind];
    }
    private bool EnemyInLineOfSight(Transform target)
    {
        RaycastHit2D sightInfo = Physics2D.Linecast(this.transform.position + new Vector3(0, 1),
            target.position + new Vector3(0, 0.3f), finalMask);
        return (sightInfo.collider != null && sightInfo.collider.gameObject.CompareTag("Enemy"));
    }

    public void ABSORB()
    {
        if (absorb != null)
        {
            Transform target = ClosestEnemy();
            if (target == null)
            {
                StartCoroutine( CannotFindTarget() );
                return;
            }
            var obj = Instantiate(absorb, target.position + new Vector3(0,1), absorb.transform.rotation);
            if      (IsAtThirdEvolution())
                obj.anim.speed *= 2.5f;
            else if (IsAtSecondEvolution())
                obj.anim.speed *= 1.5f;

            obj.player = trainer;
        }
    }

    public void ULT_ABSORB()
    {
        if (absorb != null)
        {
            var obj = Instantiate(absorb, this.transform.position + new Vector3(0, 0.5f), absorb.transform.rotation);
			obj.transform.localScale *= 3;
            if      (IsAtThirdEvolution())
                obj.anim.speed *= 2.5f;
            else if (IsAtSecondEvolution())
                obj.anim.speed *= 1.5f;

            obj.player = trainer;
        }
    }

    IEnumerator CannotFindTarget()
    {
        cannotFind.SetActive(true);
        anim.SetTrigger("done");

        yield return new WaitForSeconds(1f);
        IMMEDIATE_RETURN_TO_BALL();
    }
}

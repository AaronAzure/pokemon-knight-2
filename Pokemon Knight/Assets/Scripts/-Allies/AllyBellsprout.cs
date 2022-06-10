using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyBellsprout : Ally
{
    [Space] [Header("Bellsprout")] [SerializeField] private AllyProjectile razorLeafObj;
    [SerializeField] private AllyProjectile ultObj;
    [SerializeField] private Transform atkPos;
    [SerializeField] private Transform frontPos;
    [Space] [SerializeField] private DetectEnemy detection;
    [Space] [SerializeField] private LayerMask whatIsEnemy;
    [Space] [SerializeField] private LayerMask whatIsGhost;
    [SerializeField] private LayerMask finalMask;
    [SerializeField] private GameObject cannotFind;
	private bool firstAtk=true;
    
    protected override void Setup() 
    {
        if (useUlt && anim != null)
        {
            outTime = 2.5f;
            anim.SetTrigger("ult");

            
            if      (IsAtThirdEvolution())
                ultObj.atkDmg = Mathf.RoundToInt(this.atkDmg * 1.5f);
            else if (IsAtSecondEvolution())
                ultObj.atkDmg = Mathf.RoundToInt(this.atkDmg);
            else
                ultObj.atkDmg = Mathf.RoundToInt(this.atkDmg / 2);

            ultObj.atkForce = Mathf.RoundToInt(this.atkForce / 3);
            ultObj.spawnedPos = this.transform.position;
        }
        cannotFind.SetActive(false);
    }

    protected override void OnSecondEvolution()
    {
        if (!useUlt)
        {
            anim.SetTrigger("repeat");
            outTime *= 2;
        }
    }

    public void RAZOR_LEAF()
    {
        // yield return new WaitForSeconds(0.333f);
        Vector3 closestEnemy = ClosestEnemy();

        //* CANNOT FIND A TARGET
        if (closestEnemy == Vector3.zero)
            return;
            // yield break;
        
        var obj = Instantiate(razorLeafObj, atkPos.position, razorLeafObj.transform.rotation);
		if (firstAtk)
		{
        	obj.atkDmg = this.atkDmg;
			firstAtk = false;
		}
		else
        	obj.atkDmg = Mathf.RoundToInt( this.atkDmg / 2f );
		
        obj.atkForce = this.atkForce;
        obj.spBonus = this.spBonus;
        obj.spawnedPos = this.transform.position;
        
        obj.body.velocity = (closestEnemy - obj.transform.position).normalized * obj.velocity;
        if (closestEnemy.x > this.transform.position.x) // to the right
            model.transform.eulerAngles = new Vector3(0, 0);
        else                                                     // to the left
            model.transform.eulerAngles = new Vector3(0, 180);
    }

    private Vector3 ClosestEnemy()
    {
        if (detection == null)
        {
            CannotFindTarget();
            return Vector3.zero;
        }
        
        float distance = Mathf.Infinity;
        List<Transform> enemies = detection.detected;

        if (enemies == null || enemies.Count == 0)
        {
            CannotFindTarget();
            return Vector3.zero;
        }
        
        int ind = -1;
        for (int i=0 ; i<enemies.Count ; i++)
        {
            float distToSelf = Mathf.Abs(Vector2.Distance(atkPos.position, enemies[i].position));
            if (distToSelf < distance && EnemyInLineOfSight(enemies[i]))
            {
                distance = distToSelf;
                ind = i;
            }
        }
        if (ind == -1)
        {
            CannotFindTarget();
            return Vector3.zero;
        }

        return enemies[ind].position + new Vector3(0,0.3f);
    }
    private bool EnemyInLineOfSight(Transform target)
    {
        RaycastHit2D sightInfo = Physics2D.Linecast(this.transform.position + new Vector3(0, 1),
            target.position + new Vector3(0, 0.3f), finalMask);
        return (sightInfo.collider != null && sightInfo.collider.gameObject.CompareTag("Enemy"));
    }

    private void CannotFindTarget()
    {
        anim.SetTrigger("done");
        cannotFind.SetActive(true);
    }
}

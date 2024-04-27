using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyEevee : Ally
{
    [Space] [Header("Eevee")] 
    [SerializeField] private GameObject flashObj;
    [SerializeField] private float quickAtkForce;
    [SerializeField] private Transform atkPos;
    [SerializeField] private Transform frontPos;
    [SerializeField] private Transform targetPos;
    [Space] [SerializeField] private DetectEnemy detection;
    [Space] [SerializeField] private LayerMask whatIsEnemy;
    [Space] [SerializeField] private LayerMask whatIsGhost;
    [SerializeField] private LayerMask finalMask;
    [Space] public Vector2 quickAtkOffset;
    [SerializeField] private GameObject cannotFind;
    [SerializeField] private AllyAttack ultAtk;

    

    protected override void Setup() 
    {
        finalMask = (whatIsEnemy | whatIsGround | whatIsGhost);
        cannotFind.SetActive(false);
        
        if (useUlt && anim != null)
        {
            outTime = ultOutTime;
            anim.SetTrigger("ult");
            body.velocity = Vector2.zero;
        }
        else
        {
            StartCoroutine( Detect() );
        }

    }

    protected override void OnSecondEvolution()
    {
        if (!useUlt)
            resummonTime = 0.75f;
    }
    protected override void OnThirdEvolution()
    {
        if (!useUlt)
            resummonTime = 0.5f;
    }

    public override void CallChildIsGrounded() 
    {
        RaycastHit2D groundInfo = Physics2D.Raycast(this.transform.position, Vector2.down, feetRadius, whatIsGround);
        if (groundInfo && body != null && body.velocity.y <= 0)
        {
            body.velocity = Vector2.zero;
            CallChildOnLanded();
            anim.SetTrigger("done");
        }
    }

    IEnumerator Detect()
    {
        yield return new WaitForSeconds(0.1f);
        flashObj.transform.parent = null;
        ClosestEnemy();
    }

    public void QuickAttack()
    {
        anim.SetTrigger("atk");
        body.gravityScale = 0;
        body.velocity = Vector2.zero;

		if (targetPos != null)
		{
			if (this.transform.eulerAngles.y > 0)   // attacking from left
				this.transform.position = targetPos.position + new Vector3( quickAtkOffset.x, quickAtkOffset.y);
			else   // attacking from right
				this.transform.position = targetPos.position + new Vector3(-quickAtkOffset.x, quickAtkOffset.y);
			Vector2 dir = (targetPos.position - transform.position).normalized;
			body.AddForce(dir * quickAtkForce, ForceMode2D.Impulse);
		}


        once = false;
    }

    private void ClosestEnemy()
    {
        if (detection == null)
        {
            CannotFindTarget();
            return;
        }
        
        float distance = Mathf.Infinity;
        List<Transform> enemies = detection.detected;

        if (enemies == null || enemies.Count == 0)
        {
            CannotFindTarget();
            return;
        }
        
        int ind = -1;
        for (int i=0 ; i<enemies.Count ; i++)
        {
			if (enemies[i] != null)
			{
				float distToSelf = Mathf.Abs(Vector2.Distance(atkPos.position, enemies[i].position));
				if (distToSelf < distance && EnemyInLineOfSight(enemies[i]))
				{
					distance = distToSelf;
					ind = i;
				}
			}
        }
        if (ind == -1)
        {
            CannotFindTarget();
            return;
        }

        targetPos = enemies[ind];
        if (targetPos != null)
        {
            body.velocity = new Vector2(0, body.velocity.y);
            anim.SetTrigger("atk");
        }
    }
    private bool EnemyInLineOfSight(Transform target)
    {
        RaycastHit2D sightInfo = Physics2D.Linecast(this.transform.position + new Vector3(0, 1),
            target.position + new Vector3(0, 0.3f), finalMask);
        return (sightInfo.collider != null && sightInfo.collider.gameObject.CompareTag("Enemy"));
    }


    private void CannotFindTarget()
    {
        cannotFind.SetActive(true);
    }



    public void ULT_ATK()
    {
        if (ultAtk != null)
        {
            ultAtk.atkDmg = this.atkDmg * 2;
            ultAtk.atkForce = this.atkForce * 2;
            ultAtk.origin = this.transform;

            List<Transform> enemies = detection.detected;
            foreach (Transform enemy in enemies)
                if (enemy != null)
                    Instantiate(ultAtk, enemy.position, ultAtk.transform.rotation);

        }
    }
}

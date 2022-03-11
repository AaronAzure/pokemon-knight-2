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
    [SerializeField] private LayerMask finalMask;
    
    protected override void Setup() 
    {
        if (useUlt && anim != null)
        {
            outTime = 4f;
            anim.SetTrigger("ult");
            ultObj.atkDmg = Mathf.RoundToInt(this.atkDmg / 2);
            ultObj.atkForce = Mathf.RoundToInt(this.atkForce / 3);
            ultObj.spawnedPos = this.transform.position;
        }
        else
        {
            StartCoroutine( RazorLeaf() );
        }

    }

    // IEnumerator UltimateAttack()
    // {
    //     yield return new WaitForSeconds(0.2f);
    //     for (int i=0 ; i<15 ; i++)
    //     {
    //         UltimateWatergun(i);
    //         yield return new WaitForSeconds(0.02f);
    //     }
    // }
    
    // void UltimateWatergun(int x)
    // {
    //     float offset = 0;
    //     if (x % 3 == 1)
    //         offset = Random.Range(0.1f ,0.25f);
    //     else if (x % 3 == 2)
    //         offset = Random.Range(-0.25f ,-0.1f);
    //     var obj = Instantiate(watergunObj, atkPos.position + new Vector3(0, offset), Quaternion.identity);
    //     obj.atkDmg = (this.atkDmg / 2);
    //     obj.atkForce = (this.atkForce);
    //     obj.spawnedPos = this.transform.position;
    //     obj.velocity *= 1.5f;
    //     if (this.transform.localScale.x < 0 || this.transform.eulerAngles.y == 180)    // looking left
    //         obj.velocity *= -1;
    // }
    // private void FixedUpdate() 
    // {
    //     float distance = Mathf.Infinity;
    //     List<Transform> enemies = detection.detected;
    //     int ind = -1;
    //     for (int i=0 ; i<enemies.Count ; i++)
    //     {
    //         float distToSelf = Mathf.Abs(Vector2.Distance(atkPos.position, enemies[i].position));
    //         if (distToSelf < distance)
    //         {
    //             distance = distToSelf;
    //             ind = i;
    //         }
    //     }
    //     if (ind != -1)
    //     {
    //         for (int i=0 ; i<enemies.Count ; i++)
    //         {
    //             if (ind != i)
    //                 Debug.DrawLine(atkPos.position, enemies[i].position + new Vector3(0,0.3f),Color.cyan);
    //             else
    //                 Debug.DrawLine(atkPos.position, enemies[i].position + new Vector3(0,0.3f),Color.red);
    //             if (!EnemyInLineOfSight(enemies[i]))
    //                 Debug.DrawLine(atkPos.position, enemies[i].position + new Vector3(0,0.3f),Color.magenta);
    //         }
    //     }
    // }
    IEnumerator RazorLeaf()
    {
        yield return new WaitForSeconds(0.333f);
        var obj = Instantiate(razorLeafObj, atkPos.position, razorLeafObj.transform.rotation);
        obj.atkDmg = this.atkDmg;
        obj.atkForce = this.atkForce;
        obj.spBonus = this.spBonus;
        obj.spawnedPos = this.transform.position;
        
        Vector3 closestEnemy = ClosestEnemy();
        obj.body.velocity = (closestEnemy - obj.transform.position).normalized * obj.velocity;
        if (closestEnemy.x > this.transform.position.x) // to the right
            model.transform.eulerAngles = new Vector3(0, 0);
        else                                                     // to the left
            model.transform.eulerAngles = new Vector3(0, 180);
    }

    private Vector3 ClosestEnemy()
    {
        if (detection == null)
            return frontPos.position;
        
        float distance = Mathf.Infinity;
        List<Transform> enemies = detection.detected;

        if (enemies == null || enemies.Count == 0)
            return frontPos.position;
        
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
            return frontPos.position;

        return enemies[ind].position + new Vector3(0,0.3f);
    }
    private bool EnemyInLineOfSight(Transform target)
    {
        RaycastHit2D sightInfo = Physics2D.Linecast(this.transform.position + new Vector3(0, 1),
            target.position + new Vector3(0, 0.3f), finalMask);
        return (sightInfo.collider != null && sightInfo.collider.gameObject.CompareTag("Enemy"));
    }

}

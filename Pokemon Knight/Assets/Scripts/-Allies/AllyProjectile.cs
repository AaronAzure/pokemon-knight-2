using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyProjectile : MonoBehaviour
{
    public int atkDmg;
    public int atkForce;    // knockback force
    public int spBonus;
    
    [HideInInspector] public Vector3 spawnedPos;
    [SerializeField] private GameObject trailObj;
    [SerializeField] private float destroyAfter=0.5f;
    [SerializeField] private GameObject explosionObj;
    [SerializeField] private bool destoryOnCollision=true;

    public float velocity=0;
    public Vector2 direction = Vector2.right;
    [SerializeField] private bool customTrajectory;
    public Rigidbody2D body;
    public Animator anim;


    [Header("Explosive Hitbox")]
    [Space] public bool explosiveHitbox;
    [Space] public int explosiveAtk;
    public int explosiveKb;
    [SerializeField] private AllyProjectile explosiveHitboxObj;
    // public float detonateAfter;

    [Header("Absorb")]
    [Space] public bool absorbEffect;
    [HideInInspector] public PlayerControls player;
    public FollowTowards absorbReturnObj;
    public int maxDrain=5;


    private void Start() 
    {
        if (trailObj != null && !customTrajectory)
            StartCoroutine(UnparentTrail());
        if (body != null && velocity != 0 && !customTrajectory)
            body.velocity = direction * velocity;
    }
    public IEnumerator UnparentTrail()
    {
        yield return new WaitForSeconds(destroyAfter);
        if (trailObj != null) 
            trailObj.transform.parent = null;

        yield return new WaitForEndOfFrame();
        if (this.gameObject != null)
            Destroy(this.gameObject);
    }
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Ground") || other.CompareTag("Tree") || other.CompareTag("Enemy"))
        {
            if (trailObj != null) 
                trailObj.transform.parent = null;

            if (other.tag == "Enemy")
            {
                Component[] scripts = other.GetComponents(typeof(Enemy));
                foreach (var script in scripts)
                {
					// STEAL HEALTH
                    var foe = script.GetComponent<Enemy>();
                    if (absorbEffect && !foe.destroyable && player != null)
                    {
                        float hpRecoverPercent = (float) (atkDmg / 2f);
                        if (foe.hp < atkDmg)
                            hpRecoverPercent = (float) (foe.hp / 2f);
                        int hpRecover = Mathf.Abs( Mathf.RoundToInt(hpRecoverPercent * player.maxHp) );
                        hpRecover = Mathf.Min(maxDrain, hpRecover);
                        
                        if (absorbReturnObj != null)
                        {
                            var obj = Instantiate(absorbReturnObj, foe.transform.position + Vector3.up, 
                                absorbReturnObj.transform.rotation);
                            obj.target = player.transform;
                            obj.isPokemonReturning = false;
                            obj.isAllyAbsorbEffect = true;
                            obj.player = this.player;
                            obj.hpRecover = hpRecover;
                        }
                        else
                        {
                            if ((player.hp + hpRecover) < player.maxHp)
                                player.hp += hpRecover;
                            else
                                player.hp = player.maxHp;
                        }
                    }
                    foe.TakeDamage(atkDmg, spawnedPos, atkForce, true, spBonus);
                }
            }
            if (explosiveHitbox && explosiveHitboxObj != null)
            {
                var obj = Instantiate(explosiveHitboxObj, this.transform.position, Quaternion.identity);
                obj.atkDmg = explosiveAtk;
                obj.atkForce = explosiveKb;
            }
            else if (destoryOnCollision && explosionObj != null)
            {
                var obj = Instantiate(explosionObj, this.transform.position, Quaternion.identity);
            }

            if (destoryOnCollision)
                Destroy(this.gameObject);
        }

        if (other.CompareTag("Crystal"))
        {
            Component[] scripts = other.GetComponents(typeof(CrystalBarrier));
            foreach (var script in scripts)
            {
                script.GetComponent<CrystalBarrier>().BreakCrystal();
            }
            
            if (destoryOnCollision && explosionObj != null)
            {
                var obj = Instantiate(explosionObj, this.transform.position, Quaternion.identity);
            }

            if (destoryOnCollision)
                Destroy(this.gameObject);
        }
    }
}

using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public int atkDmg=10;
    public float kbForce=10;
    [Space] public bool destoryOnPlayerCollision;
    [Space] public bool destoryOnWallCollision;
    public GameObject explosion;
    public Rigidbody2D body;
    public Vector2 direction;
    public float speed;
    [Space] public Animator anim;


    [Header("Sleep")]
    [Space] public bool sleepEffect;
    [Space] public float sleepDelay;
    
    
    [Header("Paralysis")]
    [Space] public bool paralysisEffect;
    [Space] public float paralysisDelay;
    
    
    [Header("Absorb")]
    [Space] public bool absorbEffect;
    [HideInInspector] public Enemy moveMaster;
    public FollowTowards absorbReturnObj;



    void Start()
    {
        if (body != null)
            body.velocity = direction * speed;
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))    
        {
            PlayerControls player = other.GetComponent<PlayerControls>();
            if (!absorbEffect)
                player.TakeDamage(atkDmg, this.transform, kbForce);
            else
                player.TakeSpecialDamage(atkDmg, this.transform, kbForce);

            if (sleepEffect)
                player.PutToSleep(sleepDelay);
            
            if (paralysisEffect)
                player.Paralysed(paralysisDelay);
            
            if (absorbEffect && moveMaster != null)
            {
                float hpRecoverPercent = ((float) atkDmg / (float) player.maxHp) / 2f;
                int hpRecover = Mathf.RoundToInt(hpRecoverPercent * moveMaster.maxHp);
                if (absorbReturnObj != null)
                {
                    var obj = Instantiate(absorbReturnObj, player.transform.position + Vector3.up, 
                        absorbReturnObj.transform.rotation);
                    obj.target = moveMaster.transform;
                    obj.isPokemonReturning = false;
                    obj.isAbsorbEffect = true;
                    obj.moveMaster = this.moveMaster;
                    obj.hpRecover = hpRecover;
                }
                else
                {
                    if ((moveMaster.hp + hpRecover) < moveMaster.maxHp)
                        moveMaster.hp += hpRecover;
                    else
                        moveMaster.hp = moveMaster.maxHp;
                }
            }
            
            if (destoryOnPlayerCollision)
            {
                if (explosion != null)
                    Instantiate(explosion, this.transform.position, Quaternion.identity);

                Destroy(this.gameObject);
            }
        }
        if (destoryOnWallCollision && other.CompareTag("Ground"))    
        {
            if (explosion != null)
                Instantiate(explosion, this.transform.position, Quaternion.identity);
            Destroy(this.gameObject);
        }
    }
}

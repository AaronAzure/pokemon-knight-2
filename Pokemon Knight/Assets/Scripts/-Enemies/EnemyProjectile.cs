using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    [HideInInspector] public Enemy moveMaster;
    public int atkDmg=10;
    public float kbForce=10;
    [Space] public bool destoryOnPlayerCollision;
    [Space] public bool destoryOnWallCollision;
    public GameObject explosion;
    public Rigidbody2D body;
    public Vector2 direction;
    public float speed;
    [Space] public bool sleepEffect;
    [Space] public bool absorbEffect;
    public FollowTowards absorbReturnObj;
    [Space] public float sleepDelay;
    public Animator anim;

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
            player.TakeDamage(atkDmg, this.transform, kbForce);

            if (sleepEffect)
                player.PutToSleep(sleepDelay);
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

using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public int atkDmg=10;
    public float kbForce=10;
    [Space] public bool ignoreInvincible;
    [Space] public bool destoryOnPlayerCollision;
    [Space] public bool destoryOnWallCollision;
    public GameObject explosion;
    [SerializeField] private GameObject trailObj;
    public Rigidbody2D body;
    public Vector2 direction;
    public float speed;
    [Space] public Animator anim;

    
	[Header("2nd Hitbox")]
    public EnemyProjectile explosiveHitbox;


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

	public void LaunchAt(Vector3 direction)
	{
		if (body != null)
            body.velocity = direction * speed;
	}

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))    
        {
            PlayerControls player = other.GetComponent<PlayerControls>();
            
            player.TakeDamage(atkDmg, this.transform, kbForce, ignoreInvincible);

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
                else if (moveMaster.hp > 0)
                {
                    if ((moveMaster.hp + hpRecover) < moveMaster.maxHp)
                        moveMaster.hp += hpRecover;
                    else
                        moveMaster.hp = moveMaster.maxHp;
                }
            }
            
            if (destoryOnPlayerCollision)
            {
                CreateExplosion();
            }
        }
        if (destoryOnWallCollision && other.CompareTag("Ground"))    
        {
            CreateExplosion();
        }
    }

	void CreateExplosion()
	{
		if (explosion != null)
			Instantiate(explosion, this.transform.position, Quaternion.identity);
		if (explosiveHitbox != null)
		{
			var obj = Instantiate(explosiveHitbox, this.transform.position, explosiveHitbox.transform.rotation);
			obj.atkDmg = this.atkDmg;
			obj.kbForce = this.kbForce;
		}
		if (trailObj != null) 
			trailObj.transform.parent = null;
		Destroy(this.gameObject);
	}
}

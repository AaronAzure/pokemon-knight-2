using UnityEngine;

public class AllyAttack : MonoBehaviour
{
    // public Transform atkPos;
    // public LayerMask whatIsEnemy;
    // public float atkRange;
    public int atkDmg;
    public int atkForce;    // knockback force

    [SerializeField] private GameObject spawnEffectObj;
    [SerializeField] private bool spawnEffect;

    [Header("Moving projectile")] public float velocity=0;
    public Rigidbody2D body;


    private void Start() 
    {
        if (body != null && velocity != 0)
            body.velocity = Vector2.right * velocity;
    }
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Enemy"))
        {
            Component[] scripts = other.GetComponents(typeof(Enemy));
            foreach (var script in scripts)
            {
                script.GetComponent<Enemy>().TakeDamage(atkDmg, this.transform.position, atkForce);
                if (spawnEffect && spawnEffectObj != null)
                {
                    var obj = Instantiate(spawnEffectObj, script.gameObject.transform.position, Quaternion.identity);
                    Destroy(obj.gameObject, 0.5f);
                }
            }
        }
    }

}

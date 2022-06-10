using UnityEngine;

public class AllyAttack : MonoBehaviour
{
    public int atkDmg;
    public int atkForce;    // knockback force
    public int spBonus;
    public bool registerOneHitOnly;
    [SerializeField] private bool yKb;

    [SerializeField] private GameObject spawnEffectObj;
    [SerializeField] private bool spawnEffect;

    [Header("Moving projectile")] public float velocity=0;
    public Rigidbody2D body;
    public Transform origin;


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
                Enemy enemy = script.GetComponent<Enemy>();
                if (registerOneHitOnly)
                {
                    // enemy.col.offset;
                    if (origin != null)
                        enemy.TakeDamage(atkDmg, origin.position, atkForce, true, spBonus, this, false, yKb);
                    else
                        enemy.TakeDamage(atkDmg, this.transform.position, atkForce, true, spBonus, this, false, yKb);
                }
                else
                {
                    if (origin != null)
                        enemy.TakeDamage(atkDmg, origin.position, atkForce, true, spBonus, null, false, yKb);
                    else
                        enemy.TakeDamage(atkDmg, this.transform.position, atkForce, true, spBonus, null, false, yKb);
                }
                if (spawnEffect && spawnEffectObj != null)
                {
                    var obj = Instantiate(spawnEffectObj, (
                        Vector2) enemy.transform.position + enemy.col.offset, Quaternion.identity);
                    // var obj = Instantiate(spawnEffectObj, script.gameObject.transform.position, Quaternion.identity);
                    Destroy(obj.gameObject, 0.5f);
                }
            }
        }
        if (other.CompareTag("Crystal"))
        {
            Component[] scripts = other.GetComponents(typeof(CrystalBarrier));
            foreach (var script in scripts)
            {
                script.GetComponent<CrystalBarrier>().BreakCrystal();
                if (spawnEffect && spawnEffectObj != null)
                {
                    var obj = Instantiate(spawnEffectObj, script.gameObject.transform.position, Quaternion.identity);
                    Destroy(obj.gameObject, 0.5f);
                }
            }
        }
    }

}

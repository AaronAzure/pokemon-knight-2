using UnityEngine;

public class AllyAttack : MonoBehaviour
{
    // public Transform atkPos;
    // public LayerMask whatIsEnemy;
    // public float atkRange;
    [HideInInspector] public int atkDmg;
    [HideInInspector] public int atkForce;    // knockback force

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
        // Debug.Log("-- logging " + other.name);
        if (other.CompareTag("Enemy"))
        {
            Component[] scripts = other.GetComponents(typeof(Enemy));
            foreach (var script in scripts)
            {
                // Debug.Log(script.name + "  -  " + script.GetType());
                script.GetComponent<Enemy>().TakeDamage(atkDmg, this.transform, atkForce);
                if (spawnEffect && spawnEffectObj != null)
                {
                    var obj = Instantiate(spawnEffectObj, script.gameObject.transform.position, Quaternion.identity);
                    Destroy(obj.gameObject, 0.5f);
                }
                // script.SendMessage("TakeDamage", atkPower, SendMessageOptions.DontRequireReceiver); //! SendMessage = calling methods from unknown classes
            }
        }
    }

}

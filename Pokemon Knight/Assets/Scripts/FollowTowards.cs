using UnityEngine;
using System.Collections;

public class FollowTowards : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0,1);
    [SerializeField] private float speed = 5;
    private bool done;
    public bool isPokemonReturning=true;
    public bool isAbsorbEffect=false;
    public Enemy moveMaster;
    public int hpRecover;
    [Space] [SerializeField] private GameObject healObj;
    [Space] [SerializeField] private ParticleSystem ps;

    
    [Header("Pokemon Returned")]
    public bool powerup;
    public PlayerControls player;
    public string button;
    public string powerupName;
    public bool justForShow=false;
    public float cooldownTime=0.5f;
    [SerializeField] private GameObject powerupAcquired;
    public WaveSpawner spawner;
    public BossRoom bossRoom;
    [Space] public bool isButterfree;
    private bool lostTarget;


    // Start is called before the first frame update
    void Start()
    {
        if (target == null)
            Destroy(this.gameObject);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!done && target != null)
        {
            float step = speed * Time.fixedDeltaTime;

            // move sprite towards the target location
            transform.position = Vector2.MoveTowards(transform.position, target.position + offset, step);

            // Reach destination
            if (Mathf.Abs(Vector3.Distance(target.position + offset, transform.position)) < 0.01f )
            {
                done = true;
                StartCoroutine( Done() );
            }
        }
        else if (target == null && !done && !lostTarget)
        {
            lostTarget = true;
            StartCoroutine( LostTarget() );
        }
    }

    IEnumerator LostTarget()
    {
        this.transform.parent = target.transform;
        var main = ps.main;
        main.loop = false;
        yield return new WaitForSeconds(0.25f);
        Destroy(this.gameObject);
    }

    IEnumerator Done()
    {
        if (isPokemonReturning)
        {
            this.transform.parent = target.transform;
            // ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            var main = ps.main;
            main.loop = false;
            if (player != null && isButterfree)
            {
                player.butterfreeOut = false; 
                player.ButterfreeReturned();
            }
            if (!justForShow && player != null)
            {
                player.PokemonReturned(button, cooldownTime);
            }
            if (powerupName.Length > 0 && player != null)
            {
                var obj = Instantiate(powerupAcquired, this.transform.position,
                     powerupAcquired.transform.rotation, player.transform);
                player.GainPowerup(powerupName);
            }

            if (spawner != null)
                spawner.SpawnedDefeated();
            if (bossRoom != null)
                bossRoom.Walls(false);

            yield return new WaitForSeconds(0.25f);
            Destroy(this.gameObject);
        }
        else if (isAbsorbEffect && moveMaster != null)
        {
            if (hpRecover > 0 && healObj != null)
                Instantiate(healObj, moveMaster.transform.position, healObj.transform.rotation);
            if ((moveMaster.hp + hpRecover) < moveMaster.maxHp)
                moveMaster.hp += hpRecover;
            else
                moveMaster.hp = moveMaster.maxHp;

            this.transform.parent = target.transform;
            var main = ps.main;
            main.loop = false;
            yield return new WaitForSeconds(0.25f);
            Destroy(this.gameObject);
        }
    }
}

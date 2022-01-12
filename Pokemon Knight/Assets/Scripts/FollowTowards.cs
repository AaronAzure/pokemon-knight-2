using UnityEngine;
using System.Collections;

public class FollowTowards : MonoBehaviour
{
    public Transform target;
    public Vector3 offset = new Vector3(0,1);
    [SerializeField] private float speed = 5;
    private bool done;

    [Space] [SerializeField] private ParticleSystem ps;


    // Start is called before the first frame update
    void Start()
    {
        if (target == null)
            Destroy(this.gameObject);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!done)
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
    }

    IEnumerator Done()
    {
        this.transform.parent = target.transform;
        // ps.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        var main = ps.main;
        main.loop = false;

        yield return new WaitForSeconds(0.5f);
        Destroy(this.gameObject);

    }
}

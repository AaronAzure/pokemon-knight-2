using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LaserController : MonoBehaviour
{

    public ParticleSystem laserEndParticle;

    public LineRenderer line;
    public float lineLength = 12f;
    public LayerMask mask;

    private bool playEndEffects;
    private RaycastHit2D hit;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        line.enabled = true;

        hit = Physics2D.Raycast(transform.position, Vector2.right, lineLength, mask);
        if (hit)
        {
            float distance = ( hit.point - (Vector2) this.transform.position ).magnitude;
            line.SetPosition(1, new Vector3(distance, 0, 0) );
            playEndEffects = true;
            if (laserEndParticle != null)
            {
                laserEndParticle.gameObject.transform.position = hit.point;
                laserEndParticle.Play(true);
            }
        }
        else
        {
            line.SetPosition(1, new Vector3(lineLength, 0, 0) );
            playEndEffects = false;
            if (laserEndParticle != null)
                laserEndParticle.Stop(true);
        }
    }
}

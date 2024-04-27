using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class CrystalBarrierSupport : MonoBehaviour
{
    public CrystalBarrier cb;
    [Space] public ParticleSystem barrierPs;  // indicate if player is close enough


    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player") && cb != null)
        {
            cb.canBreak = true;
            barrierPs.Stop(true, ParticleSystemStopBehavior.StopEmitting);
        }
    }
    private void OnTriggerExit2D(Collider2D other) 
    {
        if (other.CompareTag("Player") && cb != null)
        {
            cb.canBreak = false;
            barrierPs.Play();
        }
    }

}

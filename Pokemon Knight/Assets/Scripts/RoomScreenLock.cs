using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class RoomScreenLock : MonoBehaviour
{
    [SerializeField] private CinemachineConfiner confiner;
    [SerializeField] private CinemachineVirtualCamera cm;
    private bool once;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (!once && other.CompareTag("Player"))    
        {
            once = true;
            StartCoroutine( LockScreen() );
        }
    }
    IEnumerator LockScreen()
    {
        if (confiner != null)
            confiner.m_Damping = 1f;

        yield return new WaitForSeconds(0.1f);
        if (cm != null) 
            cm.Follow = this.transform;
    }
}


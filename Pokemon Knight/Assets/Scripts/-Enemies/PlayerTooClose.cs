using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTooClose : MonoBehaviour
{
    [SerializeField] private Enemy enemy;

    private void Start() 
    {
        StartCoroutine( StayAlive() );
    }

    IEnumerator StayAlive()
    {
        yield return new WaitForSeconds(0.1f);
        Destroy( this.gameObject );
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player") && enemy != null)
            Destroy( enemy.gameObject );
    }
}

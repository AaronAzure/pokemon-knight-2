using System.Collections.Generic;
using UnityEngine;

public class DetectEnemy : MonoBehaviour
{
    // public Collider2D col;
    public List<Transform> detected = new List<Transform>();

    public List<Transform> DetectEnemies()
    {
        return detected;
    }

    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Enemy"))
        {
            if (!detected.Contains(other.transform))
                detected.Add(other.transform);
        }
    }

}

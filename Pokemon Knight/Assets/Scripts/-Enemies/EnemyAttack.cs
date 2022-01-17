﻿using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    public int atkDmg=3;
    private void OnTriggerEnter2D(Collider2D other) 
    {
        if (other.CompareTag("Player"))    
        {
            other.GetComponent<PlayerControls>().TakeDamage(atkDmg);
        }
    }
}

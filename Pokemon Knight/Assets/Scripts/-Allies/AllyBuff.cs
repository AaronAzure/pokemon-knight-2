using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyBuff : MonoBehaviour
{
	[System.Serializable] class HealBuff
	{
		public bool isHealing;
		public float healPortion=0.4f;
	}

	[SerializeField] private HealBuff healBuff;

	[SerializeField] private Collider2D alreadyRegistered;



    // Start is called before the first frame update
    void Start()
    {
        
    }
	
	private void OnTriggerEnter2D(Collider2D other) 
	{
		if (alreadyRegistered == null && other.CompareTag("Player"))	
		{
			alreadyRegistered = other;
			if (healBuff.isHealing)
			{
				other.GetComponent<PlayerControls>().Heal(healBuff.healPortion);
			}
		}
	}
}

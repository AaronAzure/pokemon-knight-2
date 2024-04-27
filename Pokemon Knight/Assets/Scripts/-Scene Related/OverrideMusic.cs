using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverrideMusic : MonoBehaviour
{
	private PlayerControls player;
	
	
    private void OnTriggerEnter2D(Collider2D other) 
	{
		if (other.CompareTag("Player"))
		{
			if (player == null)
				player = other.GetComponent<PlayerControls>();

			if (player != null)
			{
				// player.PlayMusic(player.musicManager.)
			}
		}
	}

    private void OnTriggerExit2D(Collider2D other) 
	{
		if (other.CompareTag("Player"))
		{
			if (player == null)
				player = other.GetComponent<PlayerControls>();

			if (player != null)
			{
				// player.PlayMusic(player.musicManager.)
			}
		}
	}
}

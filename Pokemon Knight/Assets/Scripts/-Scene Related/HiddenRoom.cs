using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HiddenRoom : MonoBehaviour
{
	public Animator anim;

    private void OnTriggerEnter2D(Collider2D other) 
	{
		if (anim != null && other.CompareTag("Player"))
		{
			anim.SetTrigger("reveal");
		}
	}

	private void OnTriggerExit2D(Collider2D other) 
	{
		if (anim != null && other.CompareTag("Player"))
		{
			anim.SetTrigger("conceal");
		}
	}
}

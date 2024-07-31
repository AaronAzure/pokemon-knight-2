using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmallTree : MonoBehaviour
{
	[SerializeField] private GameObject destroyPs;
	[SerializeField] private Collider2D col;
	[SerializeField] private SpriteRenderer sr;

	public void DestroyTree()
	{
		if (sr != null)
			sr.enabled = false;
		if (col != null)
			col.enabled = false;
		if (destroyPs != null)
			destroyPs.SetActive(true);
	}
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterTrigger : MonoBehaviour
{
	[SerializeField] BuoyancyEffector2D b;
    /// <summary>
	/// Sent when another object enters a trigger collider attached to this
	/// object (2D physics only).
	/// </summary>
	/// <param name="other">The other Collider2D involved in this collision.</param>
	void OnTriggerEnter2D(Collider2D other)
	{
		if (other.CompareTag("Player") && PlayerControls.Instance != null &&
			PlayerControls.Instance.canSwim)
		{
			b.density = 1;
		}
	}

	/// <summary>
	/// Sent when another object leaves a trigger collider attached to
	/// this object (2D physics only).
	/// </summary>
	/// <param name="other">The other Collider2D involved in this collision.</param>
	void OnTriggerExit2D(Collider2D other)
	{
		if (other.CompareTag("Player") && PlayerControls.Instance != null &&
			PlayerControls.Instance.canSwim)
		{
			b.density = 2;
		}
	}
}

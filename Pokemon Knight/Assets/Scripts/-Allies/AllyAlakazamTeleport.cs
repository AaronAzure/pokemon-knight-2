using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyAlakazamTeleport : MonoBehaviour
{
    [Tooltip("PokeballTrail prefab - return back to player")] public FollowTowards trailObj;
	[SerializeField] private GameObject bubbleObj;

	private void OnEnable() 
	{
		if (PlayerControls.Instance != null)
			bubbleObj.SetActive(PlayerControls.Instance.inWater);	
	}
	private void OnDisable() 
	{
		if (trailObj != null)
        {
            var returnObj = Instantiate(trailObj, this.transform.position, Quaternion.identity, null);
            //returnObj.button = this.button;
            //returnObj.cooldownTime = this.resummonTime;
            //ExtraTrailEffects(returnObj);

            if (PlayerControls.Instance != null)
            {
                returnObj.player = PlayerControls.Instance;
                returnObj.target = PlayerControls.Instance.transform;
            }
            else
            {
                Debug.LogError(" PlayerControls not assigned to Ally.trainer");
            }
        }
	}
}

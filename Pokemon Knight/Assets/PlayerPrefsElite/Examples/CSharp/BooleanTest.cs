using UnityEngine;
using System.Collections;

public class BooleanTest : MonoBehaviour {

	// Declare
	bool myItem;

	// Use this for initialization
	void Start () {
		
		myItem = true;
		
		// save myItem
		PlayerPrefsElite.SetBoolean("myItem", myItem);
		
		//verify and read from PlayerPrefs
		if (PlayerPrefsElite.VerifyBoolean("myItem")){
			Debug.Log("myItem verified");
		}
		
	}

}

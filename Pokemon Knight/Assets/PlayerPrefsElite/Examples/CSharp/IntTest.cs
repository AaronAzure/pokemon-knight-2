using UnityEngine;
using System.Collections;

public class IntTest : MonoBehaviour {

	// Declare
	int myInt;
	public int newInt;
	
	// Use this for initialization
	void Start () {
		
		myInt = 100;
		
		// save myInt
		PlayerPrefsElite.SetInt("myInt", myInt);
		
		//verify and read from PlayerPrefs
		if (PlayerPrefsElite.VerifyInt("myInt")){
			newInt = PlayerPrefs.GetInt("myInt");
			Debug.Log("myInt return true");
		}
		
	}

}

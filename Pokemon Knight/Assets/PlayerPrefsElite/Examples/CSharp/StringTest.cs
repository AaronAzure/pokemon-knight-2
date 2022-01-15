using UnityEngine;
using System.Collections;

public class StringTest : MonoBehaviour {

	// Declare
	string myString;
	public string newString;

	// Use this for initialization
	void Start () {

		myString = "I choose to believe what I was programmed to believe!";

		// save myString
		PlayerPrefsElite.SetString("myString", myString);

		//verify and read from PlayerPrefs
		if (PlayerPrefsElite.VerifyString("myString")){
			newString = PlayerPrefs.GetString("myString");
			Debug.Log("myString return true");
		}
	
	}
	

}

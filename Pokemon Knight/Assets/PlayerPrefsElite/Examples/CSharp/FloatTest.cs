using UnityEngine;
using System.Collections;

public class FloatTest : MonoBehaviour {
	
	// Declare
	float myFloat;
	public float newFloat;
	
	// Use this for initialization
	void Start () {
		
		myFloat = 1.0f;
		
		// save myFloat
		PlayerPrefsElite.SetFloat("myFloat", myFloat);
		
		//verify and read from PlayerPrefs
		if (PlayerPrefsElite.VerifyFloat("myFloat")){
			newFloat = PlayerPrefs.GetFloat("myFloat");
			Debug.Log("myFloat return true");
		}
		
	}
	
}

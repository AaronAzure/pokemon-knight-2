using UnityEngine;
using System.Collections;

public class EncryptTest : MonoBehaviour {

	// Declare variables 
	int Lives, Scores;


	// Use this for initialization
	void Start () {

		// Initialize variables 
		Lives = 3;
		Scores = Random.Range(0, 10000);

		// Store encrypted variables (Lives + Scores)
		PlayerPrefsElite.Encrypt("Combined", Lives.ToString() + Scores.ToString());

		// Compare between already saved "Combined" and current Lives + Scores 

		if (PlayerPrefsElite.CompareEncrypt("Combined", Lives.ToString() + Scores.ToString())){
			Debug.Log("It's true");
		}
	}
	

}
